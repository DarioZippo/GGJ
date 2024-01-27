using Pearl.Input;
using Pearl.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using static UnityEngine.UI.Button;

namespace Pearl
{
    public class InteractiveTextManager : TextManager
    {
        public enum TypeDynamicText { Interactive, Wait, WaitAtSound, OnEvent }

        [Serializable]
        public struct TextWithAudio
        {
            public string text;
            public AudioClip clip;
            public bool isLocalization;
            public string clipString;

            public TextWithAudio(string text, AudioClip clip)
            {
                this.text = text;
                this.clip = clip;
                this.isLocalization = false;
                this.clipString = string.Empty;
            }

            public TextWithAudio(string text, string clipString)
            {
                this.text = text;
                this.clip = null;
                this.isLocalization = true;
                this.clipString = clipString;
            }
        }

        #region Inspector

        [Header("Interactive")]

        [SerializeField]
        private TypeDynamicText typeDynamicText = TypeDynamicText.Interactive;
        [SerializeField, ConditionalField("@typeDynamicText == Wait")]
        private float waitBeetweenText = 4f;
        [SerializeField, ConditionalField("@typeDynamicText == Interactive")]
        private bool useNavbar = false;
        [SerializeField, ConditionalField("@typeDynamicText == Interactive && !@useNavbar")]
        private NavbarElement navbarElement = null;
        [SerializeField, ConditionalField("@typeDynamicText == Interactive && @useNavbar")]
        private DynamicNavbar navbar = null;

        [SerializeField, ConditionalField("@typeDynamicText == Interactive")]
        private string navbarInputName = "Use";
        [SerializeField, ConditionalField("@typeDynamicText == Interactive")]
        private string navbarLabel = string.Empty;
        [SerializeField, ConditionalField("@typeDynamicText == Interactive")]
        private InputDataType navbarInputDataType = InputDataType.Image;


        [SerializeField, ConditionalField("@typeDynamicText == Interactive")]
        private bool useSkipText = true;

        [Header("Subdivide")]

        [SerializeField]
        private bool isAutomaic = true;
        [SerializeField]
        [ConditionalField("!@isAutomaic")]
        private int maxChar = 30;

        [Header("Sound")]
        [SerializeField]
        private AudioSourceManager audioSource = null;
        [SerializeField]
        private string tableForAudioLocalization = null;
        #endregion

        #region Property
        public AudioSourceManager Audio { set { this.audioSource = value; } }

        public float WaitBeetweenText { set { this.waitBeetweenText = value; } }
        #endregion

        #region Private Methods
        private readonly List<string> _piecesOfText = new();
        private int _indexPieces = 0;
        private AudioClip _currentClip = null;
        #endregion

