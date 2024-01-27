using Pearl.Debugging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace Pearl
{
    public enum MemberEnum { Field, Property, Method, Unkown }

    #region Struct
    [Serializable]
    public struct NameClass
    {
        public string classString;
        public string namespaceString;
        public string assembly;

        public NameClass(string classString, string namespaceString, string assembly)
        {
            this.namespaceString = namespaceString;
            this.classString = classString;
            this.assembly = assembly;
        }

        public string FullNameClass
        {
            get
            {
                return namespaceString != "" ? namespaceString + "." + classString : classString;
            }
        }

        public Type Type
        {
            get
            {
                string fullName = FullNameClass;
                string result = assembly != null && assembly != "" ? fullName + ", " + assembly : fullName;
                return Type.GetType(result);
            }
        }
    }

    public class MemberComplexInfo
    {
        public MemberInfo memberInfo;
        public object container;
        public bool isArray;
        public int index = 0;

        public MemberComplexInfo(MemberInfo memberInfo, object container)
        {
            this.memberInfo = memberInfo;
            this.container = container;
        }

        public MemberComplexInfo(MemberInfo memberInfo, object container, bool isArray, int index)
        {
            this.memberInfo = memberInfo;
            this.container = container;
            this.isArray = isArray;
            this.index = index;
        }

        public object GetValue(params object[] paramaters)
        {
            if (memberInfo != null)
            {
                if (isArray)
                {
                    var result = memberInfo.Getter(container);
                    if (result is IEnumerable enumerable)
                    {
                        int i = 0;
                        foreach (object element in enumerable)
                        {
                            if (i == index)
                            {
                                return element;
                            }
                            i++;
                        }
                    }
                    return null;
                }
                else
                {
                    return memberInfo.Getter(container, paramaters);
                }
            }
            return null;
        }

        public T GetValue<T>(params object[] paramaters)
        {
            object obj = GetValue(paramaters);
            if (obj != null && obj is T result)
            {
                return result;
            }

            return default;
        }

        public void SetValue(object newValue)
        {
            ReflectionExtend.SetterMemberInfo(memberInfo, container, newValue);
        }
    }

    #endregion

    //Metodi di utilità della reflection.
    public static class ReflectionExtend
    {
        public const BindingFlags FLAGS_ALL = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
        public const BindingFlags FLAGS_ALL_DECLARED = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;


        #region Utility
        public static Type GetType(string typeString, string assemblyName = null)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            Type result;
            foreach (var assembly in assemblies)
            {
                if (assemblyName == null || assembly.GetName().Name.Contains(assemblyName, StringComparison.CurrentCultureIgnoreCase))
                {
                    result = assembly.GetType(typeString);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public static Type GetOutputType(this MemberInfo @this)
        {
            if (@this is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }
            if (@this is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType;
            }
            if (@this is MethodInfo methodInfo)
            {
                return methodInfo.ReturnType;
            }

            Debug.LogWarning("the member isn't support");
            return null;
        }

        public static MemberInfo[] GetPropertiesAndFields(this Type type, BindingFlags bindingFlags = FLAGS_ALL)
        {
            List<MemberInfo> memberInfos = new();

            if (type == null)
            {
                return null;
            }

            memberInfos.AddRange(type.GetFields(bindingFlags));
            memberInfos.AddRange(type.GetProperties(bindingFlags));

            return memberInfos.ToArray();
        }

        public static Type[] FindAllDerivedTypes<T>(bool useSoloThisAssembly = false)
        {
            if (useSoloThisAssembly)
            {
                return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
            }
            else
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                List<Type> result = new();

                foreach (var assembly in assemblies)
                {
                    result.AddRange(FindAllDerivedTypes<T>(assembly));
                }

                return result.ToArray();
            }
        }

        public static Type[] FindAllDerivedTypes<T>(Assembly assembly)
        {
            var derivedType = typeof(T);
            return assembly.GetTypes().Where(t => t != derivedType && derivedType.IsAssignableFrom(t)).ToArray();

        }
        #endregion

        #region Getter
        public static T Getter<T>(object container, string memberName, params object[] paramaters)
        {
            if (container == null) return default;

            Type dynamicType;
            if (container is Type type)
            {
                dynamicType = type;
                container = null;
            }
            else
            {
                dynamicType = container.GetType();
            }

            return Getter<T>(container, memberName, dynamicType, paramaters);
        }

        public static T Getter<T>(string memberName, Type type, params object[] paramaters)
        {
            return Getter<T>(null, memberName, type, paramaters);
        }

        private static T Getter<T>(object container, string memberName, Type type, params object[] paramaters)
        {
            var members = type.GetMember(memberName, FLAGS_ALL);

            foreach (var member in members)
            {
                return (T)Getter(member, container, paramaters);
            }

            Debug.LogWarning("the member isn't support");
            return default;
        }

        public static object Getter(this MemberInfo @this, object container, params object[] paramaters)
        {
            try
            {
                if (@this is FieldInfo fieldInfo)
                {
                    return fieldInfo.GetValue(container);
                }
                else if (@this is PropertyInfo propertyInfo)
                {
                    return propertyInfo.GetValue(container);
                }
                else if (@this is MethodInfo methodInfo)
                {
                    return methodInfo.Invoke(container, paramaters);
                }
            }
            catch (ArgumentException e)
            {
                LogManager.LogWarning(e);
                return null;
            }

            return null;
        }

        public static T Getter<T, F>(params string[] fieldsName)
        {
            return Getter<T>(typeof(F), fieldsName);
        }

        public static T Getter<T>(Type type, params string[] fieldsName)
        {
            MemberComplexInfo membrInfo = GetValueInfo(type, fieldsName);
            return membrInfo.GetValue<T>();
        }

        public static MemberComplexInfo GetValueInfo<T>(params string[] fieldsName)
        {
            return GetValueInfo(typeof(T), fieldsName);
        }

        public static MemberComplexInfo GetValueInfo(Type type, params string[] fieldsName)
        {
            object container;
            if (type.IsSubClass(typeof(UnityEngine.Object)))
            {
                container = GameObject.FindFirstObjectByType(type);
            }
            else
            {
                container = type;
            }

            return GetValueInfo(container, fieldsName);
        }

        public static MemberComplexInfo GetValueInfo(object container, params string[] fieldsName)
        {
            if (!fieldsName.IsAlmostSpecificCount()) return default;

            for (int i = 0; i < fieldsName.Length; i++)
            {
                string fieldName = fieldsName[i];
                string[] aux = fieldName.Split('[');

                if (aux == null) continue;

                if (aux.Length <= 1)
                {
                    if (i == fieldsName.Length - 1)
                    {
                        if (GetMemberInfo(container, fieldName, out MemberInfo memberInfo))
                        {
                            return new MemberComplexInfo(memberInfo, container);
                        }
                        break;
                    }
                    else
                    {
                        container = Getter<object>(container, fieldName);
                        if (container == null) break;
                    }
                }
                else
                {
                    string nameArray = aux[0];
                    string indexString = aux[1].Replace("]", "");
                    int index = -1;

                    try
                    {
                        index = Int32.Parse(indexString);
                    }
                    catch (Exception)
                    {
                    }

                    if (index != -1)
                    {
                        if (i == fieldsName.Length - 1)
                        {
                            GetMemberInfo(container, nameArray, out MemberInfo memberInfo);
                            return new MemberComplexInfo(memberInfo, container, true, index);

                        }
                        else
                        {
                            ReflectionExtend.GetCollectionsElement(container, nameArray, index, out container);
                        }
                    }
                }
            }

            LogManager.LogWarning("The path is wrong");
            return default;
        }

        public static bool GetMemberInfo(object container, string name, out MemberInfo result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            bool isGet = GetFieldInfo(container, name, out FieldInfo fieldInfo, bindingFlags);
            result = fieldInfo;
            if (!isGet)
            {
                isGet = GetPropertyInfo(container, name, out PropertyInfo propertyInfo, bindingFlags);
                result = propertyInfo;
                if (!isGet)
                {
                    isGet = GetMethodInfo(container, name, out MethodInfo methodInfo, bindingFlags);
                    result = methodInfo;
                }
            }

            return isGet;
        }

        public static void GetCollectionsElement(object container, string name, int index, out object result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            result = ReflectionExtend.Getter<object>(container, name);
            if (result != null)
            {
                if (result is IEnumerable enumerable)
                {
                    int i = 0;
                    foreach (object element in enumerable)
                    {
                        if (i == index)
                        {
                            result = element;
                            break;
                        }
                        i++;
                    }
                }
            }
        }

        #endregion

        #region Setter
        public static void Setter<T, Type>(T value, params string[] fieldsName)
        {
            Setter<T>(typeof(Type), value, fieldsName);
        }

        public static void Setter<T>(Type type, T value, params string[] fieldsName)
        {
            MemberComplexInfo member = GetValueInfo(type, fieldsName);
            member?.SetValue(value);
        }

        public static bool SetterField(object container, string name, object newValue, BindingFlags bindingFlags = FLAGS_ALL)
        {
            GetFieldInfo(container, name, out FieldInfo fieldInfo, bindingFlags);

            if (fieldInfo != null)
            {
                fieldInfo.SetValue(container, newValue);
                return true;
            }

            return false;
        }

        public static bool SetterProperty(object container, string name, object newValue, BindingFlags bindingFlags = FLAGS_ALL)
        {
            GetPropertyInfo(container, name, out PropertyInfo fieldInfo, bindingFlags);

            if (fieldInfo != null && fieldInfo.CanWrite)
            {
                fieldInfo.SetValue(container, newValue);
                return true;
            }

            return false;
        }

        public static void SetterMemberInfo(this MemberInfo memberInfo, object container, object newValue)
        {
            if (memberInfo != null)
            {
                if (memberInfo is FieldInfo fieldInfo)
                {
                    fieldInfo.SetValue(container, newValue);
                }
                else if (memberInfo is PropertyInfo propertyInfo && propertyInfo.CanWrite)
                {
                    propertyInfo.SetValue(container, newValue);
                }
                else if (memberInfo is MethodInfo methidInfo)
                {
                    methidInfo.Invoke(container, newValue);
                }
            }
        }

        public static bool Setter(object container, string fieldName, object newValue, BindingFlags bindingFlags = FLAGS_ALL)
        {
            bool isSet = SetterField(container, fieldName, newValue, bindingFlags);
            if (!isSet)
            {
                isSet = SetterProperty(container, fieldName, newValue, bindingFlags);
            }
            if (!isSet)
            {
                Invoke(container, fieldName, newValue);
            }

            return isSet;
        }
        #endregion

        #region GetField
        public static bool GetField<T>(object container, string name, out T result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            if (container != null && name != null)
            {
                GetFieldPrivate(container, name, out object var, bindingFlags);
                if (var != null && var is T tVar)
                {
                    result = tVar;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static bool GetField<T>(Type type, string name, out T result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            if (type != null && name != null)
            {
                var info = type.GetField(name, bindingFlags);
                result = (T)info.GetValue(null);

                if (result != null && result is T tVar)
                {
                    result = tVar;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static bool GetField<Type>(string name, out object result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            object container = GameObject.FindFirstObjectByType(typeof(Type));

            return GetFieldPrivate(container, name, out result, bindingFlags);
        }

        public static bool GetField<Result, Type>(string name, out Result result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            GetField<Type>(name, out object var, bindingFlags);
            if (var != null && var is Result tVar)
            {
                result = tVar;
                return true;
            }
            result = default;
            return false;
        }

        private static bool GetFieldPrivate(object container, string name, out object result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            result = null;
            if (GetFieldInfo(container, name, out FieldInfo fieldInfo, bindingFlags))
            {
                result = fieldInfo.GetValue(container);
                return true;
            }
            return false;
        }

        private static bool GetFieldInfo(object container, string name, out FieldInfo result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            result = null;
            if (container != null && name != null)
            {
                Type type = (container is Type aux) ? aux : container.GetType();
                while ((result = type.GetField(name, bindingFlags)) == null && (type = type.BaseType) != null) ;

                if (result != null)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region GetProperty
        public static bool GetProperty(object container, string name, out object result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            result = null;
            if (GetPropertyInfo(container, name, out PropertyInfo propertyInfo, bindingFlags))
            {
                result = propertyInfo.GetValue(container);
                return true;
            }
            return false;
        }

        public static bool GetProperty<T>(object container, string name, out T result) where T : UnityEngine.Object
        {
            if (container != null && name != null)
            {
                GetProperty(container, name, out object var);
                if (var != null && var is T tVar)
                {
                    result = tVar;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static bool GetProperty<T>(Type type, string name, out T result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            if (type != null && name != null)
            {
                var info = type.GetProperty(name, bindingFlags);
                result = (T)info.GetValue(null);

                if (result != null && result is T tVar)
                {
                    result = tVar;
                    return true;
                }
            }
            result = default;
            return false;
        }

        public static bool GetProperty<Type>(string name, out object result)
        {
            object container = GameObject.FindFirstObjectByType(typeof(Type));

            return GetProperty(container, name, out result);
        }

        public static bool GetProperty<Result, Type>(string name, out Result result)
        {
            GetProperty<Type>(name, out object var);
            if (var != null && var is Result tVar)
            {
                result = tVar;
                return true;
            }
            result = default;
            return false;
        }

        public static bool GetPropertyInfo(object container, string name, out PropertyInfo result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            result = null;
            if (container != null && name != null)
            {
                Type type = container.GetType();

                while ((result = type.GetProperty(name, bindingFlags)) == null && (type = type.BaseType) != null) ;

                if (result != null && result.CanRead)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region GetMethod
        public static bool GetMethodInfo(object container, string name, out MethodInfo result, BindingFlags bindingFlags = FLAGS_ALL)
        {
            result = null;
            if (container != null && name != null)
            {
                Type type = container.GetType();

                while ((result = type.GetMethod(name, bindingFlags)) == null && (type = type.BaseType) != null) ;

                if (result != null)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region Attribute
        public static MemberInfo[] GetCustomAttributes<Attr>(this Type type, string memberName, bool inherit = false, BindingFlags bindings = FLAGS_ALL) where Attr : Attribute
        {
            MemberInfo[] memberInfos = type.GetMember(memberName, bindings);
            return GetCustomAttributes<Attr>(memberInfos, inherit);
        }

        public static MemberInfo[] GetPropertiesAndFieldsWithAttribute<Attr>(this Type type, bool inherit = false, BindingFlags bindingFlags = FLAGS_ALL) where Attr : Attribute
        {
            MemberInfo[] memberInfos = GetPropertiesAndFields(type, bindingFlags);
            return GetCustomAttributes<Attr>(memberInfos, inherit);
        }

        public static MemberInfo[] GetCustomAttributes<Attr>(MemberInfo[] memberInfos, bool inherit = false) where Attr : Attribute
        {
            List<MemberInfo> result = new();
            foreach (var memberInfo in memberInfos)
            {
                if (memberInfo.GetCustomAttribute(typeof(Attr), inherit) is Attr)
                {
                    result.Add(memberInfo);
                }
            }

            if (result.IsAlmostSpecificCount())
            {
                return result.ToArray();
            }
            return null;
        }

        public static Result GetValueAttribute<Attr, Result>(MemberInfo member, Func<Attr, Result> func) where Attr : System.Attribute
        {
            if (member != null)
            {
                System.Attribute[] attrs = System.Attribute.GetCustomAttributes(member);

                foreach (System.Attribute attr in attrs)
                {
                    if (attr is Attr specificAttr)
                    {
                        return func(specificAttr);
                    }
                }
            }

            Debug.LogWarning("Wrong");
            return default;
        }
        #endregion

        #region Methods
        public static void Invoke(this MethodInfo methodInfo, object container, object newValue)
        {
            methodInfo?.Invoke(container, new object[] { newValue });
        }

        public static bool Invoke(object container, string nameMethod, params object[] parameters)
        {
            if (container == null) return false;

            if (container is Type type)
            {
                return Invoke(null, type, nameMethod, parameters);
            }
            else
            {
                return Invoke(container, container.GetType(), nameMethod, parameters);
            }
        }

        public static bool Invoke(Type type, string nameMethod, params object[] parameters)
        {
            return Invoke(null, type, nameMethod, parameters);
        }

        private static bool Invoke(object container, Type type, string nameMethod, params object[] parameters)
        {
            var specificMethods = GetMethods(type, nameMethod);
            if (specificMethods == null) return false;

            if (parameters.Length == 1 && parameters[0] == null)
            {
                parameters = null;
            }

            foreach (var methodInfo in specificMethods)
            {
                try
                {
                    methodInfo.Invoke(container, parameters);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }

            return false;
        }

        public static bool Invoke<T>(object container, string nameMethod, out T result, params object[] parameters)
        {
            result = default;
            if (container == null) return false;

            return Invoke<T>(container, container.GetType(), nameMethod, out result, parameters);
        }

        public static bool Invoke<T>(Type type, string nameMethod, out T result, params object[] parameters)
        {
            return Invoke<T>(null, type, nameMethod, out result, parameters);
        }

        private static bool Invoke<T>(object container, Type type, string nameMethod, out T result, params object[] parameters)
        {
            result = default;
            var specificMethods = GetMethods(type, nameMethod);

            if (specificMethods == null) return false;

            foreach (var methodInfo in specificMethods)
            {
                try
                {
                    result = (T)methodInfo.Invoke(container, parameters);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning(e);
                }
            }
            return false;
        }

        public static bool IsAction(in MethodInfo methodInfo)
        {
            return methodInfo != null && methodInfo.ReturnType.Equals((typeof(void)));
        }

        #endregion

        #region Create Delegate
        public static Delegate CreateDelegate(Type type, in string nameAction, params Type[] parameters)
        {
            return CreateDelegate(null, type, nameAction, parameters);
        }

        public static Delegate CreateDelegate(in object target, in string nameAction, params Type[] parameters)
        {
            if (target == null) return null;

            return CreateDelegate(target, target.GetType(), nameAction, parameters);
        }

        public static Delegate CreateDelegate(in object target, Type type, in string nameAction, params Type[] parameters)
        {
            MethodInfo methodInfo = null;

            while (methodInfo == null && type != null)
            {
                methodInfo = type.GetMethod(nameAction, FLAGS_ALL, null, parameters, null);
                type = type.BaseType;
            }

            return CreateDelegate(methodInfo, target);
        }

        public static Delegate CreateDelegate(this MethodInfo methodInfo, object target = null)
        {
            if (methodInfo == null || target == null) return null;

            Func<Type[], Type> getType;
            var isAction = IsAction(methodInfo);
            var types = methodInfo.GetParameters().Select(p => p.ParameterType);

            if (isAction)
            {
                getType = Expression.GetActionType;
            }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] { methodInfo.ReturnType });
            }

            if (methodInfo.IsStatic)
            {
                return Delegate.CreateDelegate(getType(types.ToArray()), methodInfo);
            }

            return Delegate.CreateDelegate(getType(types.ToArray()), target, methodInfo.Name);
        }
        #endregion

        #region ChangeEventHandler
        public static void ChangeEventHandler<T>(object target, string nameEvent, Action<T> action, ActionEvent actionEvent, BindingFlags bindingFlags = FLAGS_ALL)
        {
            ChangeDelegateEventHandler(target, nameEvent, action, actionEvent, bindingFlags);
        }

        public static void ChangeEventHandler(object target, string nameEvent, Action action, ActionEvent actionEvent, BindingFlags bindingFlags = FLAGS_ALL)
        {
            ChangeDelegateEventHandler(target, nameEvent, action, actionEvent, bindingFlags);
        }

        public static void ChangeDelegateEventHandler(object target, string nameEvent, Delegate action, ActionEvent actionEvent, BindingFlags bindingFlags = FLAGS_ALL)
        {
            if (target == null || action == null || nameEvent == null) return;

            Type t = target.GetType();
            EventInfo eventInfo = t.GetEvent(nameEvent, bindingFlags);
            if (eventInfo != null)
            {
                switch (actionEvent)
                {
                    case ActionEvent.Add:
                        eventInfo.AddEventHandler(target, action);
                        break;
                    case ActionEvent.Remove:
                        eventInfo.RemoveEventHandler(target, action);
                        break;
                }
            }
        }
        #endregion

        #region CreateInstance
        public static T CreateInstance<T>(NameClass nameClass, params object[] vars) where T : class
        {
            return CreateInstance<T>(nameClass.Type, vars);
        }

        public static T CreateInstance<T>(params object[] vars) where T : class
        {
            return CreateInstance<T>(typeof(T), vars);
        }

        public static T CreateInstance<T>(Type typeClass, params object[] vars) where T : class
        {
            if (typeClass != null)
            {
                object obj = null;
                if (vars == null || vars.Length <= 0)
                {
                    try
                    {
                        obj = Activator.CreateInstance(typeClass, true);
                    }
                    catch (Exception e)
                    {
                        Debug.LogWarning(e);
                    }
                }
                else
                {
                    var constructors = (typeof(T)).GetConstructors(FLAGS_ALL);
                    foreach (var cons in constructors)
                    {
                        try
                        {
                            obj = cons.Invoke(vars);
                            break;
                        }
                        catch
                        {
                            continue;
                        }
                    }

                    if (obj == null)
                    {
                        Debug.LogWarning("Not Exist the specific constructor");
                    }
                }

                Type otherType = typeof(T);
                if (obj != null && (typeClass == otherType || typeClass.IsSubclassOf(otherType)))
                {
                    return (T)obj;
                }
            }

            return null;
        }

        public static T[] CreateDerivedInstances<T>(params object[] vars) where T : class
        {
            Type[] types = FindAllDerivedTypes<T>();
            T[] result = new T[types.Length];

            for (int i = 0; i < types.Length; i++)
            {
                result[i] = CreateInstance<T>(types[i], vars);
            }

            return result;
        }
        #endregion

        #region Private Methods
        private static MethodInfo[] GetMethods(Type type, string nameMethod, BindingFlags bindingFlags = FLAGS_ALL)
        {
            MethodInfo[] methodsInfo = type.GetMethods(bindingFlags);
            return methodsInfo.FilterArray((x) => x.Name == nameMethod);
        }
        #endregion
    }
}
