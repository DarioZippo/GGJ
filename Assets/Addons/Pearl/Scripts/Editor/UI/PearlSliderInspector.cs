using UnityEditor;
using UnityEditor.UI;

namespace Pearl.UI
{
    [CustomEditor(typeof(PearlSlider))]
    [CanEditMultipleObjects]
    public class PearSliderInspector : SliderEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            PearlSlider _slider = (PearlSlider)target;

            EditorGUILayout.LabelField("Sound vars");
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useSound"));

            if (_slider.useSound)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useSoundInPause"));
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("useStepSize"));
            if (_slider.useStepSize)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("StepSize"));
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
