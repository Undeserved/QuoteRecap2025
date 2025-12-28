using BoxFortQuoteRecap_2025.Application.Models;
using MediatR;

namespace BoxFortQuoteRecap_2025.Application.Queries {
    public class GetClipsMostRequestedQuery : IRequest<IEnumerable<TopClipsEntry>> { }
    public class GetClipsMostRequestedQueryHandler : IRequestHandler<GetClipsMostRequestedQuery, IEnumerable<TopClipsEntry>> {
        private readonly Interfaces.IQuoteTopClipsRepository _quoteTopClipsRepository;
        public GetClipsMostRequestedQueryHandler(Interfaces.IQuoteTopClipsRepository quoteTopClipsRepository) {
            _quoteTopClipsRepository = quoteTopClipsRepository;
        }
        public async Task<IEnumerable<TopClipsEntry>> Handle(GetClipsMostRequestedQuery request, CancellationToken cancellationToken) {
            return await _quoteTopClipsRepository.GetTopClips();
        }
    }
}
