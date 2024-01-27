#if PEARL_2DSIDE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pearl;
using System;
using Pearl.UI;
using TypeReferences;
using Unity.Collections;

namespace Pearl.Side2D
{
    public class AbilityZone : MonoBehaviour
    {
        [SerializeField, ClassImplements(typeof(CharacterAbility))]
        private ClassTypeReference[] abilities;

        [SerializeField]
        private bool useZonePoint = false;
        [SerializeField, ConditionalField("@useZonePoint")]
        private string tagForPoint = "ZonePoint";

        private Collider2D _collider = null;
        private Dictionary<GameObject, List<IEffectInZone>> _enteredObj = new();

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
        }


        public void Enter(GameObject obj)
        {
            ChangeZone(obj, TriggerType.Enter);
        }

        public void Exit(GameObject obj)
        {
            ChangeZone(obj, TriggerType.Exit);
        }

        public void Stay(GameObject obj)
        {
            ChangeZone(obj, TriggerType.Stay);
        }

        private void ChangeZone(GameObject obj, TriggerType triggerType)
        {
            if (obj == null && _collider != null && _enteredObj != null) return;

            List<IEffectInZone> scripts = new();
            var manager = obj.GetComponent<CharacterManager>();
            if (manager)
            {
                foreach (var abilityContainer in abilities)
                {
                    var ability = manager.FindAbility(abilityContainer);
                    if (ability is IEffectInZone effectInZone)
                    {
                        scripts.Add(effectInZone);
                    }
                }
            }

            if (useZonePoint)
            {
                bool entered = _enteredObj.ContainsKey(obj);
                var zonePoint = obj.FindObjectWithTag("ZonePoint");
                bool zonePointInZone = _collider.bounds.Contains(zonePoint.transform.position);

                if (!zonePoint) return;

                if (entered)
                {
                    if (zonePointInZone) return;
                    _enteredObj.Remove(obj);
                    triggerType = TriggerType.Exit;
                }
                else
                {
                    if (!zonePointInZone) return;
                    _enteredObj.Add(obj, scripts);
                    triggerType = TriggerType.Enter;
                }
            }


            foreach (var script in scripts)
            {
                script.OnZone(triggerType, gameObject);
            }
        }
    }
}

#endif