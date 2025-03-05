using Api.Filters;
using Api.OpenApi;
using CSharpFunctionalExtensions;
using DeliveryApp.Api;
using DeliveryApp.Api.Adapters.BackgroundJobs;
using DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;
using DeliveryApp.Core.Application.Commands.AssignCourier;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Core.Application.Commands.MoveCouriers;
using DeliveryApp.Core.Application.Queries.GetCouriers;
using DeliveryApp.Core.Application.Queries.GetCreatedAndAssignedOrders;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Grpc.GeoService;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Primitives;
using Quartz;
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
builder.Services.AddTransient<IOrderRepository, OrderRepository>();
builder.Services.AddTransient<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
// gRPC
builder.Services.AddTransient<IGeoClient, GeoClient>();

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

// CRON Jobs
builder.Services.AddQuartz(configure =>
{
    var assignOrdersJobKey = new JobKey(nameof(AssignOrdersJob));
    var moveCouriersJobKey = new JobKey(nameof(MoveCouriersJob));
    configure
        .AddJob<AssignOrdersJob>(assignOrdersJobKey)
        .AddTrigger(
            trigger => trigger.ForJob(assignOrdersJobKey)
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInSeconds(1)
                        .RepeatForever()))
        .AddJob<MoveCouriersJob>(moveCouriersJobKey)
        .AddTrigger(
            trigger => trigger.ForJob(moveCouriersJobKey)
                .WithSimpleSchedule(
                    schedule => schedule.WithIntervalInSeconds(2)
                        .RepeatForever()));
    configure.UseMicrosoftDependencyInjectionJobFactory();
});

builder.Services.AddQuartzHostedService();


// Message Broker Consumer
builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
    options.ShutdownTimeout = TimeSpan.FromSeconds(30);
});
builder.Services.AddHostedService<ConsumerService>();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("1.0.0", new OpenApiInfo
    {
        Title = "Basket Service",
        Description = "Отвечает за формирование корзины и оформление заказа",
        Contact = new OpenApiContact
        {
            Name = "Kirill Vetchinkin",
            Url = new Uri("https://microarch.ru"),
            Email = "info@microarch.ru"
        }
    });
    options.CustomSchemaIds(type => type.FriendlyId(true));
    options.IncludeXmlComments(
        $"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}{Assembly.GetEntryAssembly()?.GetName().Name}.xml");
    options.DocumentFilter<BasePathFilter>("");
    options.OperationFilter<GeneratePathParamsValidationFilter>();
});
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddControllers(); 

var app = builder.Build();

// -----------------------------------

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseSwagger(c => { c.RouteTemplate = "openapi/{documentName}/openapi.json"; })
    .UseSwaggerUI(options =>
    {
        options.RoutePrefix = "openapi";
        options.SwaggerEndpoint("/openapi/1.0.0/openapi.json", "Swagger Basket Service");
        options.RoutePrefix = string.Empty;
        options.SwaggerEndpoint("/openapi-original.json", "Swagger Basket Service");
    });

app.UseCors();
app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

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