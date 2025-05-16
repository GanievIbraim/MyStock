using MyStock.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MyStock.Controllers
{
    [Route("api/users")]
    public class UsersController 
    {
        public UsersController(AppDbContext context)
        {
        }
    }
}
