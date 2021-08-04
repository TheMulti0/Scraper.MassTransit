using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Scraper.RabbitMq.Common;

namespace Scraper.RabbitMq.Client
{
    internal class SubscriptionsRestClient : ISubscriptionsClient
    {
        private readonly HttpClient _httpClient;

        public SubscriptionsRestClient(
            Uri serverUri,
            HttpClient httpClient = null)
        {
            _httpClient = httpClient;
            _httpClient ??= new HttpClient();

            _httpClient.BaseAddress = new Uri(serverUri, "/subscriptions/");
        }
        
        public async Task<IEnumerable<Subscription>> GetSubscriptionsAsync(CancellationToken ct)
        {
            var subscriptions = await _httpClient.GetFromJsonAsync<List<Subscription>>(
                string.Empty,
                ct);

            return subscriptions;
        }

        public async Task SubscribeAsync(
            string platform,
            string id,
            TimeSpan pollInterval,
            CancellationToken ct)
        {
            var url = $"{platform}/{id}?pollInterval={pollInterval}";

            HttpResponseMessage response = await _httpClient.PostAsync(
                url,
                null!,
                ct);

            response.EnsureSuccessStatusCode();
        }

        public async Task UnsubscribeAsync(
            string platform,
            string id,
            CancellationToken ct)
        {
            var url = $"{platform}/{id}";

            HttpResponseMessage response = await _httpClient.DeleteAsync(
                url,
                ct);

            response.EnsureSuccessStatusCode();
        }
    }
}