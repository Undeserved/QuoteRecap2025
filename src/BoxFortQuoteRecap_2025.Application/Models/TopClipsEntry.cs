namespace BoxFortQuoteRecap_2025.Application.Models {
    public class TopClipsEntry {
        public string Title { get; set; } = string.Empty;
        public string RawUrl { get; set; } = string.Empty;
        public int ViewCount { get; set; }

        public string EmbedUrl => BuildEmbedUrl(RawUrl);

        string BuildEmbedUrl(string clipUrl) {
            var slug = ExtractSlug(clipUrl);
            return $"https://clips.twitch.tv/embed?clip={slug}&parent=Undeserved.github.io";
        }

        static string ExtractSlug(string clipUrl) {
            if (string.IsNullOrWhiteSpace(clipUrl))
                throw new ArgumentException("Clip URL is required", nameof(clipUrl));

            if (!clipUrl.Contains('/'))
                return clipUrl;

            return clipUrl
                .TrimEnd('/')
                .Split('/')
                .Last();
        }

    }
}
