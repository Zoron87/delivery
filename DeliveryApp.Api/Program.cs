using CSharpFunctionalExtensions;
using DeliveryApp.Api;
using DeliveryApp.Core.Application.Commands.AssignCourier;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Application.Commands.MoveCouriers;
using DeliveryApp.Core.Application.Queries.GetCouriers;
using DeliveryApp.Core.Application.Queries.GetCreatedAndAssignedOrders;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Primitives;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Health Checks
builder.Services.AddHealthChecks();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin(); // Не делайте так в проде!
        });
});

// Configuration
builder.Services.ConfigureOptions<SettingsSetup>();
builder.Services.AddTransient<IDispatchService, DispatchService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var connectionString = builder.Configuration["CONNECTION_STRING"];

// БД, ORM 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString,
        sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });
    options.EnableSensitiveDataLogging();
}
);

// Mediator
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// Commands
builder.Services.AddTransient<IRequestHandler<CreateOrderCommand, UnitResult<Error>>, CreateOrderCommandHandler>();
builder.Services.AddTransient<IRequestHandler<MoveCouriersCommand, UnitResult<Error>>, MoveCouriersCommandHandler>();
builder.Services.AddTransient<IRequestHandler<AssignCourierCommand, UnitResult<Error>>, AssignCourierCommandHandler>();

// Queries
builder.Services.AddTransient<IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersResponse>>(
    _ =>
        new GetCreatedAndAssignedOrdersHandler(connectionString));
builder.Services.AddTransient<IRequestHandler<GetCouriersQuery, GetCourierResponse>>(_ =>
    new GetCouriersQueryHandler(connectionString));



var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();

// Apply Migrations
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//     db.Database.Migrate();
// }

app.Run();