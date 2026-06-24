using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.service
{
    public class KitchenOrchestrator : IKitchenOrchestrator
    {
        public async Task EnqueueOrderAsync(Guid kitchenOrderId)
        {
            //encolara o no en la lista de items la orden 
        }

        public async Task GetItemsFromQueueAsync()
        {
            //devolvera la lista de items limitada por MaxConcurrentDishes en la entidad kitchen configuration
        }


    }
}
