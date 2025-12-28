using BoxFortQuoteRecap_2025.Application.Models;

namespace BoxFortQuoteRecap_2025.Application.Interfaces {
    public interface IQuoteHourlySummaryRepository {
        public Task<IEnumerable<HeatmapEntry>> GetHeatmapEntries();
    }
}
