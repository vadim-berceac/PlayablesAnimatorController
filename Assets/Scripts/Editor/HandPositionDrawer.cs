using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(HandPosition))]
    public class HandPositionDrawer : PropertyDrawer
    {
        private const string VisibleModelsBacking = "<VisibleModels>k__BackingField";
        private const string AnimationSetBacking = "<AnimationSet>k__BackingField";
        private const string TypeBacking = "<Type>k__BackingField";
        private const string IndexBacking = "<WearableModelIndex>k__BackingField";
        private const string PrefabBacking = "<WearablePrefab>k__BackingField";
        private const string BoneBacking = "<BonePosition>k__BackingField"; 
        private const string NoModelsAvailable = "<No Models Available>";
        private const string EmptyModel = "<Empty>";
        private const string AnimationSetLabel = "Animation Set";
        private const string WeaponTypeLabel = "Weapon Type";
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

            var indexProp = property.FindPropertyRelative(IndexBacking);
            var currentModelIndex = indexProp?.intValue ?? -1;
           
            var options = new string[modelsCount];

            for (var i = 0; i < modelsCount; i++)
            {
                var modelProp = visibleModelsProp.GetArrayElementAtIndex(i);
                var prefabProp = modelProp.FindPropertyRelative(PrefabBacking);
                var prefab = prefabProp?.objectReferenceValue as GameObject;

                var displayName = prefab != null ? prefab.name : EmptyModel;
                options[i] = displayName;
            }
            
            var selectedIndex = currentModelIndex >= 0 && currentModelIndex < modelsCount ? currentModelIndex : 0;

            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;

            var currentY = position.y;

            // Animation Set field
            var animationSetProp = property.FindPropertyRelative(AnimationSetBacking);
            if (animationSetProp != null)
            {
                var animSetRect = new Rect(position.x, currentY, position.width, lineHeight);
                EditorGUI.PropertyField(animSetRect, animationSetProp, new GUIContent(AnimationSetLabel));
                currentY += lineHeight + spacing;
            }

            // Weapon Type field
            var typeProp = property.FindPropertyRelative(TypeBacking);
            if (typeProp != null)
            {
                var typeRect = new Rect(position.x, currentY, position.width, lineHeight);
                EditorGUI.PropertyField(typeRect, typeProp, new GUIContent(WeaponTypeLabel));
                currentY += lineHeight + spacing;
            }

            // Hand Model popup
            var popupRect = new Rect(position.x, currentY, position.width, lineHeight);
            EditorGUI.BeginChangeCheck();
            var newSelected = EditorGUI.Popup(popupRect, HandModelLabel, selectedIndex, options);
            if (EditorGUI.EndChangeCheck())
            {
                indexProp.intValue = newSelected;
            }
            currentY += lineHeight + spacing;
            
            // Bone Position field
            var boneProp = property.FindPropertyRelative(BoneBacking);
            var boneHeight = boneProp != null ? EditorGUI.GetPropertyHeight(boneProp, true) : lineHeight;

            var boneRect = new Rect(position.x, currentY, position.width, boneHeight);
            if (boneProp != null)
            {
                EditorGUI.PropertyField(boneRect, boneProp, new GUIContent(BonePositionLabel), true);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var spacing = EditorGUIUtility.standardVerticalSpacing;
            
            var totalHeight = 0f;
            
            // Animation Set
            var animationSetProp = property.FindPropertyRelative(AnimationSetBacking);
            if (animationSetProp != null)
            {
                totalHeight += lineHeight + spacing;
            }
            
            // Weapon Type
            var typeProp = property.FindPropertyRelative(TypeBacking);
            if (typeProp != null)
            {
                totalHeight += lineHeight + spacing;
            }
            
            // Hand Model popup
            totalHeight += lineHeight + spacing;
            
            // Bone Position
            var boneProp = property.FindPropertyRelative(BoneBacking);
            var boneHeight = boneProp != null ? EditorGUI.GetPropertyHeight(boneProp, true) : lineHeight;
            totalHeight += boneHeight;

            return totalHeight;
        }
    }
}