#if INK

using Pearl.Events;
using System;
using UnityEngine;
using static Pearl.InteractiveTextManager;

namespace Pearl.Ink
{
    public class TalkManager : MonoBehaviour
    {
        #region Inspector fields
        [SerializeField]
        private ComponentReference<InteractiveTextManager> textManagerComponent = null;
        [SerializeField]
        private StoryIndex storyIndex = default;
        [SerializeField]
        private bool onStart = false;
        [SerializeField]
        private StringEvent eventInText;
        #endregion

        #region Events
        public event Action OnFinishWriteTextEvent;
        public event Action OnFinishDialogEvent;
        #endregion

        #region Private fields
        private bool _active = false;
        private InteractiveTextManager _textManager;
        private int _IDDialog;
        #endregion

        #region Property
        public StoryIndex Story { set { storyIndex = value; } }
        #endregion

        #region Unity callbacks
        public void Start()
        {
            if (onStart)
            {
                DialogsManager.CreateDialog(storyIndex, out _IDDialog, OnText, OnFinishDialog);
                ActiveText();
            }
        }

        public void OnDestroy()
        {
            DialogsManager.ForceDialog(_IDDialog);
            DisactiveText();
        }
        #endregion

        #region Pubblic methods
        public void ActiveText()
        {
            if (textManagerComponent != null)
            {
                _textManager = textManagerComponent.Component;
            }

            if (_textManager != null && !_active)
            {
                _textManager.OnFinishWriteText.AddListener(OnFinishText);
                _textManager.OnEvent += OnEvent;
                _active = true;
            }
        }

        public void DisactiveText()
        {
            if (_textManager != null && _active)
            {
                _textManager.OnFinishWriteText.RemoveListener(OnFinishText);
                _textManager.OnEvent -= OnEvent;
                _active = false;
            }
        }

        public void OnText(TextStruct textStruct)
        {
            if (_textManager != null)
            {
                if (textStruct.clipID == string.Empty)
                {
                    _textManager.SetText(textStruct.text);
                }
                else
                {
                    _textManager.SetText(new TextWithAudio(textStruct.text, textStruct.clipID));
                }
            }
        }

        private void OnFinishText()
        {
            OnFinishWriteTextEvent?.Invoke();
            DialogsManager.ReadNextText(_IDDialog);
        }

        private void OnFinishDialog()
        {
            DisactiveText();
            OnFinishDialogEvent?.Invoke();
        }

        public void Talk(StoryIndex newStory)
        {
            storyIndex = newStory;
            Talk();
        }

        public void Talk()
        {
            DialogsManager.CreateDialog(storyIndex, out _IDDialog, OnText, OnFinishDialog);

            if (!_active)
            {
                ActiveText();
            }

            if (_textManager != null && _textManager.CurrentState != StateText.Null)
            {
                _textManager.SkipText();
            }
            else
            {
                DialogsManager.ReadNextText(_IDDialog);
            }
        }

        public void ChangePath(in string path)
        {
            DialogsManager.ChangePath(_IDDialog, path);
        }

        public void SetVar(string var, string newValue)
        {
            DialogsManager.SetVar(_IDDialog, var, newValue);
        }
        #endregion

        #region Private methods
        private void OnEvent(string name, string value)
        {
            eventInText?.Invoke(name);
        }
        #endregion
    }
}

#endif
