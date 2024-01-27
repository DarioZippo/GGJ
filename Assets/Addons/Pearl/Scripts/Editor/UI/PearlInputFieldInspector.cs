using TMPro.EditorUtilities;
using UnityEditor;

namespace Pearl.UI
{
    [CustomEditor(typeof(PearlInputField))]
    [CanEditMultipleObjects]
    public class PearlInputFieldInspector : TMP_InputFieldEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            PearlInputField _field = (PearlInputField)target;

            EditorGUILayout.LabelField("Navigation");
            EditorGUILayout.LabelField("Reader Numeric Info");
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("axisUtility"));
            EditorGUI.indentLevel--;

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("nextTextButton"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("clearOnDisactive"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isVector"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("nextAxis"));

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Sound vars");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useSound"));

            if (_field.useSound)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useSoundInPause"));
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Other Setting");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("useAutoSizeFont"));
            if (_field.useAutoSizeFont)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minSizeFont"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}