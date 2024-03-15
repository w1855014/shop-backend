namespace Shop.Application.Products.Queries;

using Domain.Entities;

public class GetAllProductsQuery : IRequest<Result<List<Product>>>
{
    public required string? SortBy { get; init; }
    public required string? OrderBy { get; init; }
    public required int Page { get; init; }
    public required int Size { get; init; }
}