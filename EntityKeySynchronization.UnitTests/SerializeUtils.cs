using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntyTea.EntityKeySynchronization.UnitTests
{
    internal static class SerializeUtils
    {
        public static T Clone<T>(T entity)
            where T : class
        {
            return Deserialize<T>(Serialize(entity));
        }

        public static byte[] Serialize(object o)
        {
            MemoryStream memoryStream;
            using (memoryStream = new MemoryStream())
            {
                new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Serialize(memoryStream, o);
            }
            return memoryStream.ToArray();
        }

        public static T Deserialize<T>(byte[] bytes)
            where T : class
        {
            T result;
            using (var memoryStream = new MemoryStream(bytes))
            {
                result = (T)new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter().Deserialize(memoryStream);
            }
            return result;
        }
    }
}
