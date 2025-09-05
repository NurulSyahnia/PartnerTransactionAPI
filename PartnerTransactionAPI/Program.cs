using PartnerTransactionAPI.Utilities;
using PartnerTransactionAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Bind a list from configuration
builder.Services.Configure<List<PartnerConfig>>(builder.Configuration.GetSection("AllowedPartners"));

// register validator
builder.Services.AddScoped<SubmitTrxValidator>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net("log4net.config");

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
