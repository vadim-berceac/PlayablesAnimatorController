using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public static class AdvancedSkinRetargeting
{
    /// <summary>
    /// Применяет скин с автоматической адаптацией к различным ригам
    /// </summary>
    public static void ApplyNewSkinWithRetargeting(
        this SkinnedMeshRenderer targetRenderer,
        SkinnedMeshRenderer sourceSkin,
        Animator targetAnimator,
        Animator sourceAnimator,
        RetargetingOptions options = null)
    {
        if (targetRenderer == null || sourceSkin == null || targetAnimator == null || sourceAnimator == null)
        {
            Debug.LogError("Required components are null!");
            return;
        }

        options = options ?? new RetargetingOptions();

        // Создаем копию меша
        var meshCopy = Object.Instantiate(sourceSkin.sharedMesh);
        
        // Анализируем скелеты
        var sourceInfo = AnalyzeSkeleton(sourceAnimator, sourceSkin);
        var targetInfo = AnalyzeSkeleton(targetAnimator, null);
        
        //Debug.Log($"Source skeleton: {sourceInfo.boneCount} bones, Humanoid: {sourceInfo.isHumanoid}");
        //Debug.Log($"Target skeleton: {targetInfo.boneCount} bones, Humanoid: {targetInfo.isHumanoid}");

        // Строим маппинг костей
        var mapping = BuildBoneMapping(sourceSkin, sourceAnimator, targetAnimator, sourceInfo, targetInfo, options);
        
        //Debug.Log($"Mapped {mapping.mappedBones}/{mapping.totalBones} bones ({mapping.mappingQuality:F1}% quality)");

        // Определяем нужен ли пересчет bindpose
        var needsBindposeRecalculation = ShouldRecalculateBindpose(sourceInfo, targetInfo, mapping, options);
        
        if (needsBindposeRecalculation)
        {
            //Debug.Log("Recalculating bindposes for better fit...");
            RecalculateBindposes(meshCopy, mapping, sourceSkin);
        }

        // Применяем результат
        targetRenderer.sharedMesh = meshCopy;
        targetRenderer.bones = mapping.targetBones;
        targetRenderer.rootBone = mapping.rootBone;
        targetRenderer.sharedMaterials = sourceSkin.sharedMaterials;
        targetRenderer.localBounds = CalculateOptimalBounds(meshCopy, mapping);
        
        // Дополнительные настройки
        targetRenderer.quality = sourceSkin.quality;
        targetRenderer.updateWhenOffscreen = options.updateWhenOffscreen;
        targetRenderer.skinnedMotionVectors = true;

        // Валидация результата
        ValidateRetargeting(targetRenderer, mapping);
    }

    #region Skeleton Analysis

    private class SkeletonInfo
    {
        public int boneCount;
        public bool isHumanoid;
        public Vector3 averageBoneLength;
        public float skeletonHeight;
        public Dictionary<HumanBodyBones, Transform> humanBones = new Dictionary<HumanBodyBones, Transform>();
        public Transform rootBone;
    }

    private static SkeletonInfo AnalyzeSkeleton(Animator animator, SkinnedMeshRenderer renderer)
    {
        var info = new SkeletonInfo();
        
        // Проверяем humanoid
        info.isHumanoid = animator.isHuman;
        
        if (info.isHumanoid)
        {
            // Собираем humanoid кости
            foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone == HumanBodyBones.LastBone) continue;
                var t = animator.GetBoneTransform(bone);
                if (t != null)
                {
                    info.humanBones[bone] = t;
                }
            }
            
            info.boneCount = info.humanBones.Count;
            
            // Вычисляем высоту скелета
            var head = animator.GetBoneTransform(HumanBodyBones.Head);
            var hips = animator.GetBoneTransform(HumanBodyBones.Hips);
            if (head != null && hips != null)
            {
                info.skeletonHeight = Vector3.Distance(head.position, hips.position);
            }
        }
        else if (renderer != null)
        {
            info.boneCount = renderer.bones.Length;
            info.rootBone = renderer.rootBone;
        }
        
        return info;
    }

    #endregion

    #region Bone Mapping

    private class BoneMapping
    {
        public Transform[] targetBones;
        public Transform rootBone;
        public int mappedBones;
        public int totalBones;
        public float mappingQuality;
        public Dictionary<int, MappingInfo> mappingDetails = new Dictionary<int, MappingInfo>();
    }

    private class MappingInfo
    {
        public Transform sourceBone;
        public Transform targetBone;
        public HumanBodyBones humanBone;
        public MappingMethod method;
        public float confidence;
    }

    private enum MappingMethod
    {
        HumanoidDirect,      // Прямой маппинг через HumanBodyBones
        NameMatch,           // Совпадение по имени
        HierarchyMatch,      // Совпадение по иерархии
        FuzzyNameMatch,      // Нечеткое совпадение имени
        Fallback            // Fallback решение
    }

    private static BoneMapping BuildBoneMapping(
        SkinnedMeshRenderer sourceSkin,
        Animator sourceAnimator,
        Animator targetAnimator,
        SkeletonInfo sourceInfo,
        SkeletonInfo targetInfo,
        RetargetingOptions options)
    {
        var mapping = new BoneMapping();
        var sourceBones = sourceSkin.bones;
        mapping.totalBones = sourceBones.Length;
        mapping.targetBones = new Transform[sourceBones.Length];

        // Строим карту source костей -> HumanBodyBones
        var sourceBoneMap = new Dictionary<Transform, HumanBodyBones>();
        if (sourceInfo.isHumanoid)
        {
            foreach (var kvp in sourceInfo.humanBones)
            {
                sourceBoneMap[kvp.Value] = kvp.Key;
            }
        }

        // Находим rootBone
        mapping.rootBone = FindRootBone(sourceSkin, targetAnimator, targetInfo);

        // Маппим каждую кость
        for (var i = 0; i < sourceBones.Length; i++)
        {
            var sourceBone = sourceBones[i];
            if (sourceBone == null)
            {
                mapping.targetBones[i] = null;
                continue;
            }

            var mappingInfo = new MappingInfo { sourceBone = sourceBone };

            // Метод 1: Прямой humanoid маппинг (самый надежный)
            if (sourceBoneMap.TryGetValue(sourceBone, out HumanBodyBones humanBone) && targetInfo.isHumanoid)
            {
                var targetBone = targetAnimator.GetBoneTransform(humanBone);
                if (targetBone != null)
                {
                    mappingInfo.targetBone = targetBone;
                    mappingInfo.humanBone = humanBone;
                    mappingInfo.method = MappingMethod.HumanoidDirect;
                    mappingInfo.confidence = 1.0f;
                }
            }

            // Метод 2: Точное совпадение имени
            if (mappingInfo.targetBone == null && options.useNameMatching)
            {
                var targetBone = FindBoneByName(targetAnimator.transform, sourceBone.name);
                if (targetBone != null)
                {
                    mappingInfo.targetBone = targetBone;
                    mappingInfo.method = MappingMethod.NameMatch;
                    mappingInfo.confidence = 0.9f;
                }
            }

            // Метод 3: Нечеткое совпадение имени (для разных naming conventions)
            if (mappingInfo.targetBone == null && options.useFuzzyMatching)
            {
                var targetBone = FindBoneByFuzzyName(targetAnimator.transform, sourceBone.name);
                if (targetBone != null)
                {
                    mappingInfo.targetBone = targetBone;
                    mappingInfo.method = MappingMethod.FuzzyNameMatch;
                    mappingInfo.confidence = 0.7f;
                }
            }

            // Метод 4: Маппинг по иерархии (для non-humanoid)
            if (mappingInfo.targetBone == null && options.useHierarchyMatching)
            {
                var targetBone = FindBoneByHierarchy(sourceBone, sourceSkin.rootBone, 
                    mapping.rootBone, targetAnimator.transform);
                if (targetBone != null)
                {
                    mappingInfo.targetBone = targetBone;
                    mappingInfo.method = MappingMethod.HierarchyMatch;
                    mappingInfo.confidence = 0.6f;
                }
            }

            mapping.targetBones[i] = mappingInfo.targetBone;
            mapping.mappingDetails[i] = mappingInfo;

            if (mappingInfo.targetBone != null)
            {
                mapping.mappedBones++;
            }
            else if (options.logUnmappedBones)
            {
                Debug.LogWarning($"Failed to map bone '{sourceBone.name}' (index {i})");
            }
        }

        mapping.mappingQuality = (float)mapping.mappedBones / mapping.totalBones * 100f;
        
        return mapping;
    }

    private static Transform FindRootBone(SkinnedMeshRenderer sourceSkin, Animator targetAnimator, SkeletonInfo targetInfo)
    {
        Transform rootBone = null;

        // Пытаемся найти по имени
        if (sourceSkin.rootBone != null)
        {
            rootBone = FindBoneByName(targetAnimator.transform, sourceSkin.rootBone.name);
        }

        // Fallback на Hips для humanoid
        if (rootBone == null && targetInfo.isHumanoid)
        {
            rootBone = targetAnimator.GetBoneTransform(HumanBodyBones.Hips);
        }

        // Последний fallback - первая кость скелета
        if (rootBone == null)
        {
            rootBone = targetAnimator.transform;
        }

        return rootBone;
    }

    #endregion

    #region Bone Finding Methods

    private static Transform FindBoneByName(Transform root, string boneName)
    {
        if (string.IsNullOrEmpty(boneName)) return null;
        
        Transform[] allTransforms = root.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in allTransforms)
        {
            if (t.name == boneName) return t;
        }
        
        return null;
    }

    private static Transform FindBoneByFuzzyName(Transform root, string sourceName)
    {
        if (string.IsNullOrEmpty(sourceName)) return null;

        // Нормализуем имя (убираем префиксы, суффиксы, разделители)
        string normalizedSource = NormalizeBoneName(sourceName);
        
        Transform[] allTransforms = root.GetComponentsInChildren<Transform>(true);
        Transform bestMatch = null;
        float bestScore = 0f;

        foreach (Transform t in allTransforms)
        {
            string normalizedTarget = NormalizeBoneName(t.name);
            float score = CalculateNameSimilarity(normalizedSource, normalizedTarget);
            
            if (score > bestScore && score > 0.7f) // Порог схожести 70%
            {
                bestScore = score;
                bestMatch = t;
            }
        }

        return bestMatch;
    }

    private static string NormalizeBoneName(string name)
    {
        // Убираем общие префиксы и суффиксы
        string normalized = name.ToLower();
        
        // Убираем разделители
        normalized = normalized.Replace("_", "").Replace("-", "").Replace(".", "").Replace(" ", "");
        
        // Убираем общие префиксы (mixamo:, bip01, etc)
        string[] prefixes = { "mixamo", "bip01", "bip001", "valvebiped" };
        foreach (string prefix in prefixes)
        {
            if (normalized.StartsWith(prefix))
            {
                normalized = normalized.Substring(prefix.Length);
            }
        }
        
        return normalized;
    }

    private static float CalculateNameSimilarity(string a, string b)
    {
        if (a == b) return 1.0f;
        
        // Простой Levenshtein distance
        int maxLen = Mathf.Max(a.Length, b.Length);
        if (maxLen == 0) return 1.0f;
        
        int distance = LevenshteinDistance(a, b);
        return 1.0f - (float)distance / maxLen;
    }

    private static int LevenshteinDistance(string a, string b)
    {
        int[,] d = new int[a.Length + 1, b.Length + 1];
        
        for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) d[0, j] = j;
        
        for (int j = 1; j <= b.Length; j++)
        {
            for (int i = 1; i <= a.Length; i++)
            {
                int cost = (a[i - 1] == b[j - 1]) ? 0 : 1;
                d[i, j] = Mathf.Min(
                    Mathf.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost
                );
            }
        }
        
        return d[a.Length, b.Length];
    }

    private static Transform FindBoneByHierarchy(Transform sourceBone, Transform sourceRoot, 
        Transform targetRoot, Transform targetSkeleton)
    {
        if (sourceBone == null || sourceRoot == null || targetRoot == null) return null;

        // Строим путь от source кости до root
        List<string> path = new List<string>();
        Transform current = sourceBone;
        while (current != null && current != sourceRoot && path.Count < 20)
        {
            path.Add(current.name);
            current = current.parent;
        }
        path.Reverse();

        // Пытаемся найти по тому же пути в target
        current = targetRoot;
        foreach (string boneName in path)
        {
            Transform child = FindChildByName(current, boneName);
            if (child != null)
            {
                current = child;
            }
            else
            {
                return null;
            }
        }

        return current;
    }

    private static Transform FindChildByName(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name) return child;
        }
        return null;
    }

    #endregion

    #region Bindpose Recalculation

    private static bool ShouldRecalculateBindpose(SkeletonInfo sourceInfo, SkeletonInfo targetInfo, 
        BoneMapping mapping, RetargetingOptions options)
    {
        if (!options.allowBindposeRecalculation) return false;

        // Пересчитываем, если:
        // 1. Скелеты разного размера
        if (Mathf.Abs(sourceInfo.skeletonHeight - targetInfo.skeletonHeight) > 0.1f)
            return true;

        // 2. Низкое качество маппинга
        if (mapping.mappingQuality < 95f)
            return true;

        // 3. Есть non-humanoid кости
        if (!sourceInfo.isHumanoid || !targetInfo.isHumanoid)
            return true;

        return false;
    }

    private static void RecalculateBindposes(Mesh mesh, BoneMapping mapping, SkinnedMeshRenderer sourceSkin)
    {
        Matrix4x4[] originalBindposes = mesh.bindposes;
        Matrix4x4[] newBindposes = new Matrix4x4[originalBindposes.Length];

        for (int i = 0; i < mapping.targetBones.Length; i++)
        {
            Transform targetBone = mapping.targetBones[i];
            
            if (targetBone != null && mapping.rootBone != null)
            {
                // Новая bindpose: преобразует из bone space в mesh space (root bone space)
                newBindposes[i] = targetBone.worldToLocalMatrix * mapping.rootBone.localToWorldMatrix;
            }
            else
            {
                // Сохраняем оригинальную
                newBindposes[i] = originalBindposes[i];
            }
        }

        mesh.bindposes = newBindposes;
    }

    #endregion

    #region Bounds Calculation

    private static Bounds CalculateOptimalBounds(Mesh mesh, BoneMapping mapping)
    {
        // Если большинство костей замаплено, используем bounds меша
        if (mapping.mappingQuality > 90f)
        {
            return mesh.bounds;
        }

        // Иначе расширяем bounds для безопасности
        Bounds bounds = mesh.bounds;
        bounds.Expand(bounds.size.magnitude * 0.2f);
        return bounds;
    }

    #endregion

    #region Validation

    private static void ValidateRetargeting(SkinnedMeshRenderer renderer, BoneMapping mapping)
    {
        // Проверяем наличие null костей
        int nullBones = mapping.targetBones.Count(b => b == null);
        if (nullBones > 0)
        {
            Debug.LogWarning($"Retargeting has {nullBones} unmapped bones. This may cause visual artifacts.");
        }

        // Проверяем bounds
        if (renderer.localBounds.size.magnitude < 0.01f)
        {
            Debug.LogWarning("Renderer bounds are very small. Mesh might not be visible.");
        }

        // Проверяем материалы
        if (renderer.sharedMaterials == null || renderer.sharedMaterials.Length == 0)
        {
            Debug.LogWarning("No materials assigned to renderer.");
        }
    }

    #endregion

    #region Options

    public class RetargetingOptions
    {
        public bool useNameMatching = true;
        public bool useFuzzyMatching = true;
        public bool useHierarchyMatching = true;
        public bool allowBindposeRecalculation = true;
        public bool updateWhenOffscreen = false;
        public bool logUnmappedBones = true;
    }

    #endregion
}