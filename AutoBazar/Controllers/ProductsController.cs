using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoBazar.BLL.DTO;
using AutoBazar.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AutoBazar.Controllers
{

    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize]
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
            //List<ProductDTO> list = new List<ProductDTO>
            //{
            //    new ProductDTO
            //    {
            //        title="Vagabond sack",
            //        url=$"{domain}android/0-0.jpg",
            //        price="$120"
            //    },
            //    new ProductDTO
            //    {
            //        title="Stella sunglasses",
            //        url=$"{domain}android/1-0.jpg",
            //        price="$58"
            //    }
            //};
            var model = _context.Products
               .Select(p => new ProductDTO
               {
                   title = p.Title,
                   price = p.Price,
                   url = $"{domain}android/{p.Url}"
               }).ToList();

            Thread.Sleep(2000);
            return Ok(model);
        }
        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public IActionResult Create([FromBody]ProductCreateDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    invalid = "Не валідна модель"
                });
            }
            Random r = new Random();
            //var faker = new Faker();
            Product product = new Product
            {
                Title = model.title,
                Url = r.Next(1, 9).ToString() + "-0.jpg",
                Price = model.price
            };
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok(
            new
            {
                id = product.Id
            });
        }

    }
}