using Polly;
using Polly.Fallback;
using Polly.Retry;
using System.Net.Http;

namespace WebApi.PollyExtend
{
    public class ClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> ImmediateHttpRetry { get; }
        public AsyncFallbackPolicy<HttpRequestMessage> HttpFallback { get; }

        public ClientPolicy()
        {
            ImmediateHttpRetry = Policy
                .HandleResult<HttpResponseMessage>(res => !res.IsSuccessStatusCode)
                .RetryAsync(5);           
        }
    }
}
