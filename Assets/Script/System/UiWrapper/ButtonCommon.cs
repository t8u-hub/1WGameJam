using UnityEngine.UI;
using UnityEngine;
using UnityEditor;


public class ButtonCommon : Button
{
    [SerializeField]
    Animator _buttonAnimator;

    public void SetOnClick(System.Action onClick)
    {
        this.onClick.RemoveAllListeners();
        this.onClick.AddListener(() =>
        {
            onClick?.Invoke();
        });
    }

    protected override void Awake()
    {
        _buttonAnimator = this.GetComponent<Animator>();
    }

    public void OnPointerDown()
    {
        _buttonAnimator.Play("Selected");
    }

    public void OnPointerExit()
    {
        _buttonAnimator.Play("Default");
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(ButtonCommon))]
    public class CustomButtonEditor : UnityEditor.UI.ButtonEditor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var component = (ButtonCommon)target;

            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(component._buttonAnimator)), new GUIContent("Animator"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
