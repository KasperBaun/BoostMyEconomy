

using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/* Setup Swagger/OpenAPI UI for debugging purposes - Learn more about configuring at https://aka.ms/aspnetcore/swashbuckle */
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/* Setup DBContext and add configuration options */
/*
builder.Services.AddDbContext<bmeContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString));
});
*/


//builder.Services.AddControllers();

/* After initial config, build the app */
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

/* CORS */
app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

//app.UseAuthentication();

//app.UseAuthorization();

//app.MapControllers();

app.Run();
