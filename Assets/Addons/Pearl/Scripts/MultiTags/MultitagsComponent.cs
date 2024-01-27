using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Pearl.Multitags
{
    public struct TagForValue
    {
        public string tag;
        public string value;

        public TagForValue(string tag, string value)
        {
            this.tag = tag;
            this.value = value;
        }
    }

    [DisallowMultipleComponent]
    public class MultitagsComponent : PearlBehaviour, IReset
    {
        #region Inspector fields
        [SerializeField]
        private StringStringDictionary _tags = new();

        private List<string> _tagsList = new();
        #endregion

        #region Private fields
        private bool _isAdded = false;
        #endregion

        #region Properties
        public List<string> ListTags { get { return _tagsList; } }
        #endregion

        #region Button
        [InspectorButton("ValidateTags")]
        [SerializeField]
        private bool validate;
        #endregion

        #region Unity Callbacks
        protected override void PearlAwake()
        {
            ValidateTags();
            ResetElement();
        }

        protected override void OnDestroy()
        {
            DisableElement();
        }
        #endregion

        #region Public Methods
        public void AddTags(params string[] newTags)
        {
            foreach (var tag in newTags)
            {
                AddTag(tag);
            }
        }

        public void AddTags(params TagForValue[] newTags)
        {
            foreach (var tuple in newTags)
            {
                AddTag(tuple.tag, tuple.value);
            }
        }

        public void RemoveTags(params string[] tags)
        {
            foreach (var tag in tags)
            {
                RemoveTag(tag);
            }
        }

        public void ClearTags()
        {
            if (_tagsList != null)
            {
                _tagsList.Clear();
                _tags.Clear();
            }
        }

        public bool HasTags(bool only, params string[] tags)
        {
            if ((tags == null && _tagsList == null) || (tags.Length == 0 && _tagsList.Count == 0))
            {
                return true;
            }

            if (_tagsList == null || _tagsList.Count == 0)
            {
                return false;
            }


            if (tags == null || tags.Length == 0)
            {
                return !only;
            }


            if (only && tags.Length != _tagsList.Count)
            {
                return false;
            }

            bool aux;
            foreach (var tag in tags)
            {
                aux = SearchTag(tag);
                if (!aux)
                {
                    return false;
                }
            }
            return true;
        }

        public string GetTagValue(string tag)
        {
            if (_tags != null)
            {
                _tags.IsNotNullAndTryGetValue(tag, out string result);
                return result;
            }
            return null;
        }

        public int GetTagIntValue(string tag)
        {
            string aux = GetTagValue(tag);
            return ConvertExtend.FromStringToInt(aux);
        }

        public float GetTagFloatValue(string tag)
        {
            string aux = GetTagValue(tag);
            return ConvertExtend.FromStringToFloat(aux);
        }

        public void ChangeTagValue(TagForValue tag)
        {
            _tags ??= new StringStringDictionary();
            _tags.Update(tag.tag, tag.value);
        }

        public int GetCountTag()
        {
            if (_tagsList == null)
            {
                return -1;
            }

            return _tagsList.Count;
        }

        public string GetTag(in int index = 0)
        {
            if (_tagsList.IsAlmostSpecificCount(index))
            {
                return _tagsList[index];
            }
            return null;
        }
        #endregion

        #region Private Methods
        private void ValidateTags()
        {
            ModyfyStringsInLowerCase();
            if (_tags != null)
            {
                _tagsList?.AddRange(_tags.Keys.ToList());
                _tagsList?.Sort();
            }
        }

        private bool SearchTag(in string tag)
        {
            return tag != null && _tagsList != null && _tagsList.BinarySearch(tag.ToLower()) >= 0;
        }

        private void ModyfyStringsInLowerCase()
        {
            if (_tags != null)
            {
                for (int i = _tags.Count - 1; i >= 0; i--)
                {
                    var key = _tags.Keys.Get(i);
                    var newKey = key.ToLower();
                    _tags.RenameKey(key, newKey);
                }
            }
        }

        private void RemoveTag(string tag)
        {
            if (tag != null)
            {
                tag = tag.ToLower();
                _tags?.Remove(tag);
                _tagsList?.Remove(tag);
            }
        }

        private void AddTag(string newTag)
        {
            AddTag(newTag, "0");
        }

        private void AddTag(string newTag, string value)
        {
            newTag = newTag.ToLower();
            _tags ??= new StringStringDictionary();
            _tagsList ??= new List<string>();

            if (!_tags.ContainsKey(newTag))
            {
                _tags.Add(newTag, value);
            }

            if (!SearchTag(newTag))
            {
                _tagsList.AddInSort(newTag);
            }
        }
        #endregion

        #region Interface Methods
        public void ResetElement()
        {
            if (!_isAdded)
            {
                MultiTagsManager.NewElement(this);
                _isAdded = true;
            }
        }

        public void DisableElement()
        {
            if (_isAdded)
            {
                MultiTagsManager.RemoveElement(this);
                _isAdded = false;
            }
        }
        #endregion
    }
}
