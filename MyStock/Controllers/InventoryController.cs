using MyStock.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MyStock.Controllers
{
    [Route("api/inventory")]
    public class InventoryController
    {
        public InventoryController(AppDbContext context)
        {
        }
    }
}
