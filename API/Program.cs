using API.Extensions;
using Lib.Repository.Repository;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureCors();
builder.Services.AddApplicationServices();
builder.Services.ConfigureServices(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddScoped<IBattleOfMonstersRepository, BattleOfMonstersRepository>();

WebApplication app = builder.Build();
app.UseCors("CorsPolicy");
app.UseAuthorization();
app.MapControllers();
app.Run();
