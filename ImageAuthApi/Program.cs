using ImageAuthApi.Models;
using ImageAuthApi.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton<ContractManager>();
builder.Services.AddSingleton<DataBaseManager>();
builder.Services.AddDbContext<HashDataContext>(opt => opt.UseInMemoryDatabase("HashData"));
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

app.Run();
