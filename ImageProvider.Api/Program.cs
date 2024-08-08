using Azure.Storage.Blobs;
using ImageProvider.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Azure Blob Storage
builder.Services.AddSingleton(x =>
{
    var connectionString = Environment.GetEnvironmentVariable("BlobStorage_ConnectionString") ?? builder.Configuration.GetConnectionString("BlobStorage");
    return new BlobServiceClient(connectionString);
});

builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();