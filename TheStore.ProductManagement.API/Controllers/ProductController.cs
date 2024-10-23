
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Data;
using TheStore.ProductManagement.API.Authentication;
using TheStore.ProductManagement.API.Database;
using TheStore.ProductManagement.API.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TheStore.ProductManagement.API.Controllers
{

    [Route("api/[controller]")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [ProducesResponseType(500, Type = typeof(Error))]
    [ProducesResponseType(401, Type = typeof(Error))]
    public class ProductController : ControllerBase
    {
        private readonly IDatabaseService _dbService;
        private readonly IMapper _mapper;
        public ProductController(IDatabaseService dbService, IMapper mapper)
        {
            _dbService = dbService;
            _mapper = mapper;
        }


        [HttpGet("byName/{productName}")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400, Type = typeof(Error))]
        [ProducesResponseType(404, Type = typeof(Error))]
        [SwaggerOperation(Summary = "Retrieves product by its name", Description = "This endpoint allows retriving existing product from the store by its name.")]
        public async Task<IActionResult> GetByName(string productName)
        {

            var dbResults = await _dbService.GetProductDataFromDb(productName, null);

            if (dbResults.Status == StatusCodes.Status404NotFound)
                return NotFound(_mapper.Map<Error>(dbResults));

            
            return dbResults.Status != StatusCodes.Status200OK
                ?BadRequest(_mapper.Map<Error>(dbResults)) :
                Ok(dbResults.Data.FirstOrDefault());

        }

        [HttpGet("byId/{id}")]
        [ProducesResponseType(200, Type = typeof(Product))]
        [ProducesResponseType(400, Type = typeof(Error))]
        [ProducesResponseType(404, Type = typeof(Error))]
        [SwaggerOperation(Summary = "Retrieves product by its id", Description = "This endpoint allows retriving existing product from the store by its id.")]
        public async Task<IActionResult> GetById(int id)
        {
            var dbResults = await _dbService.GetProductDataFromDb(null, id);

            if (dbResults.Status == StatusCodes.Status404NotFound)
                return NotFound(_mapper.Map<Error>(dbResults));

            return dbResults.Status != StatusCodes.Status200OK
                ? BadRequest(_mapper.Map<Error>(dbResults)) :
                Ok(dbResults.Data.FirstOrDefault());
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(Product[]))]
        [ProducesResponseType(400, Type = typeof(Error))]
        [ProducesResponseType(404, Type = typeof(Error))]
        [SwaggerOperation(Summary = "Retrieves all products", Description = "This endpoint allows retriving all existing products from the store.")]
        public async Task<IActionResult> GetAll([Range(1, int.MaxValue)] int pageNumber = 1, [Range(1, 100)] int pageSize = 10)
        {
            var dbResults = await _dbService.GetAllProductDataFromDb((int)pageNumber, pageSize);

            if (dbResults.Status == StatusCodes.Status404NotFound)
                return NotFound(_mapper.Map<Error>(dbResults));

            return dbResults.Status != StatusCodes.Status200OK
                ? BadRequest(_mapper.Map<Error>(dbResults)) :
                Ok(dbResults.Data);
        }


        [HttpPost]
        [ProducesResponseType(200, Type = typeof(DbResults<string>))]
        [ProducesResponseType(400, Type = typeof(Error))]
        [SwaggerOperation(Summary = "Adds a new product", Description = "This endpoint allows adding a new product to the store.")]
        public async Task<IActionResult> Post([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest(new Error(StatusCodes.Status400BadRequest, new List<string?> { "Product cannot be null" }));
            }

             var dbResults = await _dbService.AddProductDataIntoDb(product);

            if (dbResults.Status != StatusCodes.Status200OK)
            {
                return StatusCode(dbResults.Status, new Error(dbResults.Status, new List<string?> { dbResults.Message }));
            }

            return Ok(dbResults);
        }

    }
}
