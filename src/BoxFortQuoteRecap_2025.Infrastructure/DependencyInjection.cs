using BoxFortQuoteRecap_2025.Application.Interfaces;
using BoxFortQuoteRecap_2025.Infrastructure.Repositories;
using BoxFortQuoteRecap_2025.Infrastructure.Services;
using BoxFortQuoteRecap_2025.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace BoxFortQuoteRecap_2025.Infrastructure {
    public static class DependencyInjection {
        public static async Task<IServiceCollection> AddInfrastructureAsync(this IServiceCollection services, IConfiguration configuration, string baseAddress) {
            services.AddScoped<IWordCloudGenerator, BoxFortWordCloudGenerator>();

            services.AddScoped<IQuoteWordCloudEntryRepository, QuoteWordCloudEntryRepository>(x => {
                return new QuoteWordCloudEntryRepository(
                    x.GetRequiredService<IOptions<AppSettings>>(),
                    x.GetRequiredService<HttpClient>()
                );
            });

            services.AddScoped<IQuoteStatRepository, QuoteStatRepository>(x => {
                return new QuoteStatRepository(
                    x.GetRequiredService<IOptions<AppSettings>>(),
                    x.GetRequiredService<HttpClient>()
                );
            });

            services.AddScoped<IQuoteGameStatRepository, QuoteGameStatRepository>(x => {
                return new QuoteGameStatRepository(
                    x.GetRequiredService<IOptions<AppSettings>>(),
                    x.GetRequiredService<HttpClient>()
                );
            });

            services.AddScoped<IQuoteHistogramRepository, QuoteHistogramRepository>(x => {
                return new QuoteHistogramRepository(
                    x.GetRequiredService<IOptions<AppSettings>>(),
                    x.GetRequiredService<HttpClient>()
                );
            });

            services.AddScoped<IQuoteHourlySummaryRepository, QuoteHourlySummaryRepository>(x => {
                return new QuoteHourlySummaryRepository(
                    x.GetRequiredService<IOptions<AppSettings>>(),
                    x.GetRequiredService<HttpClient>()
                    );
            });

            services.AddScoped<IQuoteTopQuotesRepository, QuoteTopQuotesRepository>(x => {
                return new QuoteTopQuotesRepository(
                    x.GetRequiredService<IOptions<AppSettings>>(),
                    x.GetRequiredService<HttpClient>()
                    );
            });

            services.AddScoped<IQuoteTopClipsRepository, QuoteTopClipsRepository>(x => {
                return new QuoteTopClipsRepository(
                    x.GetRequiredService<IOptions<AppSettings>>(),
                    x.GetRequiredService<HttpClient>()
                    );
            });

            return services;
        }
    }
}
