using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using MediatR;

namespace BoxFortQuoteRecap_2025.Application.Services {
    public class GenerateWordCloudCommand : IRequest<IEnumerable<WordCloudDrawItem>> {
    }

    public class GenerateWordCloudCommandHandler : IRequestHandler<GenerateWordCloudCommand, IEnumerable<WordCloudDrawItem>> {
        private readonly IWordCloudGenerator _wordCloudGenerator;

        public GenerateWordCloudCommandHandler(IWordCloudGenerator wordCloudGenerator) {
            _wordCloudGenerator = wordCloudGenerator;
        }

        public async Task<IEnumerable<WordCloudDrawItem>> Handle(GenerateWordCloudCommand request, CancellationToken cancellationToken) {
            return await _wordCloudGenerator.GenerateWordCloudAsync();
        }
    }
}