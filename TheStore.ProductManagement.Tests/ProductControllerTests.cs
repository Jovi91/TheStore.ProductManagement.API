using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheStore.ProductManagement.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using TheStore.ProductManagement.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AutoMapper;
using FluentAssertions;
using TheStore.ProductManagement.API.Database;

namespace TheStore.ProductManagement.Tests;

public class ProductControllerTests
{
    private readonly ProductController _controller;
    private readonly Mock<IDatabaseService> _mockDbService;
    private readonly Mock<IMapper> _mockMapper;
    public ProductControllerTests()
    {
        _mockDbService = new Mock<IDatabaseService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new ProductController(_mockDbService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task GetbyName_NotExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var dbResults = new DbResults<Product[]>(Array.Empty<Product>(), StatusCodes.Status404NotFound, "Product not found");
        var expectedError = new Error (StatusCodes.Status404NotFound, new List<string> {"Product not found" });
        _mockDbService.Setup(x => x.GetProductDataFromDb(It.IsAny<string>(), null))
            .ReturnsAsync(dbResults);

        _mockMapper.Setup(m => m.Map<Error>(dbResults)) 
            .Returns(expectedError);

        // Act
        var result = await _controller.GetByName("Nonexistent Product");

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();  

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();            

        // Additional assertions
        notFoundResult?.StatusCode.Should().Be(StatusCodes.Status404NotFound); 
        notFoundResult?.Value.Should().BeEquivalentTo(expectedError);
    }

    [Fact]
    public async Task GetbyName_ProductsExist_ReturnOk()
    {
        // Arrange
        var name = "Test Product";
        var sampleProduct = new Product
        {
            Name = name,
            Description = "This is a test product.",
            Brand = "Test Brand",
            Prices = new List<Price>
            {
                new Price
                {
                    Currency = "PLN",
                    PriceValue = 10000,
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = DateTime.Now.AddDays(10)
                }
            }
        };

        var productArray = new Product[] { sampleProduct };
        var dbResults = new DbResults<Product[]>(productArray, StatusCodes.Status200OK, "Ok");

        _mockDbService.Setup(x => x.GetProductDataFromDb(It.IsAny<string>(), null))
            .ReturnsAsync(dbResults);

        _mockMapper.Setup(m => m.Map<Product>(It.IsAny<Product>()))
            .Returns(sampleProduct);

        // Act
        var result = await _controller.GetByName(name);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult?.Value.Should().BeOfType<Product>();  
        okResult?.Value.Should().BeEquivalentTo(sampleProduct);
    }

    [Fact]
    public async Task GetbyId_NotExistingProduct_ReturnsNotFound()
    {
        // Arrange
        var dbResults = new DbResults<Product[]>(Array.Empty<Product>(), StatusCodes.Status404NotFound, "Product not found");
        var expectedError = new Error(StatusCodes.Status404NotFound, new List<string> { "Product not found" });
        _mockDbService.Setup(x => x.GetProductDataFromDb(null, It.IsAny<int>()))
            .ReturnsAsync(dbResults);

        _mockMapper.Setup(m => m.Map<Error>(dbResults))
            .Returns(expectedError);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();

        notFoundResult?.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        notFoundResult?.Value.Should().BeEquivalentTo(expectedError);

    }

    [Fact]
    public async Task GetbyId_ProductsExist_ReturnOk()
    {
        // Arrange
        var name = "Test Product";
        var sampleProduct = new Product
        {
            Name = name,
            Description = "This is a test product.",
            Brand = "Test Brand",
            Prices = new List<Price>
            {
                new Price
                {
                    Currency = "PLN",
                    PriceValue = 10000,
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = DateTime.Now.AddDays(10)
                }
            }
        };

        var productArray = new Product[] { sampleProduct };
        var dbResults = new DbResults<Product[]>(productArray, StatusCodes.Status200OK, "Ok");

        _mockDbService.Setup(x => x.GetProductDataFromDb(null, It.IsAny<int>()))
            .ReturnsAsync(dbResults);

        _mockMapper.Setup(m => m.Map<Product>(It.IsAny<Product>()))
            .Returns(sampleProduct);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult?.Value.Should().BeOfType<Product>();
        okResult?.Value.Should().BeEquivalentTo(sampleProduct);

    }

    [Fact]
    public async Task GetAll_ProductsExist_ReturnsOk()
    {
        // Arrange
        var sampleProducts = new Product[]
        {
        new Product
        {
            Name = "Product 1",
            Description = "Description 1",
            Brand = "Brand 1",
            Prices = new List<Price>
            {
                new Price
                {
                    Currency = "USD",
                    PriceValue = 50,
                    StartDate = DateTime.Now.AddDays(-10),
                    EndDate = DateTime.Now.AddDays(5)
                }
            }
        },
        new Product
        {
            Name = "Product 2",
            Description = "Description 2",
            Brand = "Brand 2",
            Prices = new List<Price>
            {
                new Price
                {
                    Currency = "EUR",
                    PriceValue = 40,
                    StartDate = DateTime.Now.AddDays(-15),
                    EndDate = DateTime.Now.AddDays(3)
                }
            }
        }
        };

        var dbResults = new DbResults<Product[]>(sampleProducts, StatusCodes.Status200OK, "Ok");

    
        _mockDbService.Setup(x => x.GetProductDataFromDb(null, null))
            .ReturnsAsync(dbResults);

        _mockMapper.Setup(m => m.Map<Product[]>(It.IsAny<Product[]>()))
            .Returns(sampleProducts);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(sampleProducts); 
    }

    [Fact]
    public async Task Post_ValidProduct_ReturnsOk()
    {
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Brand = "Test Brand",
            Prices = new List<Price>
        {
            new Price
            {
                Currency = "PLN",
                PriceValue = 10000,
                StartDate = DateTime.Now.AddDays(-1),
                EndDate = DateTime.Now.AddDays(10)
            }
        }
        };


        var dbResults = new DbResults<string>("ProductId: 1", StatusCodes.Status200OK, "Product added successfully");

        _mockDbService.Setup(x => x.AddProductDataIntoDb(It.IsAny<Product>()))
            .ReturnsAsync(dbResults);  

        // Act
        var result = await _controller.Post(product);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<OkObjectResult>();

        var okResult = result as OkObjectResult;
        okResult?.Value.Should().BeEquivalentTo(dbResults);
    }

