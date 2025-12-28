using BoxFortQuoteRecap_2025.Application.Models;

namespace BoxFortQuoteRecap_2025.Application.Interfaces {
    public interface IWordCloudGenerator {
        Task<IEnumerable<WordCloudDrawItem>> GenerateWordCloudAsync();
    }
}
