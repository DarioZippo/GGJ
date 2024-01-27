using UnityEditor;
using UnityEditor.UI;

namespace Pearl.UI
{
    [CustomEditor(typeof(PearlButton))]
    [CanEditMultipleObjects]
    public class PearlButtonInspector : ButtonEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            PearlButton _button = (PearlButton)target;

            EditorGUILayout.LabelField("Complex Click");
            if (!_button.useDelayForClick)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useDoubleClick"));
            }

            if (!_button.useDoubleClick)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useDelayForClick"));

                if (_button.useDelayForClick)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("delayForClick"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("fillObject"));
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Sound vars");
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useSound"));

            if (_button.useSound)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useBackSound"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useSoundInPause"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
