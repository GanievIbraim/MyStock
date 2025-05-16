using MyStock.Entities;
using Microsoft.AspNetCore.Mvc;

namespace MyStock.Controllers
{
    [Route("api/contacts")]
    public class ContactsController
    {
        public ContactsController(AppDbContext context)
        {
        }
    }
}
