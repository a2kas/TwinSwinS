using Microsoft.EntityFrameworkCore;
using TwinsWins.Data;
using TwinsWins.Data.Repository;
using TwinsWins.Hubs;
using TwinsWins.Middleware;
using TwinsWins.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContext<DatabseContext>(options =>
       options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITonWalletService, TonWalletService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddSingleton(new ImageService(@"C:\images"));
builder.Services.AddHttpContextAccessor();


var app = builder.Build();
app.UseMiddleware<ImageMiddleware>(@"C:\images"); 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapHub<GameHub>("/gamehub");
app.MapFallbackToPage("/_Host");

app.Run();
