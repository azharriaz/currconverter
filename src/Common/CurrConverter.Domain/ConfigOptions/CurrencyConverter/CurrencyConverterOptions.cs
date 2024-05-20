namespace CurrConverter.Domain.ConfigOptions.CurrencyConverter
{
    public class CurrencyConverterOptions
    {
        public string BaseUrl { get; set; }
        public string[] CurrencyExclusions {  get; set; }
        public RetryPolicyOptions RetryPolicy { get; set; }
    }

    public class RetryPolicyOptions
    {
        public int MaxRetries { get; set; }
        public int RetryIntervalSeconds { get; set; }
        public int TimeoutSeconds { get; set; }
    }
}
