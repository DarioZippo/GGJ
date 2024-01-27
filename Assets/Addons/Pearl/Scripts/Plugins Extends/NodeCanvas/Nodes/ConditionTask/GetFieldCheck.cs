#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System;
using UnityEngine;

namespace Pearl.NodeCanvas.Tasks.Conditions
{
    [Category("Pearl")]
    public class GetFieldCheck : ConditionTask
    {
        public BBParameter<NameClass> nameClass = default;
        public BBParameter<PrimitiveEnum> primitiveStruct = default;

        public BBParameter<bool> useStatic = false;
        [Conditional("useStatic", 0)]
        public BBParameter<bool> useAgent = true;
        [Conditional("useStatic", 0), Conditional("useAgent", 0)]
        public BBParameter<string> nameRoot = default;

        public BBParameter<string> nameField = default;

        protected override string info { get { return "[" + nameField.value + "]"; } }

        protected override bool OnCheck()
        {
            if (useAgent == null || nameClass == null || useStatic == null)
            {
                return false;
            }

            GameObject obj;

            if (useStatic.value)
            {
                obj = null;
            }
            else
            {
                if (!useAgent.value && nameRoot != null)
                {
                    string root = nameRoot.value;
                    obj = GameObject.Find(root);
                }
                else
                {
                    obj = agent.gameObject;
                }
            }

            Type type = nameClass.value.Type;
            Component component = obj != null ? obj.GetChildInHierarchy(type) : null;

            bool result;
            if (useStatic.value)
            {
                result = ReflectionExtend.Getter<bool>(type, nameField.value);
            }
            else
            {
                result = ReflectionExtend.Getter<bool>(component, nameField.value);
            }

            return result;
        }
    }
}

#endif