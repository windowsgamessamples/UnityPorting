using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LegacySystem
{
    [Flags]
    public enum BindingFlags
    {
        Default,
        Public,
        Instance,
        InvokeMethod,
        NonPublic,
        Static,
        FlattenHierarchy,
        DeclaredOnly
    }

    public static class ReflectionExtensions
    {

        public static bool IsClass(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

        public static bool IsEnum(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().IsEnum;
#else
            return type.IsEnum;
#endif
        }

        public static Type GetInterface(this Type type, string name)
        {
#if NETFX_CORE
            return type.GetTypeInfo().ImplementedInterfaces.FirstOrDefault(t => t.Name == name);
#else
            throw new NotImplementedException();
#endif
        }

        public static PropertyInfo[] GetProperties(this Type type)
        {
#if NETFX_CORE
            return type.GetTypeInfo().DeclaredProperties != null ? type.GetTypeInfo().DeclaredProperties.ToArray() : null;
#else
            throw new NotImplementedException();
#endif
        }

        public static MethodInfo[] GetMethods(this Type t, BindingFlags flags)
        {
#if NETFX_CORE
            if (!flags.HasFlag(BindingFlags.Instance) && !flags.HasFlag(BindingFlags.Static)) return null;

            var ti = t.GetTypeInfo();
            var allMethods = ti.DeclaredMethods;
            var resultList = new List<MethodInfo>();
            foreach (var method in allMethods)
            {
                var isValid = (flags.HasFlag(BindingFlags.Public) && method.IsPublic)
                    || (flags.HasFlag(BindingFlags.NonPublic) && !method.IsPublic);
                isValid &= (flags.HasFlag(BindingFlags.Static) && method.IsStatic) || (flags.HasFlag(BindingFlags.Instance) && !method.IsStatic);
                if (flags.HasFlag(BindingFlags.DeclaredOnly))
                    isValid &= method.DeclaringType == t;

                if (isValid)
                    resultList.Add(method);
            }
            return resultList.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static MemberInfo[] GetMembers(this Type t, BindingFlags flags)
        {
#if NETFX_CORE
            if (!flags.HasFlag(BindingFlags.Instance) && !flags.HasFlag(BindingFlags.Static)) return null;

            var ti = t.GetTypeInfo();
            var result = new List<MemberInfo>();
            result.AddRange(ti.DeclaredMembers);
            return result.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static Object InvokeMember(this Type t, string name, BindingFlags flags, object binder, object target, object[] args)
        {
#if NETFX_CORE
            if (binder != null || target != null)
                throw new ArgumentException("doesn't support binder or target when invoking");
            // We only support invoking a normal method, not a field/property/other member
            var ti = t.GetTypeInfo();
            foreach (var m in ti.DeclaredMethods)
            {
                if (m.Name.Equals(name))
                {
                    return m.Invoke(t, args);
                }
            }
            return null;
#else
            throw new NotImplementedException();
#endif
        }

        public static FieldInfo[] GetFields(this Type type)
        {
            return GetFields(type, BindingFlags.Default);
        }

        public static FieldInfo[] GetFields(this Type t, BindingFlags flags)
        {
#if NETFX_CORE
            if (!flags.HasFlag(BindingFlags.Instance) && !flags.HasFlag(BindingFlags.Static)) return null;

            var ti = t.GetTypeInfo();
            var origFields = ti.DeclaredFields;
            var results = new List<FieldInfo>();
            foreach (var field in origFields)
            {
                var isValid = (flags.HasFlag(BindingFlags.Public) && field.IsPublic)
                    || (flags.HasFlag(BindingFlags.NonPublic) && !field.IsPublic);
                isValid &= (flags.HasFlag(BindingFlags.Static) && field.IsStatic) || (flags.HasFlag(BindingFlags.Instance) && !field.IsStatic);
                if (flags.HasFlag(BindingFlags.DeclaredOnly))
                    isValid &= field.DeclaringType == t;

                results.Add(field);
            }
            return results.ToArray();
#else
            throw new NotImplementedException();
#endif
        }

        public static MethodInfo GetMethod(this Type type, string name)
        {
            return GetMethod(type, name, BindingFlags.Default, null);
        }

        public static MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            return GetMethod(type, name, BindingFlags.Default, types);
        }

        public static MethodInfo GetMethod(this Type t, string name, BindingFlags flags)
        {
            return GetMethod(t, name, flags, null);
        }

        public static MethodInfo GetMethod(Type t, string name, BindingFlags flags, Type[] parameters)
        {
#if NETFX_CORE
            var ti = t.GetTypeInfo();
            var methods = ti.GetDeclaredMethods(name);
            foreach (var m in methods)
            {
                var plist = m.GetParameters();
                bool match = true;
                foreach (var param in plist)
                {
                    bool valid = true;
                    if (parameters != null)
                    {
                        foreach (var ptype in parameters)
                            valid &= ptype == param.ParameterType;
                    }
                    match &= valid;
                }
                if (match)
                    return m;
            }
            return null;
#else
            throw new NotImplementedException();
#endif

        }

        public static Type[] GetGenericArguments(this Type t)
        {
#if NETFX_CORE
            var ti = t.GetTypeInfo();
            return ti.GenericTypeArguments;
#else
            throw new NotImplementedException();
#endif
        }

        public static bool IsAssignableFrom(this Type current, Type toCompare)
        {
#if NETFX_CORE
            return current.GetTypeInfo().IsAssignableFrom(toCompare.GetTypeInfo());
#else
            throw new NotImplementedException();
#endif
        }

        public static bool IsPrimitive(this Type current)
        {
            if (current == typeof(Boolean)) return true;
            if (current == typeof(Byte)) return true;
            if (current == typeof(SByte)) return true;
            if (current == typeof(Int16)) return true;
            if (current == typeof(UInt16)) return true;
            if (current == typeof(Int32)) return true;
            if (current == typeof(UInt32)) return true;
            if (current == typeof(Int64)) return true;
            if (current == typeof(UInt64)) return true;
            if (current == typeof(IntPtr)) return true;
            if (current == typeof(UIntPtr)) return true;
            if (current == typeof(Char)) return true;
            if (current == typeof(Double)) return true;
            if (current == typeof(Single)) return true;
            return false;
        }

        /**
         * Missing IsSubclassOf, this works well
         */
        public static bool IsSubclassOf(this Type type, System.Type parent)
        {
#if NETFX_CORE
            return parent.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
#else
            throw new NotImplementedException();
#endif
        }

    }
}