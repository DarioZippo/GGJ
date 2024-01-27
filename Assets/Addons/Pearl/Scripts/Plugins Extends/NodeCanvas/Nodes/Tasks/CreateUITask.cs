#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using Pearl.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks
{

    [Category("Pearl")]
    public class CreateUITask : ActionTask
    {
        public BBParameter<bool> useDifferenceAspectRatio = false;

        [Conditional("useDifferenceAspectRatio", 1)]
        public BBParameter<Dictionary<float, GameObject>> aspectRatioDictionary;
        [Conditional("useDifferenceAspectRatio", 0)]
        public BBParameter<GameObject> widget;
        public BBParameter<bool> destroyAtFinishState = false;

        public BBParameter<CanvasTipology> canvasTipology;
        public BBParameter<TypeSibilling> typeSibilling;

        [Conditional("typeSibilling", (int)TypeSibilling.SpecificIndex)]
        public BBParameter<int> positionChild;
        public BBParameter<bool> isDisabled;
        public BBParameter<string> nameElement;
        public BBParameter<string> mode;
        public BBParameter<GameObject> saveIstanceLocation;

        private readonly RangeDictionary<GameObject> _dict = new();

        private Transform _container;

        protected override void OnExecute()
        {
            _dict.Clear();

            if (useDifferenceAspectRatio.value)
            {
                foreach (var pair in aspectRatioDictionary.value)
                {
                    _dict.Add(pair.Key, pair.Value);
                }
            }
            else
            {
                _dict.Add(1, widget.value);
            }


            GameObject prefab = _dict.Find(ScreenExtend.AspectRatio);

            if (prefab)
            {
                _container = CanvasManager.GetWidget(prefab.name, canvasTipology.value);

                if (_container == null)
                {
                    GameObjectExtend.CreateUIlement(prefab, out _container, canvasTipology: canvasTipology.value);
                    if (_container != null && !string.IsNullOrEmpty(nameElement.value))
                    {
                        _container.name = nameElement.value;
                    }
                }

                if (_container != null)
                {
                    saveIstanceLocation.value = _container.gameObject;

                    _container.gameObject.SetActive(!isDisabled.value);

                    int pos = positionChild != null ? positionChild.value : 0;
                    _container.SetSibilling(typeSibilling.value, pos);


                    if (!string.IsNullOrWhiteSpace(mode.value))
                    {
                        var genericPage = _container.GetComponent<GenericPage>();
                        if (genericPage)
                        {
                            genericPage.Mode = mode.value;
                        }
                    }
                }
            }

            if (!destroyAtFinishState.value)
            {
                EndAction();
            }
        }

        protected override void OnStop()
        {
            base.OnStop();

            if (_container != null && destroyAtFinishState.value)
            {
                GameObjectExtend.DestroyExtend(_container.gameObject);
            }
        }
    }
}
#endif
