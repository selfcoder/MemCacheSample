using System;
using System.IO;
using System.Text;
using MsgPack.Serialization;

namespace MemCache.Client
{
    internal static class Serializer
    {
        public static byte[] Serialize(object value)
        {
            if (value == null)
                return null;

            var valueType = value.GetType();
            var serializer = MessagePackSerializer.Get(valueType);
            var valueTypeName = Encoding.UTF8.GetBytes(TypeHelper.GetTypeName(valueType));
            using (var stream = new MemoryStream())
            {
                stream.Write(BitConverter.GetBytes((ushort)valueTypeName.Length), 0, 2);
                stream.Write(valueTypeName, 0, valueTypeName.Length);
                serializer.Pack(stream, value);
                return stream.ToArray();
            }
        }

        public static object Deserialize(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return null;

            var typeNameLength = BitConverter.ToUInt16(buffer, 0);
            var typeName = Encoding.UTF8.GetString(buffer, 2, typeNameLength);
            var type = TypeHelper.GetType(typeName);
            var serializer = MessagePackSerializer.Get(type);
            using (var stream = new MemoryStream(buffer))
            {
                stream.Seek(typeNameLength + 2, SeekOrigin.Begin);
                return serializer.Unpack(stream);
            }
        }
    }
}