using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using MediatR;

namespace BoxFortQuoteRecap_2025.Application.Queries {
    public class GetQuoteHourlySummaryQuery : IRequest<IEnumerable<HeatmapEntry>> { }

    public class GetQuoteHourlySummaryHandler : IRequestHandler<GetQuoteHourlySummaryQuery, IEnumerable<HeatmapEntry>> {

        private readonly IQuoteHourlySummaryRepository _quoteGameHourlySummaryRepository;
        public GetQuoteHourlySummaryHandler(IQuoteHourlySummaryRepository quoteGameStatRepository) {
            _quoteGameHourlySummaryRepository = quoteGameStatRepository;
        }
        public async Task<IEnumerable<HeatmapEntry>> Handle(GetQuoteHourlySummaryQuery request, CancellationToken cancellationToken) {
            return await _quoteGameHourlySummaryRepository.GetHeatmapEntries();
        }
    }
}
