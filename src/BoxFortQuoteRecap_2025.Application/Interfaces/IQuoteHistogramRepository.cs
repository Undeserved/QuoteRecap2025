using BoxFortQuoteRecap_2025.Application.Models;

namespace BoxFortQuoteRecap_2025.Application.Interfaces {
    public interface IQuoteHistogramRepository {
        public Task<IEnumerable<HistogramEntry>> GetHistogramEntriesAsync();
    }
}
