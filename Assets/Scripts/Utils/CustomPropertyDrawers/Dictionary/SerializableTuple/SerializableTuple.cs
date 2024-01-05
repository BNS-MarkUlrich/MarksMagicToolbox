using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class SerializableTuple<T1, T2>
{
    public T1 Item1;
    public T2 Item2;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SerializableTuple<,>))]
public class SerializableTupleDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Rect contentPosition = EditorGUI.PrefixLabel(position, label);

        float labelWidth = 40f; // Adjust label width as needed
        float halfWidth = (contentPosition.width - labelWidth * 2 - 4) * 0.5f;

        Rect keyLabelRect = new Rect(contentPosition.x, contentPosition.y, labelWidth, contentPosition.height);
        Rect keyFieldRect = new Rect(contentPosition.x + labelWidth, contentPosition.y, halfWidth, contentPosition.height);

        EditorGUI.LabelField(keyLabelRect, "Key");
        EditorGUI.PropertyField(keyFieldRect, property.FindPropertyRelative("Item1"), GUIContent.none);

        Rect valueLabelRect = new Rect(contentPosition.x + labelWidth + halfWidth + 4, contentPosition.y, labelWidth, contentPosition.height);
        Rect valueFieldRect = new Rect(contentPosition.x + labelWidth * 2 + halfWidth + 4, contentPosition.y, halfWidth, contentPosition.height);

        EditorGUI.LabelField(valueLabelRect, "Value");
        EditorGUI.PropertyField(valueFieldRect, property.FindPropertyRelative("Item2"), GUIContent.none);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
#endif
