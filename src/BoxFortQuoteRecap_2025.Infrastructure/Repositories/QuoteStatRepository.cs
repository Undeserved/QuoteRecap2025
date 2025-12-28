using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using BoxFortQuoteRecap_2025.Domain.Entities;
using BoxFortQuoteRecap_2025.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BoxFortQuoteRecap_2025.Infrastructure.Repositories {
    public class QuoteStatRepository : IQuoteStatRepository {

        private static QuoteStats? _stats;
        private static readonly object _lock = new();

        private readonly HttpClient _httpClient;
        private readonly string _statsFilename;

        public QuoteStatRepository(IOptions<AppSettings> appSettings, HttpClient httpClient) {
            _httpClient = httpClient;
            _statsFilename = appSettings.Value.QuoteStatsRepositoryFilename;
        }

        public async Task<QuoteStatsEntry> GetStats() {
            await InMemoryLoad();
            return new QuoteStatsEntry {
                QuoteCount = _stats!.QuoteCount,
                YearCount = _stats.YearCount,
                GameCount = _stats.GameCount,
                RequestCount = _stats.RequestCount,
                MostRequestedQuoteId = _stats.MostRequestedQuoteId,
                MostRequestedQuoteContent = _stats.MostRequestedQuoteContent,
                MostQuotedGame = _stats.MostQuotedGame,
                MemeStat = _stats.MemeStat,
                LongestGapInDays = _stats.LongestGapInDays,
                PeakRequestCount = _stats.PeakRequestCount,
                PeakRequestDay = _stats.PeakRequestDay,
                UnrequestedQuote = _stats.UnrequestedQuote
            };
        }

        private async Task<QuoteStats> InMemoryLoad() {
            if (_stats != null)
                return _stats;

            var loaded = await _httpClient.GetFromJsonAsync<QuoteStats>(_statsFilename)
                         ?? new QuoteStats();

            lock (_lock) {
                _stats ??= loaded;
            }

            return _stats;
        }
    }
}
