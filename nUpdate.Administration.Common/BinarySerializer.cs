// Author: Dominic Beger (Trade/ProgTrade) 2017

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace nUpdate.Administration.Common
{
    internal static class BinarySerializer
    {
        /// <summary>
        ///     Deserializes the specified graph.
        /// </summary>
        public static object Deserialize(byte[] graph)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream(graph);
            return bf.Deserialize(ms);
        }

        /// <summary>
        ///     Deserializes the specified graph.
        /// </summary>
        /// <exception cref="System.InvalidCastException"></exception>
        public static T DeserializeType<T>(byte[] graph)
            where T : class
        {
            return (T) Deserialize(graph);
        }

        /// <summary>
        ///     Serializes the specified object.
        /// </summary>
        public static byte[] Serialize(object obj)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }
}