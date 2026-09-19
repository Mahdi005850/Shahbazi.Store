using Microsoft.EntityFrameworkCore;
using Shahbazi.Store.Data;
using Shahbazi.Store.DTOs;
using Shahbazi.Store.Enums;
using Shahbazi.Store.MiddleWare;
using Shahbazi.Store.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ProductServices>();
builder.Services.AddScoped<CartServices>();
builder.Services.AddScoped<OrderServices>();
builder.Services.AddScoped<UserServices>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("ShahbaziStoreDb"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddValidation();
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleWare>();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Shahbazi Store API Is running !!");
app.MapGet("/api/products", (ProductServices productServices) =>
{
    return productServices.GetAllProducts();
});

app.MapPost("/api/products", (CreateProductDto dto, ProductServices productServices) =>
{
    productServices.AddProduct(dto.ProductName, dto.ProductPrice, dto.ProductDescription, dto.Stock);
    return Results.Ok("Product Created Successfully !!");
});
app.MapGet("/api/products/{id}", (int id, ProductServices productServices) =>
{
    var product = productServices.GetProduct(id);
    if (product == null)
    {
        return Results.NotFound("Product didn't find !!");
    }
    return Results.Ok(product);
});
app.MapGet("/api/products/search", (string search, ProductServices productServices) =>
{
    return productServices.SearchProducts(search);
});
app.MapGet("/api/products/filter", (decimal? minPrice, decimal? maxPrice, ProductServices productServices) =>
{
    var products = productServices.FilterProducts(minPrice, maxPrice);
    return Results.Ok(products);
});
app.MapGet("api/products/Sort", (bool ascending, ProductServices productServices) =>
{
    var products = productServices.SortProducts(ascending);
    return Results.Ok(products);
});
app.MapPut("/api/products/{id}", (int id, UpdateProductDto dto, ProductServices productServices) =>
{
    try
    {
        productServices.UpdateProduct(id, dto.ProductName, dto.ProductPrice, dto.ProductDescription);
        return Results.Ok("Product Updated Successfully !!");
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
});
app.MapDelete("/api/products/{id}", (int id, ProductServices productServices) =>
{
    try
    {
        productServices.DeleteProduct(id);
        return Results.Ok("Product Deleted Successfully !!");
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
});
app.MapGet("/api/products/paged", (int page, int pageSize, ProductServices productServices) =>
{
    var result = productServices.GetProducts(page, pageSize);
    return Results.Ok(result);
});

app.MapPost("/api/users", (UserServices userServices, string fullName, string phoneNumber, string address) =>
{
    userServices.AddUser(fullName, phoneNumber, address);
    return Results.Ok("User Created Successfully !!");
});
app.MapGet("/api/users", (UserServices userServices) =>
{
    return Results.Ok(userServices.GetAllUsers());
});
app.MapGet("/api/users/{id}", (int id, UserServices userServices) =>
{
    var user = userServices.GetUser(id);

    if (user == null)
    {
        return Results.NotFound("User didn't find !!");
    }

    return Results.Ok(user);
});
app.MapPut("/api/users/{id}", (int id, string fullName, string phoneNumber, string address, UserServices userServices) =>
{
    try
    {
        userServices.UpdateUser(id, fullName, phoneNumber, address);
        return Results.Ok("User Updated Successfully !!");
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
});
app.MapDelete("/api/users/{id}", (int id, UserServices userServices) =>
{
    try
    {
        userServices.DeleteUser(id);
        return Results.Ok("User Deleted Successfully !!");
    }
    catch (InvalidOperationException ex)
    {
        return Results.NotFound(ex.Message);
    }
});

app.MapPost("/api/carts/{userId}", (int userId, CartServices cartServices) =>
{
    var cart = cartServices.CreatCart(userId);
    return Results.Ok(cart);
});
app.MapGet("/api/carts/{userId}", (int userId, CartServices cartServices) =>
{
    var cart = cartServices.GetCart(userId);

    if (cart == null)
    {
        return Results.NotFound("Cart didn't find !!");
    }

    return Results.Ok(cart);
});

app.MapPost("/api/carts/{userId}/items",
    (int userId, int productId, int quantity, CartServices cartServices) =>
    {
        cartServices.AddToCart(userId, productId, quantity);

        return Results.Ok("Product Added To Cart Successfully !!");
    });
app.MapDelete("/api/carts/{userId}/items/{productId}",
    (int userId, int productId, CartServices cartServices) =>
    {
        cartServices.RemoveFromCart(userId, productId);

        return Results.Ok("Product Removed From Cart Successfully !!");
    });
app.MapPut("/api/carts/{userId}/items/{productId}/increase",
    (int userId, int productId, int amount, CartServices cartServices) =>
    {
        cartServices.IncreaseQuantity(userId, productId, amount);

        return Results.Ok("Cart Item Quantity Increased Successfully !!");
    });
app.MapPut("/api/carts/{userId}/items/{productId}/decrease",
    (int userId, int productId, int amount, CartServices cartServices) =>
    {
        cartServices.DecreaseQuantity(userId, productId, amount);

        return Results.Ok("Cart Item Quantity Decreased Successfully !!");
    });
app.MapDelete("/api/carts/{userId}/items",
    (int userId, CartServices cartServices) =>
    {
        cartServices.ClearCart(userId);

        return Results.Ok("Cart Cleared Successfully !!");
    });

app.MapPost("/api/orders/{userId}",
    (int userId, string shippingAddress, OrderServices orderServices) =>
    {
        var order = orderServices.CreateOrder(userId, shippingAddress);
        return Results.Ok(order);
    });
app.MapGet("/api/orders/{id}",
    (int id, OrderServices orderServices) =>
    {
        var order = orderServices.GetOrder(id);
        if (order == null)
        {
            return Results.NotFound("Order didn't find !!");
        }
        return Results.Ok(order);
    });
app.MapGet("/api/users/{userId}/orders",
    (int userId, OrderServices orderServices) =>
    {
        return Results.Ok(orderServices.GetUserOrder(userId));
    });
app.MapPut("/api/orders/{id}/cancel",
    (int id, OrderServices orderServices) =>
    {
        try
        {
            orderServices.CancelOrder(id);
            return Results.Ok("Order Cancelled Successfully !!");
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(ex.Message);
        }
    });
app.MapPut("/api/orders/{id}/status",
    (int id, OrderStatus status, OrderServices orderServices) =>
    {
        try
        {
            orderServices.ChangeOrderStatus(id, status);

            return Results.Ok("Order Status Changed Successfully !!");
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(ex.Message);
        }
    });

app.Run();