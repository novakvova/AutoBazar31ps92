using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoBazar.BLL.DTO;
using AutoBazar.Entities;
using AutoBazar.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public ProductsController(ApplicationDbContext context,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
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
                   id = p.Id,
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
                var errrors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errrors);
            }

            var imageName = Path.GetRandomFileName() + ".jpg";
            string savePath = _env.ContentRootPath;
            string folderImage = "Uploading";
            savePath = Path.Combine(savePath, folderImage);
            savePath = Path.Combine(savePath, imageName);

            try
            {
                byte[] byteBuffer = Convert.FromBase64String(model.imageBase64);
                using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
                {
                    memoryStream.Position = 0;
                    using (Image imgReturn = Image.FromStream(memoryStream))
                    {
                        memoryStream.Close();
                        byteBuffer = null;
                        var bmp = new Bitmap(imgReturn);
                        bmp.Save(savePath, ImageFormat.Jpeg);
                    }
                }
            }
            catch
            {
                return BadRequest(new
                {
                    invalid = "Помилка обробки фото"
                });
            }

            Product product = new Product
            {
                Title = model.title,
                Url = imageName,
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

        [HttpDelete("delete/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int id)
        {
            var p = _context.Products.SingleOrDefault(x => x.Id == id);
            if (p != null)
            {
                var imageName = p.Url;
                string savePath = _env.ContentRootPath;
                string folderImage = "Uploading";
                savePath = Path.Combine(savePath, folderImage);
                savePath = Path.Combine(savePath, imageName);
                if (System.IO.File.Exists(savePath))
                {
                    System.IO.File.Delete(savePath);
                }
                _context.Products.Remove(p);
                _context.SaveChanges();
                return Ok();
            }
            else
            {
                return BadRequest(new
                {
                    invalid = "Такого продукта немає!"
                });
            }


        }

        [HttpGet("edit/{id}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit([FromRoute] int id)
        {
            var item = _context.Products.SingleOrDefault(x => x.Id == id);
            if (item != null)
            {
                string domain = (string)_configuration.GetValue<string>("BackendDomain");
                ProductEditDTO product = new ProductEditDTO()
                {
                    Id = item.Id,
                    price = item.Price,
                    title = item.Title,
                    url = $"{domain}android/{item.Url}"
                };
                return Ok(product);
            }
            else
            {
                return BadRequest(new
                {
                    invalid = "Не знайдено по даному id"
                });
            }
        }

        [HttpPut("edit")]
        [Authorize(Roles = "Admin")]
        public IActionResult Edit([FromBody]ProductEditDTO model)
        {
            if (!ModelState.IsValid)
            {
                var errrors = CustomValidator.GetErrorsByModel(ModelState);
                return BadRequest(errrors);
            }
            var item = _context.Products.SingleOrDefault(x => x.Id == model.Id);
            if (item != null)
            {
                if (model.imageBase64 != null)
                {
                    var imageName = item.Url;
                    string savePath = _env.ContentRootPath;
                    string folderImage = "Uploading";
                    savePath = Path.Combine(savePath, folderImage);
                    savePath = Path.Combine(savePath, imageName);
                    try
                    {
                        byte[] byteBuffer = Convert.FromBase64String(model.imageBase64);
                        using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
                        {
                            memoryStream.Position = 0;
                            using (Image imgReturn = Image.FromStream(memoryStream))
                            {
                                memoryStream.Close();
                                byteBuffer = null;
                                var bmp = new Bitmap(imgReturn);
                                bmp.Save(savePath, ImageFormat.Jpeg);
                            }
                        }
                    }
                    catch
                    {
                        return BadRequest(new
                        {
                            invalid = "Помилка обробки фото"
                        });
                    }
                }

                item.Price = model.price;
                item.Title = model.title;

                //_context.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();
                return Ok();

            }
            else
            {
                return BadRequest(new
                {
                    invalid = "Не знайдено по даному id"
                });
            }
        }

    }
}