using BoxFortQuoteRecap_2025.Application.Models;

namespace BoxFortQuoteRecap_2025.Application.Interfaces {
    public interface IQuoteTopQuotesRepository {
        public Task<IEnumerable<TopQuotesEntry>> GetTopQuotes();
    }
}
