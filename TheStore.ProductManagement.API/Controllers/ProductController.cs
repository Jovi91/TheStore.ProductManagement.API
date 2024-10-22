
using Microsoft.AspNetCore.Mvc;
using TheStore.ProductManagement.API.Authentication;
using TheStore.ProductManagement.API.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TheStore.ProductManagement.API.Controllers
{

    [Route("api/[controller]")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
   // [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IDatabaseService _dbService;
        public ProductController(IDatabaseService dbService)
        {
            _dbService = dbService;
        }


        [HttpGet("byName/{productName}")]
        public async Task<ActionResult<Product>> GetbyName(string productName)
        {

            var dbResults = await _dbService.GetProductDataFromDb(productName, null);

            if (dbResults.Status == StatusCodes.Status404NotFound)
                return NotFound(dbResults);

            
                

            return dbResults.Status != 200
                ?BadRequest(dbResults) :
                Ok(dbResults.Data.FirstOrDefault());

        }

        [HttpGet("byId/{id}")]
        public async Task<ActionResult<Product>> GetById(int id)
        {
            var dbResults = await _dbService.GetProductDataFromDb(null, id);

            if (dbResults.Status == 404)
                return NotFound(dbResults);

            return dbResults.Status != 200
                ? BadRequest(dbResults) :
                Ok(dbResults.Data.FirstOrDefault());

        }

        [HttpGet]
        public async Task<ActionResult<Product>> Get()
        {
            var dbResults = await _dbService.GetProductDataFromDb(null, null);

            if (dbResults.Status == 404)
                return NotFound(dbResults);

            return dbResults.Status != 200
                ? BadRequest(dbResults) :
                Ok(dbResults.Data);

        }


        [HttpPost]
        public async Task<ActionResult<DbResults<int>>> Post([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest(new DbResults<string>(null, 400, "Product cannot be null"));
            }


             var dbResults = await _dbService.AddProductDataIntoDb(product);

            return dbResults.Status != 200
            ? BadRequest(dbResults) :
            Ok(dbResults);
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
