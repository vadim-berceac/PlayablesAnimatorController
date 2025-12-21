using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomPropertyDrawer(typeof(WearableModel))]
    public class WearableModelDrawer: PropertyDrawer
    {
        private const string DefaultName = "Missed";
        private const string PropertyName = "<WearablePrefab>k__BackingField";

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var modelProp = property.FindPropertyRelative(PropertyName);

            var displayName = DefaultName;
            
            if (modelProp != null && modelProp.objectReferenceValue is GameObject model && model != null)
            {
                displayName = model.name;
            }
            
            EditorGUI.PropertyField(position, property, new GUIContent(displayName), true);
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, true);
        }
    }
}

