using BoxFortQuoteRecap_2025.Application.Models;

namespace BoxFortQuoteRecap_2025.Application.Interfaces {
    public interface IQuoteTopClipsRepository {
        public Task<IEnumerable<TopClipsEntry>> GetTopClips();
    }
}
