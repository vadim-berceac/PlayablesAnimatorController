using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(ClipBlendData))]
    public class ClipBlendDataDrawer : PropertyDrawer
    {
        private const string DefaultName = "Missed";
        private const string PropertyName = "<Clip>k__BackingField";
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var clipProp = property.FindPropertyRelative(PropertyName);

            var displayName = DefaultName;

            if (clipProp != null && clipProp.objectReferenceValue is AnimationClip clip && clip != null)
            {
                displayName = clip.name;
            }

            EditorGUI.PropertyField(position, property, new GUIContent(displayName), true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }
    }
}