using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using BoxFortQuoteRecap_2025.Domain.Entities;
using BoxFortQuoteRecap_2025.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BoxFortQuoteRecap_2025.Infrastructure.Repositories {
    public class QuoteHourlySummaryRepository : IQuoteHourlySummaryRepository {
        private static IEnumerable<QuoteHourlySummary>? _quotes;
        private static readonly object _lock = new();

        private readonly HttpClient _httpClient;
        private readonly string _quotesFilename;

        public QuoteHourlySummaryRepository(IOptions<AppSettings> appSettings, HttpClient httpClient) {
            _httpClient = httpClient;
            _quotesFilename = appSettings.Value.QuoteHourlySummaryRepositoryFilename;
        }

        public async Task<IEnumerable<HeatmapEntry>> GetHeatmapEntries() {
            var quotes = await LoadHourlySummariesAsync();
            return quotes
                .Select(x => new HeatmapEntry {
                    Weekday = x.Weekday,
                    Hour = x.Hour,
                    Count = x.Count
                });
        }

        private async Task<IEnumerable<QuoteHourlySummary>> LoadHourlySummariesAsync() {
            if (_quotes == null) {
                var quotes = await _httpClient.GetFromJsonAsync<IEnumerable<QuoteHourlySummary>>(_quotesFilename)
                             ?? new List<QuoteHourlySummary>();
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
