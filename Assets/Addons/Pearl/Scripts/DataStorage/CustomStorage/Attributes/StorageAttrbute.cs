using System;
using System.IO;

namespace Pearl.Storage
{
    [System.AttributeUsage(System.AttributeTargets.Field |
                   System.AttributeTargets.Property,
                   AllowMultiple = false)]
    public class StorageAttribute : Attribute
    {
        private readonly string _relativePath = string.Empty;
        private readonly string _absolutePath = string.Empty;
        private readonly bool _isSlot = false;
        private readonly bool _isGetter;

        public string RelativePath { get { return _relativePath; } }
        public string AbsolutePath { get { return _absolutePath; } }
        public bool IsSlot { get { return _isSlot; } }
        public bool IsGetter { get { return _isGetter; } }


        public StorageAttribute(string relativePath, bool isSlot = false, bool isCompany = false, bool isGetter = false)
        {
            _absolutePath = StorageManager.PersistentDataPath;
            if (isCompany)
            {
                _absolutePath = Directory.GetParent(_absolutePath).FullName;
            }

            _isSlot = isSlot;
            _relativePath = relativePath;
            _isGetter = isGetter;
        }
    }
}