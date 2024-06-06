using System;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using X.PagedList;

namespace AppleStore.Helpers
{
    public static class HtmlHelpers
    {
        public delegate string PageUrlDelegate(int page);

        public static IHtmlContent CustomPagedListPager(this IHtmlHelper html, IPagedList pagedList, PageUrlDelegate pageUrl)
        {
            if (pagedList.PageCount <= 1)
                return HtmlString.Empty;

            var ulTag = new Microsoft.AspNetCore.Mvc.Rendering.TagBuilder("ul");
            ulTag.AddCssClass("pagination-list");

            for (int i = 1; i <= pagedList.PageCount; i++)
            {
                var liTag = new Microsoft.AspNetCore.Mvc.Rendering.TagBuilder("li");

                if (i == pagedList.PageNumber)
                {
                    liTag.AddCssClass("active");
                }

                var aTag = new Microsoft.AspNetCore.Mvc.Rendering.TagBuilder("a");
                aTag.Attributes["href"] = pageUrl(i);
                aTag.InnerHtml.Append(i.ToString());

                liTag.InnerHtml.AppendHtml(aTag);
                ulTag.InnerHtml.AppendHtml(liTag);
            }

            var divTag = new Microsoft.AspNetCore.Mvc.Rendering.TagBuilder("div");
            divTag.AddCssClass("pagination left");
            divTag.InnerHtml.AppendHtml(ulTag);

            return divTag;
        }
    }
}