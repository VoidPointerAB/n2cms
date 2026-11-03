using System.Web.Mvc;

namespace N2.Web.Mvc.Html
{
    public static class ZoneExtensions
    {
        /// <summary>
        /// Gets the HTML for all items in the Zone of the given name from the current item.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public static ZoneHelper Zone(this HtmlHelper helper, string zoneName)
        {
            return helper.Zone(helper.CurrentItem(), zoneName);
        }

        /// <summary>
        /// Gets the HTML for all items in the Zone of the given name from the <paramref name="item" /> given.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="zoneName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ZoneHelper Zone(this HtmlHelper helper, ContentItem item, string zoneName)
        {
            return new ZoneHelper(helper, zoneName, item);
        }

        /// <summary>
        /// Renders all items in the Zone of the given name from the current item.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public static void RenderZone(this HtmlHelper helper, string zoneName)
        {
            helper.RenderZone(helper.CurrentItem(), zoneName);
        }

        /// <summary>
        /// Renders all items in the Zone of the given name from the <paramref name="item" /> given.
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="zoneName"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static void RenderZone(this HtmlHelper helper, ContentItem item, string zoneName)
        {
            new ZoneHelper(helper, zoneName, item).Render(helper.ViewContext.Writer);
        }

		public static PartHelper Part(this HtmlHelper helper, ContentItem item)
		{
			return new PartHelper(helper, item);
		}
    }
}
