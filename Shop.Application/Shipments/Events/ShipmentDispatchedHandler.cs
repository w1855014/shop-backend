using Microsoft.Extensions.Logging;

namespace Shop.Application.Shipments.Events;

using Common.Interfaces;
using Domain.Events;

public class ShipmentDispatchedHandler : INotificationHandler<ShipmentDispatched>
{
    private readonly ILogger<ShipmentDeliveredHandler> _logger;
    private readonly IApplicationDbContext _context;

    public ShipmentDispatchedHandler(ILogger<ShipmentDeliveredHandler> logger, IApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public async Task Handle(ShipmentDispatched notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{@DateTime}: Started {@Request}",
            nameof(ShipmentDispatchedHandler), DateTime.UtcNow);
        
        var order = await _context.Orders.Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == notification.Shipment.OrderId, cancellationToken);
        
        if (order == null) throw new Exception();

        foreach (var item in order.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
            
            product?.AdjustStock(-item.Quantity);
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("{@DateTime}: Completed {@Request}",
            nameof(ShipmentDispatchedHandler), DateTime.UtcNow);
    }
}