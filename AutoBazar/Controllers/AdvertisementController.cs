using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementController : ControllerBase
    {
        // GET Advertisement list
        //[HttpGet]
        //public IEnumerable<AdvertisementDTO> GetAllAdvertisement()
        //{
        //    return new string[] { "Advertisement1", "Advertisement2" };
        //}


        //// GET User Advertisement
        //[HttpGet("{id}")]
        //public IEnumerable<AdvertisementDTO> GetUserAdvertisement(int id)
        //{
        //    return "Advertisement";
        //}

        // add Advertisement
        [HttpPost]
        public void AdvertisementPost([FromBody] string value)
        {
        }

        // edit Advertisement
        [HttpPut("{id}")]
        public void EditAdvertisement(int id, [FromBody] string value)
        {
        }

        // DELETE Advertisement
        [HttpDelete("{id}")]
        public void DeleteAdvertisement(int id)
        {
        }
    }
}
