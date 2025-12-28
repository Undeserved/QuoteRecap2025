using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using MediatR;

namespace BoxFortQuoteRecap_2025.Application.Queries {
    public class GetQuoteHistogramQuery : IRequest<IEnumerable<HistogramEntry>> { }
    public class GetQuoteHistogramQueryHandler : IRequestHandler<GetQuoteHistogramQuery, IEnumerable<HistogramEntry>> {
        private readonly IQuoteHistogramRepository _quoteHistogramRepository;
        public GetQuoteHistogramQueryHandler(IQuoteHistogramRepository quoteHistogramRepository) {
            _quoteHistogramRepository = quoteHistogramRepository;
        }
        public async Task<IEnumerable<HistogramEntry>> Handle(GetQuoteHistogramQuery request, CancellationToken cancellationToken) {
            return await _quoteHistogramRepository.GetHistogramEntriesAsync();
        }
    }
}
