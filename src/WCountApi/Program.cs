using System.Security.Claims;
using System.Threading.RateLimiting;
using AlastairLundy.WCountLib.Extensions.DependencyInjection;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("fixed", config =>
    {
        config.PermitLimit = 10;
        config.Window = TimeSpan.FromMinutes(1);
        config.AutoReplenishment = true;
        config.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // options.AddPolicy("per-user", context  =>
    // {
    //     context.User.FindFirstValue("")
        
    //     return RateLimitPartition.GetFixedWindowLimiter("anonymous", _ => new FixedWindowRateLimiterOptions
    //     {
    //         PermitLimit = 10,
    //         Window = TimeSpan.FromMinutes(1),
    //         AutoReplenishment = true,
    //         QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
    //         QueueLimit = 10,
    //     });
    // });

    /*options.AddSlidingWindowLimiter("sliding", limiterOptions =>
    {
        limiterOptions.PermitLimit = 30;
        limiterOptions.Window = TimeSpan.FromMinutes(3);
        limiterOptions.AutoReplenishment = true;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = 100;
    });*/
});

builder.Services.AddWCount();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseRateLimiter();

app.Run();