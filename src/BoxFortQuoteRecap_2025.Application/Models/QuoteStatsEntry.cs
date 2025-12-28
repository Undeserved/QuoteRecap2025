namespace BoxFortQuoteRecap_2025.Application.Models {
    public class QuoteStatsEntry {
        public int QuoteCount { get; set; }
        public int YearCount { get; set; }
        public int GameCount { get; set; }
        public int RequestCount { get; set; }
        public int MostRequestedQuoteId { get; set; }
        public string MostRequestedQuoteContent { get; set; } = string.Empty;
        public string MostQuotedGame { get; set; } = string.Empty;
        public int MemeStat { get; set; }
        public int LongestGapInDays { get; set; }
        public DateTime PeakRequestDay { get; set; }
        public int PeakRequestCount { get; set; }
        public int UnrequestedQuote { get; set; }
    }
}
