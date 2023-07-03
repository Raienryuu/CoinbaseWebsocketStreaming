
using Microsoft.EntityFrameworkCore;
using StreamingWithBackpressure.Connections;
using StreamingWithBackpressure.ResponseModels;

namespace StreamingWithBackpressure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration["Storage"];

            //builder.Services.AddDbContext<DatabaseContext>(options =>
            //options.UseSqlServer(connectionString));

            //var contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            //    .UseSqlServer(connectionString)
            //    .Options;

            //using var context = new DatabaseContext(contextOptions);

            //// Add services to the container.

            //builder.Services.AddControllersWithViews();

            //var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (!app.Environment.IsDevelopment())
            //{
            //    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            //    app.UseHsts();
            //}

            //app.UseHttpsRedirection();
            //app.UseStaticFiles();
            //app.UseRouting();

            //app.UseCors();
            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller}/{action=Index}/{id?}");

            //app.MapFallbackToFile("index.html");


            StatusConnection statusConnection = new();
            //Task task1 = Task.Factory.StartNew(statusConnection.TryGetDataFromWebSocketAsync);
            Level2Connection level2Connection = new();
            Task task2 = Task.Factory.StartNew(level2Connection.TryGetDataFromWebSocketAsync);
            Console.ReadKey();

            //app.Run();


        }

    }
}