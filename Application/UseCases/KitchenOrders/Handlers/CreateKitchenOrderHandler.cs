using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.UseCases.KitchenOrders.Comands;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.UseCases.KitchenOrders.Handlers
{
    public class CreateKitchenOrderHandler : ICreateKitchenOrderHandler
    {
        private readonly IKitchenOrderRepository _repository;

        public CreateKitchenOrderHandler(IKitchenOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<KitchenOrder> CreateKitchenOrder(CreateKitchenOrderCommand command)
        {
            // 1. Validar que haya items
            if (command.Items == null || !command.Items.Any())
            {
                throw new ValidationExceptions(new Dictionary<string, string[]>
            {
                { "items", new[] { "La orden debe contener al menos un item." } }
            });
            }

            // 2. Validar número de mesa
            if (command.TableNumber <= 0)
            {
                throw new ValidationExceptions(new Dictionary<string, string[]>
            {
                { "tableNumber", new[] { "El número de mesa debe ser mayor a 0." } }
            });
            }


            // 4. Validar cada item
            var validationErrors = new Dictionary<string, string[]>();
            for (int i = 0; i < command.Items.Count; i++)
            {
                var item = command.Items[i];
                var itemErrors = new List<string>();

                if (item.ProductId == Guid.Empty)
                    itemErrors.Add("El ProductId es obligatorio.");

                if (string.IsNullOrWhiteSpace(item.ProductName))
                    itemErrors.Add("El nombre del producto es obligatorio.");

                if (item.DurationMinutes <= 0)
                    itemErrors.Add("El tiempo estimado debe ser mayor a 0 minutos.");

                if (itemErrors.Any())
                {
                    validationErrors[$"items[{i}]"] = itemErrors.ToArray();
                }
            }

            if (validationErrors.Any())
            {
                throw new ValidationExceptions(validationErrors);
            }
            // 1. Crear la orden principal
            var order = new KitchenOrder
            {
                Id = Guid.NewGuid(),
                OrderId = command.OrderId,
                TableNumber = command.TableNumber,
                WaiterId = command.WaiterId,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                TotalItems = command.Items.Count,
                CompletedItems = 0,
                LastUpdatedAt = DateTime.UtcNow,
                Items = new List<KitchenOrderItem>()
            };

            // 2. Crear los items de la orden
            foreach (var itemDto in command.Items)
            {
                var item = new KitchenOrderItem
                {
                    Id = Guid.NewGuid(),
                    KitchenOrderId = order.Id,
                    ProductId = itemDto.ProductId,
                    ProductName = itemDto.ProductName,
                    EstimatedTime = itemDto.DurationMinutes,
                    StartTime = null,
                    FinishTime = null,
                    Status = ItemStatus.Pending,
                    PriorityScore = 0, // Se calcula en el algoritmo
                    Position = 0,
                    Notes = itemDto.Notes,
                    IsRushed = false
                };

                order.Items.Add(item);
            }

            // 3. APLICAR EL ALGORITMO DE SINCRONIZACIÓN (Motor de Orquestación)
            CalculateSyncTimes(order.Items.ToList());

            // 4. Calcular el tiempo estimado de finalización de toda la orden
            var maxTime = order.Items.Max(i => i.EstimatedTime);
            order.EstimatedFinishTime = DateTime.UtcNow.AddMinutes(maxTime);

            // 5. Guardar en la base de datos
            return await _repository.CreateAsync(order);
        }

       
        /// Motor de Orquestación: calcula cuándo empezar cada plato
        /// para que todos terminen al mismo tiempo (sincronización de mesa).
       
        private void CalculateSyncTimes(List<KitchenOrderItem> items)
        {
            if (!items.Any()) return;

            // Paso 1: Encontrar el plato más lento (el que más tiempo tarda)
            var maxTime = items.Max(i => i.EstimatedTime);

            // Paso 2: Calcular el tiempo objetivo de finalización
            // Todos los platos de la mesa deben estar listos en este momento
            var targetFinishTime = DateTime.UtcNow.AddMinutes(maxTime);

            // Paso 3: Asignar StartTime y PriorityScore a cada item
            foreach (var item in items)
            {
                // El StartTime es: Target - lo que tarda mi plato
                // Ej: Si el target es 20:15 y mi plato tarda 8 min → empiezo a las 20:07
                item.StartTime = targetFinishTime.AddMinutes(-item.EstimatedTime);

                // a más tiempo, más prioridad
                // Esto hace que los platos lentos aparezcan primero en el KDS
                item.PriorityScore = item.EstimatedTime;
            }
        }
    }
}
