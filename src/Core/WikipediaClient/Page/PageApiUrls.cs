using System;

namespace Geekbot.Core.WikipediaClient.Page
{
    public class PageApiUrls
    {
        public Uri Summary { get; set; }
        public Uri Metadata { get; set; }
        public Uri References { get; set; }
        public Uri Media { get; set; }
        public Uri EditHtml { get; set; }
        public Uri TalkPageHtml { get; set; }
    }
}