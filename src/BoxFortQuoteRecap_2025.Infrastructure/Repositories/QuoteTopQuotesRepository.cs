using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using BoxFortQuoteRecap_2025.Domain.Entities;
using BoxFortQuoteRecap_2025.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BoxFortQuoteRecap_2025.Infrastructure.Repositories {
    public class QuoteTopQuotesRepository : IQuoteTopQuotesRepository {
        private static IEnumerable<QuoteRequestStats>? _quotes;
        private static readonly object _lock = new();

        private readonly HttpClient _httpClient;
        private readonly string _quotesFilename;

        public QuoteTopQuotesRepository(IOptions<AppSettings> appSettings, HttpClient httpClient) {
            _httpClient = httpClient;
            _quotesFilename = appSettings.Value.QuoteTopQuotesRepositoryFilename;
        }

        public async Task<IEnumerable<TopQuotesEntry>> GetTopQuotes() {
            var quotes = await LoadGameStatsAsync();
            return quotes
                .Select(x => new TopQuotesEntry {
                    QuoteId = x.QuoteId,
                    Content = x.Content,
                    Hits = x.Hits
                });
        }

        private async Task<IEnumerable<QuoteRequestStats>> LoadGameStatsAsync() {
            if (_quotes == null) {
                var quotes = await _httpClient.GetFromJsonAsync<IEnumerable<QuoteRequestStats>>(_quotesFilename)
                             ?? new List<QuoteRequestStats>();
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
