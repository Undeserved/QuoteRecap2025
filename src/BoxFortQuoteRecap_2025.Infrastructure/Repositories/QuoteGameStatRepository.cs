using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using BoxFortQuoteRecap_2025.Domain.Entities;
using BoxFortQuoteRecap_2025.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BoxFortQuoteRecap_2025.Infrastructure.Repositories {
    public class QuoteGameStatRepository : IQuoteGameStatRepository {
        private static IEnumerable<QuoteGameStats>? _quotes;
        private static readonly object _lock = new();

        private readonly HttpClient _httpClient;
        private readonly string _quotesFilename;

        public QuoteGameStatRepository(IOptions<AppSettings> appSettings, HttpClient httpClient) {
            _httpClient = httpClient;
            _quotesFilename = appSettings.Value.QuoteGameStatsRepositoryFilename;
        }

        private async Task<IEnumerable<QuoteGameStats>> LoadGameStatsAsync() {
            if (_quotes == null) {
                var quotes = await _httpClient.GetFromJsonAsync<IEnumerable<QuoteGameStats>>(_quotesFilename)
                             ?? new List<QuoteGameStats>();
                lock (_lock) {
                    if (_quotes == null) {
                        _quotes = quotes;
                    }
                }
            }

            return _quotes!;
        }

        public async Task<IEnumerable<GameStatsEntry>> GetGameStatsAsync() {
            var quotes = await LoadGameStatsAsync();
            return quotes
                .Select(x => new GameStatsEntry { 
                    Game = x.Game, 
                    QuoteCount = x.QuoteCount 
                });
        }
    }
}
