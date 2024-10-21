
using Microsoft.AspNetCore.Mvc;
using System;
using System.Runtime.CompilerServices;
using TheStore.ProductManagement.API.Model;
using TheStore.ProductManagement.API.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TheStore.ProductManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _dbService;
        public ProductController(IProductService dbService)
        {
            _dbService = dbService;
        }


        [HttpGet("byName/{productName}")]
        public async Task<ActionResult<Product>> GetbyName(string productName)
        {

            var dbResults = await _dbService.GetProductDataFromDb(productName, null);

            if (dbResults.Status == 404)
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
            //if (!ModelState.IsValid)
            //{

            //    // Collect validation errors
            //    var errors = ModelState
            //        .Where(e => e.Value.Errors.Count > 0)
            //        .Select(e => new
            //        {
            //            Property = e.Key,
            //            Messages = e.Value.Errors.Select(error => error.ErrorMessage)
            //        });

            //    // Return validation errors in your custom format
            //    return BadRequest(new DbResults<string>(null, 400, "Validation errors occurred.")
            //    {
            //        Data = errors.ToString() // You can format this as needed
            //    });
            //}

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
