using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(ClipBlendDataCollection))]
    public class ClipBlendDataCollectionDrawer : PropertyDrawer
    {
        private const string PropertyName = "<AnimationSet>k__BackingField";
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var animationSetProp = property.FindPropertyRelative(PropertyName);

            var targetObject = property.serializedObject.targetObject as State;
            var linkToWeaponIndex = false;

            if (targetObject != null)
                linkToWeaponIndex = targetObject.LinkToWeaponIndex;

            if (linkToWeaponIndex)
            {
                var enumName = $"{animationSetProp.enumDisplayNames[animationSetProp.enumValueIndex]} = {animationSetProp.enumValueIndex}";
                label = new GUIContent(enumName);
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}