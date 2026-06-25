using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Infrastructure.Service
{
    public class OrderServiceClient : IOrderServiceClient
    {
        private readonly HttpClient _httpClient;

        public OrderServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task NotifyOrderReady(Guid orderId)
        {
            var response = await _httpClient.PostAsync(
                $"api/orders/{orderId}/ready",   // ver bien como es el endpoint de order
                null);

            response.EnsureSuccessStatusCode();
        }
    }
}