        #region Unity Callbacks
        protected override void Awake()
        {
            base.Awake();

            if (typeDynamicText == TypeDynamicText.Interactive)
            {
                ButtonClickedEvent unityEvent = new();
                unityEvent?.AddListener(OnResume);

                NavbarInfoElement info = new(unityEvent, navbarInputName, navbarInputDataType, navbarLabel);

                if (useNavbar && navbar)
                {
                    navbar.CreateNavbarElements(info);
                }
                else if (!useNavbar && navbarElement)
                {
                    navbarElement.UpdateElement(info);
                }

                EnableNavbar(false);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            PearlInvoke.StopTimer(OnResume);
        }
        #endregion

        #region Public Methods
        public void OnResume()
        {
            if (audioSource != null)
            {
                audioSource.Stop();
                audioSource.OnFinishClip.RemoveListener(OnResume);
            }

            SetElementsOfPieces();
        }

        public void SetText(TextWithAudio textWithAudio)
        {
            AudioClip clip = textWithAudio.clip;
            #if LOCALIZATION
            if (textWithAudio.isLocalization)
            {
                clip = LocalizationManager.GetAsset<AudioClip>(tableForAudioLocalization, textWithAudio.clipString);
            }
            #endif


            SetText(textWithAudio.text, clip);
        }

        public void SetText(string text, AudioClip audio)
        {
            _currentClip = audio;

            if (audioSource)
            {
                _currentClip = audio;
                audioSource.SetClip(_currentClip, true);
            }

            SetText(text);
        }

        public override void SetText()
        {
            ResetAll();

            _rawText = _rawText.Replace("\\n", "\n");

#if LOCALIZATION
            if (isLocalize)
            {
                _auxText = ChangeLocalization();
            }
            else
            {
                _auxText = ConvertString(tableString, _rawText, out bool useLocalization, true, this);
                if (useLocalization)
                {
                    ChangeActionLocalize(true);
                }
            }
#else
            _auxText = ConvertString(null, _rawText, out bool useLocalization, true, this);
#endif


            if (_auxText == null)
            {
                return;
            }

            InvokeStartText();

            _piecesOfText.Clear();
            SubdivideText(_auxText);
            _indexPieces = 0;
            SetElementsOfPieces();
        }
#endregion

        #region Private Methods
        private void SetElementsOfPieces()
        {
            if (typeDynamicText == TypeDynamicText.Interactive)
            {
                var input = InputManager.Input;
                if (input != null && useSkipText)
                {
                    InputManager.Input.PerformedHandle(navbarInputName, SkipText, ActionEvent.Add, StateButton.Down);
                }

                EnableNavbar(false);
            }

            ResetText();
            _stampText = string.Empty;

            if (_piecesOfText != null && _piecesOfText.Count > _indexPieces)
            {
                _auxText = _piecesOfText[_indexPieces];

                if (_auxText != null && _auxText.Length != 0 && gameObject.activeInHierarchy && isDictated)
                {
                    StartCoroutine(PrePrintText());
                }
                else
                {
                    PrintText(_auxText);
                    PreFinishText();
                }
            }
            else
            {
                OnFinishText();
            }
        }

        protected override void ResetTextContainer()
        {
            PearlInvoke.StopTimer(OnResume);

            base.ResetTextContainer();
        }

        public override void SkipText()
        {
            if (_currentState == StateText.Dictated)
            {
                _skipText = true;
            }
            else if (_currentState == StateText.Waiting)
            {
                _skipText = false;
                PearlInvoke.StopTimer(OnResume);
                OnResume();
            }
        }

        protected override void PreFinishText()
        {
            _indexPieces++;
            _currentState = StateText.Waiting;
            if (typeDynamicText == TypeDynamicText.Interactive)
            {
                var input = InputManager.Input;
                if (input != null && useSkipText)
                {
                    input.PerformedHandle(navbarInputName, SkipText, ActionEvent.Remove, StateButton.Down);
                }

                EnableNavbar(true);
            }
            else if (typeDynamicText == TypeDynamicText.Wait)
            {
                PearlInvoke.WaitForMethod(waitBeetweenText, OnResume);
            }
            else if (audioSource != null && typeDynamicText == TypeDynamicText.WaitAtSound)
            {
                if (!audioSource.IsPlaying)
                {
                    OnResume();
                }
                else
                {
                    audioSource.OnFinishClip.AddListener(OnResume);
                }
            }
        }

        private void EnableNavbar(bool enable)
        {
            if (useNavbar && navbar)
            {
                navbar.ActiveNavbarElements(enable);
            }
            else if (!useNavbar && navbarElement)
            {
                navbarElement.gameObject.SetActive(enable);
            }
        }


        private void SubdivideText(string text)
        {
            if (textContainer)
            {
                int length = 0;

                while (length >= 0 && text != null)
                {
                    GetTextWithoutMarks(text, out string textWithoutCustomTag, out string soloText);

                    text = text.Trim();
                    textContainer.text = textWithoutCustomTag;
                    textContainer.ForceMeshUpdate();

                    length = isAutomaic ? textContainer.firstOverflowCharacterIndex : maxChar;

                    bool lastIndex = true;
                    if (length >= 1)
                    {
                        var aux = soloText.LastIndexOf(".", length - 1);
                        if (aux == -1)
                        {
                            aux = soloText.LastIndexOf(";", length - 1);
                            if (aux == -1)
                            {
                                aux = soloText.LastIndexOf(",", length - 1);
                                if (aux == -1)
                                {
                                    aux = soloText.LastIndexOf(" ", length - 1);
                                    if (aux == -1)
                                    {
                                        lastIndex = false;
                                    }
                                }
                            }
                        }

                        if (lastIndex)
                        {
                            length = aux + 2;
                        }

                    }

                    if (length < 0)
                    {
                        string subString = text.SubstringWithIndex(0, text.Length - 1);
                        _piecesOfText.Add(subString);
                    }
                    else
                    {
                        int count = 0;
                        int index;
                        for (index = 0; index < text.Length; index++)
                        {
                            var character = text[index];
                            if (character == startMarkChar && IsNotIgnorePreCharacther(text, index))
                            {
                                index = text.IndexOf(endMarkChar, index);
                            }
                            else
                            {
                                count++;
                                if (count == length)
                                {
                                    break;
                                }
                            }
                        }

                        string firstPart = text.SubstringWithIndex(0, index - 1);
                        string finalPart = text.SubstringWithIndex(index, text.Length - 1);
                        text = finalPart;
                        _piecesOfText.Add(firstPart);
                    }
                }
            }
        }

        private void GetTextWithoutMarks(in string text, out string textWithoutCustomTag, out string soloText)
        {
            textWithoutCustomTag = string.Empty;
            soloText = string.Empty;

            for (int index = 0; index < text.Length; index++)
            {
                char character = text[index];

                if (character == startMarkChar && IsNotIgnorePreCharacther(text, index))
                {
                    GetMark(text, ref index, out MarkInfo markInfo, out int indexFinalMark);

                    if (Array.Exists(unityTags, x => x == markInfo.tag))
                    {
                        textWithoutCustomTag += text.SubstringWithIndex(index, indexFinalMark);
                    }

                    index = indexFinalMark;
                }
                else
                {
                    textWithoutCustomTag += character;

                    if (Char.GetUnicodeCategory(character) != UnicodeCategory.Control)
                    {
                        soloText += character;
                    }
                }
            }
        }
        #endregion
    }
}
