using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoBazar.BLL.DTO;
using AutoBazar.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AutoBazar.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        public ProductsController(ApplicationDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string domain = (string)_configuration.GetValue<string>("AppHostName");
            List<ProductDTO> list = new List<ProductDTO>
            {
                new ProductDTO
                {
                    title="Vagabond sack",
                    url=$"{domain}android/0-0.jpg",
                    price="$120"
                },
                new ProductDTO
                {
                    title="Stella sunglasses",
                    url=$"{domain}android/1-0.jpg",
                    price="$58"
                }
            };
            return Ok(list);
        }

    }
}