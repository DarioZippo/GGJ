using System.Collections.Generic;
using UnityEngine;

namespace Pearl.Multitags
{
    /// <summary>
    /// Static class that allow you to take advantage of the multitags
    /// </summary>
    public static class MultiTagsManager
    {
        #region Auxiliars Fields
        private readonly static List<MultitagsComponent> _listGameObjectsWithTags = new();
        private readonly static List<GameObject> _results = new();
        private static GameObject[] _auxArrayGameObject;
        #endregion

        #region Static Methods
        public static void NewElement(MultitagsComponent newElement)
        {
            _listGameObjectsWithTags.AddOnce(newElement);
        }

        public static void RemoveElement(MultitagsComponent newElement)
        {
            _listGameObjectsWithTags.Remove(newElement);
        }

        /// <summary>
        /// Searches the gameobjects with tags.
        /// </summary>
        /// <param name = "onlyThoseTags">This bool specifies whether the Gameobject should have only those tags</param>
        /// <param name = "tagsParameter">The tags that must have the object</param>
        public static GameObject[] FindGameObjectsWithMultiTags(in bool onlyThoseTags, params string[] tagsParameter)
        {
            if (tagsParameter != null && tagsParameter.Length != 0 && _results != null)
            {
                _results.Clear();

                foreach (MultitagsComponent manager in _listGameObjectsWithTags)
                {
                    int count = manager.GetCountTag();
                    if ((!onlyThoseTags && count < tagsParameter.Length) || (onlyThoseTags && count != tagsParameter.Length))
                    {
                        continue;
                    }

                    if (manager.HasTags(false, tagsParameter))
                    {
                        _results.Add(manager.gameObject);
                    }
                }

                return _results.Count > 0 ? _results.ToArray() : null;
            }

            return null;
        }

        /// <summary>
        /// Searches a gameobject with these tags.
        /// </summary>
        /// <param name = "onlyThoseTags">This bool specifies whether the Gameobject should have only those tags</param>
        /// <param name = "tagsParameter">The tags that must have the object</param>
        public static GameObject FindGameObjectWithMultiTags(in bool onlyThoseTags, params string[] tagsParameter)
        {
            _auxArrayGameObject = FindGameObjectsWithMultiTags(onlyThoseTags, tagsParameter);

            return _auxArrayGameObject != null && _auxArrayGameObject.Length >= 1 ? _auxArrayGameObject[0] : null;
        }
        #endregion

        #region Extend Methods
        public static bool HasTags(this Component @this, in bool onlyThoseTags, params string[] tagsParameter)
        {
            return @this && @this.gameObject != null && @this.gameObject.HasTags(onlyThoseTags, tagsParameter);
        }

        public static bool HasTags(this GameObject @this, in bool onlyThoseTags, params string[] tagsParameter)
        {
            if (@this != null && tagsParameter != null)
            {
                if (!@this.TryGetComponent<MultitagsComponent>(out var auxManager))
                {
                    return tagsParameter.Length <= 0;
                }
                else
                {
                    auxManager.ForceAwake();
                    return auxManager.HasTags(onlyThoseTags, tagsParameter);
                }
            }
            return false;
        }



        public static void ClearTags(this Component @this)
        {
            if (@this != null && @this.gameObject != null)
            {
                @this.gameObject.ClearTags();
            }
        }

        public static void ClearTags(this GameObject @this)
        {
            if (@this != null && @this.TryGetComponent<MultitagsComponent>(out var auxManager))
            {
                auxManager.ClearTags();
            }
        }

        /// <summary>
        /// Return all tags in the specific GameObject
        /// </summary>
        /// <param name = "this">The specific gameobject</param> 
        public static List<string> GetTags(this GameObject @this)
        {
            if (@this != null && @this.TryGetComponent<MultitagsComponent>(out var auxManager))
            {
                return auxManager.ListTags;
            }
            return null;
        }

        public static List<string> GetTags(this Component @this)
        {
            if (@this != null && @this.gameObject != null)
            {
                return @this.gameObject.GetTags();
            }
            return null;
        }



