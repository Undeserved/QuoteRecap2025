using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Domain.Entities;
using BoxFortQuoteRecap_2025.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BoxFortQuoteRecap_2025.Infrastructure.Repositories {
    public class QuoteWordCloudEntryRepository : IQuoteWordCloudEntryRepository {

        private static IEnumerable<QuoteWordFrequency>? _quotes;
        private static readonly object _lock = new();

        private readonly HttpClient _httpClient;
        private readonly string _quotesFilename;

        public QuoteWordCloudEntryRepository(IOptions<AppSettings> appSettings, HttpClient httpClient) {
            _httpClient = httpClient;
            _quotesFilename = appSettings.Value.QuoteWordCloudEntryRepositoryFilename;
        }

        public async Task<IEnumerable<QuoteWordFrequency>> GetWordFrequenciesAsync() {
            return await InMemoryLoad();
        }

        private async Task<IEnumerable<QuoteWordFrequency>> InMemoryLoad() {
            if (_quotes == null) {
                var quotes = await _httpClient.GetFromJsonAsync<IEnumerable<QuoteWordFrequency>>(_quotesFilename)
                             ?? new List<QuoteWordFrequency>();
                lock (_lock) {
                    if (_quotes == null) {
                        _quotes = quotes;
                    }
                }
            }

            return _quotes!;
        }
    }
}
