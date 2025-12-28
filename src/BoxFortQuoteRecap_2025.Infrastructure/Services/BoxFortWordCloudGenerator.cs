using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using BoxFortQuoteRecap_2025.Domain.Entities;

namespace BoxFortQuoteRecap_2025.Infrastructure.Services {
    public class BoxFortWordCloudGenerator : IWordCloudGenerator {
        private const int CanvasWidth = 1024;
        private const int CanvasHeight = 256;   
        private const int MaxFontSize = 64;
        private const int MinFontSize = 16;

        private readonly IQuoteWordCloudEntryRepository _wordCloudRepository;
        private readonly Random _random = new Random();

        public BoxFortWordCloudGenerator(IQuoteWordCloudEntryRepository wordCloudRepository) {
            _wordCloudRepository = wordCloudRepository;
        }

        public async Task<IEnumerable<WordCloudDrawItem>> GenerateWordCloudAsync() {
            IEnumerable<QuoteWordFrequency> wordFrequencies = await _wordCloudRepository.GetWordFrequenciesAsync();
            return CreateDrawInstructions(wordFrequencies);
        }

        private IEnumerable<WordCloudDrawItem> CreateDrawInstructions(IEnumerable<QuoteWordFrequency> entries) {
            var items = new List<WordCloudDrawItem>();
            var placed = new List<(int X, int Y, int Width, int Height)>();

            int centerX = CanvasWidth / 2;
            int centerY = CanvasHeight / 2;

            int minFreq = entries.Min(e => e.Count);
            int maxFreq = entries.Max(e => e.Count);

            const int margin = 8; // prevents clipping

            foreach (var entry in entries.OrderByDescending(e => e.Count)) {
                var fontSize = MapFrequencyToFontSize(entry.Count, minFreq, maxFreq);
                var colour = GetRandomColour();

                int width = (int)(entry.Word.Length * fontSize * 0.60);
                int height = (int)(fontSize * 1.20);

                double angle = 0;
                double radius = 0;
                const double angleStep = 0.12;
                const double radiusStep = 0.45;

                bool placedSuccessfully = false;

                for (int attempt = 0; attempt < 6000; attempt++) {
                    // Spiral movement
                    int x = centerX + (int)(Math.Cos(angle) * radius) - width / 2;
                    int y = centerY + (int)(Math.Sin(angle) * radius) - height / 2;

                    angle += angleStep;
                    radius += radiusStep;

                    // Clamp to keep the bloody words inside the bloody canvas
                    x = Math.Clamp(x, margin, CanvasWidth - width - margin);
                    y = Math.Clamp(y, margin, CanvasHeight - height - margin);

                    // Check collision
                    if (!Intersects(x, y, width, height, placed)) {
                        placed.Add((x, y, width, height));
                        items.Add(new WordCloudDrawItem {
                            Word = entry.Word,
                            X = x,
                            Y = y,
                            Colour = colour,
                            FontSize = fontSize
                        });
                        placedSuccessfully = true;
                        break;
                    }
                }

                if (!placedSuccessfully) {
                    Console.WriteLine($"This word is TEW BEEG: '{entry.Word}'.");
                }
            }

            return items;
        }

        private bool Intersects(int x, int y, int width, int height,
            List<(int X, int Y, int Width, int Height)> placed) {
            foreach (var p in placed) {
                bool overlap =
                    x < p.X + p.Width &&
                    x + width > p.X &&
                    y < p.Y + p.Height &&
                    y + height > p.Y;

                if (overlap) return true;
            }

            return false;
        }

        private double MapFrequencyToFontSize(int frequency, int minFreq, int maxFreq) {
            if (maxFreq == minFreq)
                return (MinFontSize + MaxFontSize) / 2;

            double normalised =
                (frequency - minFreq) /
                (double)(maxFreq - minFreq);

            return MinFontSize + normalised * (MaxFontSize - MinFontSize);
        }

        private string GetRandomColour() {
            return $"rgb({_random.Next(0, 255)}, {_random.Next(0, 255)}, {_random.Next(0, 255)})";
        }
    }
}
