using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(HandPosition))]
    public class HandPositionDrawer : PropertyDrawer
    {
        private const string VisibleModelsBacking = "<VisibleModels>k__BackingField";
        private const string HandSetupsBacking = "<HandSetups>k__BackingField";
        private const string IndexBacking = "<WearableModelIndex>k__BackingField";
        private const string PrefabBacking = "<WearablePrefab>k__BackingField";
        private const string BoneBacking = "<BonePosition>k__BackingField"; 
        private const string NoModelsAvailable = "<No Models Available>";
        private const string EmptyModel = "<Empty>";
        private const string HandModelLabel = "Hand Model";
        private const string BonePositionLabel = "Bone Position";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var rootObject = property.serializedObject;

            var visibleModelsProp = rootObject.FindProperty(VisibleModelsBacking);
            if (visibleModelsProp == null || !visibleModelsProp.isArray || visibleModelsProp.arraySize == 0)
            {
                EditorGUI.LabelField(position, label.text, NoModelsAvailable);
                EditorGUI.EndProperty();
                return;
            }

            var modelsCount = visibleModelsProp.arraySize;

            var handSetupsProp = rootObject.FindProperty(HandSetupsBacking);
            if (handSetupsProp == null || !handSetupsProp.isArray)
            {
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndProperty();
                return;
            }
           
            var currentHandIndex = -1;
            var currentPath = property.propertyPath;
            for (var i = 0; i < handSetupsProp.arraySize; i++)
            {
                if (handSetupsProp.GetArrayElementAtIndex(i).propertyPath == currentPath)
                {
                    currentHandIndex = i;
                    break;
                }
            }

            if (currentHandIndex == -1)
            {
                EditorGUI.PropertyField(position, property, label, true);
                EditorGUI.EndProperty();
                return;
            }
            
            var usedIndices = new int[handSetupsProp.arraySize - 1];
            var usedCount = 0;
            for (var i = 0; i < handSetupsProp.arraySize; i++)
            {
                if (i == currentHandIndex) continue;

                var otherIndexProp = handSetupsProp.GetArrayElementAtIndex(i).FindPropertyRelative(IndexBacking);
                var idx = otherIndexProp?.intValue ?? -1;
                if (idx >= 0 && idx < modelsCount)
                {
                    usedIndices[usedCount++] = idx;
                }
            }

            var indexProp = property.FindPropertyRelative(IndexBacking);
            var currentModelIndex = indexProp?.intValue ?? -1;
           
            var options = new string[modelsCount];
            var validIndices = new int[modelsCount];
            var validCount = 0;

            for (var i = 0; i < modelsCount; i++)
            {
                var isUsed = false;
                for (var j = 0; j < usedCount; j++)
                {
                    if (usedIndices[j] == i)
                    {
                        isUsed = true;
                        break;
                    }
                }

                if (isUsed) continue;

                var modelProp = visibleModelsProp.GetArrayElementAtIndex(i);
                var prefabProp = modelProp.FindPropertyRelative(PrefabBacking);
                var prefab = prefabProp?.objectReferenceValue as GameObject;

                var displayName = prefab != null ? prefab.name : EmptyModel;
                options[validCount] = displayName;
                validIndices[validCount] = i;
                validCount++;
            }
            
            System.Array.Resize(ref options, validCount > 0 ? validCount : 1);
            System.Array.Resize(ref validIndices, validCount > 0 ? validCount : 1);
            
            if (validCount == 0)
            {
                options[0] = "<No Free Models>";
                validIndices[0] = -1;
            }

            var hasConflict = currentModelIndex >= 0 && validCount > 0;
            for (var i = 0; i < validCount; i++)
            {
                if (validIndices[i] == currentModelIndex)
                {
                    hasConflict = false;
                    break;
                }
            }

            if (hasConflict)
            {
                System.Array.Resize(ref options, validCount + 1);
                System.Array.Resize(ref validIndices, validCount + 1);
                System.Array.Copy(options, 0, options, 1, validCount);
                System.Array.Copy(validIndices, 0, validIndices, 1, validCount);

                var currentModelProp = visibleModelsProp.GetArrayElementAtIndex(currentModelIndex);
                var currentPrefabProp = currentModelProp.FindPropertyRelative(PrefabBacking);
                var currentPrefab = currentPrefabProp?.objectReferenceValue as GameObject;
                var currentName = currentPrefab != null ? currentPrefab.name : EmptyModel;

                options[0] = $"{currentName} (occupied)";
                validIndices[0] = currentModelIndex;
                validCount++;
            }
            
            var selectedIndex = 0;
            for (var i = 0; i < validCount; i++)
            {
                if (validIndices[i] == currentModelIndex)
                {
                    selectedIndex = i;
                    break;
                }
            }

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;

            var popupRect = new Rect(position.x, position.y, position.width, lineHeight);
            EditorGUI.BeginChangeCheck();
            var newSelected = EditorGUI.Popup(popupRect, HandModelLabel, selectedIndex, options);
            if (EditorGUI.EndChangeCheck())
            {
                indexProp.intValue = validIndices[newSelected];
            }
            
            var boneProp = property.FindPropertyRelative(BoneBacking);
            var boneHeight = boneProp != null ? EditorGUI.GetPropertyHeight(boneProp, true) : lineHeight;

            var boneRect = new Rect(position.x, position.y + lineHeight + spacing, position.width, boneHeight);
            if (boneProp != null)
            {
                EditorGUI.PropertyField(boneRect, boneProp, new GUIContent(BonePositionLabel), true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var boneProp = property.FindPropertyRelative(BoneBacking);
            var boneHeight = boneProp != null ? EditorGUI.GetPropertyHeight(boneProp, true) : EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing + boneHeight;
        }
    }
}