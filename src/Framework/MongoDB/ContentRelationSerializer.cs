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
    public class ContentRelationSerializer : SerializerBase<ContentRelation>
    {
        private MongoDatabaseProvider database;

        public ContentRelationSerializer(MongoDatabaseProvider database)
        {
            this.database = database;
        }

        public override ContentRelation Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var id = context.Reader.ReadInt32();
            if (id == 0)
                return ContentRelation.Empty;
            
            return new ContentRelation(id, Get);
        }

        private ContentItem Get(object id)
        {
            return database.IdentityMap.GetOrCreate(id, (i) => database.GetCollection<ContentItem>().FindOne(Query.EQ("_id", (int)i)));
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, ContentRelation value)
        {
            if (value == null || !value.ID.HasValue || value.ID.Value == 0)
                context.Writer.WriteInt32(0);
            else
                context.Writer.WriteInt32(value.ID.Value);
        }
    }
}
