using API.Infrastructure.DI;
using API.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddIdentityConfiguration(builder.Configuration);

builder.Services.AddScoped<IJwtFactory, JwtFactory>();
builder.Services.AddScoped<IJwt, Jwt>();
builder.Services.AddScoped<JwtIssuerOptions>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
