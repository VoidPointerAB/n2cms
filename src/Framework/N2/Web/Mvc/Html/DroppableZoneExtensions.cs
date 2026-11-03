using System.Web;
using System.Web.Mvc;
using N2.Web.UI.WebControls;
using System;

namespace N2.Web.Mvc.Html
{
    public static class DroppableZoneExtensions
    {
        /// <summary>
        /// Gets the HTML for all items in the Zone of the given name from the current item.
        /// </summary>
        /// <remarks>This extension method at the moment does not implement the drag &amp; drop functionality.</remarks>
        /// <param name="helper">The HTML helper.</param>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public static DroppableZoneHelper DroppableZone(this HtmlHelper helper, string zoneName)
        {
            return new DroppableZoneHelper(helper, zoneName, helper.CurrentItem());
        }

        /// <summary>
        /// Gets the HTML for all items in the Zone of the given name from the <paramref name="item" /> given.
        /// </summary>
        /// <remarks>This extension method at the moment does not implement the drag &amp; drop functionality.</remarks>
        /// <param name="helper">The HTML helper.</param>
        /// <param name="item">The content item.</param>
        /// <param name="zoneName"></param>
        /// <returns></returns>
        public static DroppableZoneHelper DroppableZone(this HtmlHelper helper, ContentItem item, string zoneName)
        {
            return new DroppableZoneHelper(helper, zoneName, item);
        }
    }
}
