using System;
using System.Collections.Generic;
using System.Linq;
using MyRestaurantApp.Domain;
using MyRestaurantApp.Application.Features.Orders.Dtos;


namespace MyRestaurantApp.Application.Features.Orders.Mapping
{
    public static class OrderMappingExtensions
    {
        public static OrderResponse ToResponse(this Order order)
        {
            ArgumentNullException.ThrowIfNull(order);

            return new OrderResponse
            {
                Id = order.Id,
                Status = order.status.ToString(),
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,

                CustomerId = order.CustomerId,
                CustomerName = order.Customer != null
                    ? $"{order.Customer.FirstName} {order.Customer.LastName}"
                    : string.Empty,

                RestaurantId = order.RestaurantId,
                RestaurantName = order.Restaurant?.Name ?? string.Empty,

                OrderItems = order.OrderItems?
                    .Select(oi => new OrderItemsResponse
                    {
                          Id = oi.Id,     
                        ProductId = oi.ProductId,
                        ProductName = oi.Product?.Name ?? string.Empty,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice
                    })
                    .ToList() ?? new List<OrderItemsResponse>()
            };
        }

      
        public static Order ToEntity(this CreateOrderRequest request, Guid customerId, List<Product> products)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(products);

            var orderItems = request.OrderItems.Select(item =>
            {
                var product = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (product == null)
                {
                    throw new Exception($"Product {item.ProductId} not found.");
                }

                return new OrderItem
                {
                    Id = Guid.NewGuid(),
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                };
            }).ToList();

            return new Order
            {
                Id = Guid.NewGuid(),
                status = statusTypes.confirm,
                OrderDate = DateTime.UtcNow,
                CustomerId = customerId,
                RestaurantId = request.RestaurantId,
                OrderItems = orderItems,
                TotalPrice = orderItems.Sum(oi => oi.UnitPrice * oi.Quantity)
            };
        }

                public static void UpdateStatus(this Order order, UpdateOrderRequest request)
        {
            ArgumentNullException.ThrowIfNull(order);
            ArgumentNullException.ThrowIfNull(request);

            order.status = request.Status;
        }

        // 4. Convert a collection of Order Entities -> OrderResponse list
        public static IEnumerable<OrderResponse> ToResponseList(this IEnumerable<Order> orders)
        {
            return orders.Select(o => o.ToResponse());
        }
    }
}