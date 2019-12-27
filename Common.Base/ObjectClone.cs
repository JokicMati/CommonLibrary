using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Common.Base
{
    public static class ObjectClone
    {
        public static T DeepthCloneByBinaryFormatter<T>(T obj)
        {
            var ret = default(T);

            if (object.ReferenceEquals(obj, null) == false)
            {
                var formatter = new BinaryFormatter();
                using (var stream = new MemoryStream())
                {
                    formatter.Serialize(stream, obj);
                    stream.Seek(0, SeekOrigin.Begin);
                    ret = (T)formatter.Deserialize(stream);
                }
            }

            return ret;
        }
    }
}
