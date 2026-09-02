using MyRestaurantApp.Domain;

using MyRestaurantApp.Application.Features.Products.Dtos; 

namespace MyRestaurantApp.Application.Features.Products.Mapping
{
    public static class ProductMappingExtensions
    {
        
        public static ProductResponse ToResponse(this Product product)
        {
            ArgumentNullException.ThrowIfNull(product);

            return new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                IsAvailable = product.IsAvailable,
               RestaurantId = product.RestaurantId, 

               //restaurant has a list of products
               //hrbot elproduct b elrestaurant cz mmkn yb2a mt3m m3ndosh nfss elproduct
               //elrestaurant marbot b elcategory ?
            
                CategoryId = product.CategoryId,
                
                CategoryName = product.Category?.Name ?? string.Empty 
            };
        }

      

        public static Product ToEntity(this CreateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            return new Product
            {
                Id = Guid.NewGuid(), 
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                Price = request.Price,
                IsAvailable = true,
                RestaurantId = request.RestaurantId,
                CategoryId = request.CategoryId
            };
        }

      
        public static void UpdateEntity(this Product product, UpdateProductRequest request)
        {
            ArgumentNullException.ThrowIfNull(product);
            ArgumentNullException.ThrowIfNull(request);

            product.Name = request.Name.Trim();
            product.Description = request.Description?.Trim() ?? product.Description;
            product.Price = request.Price;
            product.IsAvailable = request.IsAvailable;
            product.CategoryId = request.CategoryId;
            
        }

        public static IEnumerable<ProductResponse> ToResponseList(this IEnumerable<Product> products)
        {
            return products.Select(p => p.ToResponse());
        }

        
    }
}