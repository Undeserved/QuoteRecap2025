using BoxFortQuoteRecap_2025.Domain.Entities;

namespace BoxFortQuoteRecap_2025.Application.Interfaces {
    public interface IQuoteWordCloudEntryRepository {
        public Task<IEnumerable<QuoteWordFrequency>> GetWordFrequenciesAsync();
    }
}
