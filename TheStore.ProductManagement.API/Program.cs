using Microsoft.AspNetCore.Connections;
using TheStore.ProductManagement.API;
using TheStore.ProductManagement.API.Controllers;
using Insight.Database;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            // Collect all error messages
            var errorMessages = context.ModelState
                .SelectMany(v => v.Value.Errors.Select(e => e.ErrorMessage))
                .ToList();

            // Create a response that includes all error messages
            var response = new
            {
                status = StatusCodes.Status400BadRequest,
                messages = errorMessages // Include all error messages
            };

            return new BadRequestObjectResult(response);
        };
    });
    //.ConfigureApiBehaviorOptions(options =>
    //{
    //    options.SuppressModelStateInvalidFilter = true;
    //});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IProductService, ProductService>();
//builder.Services.AddScoped<ProductService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();
