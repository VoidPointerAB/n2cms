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
    public class ContentItemReferenceSerializer : SerializerBase<ContentItem>
    {
        private MongoDatabaseProvider database;

        public ContentItemReferenceSerializer(MongoDatabaseProvider database)
        {
            this.database = database;
        }

        public override ContentItem Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var id = context.Reader.ReadInt32();
            if (id == 0)
                return null;
            return database.IdentityMap.GetOrCreate(id, (i) => database.GetCollection<ContentItem>().FindOne(Query.EQ("_id", i)));
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ContentItem value)
        {
            if (value == null)
                context.Writer.WriteInt32(0);
            else
                context.Writer.WriteInt32(value.ID);
        }
    }
}
