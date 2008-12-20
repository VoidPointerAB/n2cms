using System;
using System.Reflection;
using System.Diagnostics;
using N2.Details;

namespace N2.Persistence
{
	/// <summary>
	/// Handles saving and restoring versions of items.
	/// </summary>
	public class VersionManager : IVersionManager
	{
		private IRepository<int, ContentItem> itemRepository;
		
		public VersionManager(IRepository<int, ContentItem> itemRepository)
		{
			this.itemRepository = itemRepository;
		}

		#region Versioning Methods
		/// <summary>Creates an old version of an item. This must be called before the item item is modified.</summary>
		/// <param name="item">The item to create a old version of.</param>
		/// <returns>The old version.</returns>
		public virtual ContentItem SaveVersion(ContentItem item)
		{
			CancellableItemEventArgs args = new CancellableItemEventArgs(item);
			if (ItemSavingVersion != null)
				ItemSavingVersion.Invoke(this, args);
			if (!args.Cancel)
			{
				item = args.AffectedItem;

				ContentItem oldVersion = item.Clone(false);
				oldVersion.Expires = Utility.CurrentTime().AddSeconds(-1);
				oldVersion.Updated = Utility.CurrentTime().AddSeconds(-1);
				oldVersion.Parent = null;
				oldVersion.VersionOf = item;
				if (item.Parent != null)
					oldVersion["ParentID"] = item.Parent.ID;
				itemRepository.SaveOrUpdate(oldVersion);

				if (ItemSavedVersion != null)
					ItemSavedVersion.Invoke(this, new ItemEventArgs(oldVersion));

				return oldVersion;
			}
			return null;
		}

		/// <summary>Update a page version with another, i.e. save a version of the current item and replace it with the replacement item. Returns a version of the previously published item.</summary>
		/// <param name="currentItem">The item that will be stored as a previous version.</param>
		/// <param name="replacementItem">The item that will take the place of the current item using it's ID. Any saved version of this item will not be modified.</param>
		/// <returns>A version of the previously published item.</returns>
		public virtual ContentItem ReplaceVersion(ContentItem currentItem, ContentItem replacementItem)
		{
			CancellableDestinationEventArgs args = new CancellableDestinationEventArgs(currentItem, replacementItem);
			if (ItemReplacingVersion != null)
				ItemReplacingVersion.Invoke(this, args);
			if (!args.Cancel)
			{
				currentItem = args.AffectedItem;
				replacementItem = args.Destination;

				using (ITransaction transaction = itemRepository.BeginTransaction())
				{
					ContentItem versionOfCurrentItem = SaveVersion(currentItem);
					ClearAllDetails(currentItem);

					UpdateCurrentItemData(currentItem, replacementItem);

					currentItem.Updated = Utility.CurrentTime();
					itemRepository.Update(currentItem);

					if (ItemReplacedVersion != null)
						ItemReplacedVersion.Invoke(this, new ItemEventArgs(replacementItem));
					if (replacementItem.VersionOf == currentItem)
						itemRepository.Delete(replacementItem);

					itemRepository.Flush(); 
					transaction.Commit();

					return versionOfCurrentItem;
				}
			}
			return currentItem;
		}

		#region ReplaceVersion Helper Methods

		private static void UpdateCurrentItemData(ContentItem currentItem, ContentItem replacementItem)
		{
			for (Type t = currentItem.GetType(); t.BaseType != null; t = t.BaseType)
			{
				foreach (FieldInfo fi in t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance))
				{
					if (fi.GetCustomAttributes(typeof(DoNotCopyAttribute), true).Length == 0)
						if (fi.Name != "id" && fi.Name != "expires" && fi.Name != "published")
							fi.SetValue(currentItem, fi.GetValue(replacementItem));
					if(fi.Name == "url")
						fi.SetValue(currentItem, null);
				}
			}

			foreach (Details.ContentDetail detail in replacementItem.Details.Values)
			{
				currentItem[detail.Name] = detail.Value;
			}
			foreach (Details.DetailCollection collection in replacementItem.DetailCollections.Values)
			{
				Details.DetailCollection newCollection = currentItem.GetDetailCollection(collection.Name, true);
				foreach (Details.ContentDetail detail in collection.Details)
					newCollection.Add(detail.Value);
			}
		}

		private void ClearAllDetails(ContentItem item)
		{
			item.Details.Clear();

			foreach (Details.DetailCollection collection in item.DetailCollections.Values)
			{
				collection.Details.Clear();
			}
			item.DetailCollections.Clear();
		}
		#endregion
		#endregion


		/// <summary>Occurs before an item is saved</summary>
		public event EventHandler<CancellableItemEventArgs> ItemSavingVersion;
		/// <summary>Occurs before an item is saved</summary>
		public event EventHandler<ItemEventArgs> ItemSavedVersion;
		/// <summary>Occurs before an item is saved</summary>
		public event EventHandler<CancellableDestinationEventArgs> ItemReplacingVersion;
		/// <summary>Occurs before an item is saved</summary>
		public event EventHandler<ItemEventArgs> ItemReplacedVersion;
        
	}
}
