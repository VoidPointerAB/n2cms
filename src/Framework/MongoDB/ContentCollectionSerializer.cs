using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using N2.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
    public class ContentCollectionSerializer<T> : SerializerBase<ContentList<T>>, IBsonArraySerializer
        where T : class, INameable
    {
        private readonly EnumerableInterfaceImplementerSerializer<ContentList<T>, T> serializer =
            new EnumerableInterfaceImplementerSerializer<ContentList<T>, T>();

        public override ContentList<T> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            return serializer.Deserialize(context, args);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ContentList<T> value)
        {
            serializer.Serialize(context, args, value);
        }

        public bool TryGetItemSerializationInfo(out BsonSerializationInfo serializationInfo)
        {
            return serializer.TryGetItemSerializationInfo(out serializationInfo);
        }
    }
}
