using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using BoxFortQuoteRecap_2025.Domain.Entities;
using BoxFortQuoteRecap_2025.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BoxFortQuoteRecap_2025.Infrastructure.Repositories {
    public class QuoteHistogramRepository : IQuoteHistogramRepository {
        private static IEnumerable<QuoteHistoricStats>? _quotes;
        private static readonly object _lock = new();

        private readonly HttpClient _httpClient;
        private readonly string _quotesFilename;

        public QuoteHistogramRepository(IOptions<AppSettings> appSettings, HttpClient httpClient) {
            _httpClient = httpClient;
            _quotesFilename = appSettings.Value.QuoteHistogramRepositoryFilename;
        }

        public async Task<IEnumerable<HistogramEntry>> GetHistogramEntriesAsync() {
            var quotes = await LoadHistogramAsync();
            return quotes
                .Select(x => new HistogramEntry {
                    Year = x.Year,
                    Month = x.Month,
                    QuoteCount = x.QuoteCount
                });
        }

        private async Task<IEnumerable<QuoteHistoricStats>> LoadHistogramAsync() {
            if (_quotes == null) {
                var quotes = await _httpClient.GetFromJsonAsync<IEnumerable<QuoteHistoricStats>>(_quotesFilename)
                             ?? new List<QuoteHistoricStats>();
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
