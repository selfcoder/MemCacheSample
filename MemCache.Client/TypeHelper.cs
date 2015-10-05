using System;

namespace MemCache.Client
{
    internal static class TypeHelper
    {
        public static Type GetType(string typeName)
        {
            return Type.GetType(typeName, true);
        }

        public static string GetTypeName(Type type)
        {
            return Type.GetTypeCode(type) != TypeCode.Object ? type.FullName : type.AssemblyQualifiedName;
        }
    }
}