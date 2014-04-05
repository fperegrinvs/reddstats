namespace ReddStats.Practices
{
    using System.IO;

    using ProtoBuf.Meta;

    /// <summary>
    /// Extensal para protobuf-net
    /// </summary>
    public static class ProtobufExtensions
    {
        /// <summary>
        /// Serializa um elemento
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="element">
        /// elemento a ser serializado
        /// </param>
        /// <returns>
        /// resultado da serialização
        /// </returns>
        public static byte[] Serialize<T>(this TypeModel model, T element)
        {
            using (var ms = new MemoryStream())
            {
                model.Serialize(ms, element);
                var bytes = ms.ToArray();
                return bytes;
            }
        }

        /// <summary>
        /// Serializa elemento para protobuf
        /// </summary>
        /// <typeparam name="T">tipo do elemento</typeparam>
        /// <param name="element">elemento a ser serializado</param>
        /// <returns>elemento serializado</returns>
        public static byte[] SerializeProtobuf<T>(this T element)
        {
            using (var ms = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(ms, element);
                var bytes = ms.ToArray();
                return bytes;
            }
        }

        /// <summary>
        /// Deserializa um elemento
        /// </summary>
        /// <typeparam name="T">
        /// tipo de objeto
        /// </typeparam>
        /// <param name="content">
        /// conteúdo a ser serializado
        /// </param>
        /// <returns>
        /// objeto deserializado
        /// </returns>
        public static T DeSerializeProtobuf<T>(this byte[] content)
        {
            using (var ms = new MemoryStream(content))
            {
                var item = ProtoBuf.Serializer.Deserialize<T>(ms);
                return item;
            }
        }

        /// <summary>
        /// Deserializa um elemento
        /// </summary>
        /// <param name="model">classe com o serializador</param>
        /// <param name="content">conteúdo a ser serializado</param>
        /// <returns>objeto deserializado</returns>
        public static T DeSerialize<T>(this TypeModel model, byte[] content)
        {
            using (var ms = new MemoryStream(content))
            {
                var item = (T)model.Deserialize(ms, null, typeof(T));
                return item;
            }
        }
    }
}
