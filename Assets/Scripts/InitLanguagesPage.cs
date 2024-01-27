using Pearl;
using Pearl.Multitags;
using Pearl.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class InitLanguagesPage : GenericPage, IContentFiller<string>
    {
        [Header("Language Page")]

        [SerializeField]
        private GameObject buttonPrefab = null;
        [SerializeField]
        private Transform containersButton = null;

        protected LanguageScrollViewFiller _filler;

        protected override void PearlStart()
        {
            _filler = new();
            _filler?.Fill(this);
        }

        public void FillContent(List<string> content)
        {
            content?.Sort();
            foreach (var item in content)
            {
                GameObject button = GameObject.Instantiate(buttonPrefab);
                button.transform.SetParent(containersButton);
                button.AddTags(item);

                if (button.TryGetComponent<PearlSelectableManager>(out var selectableManager))
                {
                    selectableManager.OnSelected.AddListener(OnSelectableButton);
                    selectableManager.OnPointerUp.AddListener(OnClickButton);
                }

                if (button.TryGetComponent<TextManager>(out var textManager))
                {
                    string value = LocalizationManager.Translate("Language", item, item);

                    textManager.SetText(value);
                    if (item == LocalizationManager.LanguageString)
                    {
                        FocusManager.SetFocus(button);
                    }
                }
            }
        }

        public void SetContent(string content)
        {
        }

        private void OnSelectableButton(GameObject selected)
        {
            var tag = selected.GetTag();
            LocalizationManager.LanguageString = tag;
        }

        private void OnClickButton(GameObject selected)
        {
            SaveOption();
            GameManager.CheckTransitions(true);
        }
    }
}