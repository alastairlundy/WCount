/*
    WCountLib
    Copyright (C) 2024-2026 Alastair Lundy

    This Source Code Form is subject to the terms of the Mozilla Public
    License, v. 2.0. If a copy of the MPL was not distributed with this
    file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

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