        public static string GetTag(this GameObject @this, in int index = 0)
        {
            if (@this != null && @this.TryGetComponent<MultitagsComponent>(out var auxManager))
            {
                return auxManager.GetTag(index);
            }
            return null;
        }

        public static string GetTag(this Component @this, int index = 0)
        {
            if (@this != null && @this.gameObject != null)
            {
                return @this.gameObject.GetTag(index);
            }
            return null;
        }

        /// <summary>
        /// Add these tags in the specific GameObject
        /// </summary>
        /// <param name = "this">The specific gameobject</param>
        /// <param name = "tagsParameter">The tags that must be added to the GameObject</param>
        public static void AddTags(this GameObject @this, params string[] tagsParameter)
        {
            if (@this != null && tagsParameter.IsAlmostSpecificCount())
            {
                MultitagsComponent auxManager = @this.AddOnlyOneComponent<MultitagsComponent>();
                if (auxManager)
                {
                    auxManager.AddTags(tagsParameter);
                }
            }
        }

        public static void AddTags(this GameObject @this, params TagForValue[] tagsParameter)
        {
            if (@this != null && tagsParameter.IsAlmostSpecificCount())
            {
                MultitagsComponent auxManager = @this.AddOnlyOneComponent<MultitagsComponent>();
                if (auxManager)
                {
                    auxManager.AddTags(tagsParameter);
                }
            }
        }

        public static void AddTags(this Component @this, params string[] tagsParameter)
        {
            if (@this != null && @this.gameObject != null)
            {
                @this.gameObject.AddTags(tagsParameter);
            }
        }

        public static void AddTags(this Component @this, params TagForValue[] tagsParameter)
        {
            if (@this != null && @this.gameObject != null)
            {
                @this.gameObject.AddTags(tagsParameter);
            }
        }


        /// <summary>
        /// Remove these tags in the specific GameObject
        /// </summary>
        /// <param name = "this">The specific gameobject</param>
        /// <param name = "tagsParameter">The tags that must be removed to the GameObject</param>
        public static void RemoveTags(this GameObject @this, params string[] tagsParameter)
        {
            if (@this != null && @this.TryGetComponent<MultitagsComponent>(out var auxManager))
            {
                auxManager.RemoveTags(tagsParameter);
            }
        }

        public static void RemoveTags(this Component @this, params string[] tagsParameter)
        {
            if (@this != null && @this.gameObject != null)
            {
                @this.RemoveTags(tagsParameter);
            }
        }



        public static string GetTagValue(this Component @this, string tag)
        {
            if (@this != null && @this.gameObject != null)
            {
                @this.gameObject.GetTagValue(tag);
            }

            return null;
        }

        public static string GetTagValue(this GameObject @this, string tag)
        {
            if (@this != null && @this.TryGetComponent<MultitagsComponent>(out var auxManager))
            {
                return auxManager.GetTagValue(tag);
            }
            return null;
        }

        public static int GetTagIntValue(this GameObject @this, string tag)
        {
            if (@this != null && @this.TryGetComponent<MultitagsComponent>(out var auxManager))
            {
                return auxManager.GetTagIntValue(tag);
            }
            return -1;
        }

        public static int GetTagIntValue(this Component @this, string tag)
        {
            if (@this != null && @this.gameObject != null)
            {
                return @this.gameObject.GetTagIntValue(tag);
            }
            return -1;
        }

        public static float GetTagFloatValue(this GameObject @this, string tag)
        {
            if (@this != null && @this.TryGetComponent<MultitagsComponent>(out var auxManager))
            {
                return auxManager.GetTagFloatValue(tag);
            }
            return -1f;
        }

        public static float GetTagFloatValue(this Component @this, string tag)
        {
            if (@this != null && @this.gameObject != null)
            {
                return @this.gameObject.GetTagFloatValue(tag);
            }
            return -1f;
        }



        public static int GetCountTag(this GameObject @this)
        {
            if (@this != null && @this.TryGetComponent<MultitagsComponent>(out var auxManager))
            {
                return auxManager.GetCountTag();
            }
            return -1;
        }

        public static int GetCountTag(this Component @this)
        {
            if (@this != null && @this.gameObject != null)
            {
                return @this.GetCountTag();
            }
            return -1;
        }
        #endregion
    }
}
