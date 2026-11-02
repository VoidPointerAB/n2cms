using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoDB.Driver;
using N2.Engine;
using N2.Persistence.NH;

namespace N2.Persistence.MongoDB
{
    [Service]
    [Service(typeof(IContentItemRepository), Configuration = "mongo")]
    [Service(typeof(IRepository<ContentItem>), Configuration = "mongo", Replaces = typeof(ContentItemRepository))]
    public class MongoContentItemRepository : MongoDbRepository<ContentItem>, IContentItemRepository
    {
        public MongoContentItemRepository(MongoDatabaseProvider provider)
            : base(provider)
        {
        }

        public IEnumerable<DiscriminatorCount> FindDescendantDiscriminators(ContentItem ancestor)
        {
            var pipeline = new List<BsonDocument>();
            if (ancestor != null)
            {
                pipeline.Add(new BsonDocument("$match", new BsonDocument("$or", new BsonArray
                {
                    new BsonDocument("_id", ancestor.ID),
                    new BsonDocument("AncestralTrail", new BsonRegularExpression("^" + ancestor.GetTrail() + ".*"))
                })));
            }
            pipeline.Add(new BsonDocument("$group", new BsonDocument
            {
                { "_id", "$_t" },
                { "value", new BsonDocument("$sum", 1) }
            }));
            var results = Collection.Aggregate(new AggregateArgs { Pipeline = pipeline });

            return results
                .Where(doc => !doc["_id"].IsBsonNull)
                .Select(doc => new DiscriminatorCount { Discriminator = ((BsonArray)doc["_id"]).Last().ToString(), Count = (int)doc["value"].ToDouble() })
                .OrderByDescending(dc => dc.Count)
                .ToList();
        }

        private static IMongoQuery ItselfOrBelow(ContentItem ancestor)
        {
            return Query.Or(Query.EQ("_id", ancestor.ID), Below(ancestor));
        }

        private static IMongoQuery Below(ContentItem ancestor)
        {
            return Query.Matches("AncestralTrail", "^" + ancestor.GetTrail().Replace("/", "\\/") + ".*");
        }

        public IEnumerable<ContentItem> FindDescendants(ContentItem ancestor, string discriminator)
        {
            if (ancestor == null)
                return Collection.Find(Query.EQ("_t", discriminator));
            else
                return Collection.Find(Query.And(ItselfOrBelow(ancestor), Query.EQ("_t", discriminator)));
        }

        public IEnumerable<ContentItem> FindReferencing(ContentItem linkTarget)
        {
            if (linkTarget == null)
                return Enumerable.Empty<ContentItem>();

            return Collection.Find(Query.Or(
                Query.EQ("Details.LinkedItem", linkTarget.ID),
                Query.EQ("DetailCollections.Details.LinkedItem", linkTarget.ID)));
        }

        public int RemoveReferencesToRecursive(ContentItem target)
        {
            var ids = new HashSet<int>(Collection.AsQueryable()
                .Where(i => i.ID == target.ID || i.AncestralTrail.StartsWith(target.GetTrail()))
                .Select(i => i.ID));

            int count = 0;
            foreach (var item in Collection.Find(Query.In("Details.LinkedItem", ids.Select(id => (BsonValue)id))))
            {
                foreach (var detail in item.Details.ToList())
                {
                    if (detail.LinkedItem.HasValue && ids.Contains(detail.LinkedItem.ID.Value))
                        item.Details.Remove(detail);
                }
                Collection.Save(item);
                count++;
            }

            return count;
        }

        protected override int GetEntityId(ContentItem entity)
        {
            return entity.ID;
        }
    }
}
