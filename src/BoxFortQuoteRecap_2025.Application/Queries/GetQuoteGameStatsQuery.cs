using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Application.Models;
using BoxFortQuoteRecap_2025.Domain.Entities;
using MediatR;

namespace BoxFortQuoteRecap_2025.Application.Queries {
    public class GetQuoteGameStatsQuery : IRequest<IEnumerable<GameStatsEntry>> { }
    public class GetQuoteGameStatsQueryHandler : IRequestHandler<GetQuoteGameStatsQuery, IEnumerable<GameStatsEntry>> {
        private readonly IQuoteGameStatRepository _quoteGameStatRepository;
        public GetQuoteGameStatsQueryHandler(IQuoteGameStatRepository quoteGameStatRepository) {
            _quoteGameStatRepository = quoteGameStatRepository;
        }
        public async Task<IEnumerable<GameStatsEntry>> Handle(GetQuoteGameStatsQuery request, CancellationToken cancellationToken) {
            return await _quoteGameStatRepository.GetGameStatsAsync();
        }
    }
}
