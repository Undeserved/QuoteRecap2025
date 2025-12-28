namespace BoxFortQuoteRecap_2025.Infrastructure.Configuration {
    public class AppSettings {
        public string QuoteRepositoryFilename { get; set; } = string.Empty;
        public string QuoteWordCloudEntryRepositoryFilename { get; set; } = string.Empty;
        public string QuoteStatsRepositoryFilename { get; set; } = string.Empty;
        public string QuoteGameStatsRepositoryFilename { get; set; } = string.Empty;
        public string QuoteHistogramRepositoryFilename { get; set; } = string.Empty;
        public string QuoteHourlySummaryRepositoryFilename {  get; set; } = string.Empty;
        public string QuoteTopQuotesRepositoryFilename { get; set; } = string.Empty;    
        public string QuoteTopClipsRepositoryFilename { get; set; } = string.Empty;
        public List<string> ExcludedWords { get; set; } = new List<string>();
        public char[] PunctuationMarkers {  get; set; }
    }
}
