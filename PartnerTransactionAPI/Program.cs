using PartnerTransactionAPI.Utilities;
using PartnerTransactionAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//builder.WebHost.ConfigureKestrel(options =>
//{
//    options.ListenAnyIP(int.Parse(port));
//});

// Add services to the container.

//builder.Services.Configure<PartnerConfig>(builder.Configuration.GetSection("PartnerConfig"));
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

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.ConfigureKestrel(o => o.ListenAnyIP(int.Parse(port)));


var app = builder.Build();

// Configure Kestrel to use Render's PORT (defaults to 8080)
//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//app.Urls.Add($"http://*:{port}");


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
