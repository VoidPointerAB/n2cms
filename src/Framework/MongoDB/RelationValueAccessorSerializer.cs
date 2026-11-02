using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
    public class RelationValueAccessorSerializer<T> : SerializerBase<Func<object, T>>
    {
        private ObjectSerializer serializer = new ObjectSerializer();
        private MongoDatabaseProvider databaseProvider;

        public RelationValueAccessorSerializer(MongoDatabaseProvider databaseProvider)
        {
            this.databaseProvider = databaseProvider;
        }

        private T Get(object id)
        {
            return databaseProvider.GetCollection<T>().FindOne(Query.EQ("_id", (int)id));
        }

        public override Func<object, T> Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            serializer.Deserialize(context, new BsonDeserializationArgs { NominalType = typeof(object) });
            return Get;
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Func<object, T> value)
        {
            serializer.Serialize(context, new BsonSerializationArgs { NominalType = typeof(object) }, null);
        }
    }
}
