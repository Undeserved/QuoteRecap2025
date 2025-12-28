using BoxFortQuoteRecap_2025.Application.Models;

namespace BoxFortQuoteRecap_2025.Application.Interfaces {
    public interface IQuoteGameStatRepository {
        public Task<IEnumerable<GameStatsEntry>> GetGameStatsAsync();
    }
}
