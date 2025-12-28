using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using MediatR;

namespace BoxFortQuoteRecap_2025.Application.Queries {
    public class GetQuoteStatsQuery : IRequest<QuoteStatsEntry> { }

    public class GetQuoteStatsQueryHandler : IRequestHandler<GetQuoteStatsQuery, QuoteStatsEntry> {
        private readonly IQuoteStatRepository _quoteStatRepository;
        public GetQuoteStatsQueryHandler(IQuoteStatRepository quoteStatRepository) {
            _quoteStatRepository = quoteStatRepository;
        }
        public async Task<QuoteStatsEntry> Handle(GetQuoteStatsQuery request, CancellationToken cancellationToken) {
            var stats = await _quoteStatRepository.GetStats();
            return stats ?? new QuoteStatsEntry();
        }
    }
}
