using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class KitchenOrchestratorRepository : IKitchenOrchestratorRepository
    {
        private readonly ApplicationDbContext _context;

        public KitchenOrchestratorRepository(ApplicationDbContext context)
        {
            _context = context;
        }




    }
}
