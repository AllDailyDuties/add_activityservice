using AllDailyDuties_ActivityService.Middleware;
using AllDailyDuties_ActivityService.Middleware.Messaging;
using AllDailyDuties_ActivityService.Middleware.Messaging.Interfaces;
using AllDailyDuties_ActivityService.Services;
using AllDailyDuties_ActivityService.Services.Interfaces;
using Microsoft.Azure.Cosmos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddScoped<IRabbitMQConsumer, RabbitMQConsumer>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddSingleton<CosmosClient>(serviceProvider =>
{
    IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();

    CosmosClientOptions cosmosClientOptions = new CosmosClientOptions
    {
        HttpClientFactory = httpClientFactory.CreateClient,
        ConnectionMode = ConnectionMode.Gateway
    };
    // TODO: Not hardcoded
    return new CosmosClient("AccountEndpoint=https://mydbskoen.documents.azure.com:443/;AccountKey=ateEj9j8zoUY10PaGRwpCPaafEkb9RcMzP3jceJQEp3kd5iL6NpOmH2Xpa9qKBjxZrRvG0La0jOTACDbMBhzng==", cosmosClientOptions);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
using var channel = RabbitMQConnection.Instance.Connection.CreateModel();
using (var scope = app.Services.CreateScope())
{
    var rabbiqMq = scope.ServiceProvider.GetRequiredService<IRabbitMQConsumer>();
    rabbiqMq.ConsumeMessage<List<string>>(channel, "new_activity");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run("http://+:9003");
