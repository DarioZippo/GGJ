#if NODECANVAS

using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System;
using UnityEngine;

namespace Pearl
{
    [Category("Pearl")]
    public class PearlInvokeReflectionTask : ActionTask<Transform>
    {
        public BBParameter<NameClass> nameClass = default;

        public BBParameter<bool> useStatic = false;
        [Conditional("useStatic", 0)]
        public BBParameter<bool> useAgent = true;
        [Conditional("useAgent", 0), Conditional("useStatic", 0)]
        public BBParameter<string> nameRoot = default;


        public BBParameter<string> nameMethod = default;

        public BBParameter<bool> useParameter = false;
        [Conditional("useParameter", 1)]
        public BBParameter<bool> isNotMethod = false;
        [Conditional("useParameter", 1)]
        public BBParameter<PrimitiveEnum> primitiveStruct = default;


        [Conditional("useParameter", 1), Conditional("primitiveStruct", (int)PrimitiveEnum.Bool)]
        public BBParameter<bool> boolValue = default;
        [Conditional("useParameter", 1), Conditional("primitiveStruct", (int)PrimitiveEnum.Enum)]
        public BBParameter<string> enumValue = default;
        [Conditional("useParameter", 1), Conditional("primitiveStruct", (int)PrimitiveEnum.Float)]
        public BBParameter<float> floatValue = default;
        [Conditional("useParameter", 1), Conditional("primitiveStruct", (int)PrimitiveEnum.Integer)]
        public BBParameter<int> integerValue = default;
        [Conditional("useParameter", 1), Conditional("primitiveStruct", (int)PrimitiveEnum.String)]
        public BBParameter<string> stringValue = default;
        [Conditional("useParameter", 1), Conditional("primitiveStruct", (int)PrimitiveEnum.Vector2)]
        public BBParameter<Vector2> vector2Value = default;
        [Conditional("useParameter", 1), Conditional("primitiveStruct", (int)PrimitiveEnum.Vector3)]
        public BBParameter<Vector3> vector3Value = default;

        protected override void OnExecute()
        {
            GameObject obj;
            if (useStatic.value)
            {
                obj = null;
            }
            else
            {
                if (!useAgent.value)
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
            object container = obj != null ? obj.GetChildInHierarchy(type) : type;
            ReflectionExtend.Setter(container, nameMethod.value, GetParameter());

            EndAction();
        }

        private object GetParameter()
        {
            if (!useParameter.value) return null;

            return primitiveStruct.value switch
            {
                PrimitiveEnum.Bool => boolValue.value,
                PrimitiveEnum.Enum => integerValue.value,
                PrimitiveEnum.Float => floatValue.value,
                PrimitiveEnum.Integer => integerValue.value,
                PrimitiveEnum.Vector2 => vector2Value.value,
                PrimitiveEnum.Vector3 => vector3Value.value,
                PrimitiveEnum.String => stringValue.value,
                _ => null,
            };
        }
    }
}

#endif