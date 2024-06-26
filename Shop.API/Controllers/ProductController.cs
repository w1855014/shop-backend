using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Shop.API.Controllers;

using Mappings;
using Extensions;
using Models.Requests;
using Application.Products.Commands;
using Application.Products.Queries;

[ApiController]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("/api/products/{id}")]
    public async Task<IActionResult> GetProduct([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var query = new GetProductByIdQuery { Id = id };
        
        var result = await _mediator.Send(query, cancellationToken);
        
        return result.IsSuccess ? Ok(result.Value.ToResponse()) : result.ToProblem();
    }
    
    [HttpGet("/api/products")]
    public async Task<IActionResult> GetAllProducts([FromQuery] GetAllProductsRequest request,
        CancellationToken cancellationToken)
    {
        var query = new GetAllProductsQuery
        {
            SortBy = request.Sort,
            OrderBy = request.Order,
            Page = request.Page,
            Size = request.Size
        };
        
        var result = await _mediator.Send(query, cancellationToken);

        var response = result.Value.ToResponse();
        
        return Ok(response);
    }

    [HttpGet("/api/categories/{idOrSlug}/products")]
    public async Task<IActionResult> GetProductsByCategory([FromRoute] string idOrSlug,
        [FromQuery] GetAllProductsRequest request, CancellationToken cancellationToken)
    {
        var query = new GetProductsByCategoryQuery
        {
            IdOrSlug = idOrSlug,
            SortBy = request.Sort,
            OrderBy = request.Order,
            Page = request.Page,
            Size = request.Size
        };
        
        var result = await _mediator.Send(query, cancellationToken);

        var response = result.Value.ToResponse();
        
        return Ok(response);
    }

    [Authorize(Policy = "RequireAdmin")]
    [HttpPost("/api/products")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateProductCommand
        {
            Name = request.Name,
            Stock = request.Stock,
            Price = request.Price,
            CategoryIds = request.CategoryIds
        };
        
        var result = await _mediator.Send(command, cancellationToken);

        if (result.IsFailed) return result.ToProblem();
        
        var product = result.Value.ToResponse();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }
    
    [Authorize(Policy = "RequireAdmin")]
    [HttpPut("/api/products/{id}")]
    public async Task<IActionResult> UpdateProduct([FromRoute] Guid id,
        [FromBody] UpdateProductRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateProductCommand
        {
            Id = id,
            Name = request.Name,
            Stock = request.Stock,
            Price = request.Price,
            CategoryIds = request.CategoryIds
        };
        
        var result = await _mediator.Send(command, cancellationToken);
        
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    
    [Authorize(Policy = "RequireAdmin")]
    [HttpDelete("/api/products/{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var command = new DeleteProductCommand(id);

        var result = await _mediator.Send(command, cancellationToken);
        
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}