using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace MyHub.Tools
{
    public static class JsonHelper
    {
        public static object Deserialize(Stream stream, Type type)
        {
            var serializer = new DataContractJsonSerializer(type);
            return serializer.ReadObject(stream);
        }

        public static string Serialize(Type type, object obj)
        {
            var result = string.Empty;
            var serializer = new DataContractJsonSerializer(type);
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                result = Encoding.UTF8.GetString(stream.ToArray(), 0, stream.ToArray().Length);
            }
            return result;
        }
    }
}
