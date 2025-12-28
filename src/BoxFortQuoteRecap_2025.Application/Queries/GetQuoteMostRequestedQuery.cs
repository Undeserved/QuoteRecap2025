using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using MediatR;

namespace BoxFortQuoteRecap_2025.Application.Queries {
    public class GetQuoteMostRequestedQuery : IRequest<IEnumerable<TopQuotesEntry>> { }
    public class GetQuoteMostRequestedQueryHandler : IRequestHandler<GetQuoteMostRequestedQuery, IEnumerable<TopQuotesEntry>> {
        private readonly IQuoteTopQuotesRepository _quoteTopQuotesRepository;
        public GetQuoteMostRequestedQueryHandler(Interfaces.IQuoteTopQuotesRepository quoteTopQuotesRepository) {
            _quoteTopQuotesRepository = quoteTopQuotesRepository;
        }
        public async Task<IEnumerable<TopQuotesEntry>> Handle(GetQuoteMostRequestedQuery request, CancellationToken cancellationToken) {
            return await _quoteTopQuotesRepository.GetTopQuotes();
        }
    }
}