    [Fact]
    public async Task Post_NullProduct_ReturnsBadRequest()
    {
        // Arrange
        var expectedError = new Error(StatusCodes.Status400BadRequest, new List<string?> { "Product cannot be null" });

        // Act
        var result = await _controller.Post(null); 

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<BadRequestObjectResult>(); 

        var badRequestResult = result as BadRequestObjectResult; 
        badRequestResult?.Value.Should().BeEquivalentTo(expectedError); 
    }
    [Fact]
    public async Task Post_InternalServerError_Returns500()
    {
        // Arrange
        var product = new Product
        {
            Name = "Test Product",
            Description = "Test Description",
            Brand = "Test Brand",
            Prices = new List<Price>
            {
                new Price
                {
                    Currency = "PLN",
                    PriceValue = 10000,
                    StartDate = DateTime.Now.AddDays(-1),
                    EndDate = DateTime.Now.AddDays(10)
                }
            }
        };

        var dbResults = new DbResults<string>(null, StatusCodes.Status500InternalServerError, "Internal server error");

        _mockDbService.Setup(x => x.AddProductDataIntoDb(It.IsAny<Product>()))
            .ReturnsAsync(dbResults);

        // Act
        var result = await _controller.Post(product);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<ObjectResult>();

        var objectResult = result as ObjectResult;
        objectResult?.StatusCode.Should().Be(StatusCodes.Status500InternalServerError); 
    }
}
