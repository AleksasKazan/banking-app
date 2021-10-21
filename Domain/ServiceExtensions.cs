using System;
using Domain.Clients.Firebase;
using Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            return services
                .AddClients()
                .AddServices();
        }

        private static IServiceCollection AddClients(this IServiceCollection services)
        {
            services.AddHttpClient<IFireBaseClient, FireBaseClient>();

            return services;
        }

        private static IServiceCollection AddServices(this IServiceCollection services)
        {
            services
                .AddSingleton<IAuthService, AuthService>()
                .AddSingleton<ITransactionsService, TransactionsService>()
                .AddSingleton<ICardsService, CardsService>();

            return services;
        }
    }
}
