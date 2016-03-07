using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityToolkit
{
    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsDrawer : PropertyDrawer
    {
        private FieldInfo m_EnumField;


        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var targetObject = property.serializedObject.targetObject;

            if (m_EnumField == null)
            {
                m_EnumField = targetObject.GetType().GetField(property.name);
            }

            var enumValue = (Enum)m_EnumField.GetValue(targetObject);

            property.intValue = Convert.ToInt32(EditorGUI.EnumMaskField(position, label, enumValue));
        }
    }
}
