namespace BoxFortQuoteRecap_2025.Domain.Entities {
    public class QuoteRequestStats {
        public int QuoteId { get; set; }
        public string Content { get; set; } = string.Empty;
        public int Hits { get; set; }
        public int Year { get; set; }
    }
}
