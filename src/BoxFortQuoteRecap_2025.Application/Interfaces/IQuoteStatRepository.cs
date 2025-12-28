using BoxFortQuoteRecap_2025.Application.Models;

namespace BoxFortQuoteRecap_2025.Application.Interfaces {
    public interface IQuoteStatRepository {
        Task<QuoteStatsEntry> GetStats();
    }
}
