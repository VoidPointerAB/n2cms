using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver.Builders;
using N2.Collections;
using N2.Persistence.Proxying;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace N2.Persistence.MongoDB
{
    public class ContentSerializer : IBsonSerializer, IBsonIdProvider, IBsonDocumentSerializer
    {
        private BsonClassMap classMap;
        private IBsonSerializer serializer;
        private MongoDatabaseProvider database;
        private IProxyFactory proxies;

        public ContentSerializer(Type type, MongoDatabaseProvider database, IProxyFactory proxies)
        {
            this.database = database;
            classMap = BsonClassMap.LookupClassMap(type);
            serializer = CreateClassMapSerializer(type);
            this.proxies = proxies;
        }

        internal static IBsonSerializer CreateClassMapSerializer(Type type)
        {
            var serializerType = typeof(BsonClassMapSerializer<>).MakeGenericType(type);
            return (IBsonSerializer)Activator.CreateInstance(serializerType, BsonClassMap.LookupClassMap(type));
        }

        public Type ValueType { get { return classMap.ClassType; } }

        public object Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var doc = BsonDocumentSerializer.Instance.Deserialize(context, new BsonDeserializationArgs { NominalType = typeof(BsonDocument) });
            var id = Convert.ToInt32(doc["_id"]);

            string key = MongoIdentityMap.GetKey<ContentItem>(id);

            var item = database.IdentityMap.Get(key);
            var nominalType = args.NominalType ?? classMap.ClassType;
            var type = doc.Contains("_t")
                ? BsonSerializer.LookupActualType(nominalType, doc["_t"].IsBsonArray ? doc["_t"].AsBsonArray.Last() : doc["_t"])
                : classMap.ClassType;
            if (item == null)
            {
                item = BsonClassMap.LookupClassMap(type).CreateInstance();
                database.IdentityMap.Set(key, item);
            }
            else
            {
                if (((ContentItem)item).State != ContentState.None)
                {
                    return item;
                }
            }

            try
            {
                database.IdentityMap.Current = item;
                using (var reader = new BsonDocumentReader(doc))
                {
                    var innerContext = BsonDeserializationContext.CreateRoot(reader);
                    var deserializedItem = GetSerializer(type).Deserialize(innerContext, new BsonDeserializationArgs { NominalType = type });
                    return Inject(deserializedItem);
                }
            }
            finally
            {
                database.IdentityMap.Current = null;
            }
        }

        private IBsonSerializer GetSerializer(Type type)
        {
            return type == classMap.ClassType ? serializer : CreateClassMapSerializer(type);
        }

        private ContentItem Inject(object value)
        {
            var item = (ContentItem)value;

            //SetAncestors(item);

            SetChildren(item);

            proxies.OnLoaded(item);
            return item;
        }

        private void SetChildren(ContentItem item)
        {
            item.Children = new ItemList(() => database.GetCollection<ContentItem>().Find(Query.EQ("AncestralTrail", item.GetTrail())));
        }

        private void SetAncestors(ContentItem item)
        {
            var ids = item.AncestralTrail.Split('/').Where(id => !string.IsNullOrEmpty(id)).Select(id => BsonInt32.Create(int.Parse(id))).ToList();
            if (ids.Count == 0)
                return;

            var parents = database.GetCollection<ContentItem>().Find(Query.In("_id", ids)).OrderByDescending(p => p.AncestralTrail.Length).ToList();
            for (int i = 1; i < parents.Count; i++)
            {
                parents[i - 1].Parent = parents[i];
            }
            item.Parent = parents[0];
        }

        public void Serialize(BsonSerializationContext context, BsonSerializationArgs args, object value)
        {
            proxies.OnSaving(value);
            serializer.Serialize(context, args, value);
        }

        public bool GetDocumentId(object document, out object id, out Type idNominalType, out IIdGenerator idGenerator)
        {
            return ((IBsonIdProvider)serializer).GetDocumentId(document, out id, out idNominalType, out idGenerator);
        }

        public void SetDocumentId(object document, object id)
        {
            ((IBsonIdProvider)serializer).SetDocumentId(document, id);
        }

        public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo serializationInfo)
        {
            return ((IBsonDocumentSerializer)serializer).TryGetMemberSerializationInfo(memberName, out serializationInfo);
        }
    }
}
