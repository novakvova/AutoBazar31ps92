using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoBazar.BLL.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutoBazar.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        public ProductsController()
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<ProductDTO> list = new List<ProductDTO>
            {
                new ProductDTO
                {
                    title="Vagabond sack",
                    url="http://10.0.2.2/android/0-0.jpg",
                    price="$120"
                },
                new ProductDTO
                {
                    title="Stella sunglasses",
                    url="http://10.0.2.2/android/1-0.jpg",
                    price="$58"
                }
            };
            return Ok(list);
        }

    }
}