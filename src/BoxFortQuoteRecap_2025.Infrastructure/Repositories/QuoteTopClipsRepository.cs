using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using BoxFortQuoteRecap_2025.Domain.Entities;
using BoxFortQuoteRecap_2025.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace BoxFortQuoteRecap_2025.Infrastructure.Repositories {
    public class QuoteTopClipsRepository : IQuoteTopClipsRepository {
        private static IEnumerable<Clips>? _clips;
        private static readonly object _lock = new();

        private readonly HttpClient _httpClient;
        private readonly string _clipsFilename;
        public QuoteTopClipsRepository(IOptions<AppSettings> appSettings, HttpClient httpClient) {
            _httpClient = httpClient;
            _clipsFilename = appSettings.Value.QuoteTopClipsRepositoryFilename;
        }

        public async Task<IEnumerable<TopClipsEntry>> GetTopClips() {
            var clips = await LoadClipsAsync();
            return clips
                .Select(x => new TopClipsEntry {
                    Title = x.Title,
                    RawUrl = x.RawUrl,
                    ViewCount = x.ViewCount
                });
        }

        private async Task<IEnumerable<Clips>> LoadClipsAsync() {
            if (_clips == null) {
                var clips = await _httpClient.GetFromJsonAsync<IEnumerable<Clips>>(_clipsFilename)
                             ?? new List<Clips>();
                lock (_lock) {
                    if (_clips == null) {
                        _clips = clips;
                    }
                }
            }
            return _clips!;
        }
    }    
}
