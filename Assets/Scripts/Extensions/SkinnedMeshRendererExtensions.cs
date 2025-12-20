using UnityEngine;

public static class SkinnedMeshRendererExtensions
{
    public static void ApplyNewSkinWithRebind(
        this SkinnedMeshRenderer targetRenderer,
        SkinnedMeshRenderer prefabRenderer,
        Animator targetAnimator,
        Animator prefabAnimator)
    {
        if (targetRenderer == null || prefabRenderer == null || targetAnimator == null)
            return;

        // Создаём копию меша, чтобы не портить оригинал
        Mesh meshCopy = Object.Instantiate(prefabRenderer.sharedMesh);

        Transform[] prefabBones = prefabRenderer.bones;
        Transform[] newBones = new Transform[prefabBones.Length];
        Matrix4x4[] newBindposes = new Matrix4x4[prefabBones.Length];

        // Определяем rootBone: сначала ищем по имени из префаба, иначе fallback на Hips
        Transform rootBone = FindBoneByName(targetAnimator.transform, prefabRenderer.rootBone?.name);
        if (rootBone == null)
            rootBone = targetAnimator.GetBoneTransform(HumanBodyBones.Hips);

        for (int i = 0; i < prefabBones.Length; i++)
        {
            Transform prefabBone = prefabBones[i];
            if (prefabBone == null) continue;

            // Определяем HumanBodyBones для этой кости в префабе
            HumanBodyBones matchedBone = HumanBodyBones.LastBone;
            foreach (HumanBodyBones bone in System.Enum.GetValues(typeof(HumanBodyBones)))
            {
                if (bone == HumanBodyBones.LastBone) continue;

                if (prefabAnimator != null && prefabAnimator.GetBoneTransform(bone) == prefabBone)
                {
                    matchedBone = bone;
                    break;
                }
            }

            // Берём соответствующую кость из targetAnimator
            Transform targetBone = matchedBone != HumanBodyBones.LastBone
                ? targetAnimator.GetBoneTransform(matchedBone)
                : FindBoneByName(targetAnimator.transform, prefabBone.name);

            newBones[i] = targetBone;

            if (targetBone != null)
            {
                // Пересчёт bindpose относительно rootBone
                newBindposes[i] = targetBone.worldToLocalMatrix * rootBone.localToWorldMatrix;
            }
            else
            {
                // fallback: оставляем старую матрицу
                newBindposes[i] = prefabBone.worldToLocalMatrix * prefabRenderer.rootBone.localToWorldMatrix;
            }
        }

        // Применяем новые bindposes
        meshCopy.bindposes = newBindposes;

        // Назначаем меш и кости
        targetRenderer.sharedMesh = meshCopy;
        targetRenderer.bones = newBones;
        targetRenderer.rootBone = rootBone;
    }

    private static Transform FindBoneByName(Transform root, string boneName)
    {
        if (string.IsNullOrEmpty(boneName)) return null;
        foreach (var t in root.GetComponentsInChildren<Transform>())
        {
            if (t.name == boneName) return t;
        }
        return null;
    }
}
