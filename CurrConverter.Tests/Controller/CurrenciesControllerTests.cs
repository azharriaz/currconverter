using CurrConverter.Api.Controllers;
using CurrConverter.Api.Filters;
using CurrConverter.Application.Common.Behaviours;
using CurrConverter.Application.Common.Interfaces;
using CurrConverter.Application.Common.Models;
using CurrConverter.Application.CurrencyConverter.Commands.CurrencyConvert;
using CurrConverter.Application.CurrencyConverter.Queries.GetLatestExchangeRates;
using CurrConverter.Application.Dto;
using CurrConverter.Domain.Models;
using CurrConverter.Tests.Common;

using FluentValidation;
using FluentValidation.AspNetCore;

using Mapster;
using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Moq;

using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace CurrConverter.Tests.Controller;
public class CurrenciesControllerTests
{
    private HttpClient _httpClient;
    private ServiceCollection _serviceCollection;
    private Mock<ICurrencyConverterService> _mockCurrencyConverterService = new();

    private const string _testDataPath = "./TestData/currencies.json";

    public CurrenciesControllerTests()
    {
        _serviceCollection = new ServiceCollection();

        _serviceCollection.AddSingleton(GetConfiguredMappingConfig());
        _serviceCollection.AddScoped<IMapper, ServiceMapper>();

        _serviceCollection.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        _serviceCollection.AddMediatR(typeof(RetrieveLatestExchangeRatesHandler).Assembly);


        _serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        _serviceCollection.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehaviour<,>));
    }

    #region GetLatestExchangeRates TestCases

    [Fact]
    public async Task GetLatestExchangeRates_ValidBaseCurrency_ReturnsOkResult()
    {
        // Arrange
        var requestUri = $"api/currencies/latest?baseCurrency={TestConstants.DEFAULT_BASE_CURRENCY}";
        var cancellationToken = new CancellationToken();

        LatestExchangeRatesConfigureServices(false);

        // Act
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);
        
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        
        var latestExchangeRatesResponse = JsonSerializer.Deserialize<ServiceResult<ExchangeRatesDto>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        Assert.NotNull(latestExchangeRatesResponse);

        Assert.IsType<ExchangeRatesDto>(latestExchangeRatesResponse.Data);

        Assert.True(latestExchangeRatesResponse.Succeeded);
    }

    [Fact]
    public async Task GetLatestExchangeRates_InvalidBaseCurrency_ReturnsBadRequest()
    {
        // Arrange
        var requestUri = $"api/currencies/latest?baseCurrency=InvalidCurrency";
        var cancellationToken = new CancellationToken();

        LatestExchangeRatesConfigureServices(invalidDetails: true);

        // Act
        var response = await _httpClient.GetAsync(requestUri, cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var latestExchangeRatesResponse = JsonSerializer.Deserialize<ServiceResult<ExchangeRatesDto>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        Assert.NotNull(latestExchangeRatesResponse);

        Assert.False(latestExchangeRatesResponse.Succeeded);

        Assert.Null(latestExchangeRatesResponse.Data);
    }

    private void LatestExchangeRatesConfigureServices(bool invalidDetails)
    {
        var response = JsonHelper.GetRequestModelAsync<ExchangeRateResponse>(_testDataPath, ActionTypeEnum.Latest).Result;

        _mockCurrencyConverterService
                .Setup(d => d.GetExchangeRatesAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidDetails ? default : response);

        _serviceCollection.AddSingleton(_mockCurrencyConverterService.Object);

        ConfigureServices();
    }
    #endregion

    #region ConvertCurrency TestCases

    [Fact]
    public async Task ConvertCurrency_ValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = await JsonHelper.GetRequestModelAsync<ConvertCurrencyCommand>(_testDataPath, ActionTypeEnum.Convert);
        var requestUri = $"api/currencies/convert";
        var cancellationToken = new CancellationToken();

        ConvertCurrencyConfigureServices(invalidDetails: false);

        // Act
        var response = await _httpClient.PostAsync
            (requestUri,
            new StringContent(JsonSerializer.Serialize(request),
            Encoding.UTF8,
            TestConstants.JSON_CONTENT_TYPE),
            cancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        var convertCurrencyResponse = JsonSerializer.Deserialize<ServiceResult<ExchangeRatesDto>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        Assert.NotNull(convertCurrencyResponse);

        Assert.IsType<ExchangeRatesDto>(convertCurrencyResponse.Data);

        Assert.True(convertCurrencyResponse.Succeeded);
    }

    [Fact]
    public async Task ConvertCurrency_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        ConvertCurrencyCommand request = new();
        var requestUri = "api/currencies/convert";
        var cancellationToken = new CancellationToken();

        ConvertCurrencyConfigureServices(invalidDetails: true);

        // Act
        var response = await _httpClient.PostAsync
            (requestUri,
            new StringContent(JsonSerializer.Serialize(request),
            Encoding.UTF8,
            TestConstants.JSON_CONTENT_TYPE),
            cancellationToken);

        var content = await response.Content.ReadAsStringAsync();

        var convertCurrencyResponse = JsonSerializer.Deserialize<ServiceResult<ExchangeRatesDto>>(content, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        });

        // Assert

        Assert.NotNull(convertCurrencyResponse);

        Assert.False(convertCurrencyResponse.Succeeded);

        Assert.Null(convertCurrencyResponse.Data);
    }

    private void ConvertCurrencyConfigureServices(bool invalidDetails)
    {
        var response = JsonHelper.GetRequestModelAsync<ExchangeRateResponse>(_testDataPath, ActionTypeEnum.ConvertResponse).Result;

        _mockCurrencyConverterService
                .Setup(d => d.ConvertCurrencyAsync(It.IsAny<ConvertCurrencyCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(invalidDetails ? default : response);

        _serviceCollection.AddSingleton(_mockCurrencyConverterService.Object);

        ConfigureServices();
    }
    #endregion

    #region Common

    private void ConfigureServices()
    {
        using var host = new HostBuilder()
            .ConfigureWebHost(webBuilder =>
            {
                webBuilder
                    .UseTestServer()
                    .ConfigureTestServices(services =>
                    {
                        services.AddRouting();

                        foreach (var serviceDescriptor in _serviceCollection)
                        {
                            services.Add(serviceDescriptor);
                        }

                        var feature = new ControllerFeature();
                        var assembly = typeof(CurrenciesController).Assembly; // Replace YourController with your controller type
                        var manager = new ApplicationPartManager();
                        manager.ApplicationParts.Add(new AssemblyPart(assembly));
                        manager.FeatureProviders.Add(new ControllerFeatureProvider());
                        manager.PopulateFeature(feature);

                        services.AddSingleton(feature);
                        services.AddControllers(options =>
                            options.Filters.Add<ApiExceptionFilterAttribute>())
                            .AddFluentValidation(x => x.AutomaticValidationEnabled = false)
                        .ConfigureApplicationPartManager(apm => apm.ApplicationParts.Add(new AssemblyPart(assembly)));

                        // Customise default API behaviour
                        services.Configure<ApiBehaviorOptions>(options =>
                        {
                            options.SuppressModelStateInvalidFilter = true;
                        });
                    })
                    .Configure(app =>
                    {

                        app.UseRouting();

                        app.UseEndpoints(endpoints =>
                        {
                            endpoints.MapControllers();
                        });
                    });
            })
            .StartAsync();

        _httpClient = host.Result.GetTestClient();
    }
    #endregion

    /// <summary>
    /// Mapster(Mapper) global configuration settings
    /// To learn more about Mapster,
    /// see https://github.com/MapsterMapper/Mapster
    /// </summary>
    /// <returns></returns>
    private static TypeAdapterConfig GetConfiguredMappingConfig()
    {
        var config = TypeAdapterConfig.GlobalSettings;

        IList<IRegister> registers = config.Scan(Assembly.GetExecutingAssembly());

        config.Apply(registers);

        return config;
    }
}