using CurrConverter.Application.Common.Interfaces;
using CurrConverter.Domain.Common;
using CurrConverter.Domain.ConfigOptions.CurrencyConverter;
using CurrConverter.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Polly;
using Polly.Extensions.Http;
using Polly.Registry;

using System;

namespace CurrConverter.Infrastructure;

/// <summary>
/// adds/configures infrastructure services to dependencies container
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CurrencyConverterOptions>(configuration.GetSection(Constants.CURRENCY_CONVERTER_APPSETTING_SECTION));

        var currencyConverterOptions = configuration.GetSection(Constants.CURRENCY_CONVERTER_APPSETTING_SECTION).Get<CurrencyConverterOptions>();

        services.AddHttpClient(Constants.CURRENCY_CONVERTER_CLIENT, client =>
        {
            client.BaseAddress = new Uri(currencyConverterOptions.BaseUrl);
        });

        // Register the policy registry in the service collection
        services.AddSingleton<IReadOnlyPolicyRegistry<string>>
            (GetRetryPolicy(currencyConverterOptions.RetryPolicy.MaxRetries,
            currencyConverterOptions.RetryPolicy.RetryIntervalSeconds));

        services.AddHttpClient(Constants.CURRENCY_CONVERTER_CLIENT, client =>
        {
            client.BaseAddress = new Uri(currencyConverterOptions.BaseUrl);
        })
        .AddPolicyHandlerFromRegistry(Constants.HTTP_DEFAULT_RETRY_STRATEGY); // Use the retry policy

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<ICurrencyConverterService, CurrencyConverterService>();

        return services;
    }

    /// <summary>
    /// Creates a policy registry with a retry policy configured for transient HTTP errors.
    /// </summary>
    /// <param name="retries">The maximum number of retry attempts.</param>
    /// <param name="initialDelaySeconds">The initial delay in seconds before the first retry attempt. The delay will increase exponentially with each retry.</param>
    /// <returns>A PolicyRegistry containing the configured retry policy.</returns>
    private static PolicyRegistry GetRetryPolicy(int retries, int initialDelaySeconds)
    {
        // Create the policy registry and add the default retry policy
        return new PolicyRegistry()
        {
            {
                Constants.HTTP_DEFAULT_RETRY_STRATEGY, // Retry policy name
                HttpPolicyExtensions.HandleTransientHttpError()
                    .WaitAndRetryAsync(
                        retries, // Number of retries
                        attempt => TimeSpan.FromSeconds(initialDelaySeconds),
                        onRetryAsync: async (outcome, timespan, retryCount, context) =>
                        {
                            Console.WriteLine(outcome);
                            Console.WriteLine(retryCount);
                        })
            },
        };
    }
}
