using Application.Interfaces;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Infrastructure.Service;

public sealed class OrderServiceClient : IOrderServiceClient
{
    private const string KitchenReadyPath = "api/v1/orders/{0}/kitchen-ready";

    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderServiceClient> _logger;

    public OrderServiceClient(HttpClient httpClient, ILogger<OrderServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task NotifyOrderReadyAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsync(
                string.Format(KitchenReadyPath, orderId),
                content: null,
                cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogWarning("OrderService no encontro la orden {OrderId} al notificar kitchen-ready.", orderId);
                return;
            }

            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "No se pudo notificar a OrderService que la orden de cocina {OrderId} esta lista.", orderId);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "Timeout al notificar a OrderService que la orden de cocina {OrderId} esta lista.", orderId);
        }
    }
}
