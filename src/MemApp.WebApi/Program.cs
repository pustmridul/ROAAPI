using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.SqlServer;
using MemApp.Application.DependencyResolver;
using MemApp.Application.Mem.Members.Command;
using MemApp.Application.Mem.Notification;
using MemApp.Infrastructure.DependencyResolver;
using MemApp.Infrastructure.Services;
using MemApp.WebApi;
using MemApp.WebApi.Middlewares;
using MemAppWebApi;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitMQ.Client;
using ResApp.Application.Com.Commands.CreateUser;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using FluentValidation;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });
builder.Services.AddSingleton<IConnectionFactory, ConnectionFactory>(sp =>
{
    var connectionFactory = new ConnectionFactory
    {
        // Configure RabbitMQ connection details
        Uri = new Uri("amqp://guest:guest@localhost:5672/")
    };
    return connectionFactory;
});

builder.Services.AddSingleton<IConnection>(sp =>
{
    var connectionFactory = sp.GetRequiredService<IConnectionFactory>();
    return connectionFactory.CreateConnection();
});

builder.Services.AddHttpContextAccessor();

var appSettings = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .Build();

builder.Services.AddTransient<SendSelectedMemberLedgerMailCommandHandler>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidator>();
//builder.Services.AddValidatorsFromAssemblyContaining<CreateMemberCommand>();

//builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());


builder.Services.AddSignalR();

builder.Services.ApplicationRegister();
builder.Services.AddContextInfrastructure(appSettings);
builder.Services.AddSharedUI(builder.Configuration);
builder.Services.AddWebUIServices();
builder.Services.InfrastructurRegister(appSettings);

builder.Services.AddEssentials();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHostedService<MonthlySyncService>();
//builder.Services.AddHostedService<DailyEmailService>();

builder.Services.AddHangfire(configuration => configuration
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
               .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
               .UseSimpleAssemblyNameTypeSerializer()
               .UseDefaultTypeSerializer()
               .UseSqlServerStorage(builder.Configuration.GetConnectionString("ApplicationConnection"), new SqlServerStorageOptions
               {
                   CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                   SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                   QueuePollInterval = TimeSpan.Zero,
                   UseRecommendedIsolationLevel = true,
                   DisableGlobalLocks = true
               }));
builder.Services.AddHangfireServer();

var firebaseCredentials = GoogleCredential.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configfirebase.json"));
FirebaseApp.Create(new AppOptions
{
    Credential = firebaseCredentials,
});




builder.Services.TryAdd(ServiceDescriptor.Singleton<IMemoryCache, MemoryCache>());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
        builder.WithOrigins("http://36.255.71.72:177/") // Frontend URL
               .AllowAnyMethod()
               .AllowAnyHeader());
});

//builder.Services.AddControllers();

builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("SuperAdminPolicy", p =>
    {
        p.RequireRole("SuperAdmin");
    });
    o.AddPolicy("AdminPolicy", policy =>
    {
        policy.RequireRole("Admin");
    });
    o.AddPolicy("BasicPolicy", p =>
    {
        p.RequireRole("Basic");
    });
});


//builder.Services.AddSwaggerGen(c =>
//{
//    c.OperationFilter<SwaggerFileUploadFilter>();
//});

builder.Services.AddEndpointsApiExplorer();

Log.Logger = new LoggerConfiguration()
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("ApplicationConnection"),
        sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true },
        restrictedToMinimumLevel: LogEventLevel.Information)
    .Enrich.FromLogContext()
    .CreateLogger();


//Log.Logger = new LoggerConfiguration()
//   .WriteTo.MSSqlServer(
//              connectionString: appSettings.GetConnectionString("ApplicationConnection"),
//              tableName: "Logs",
//              autoCreateSqlTable: true,
//              restrictedToMinimumLevel: LogEventLevel.Information).Enrich.FromLogContext()
//          .CreateLogger();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwaggerUI(c =>
    {
        c.ConfigObject.AdditionalItems.Add("persistAuthorization", "true");
    });
}


app.ConfigureSwagger();
app.UseHttpsRedirection();
app.UseMiddleware<CustomExceptionHandlerMiddleware>();





app.UseCors(x => x
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true)
               .AllowCredentials());

app.UseCors("AllowSpecificOrigin"); // Use CORS policy

app.UseRouting();



app.UseAuthentication();
app.UseAuthorization();
app.MapHub<NotificationHub>("/hub");
//app.UseMiddleware<CorsMiddleware>();
app.UseStaticFiles();
app.MapControllers();


app.Run();

