using Microsoft.EntityFrameworkCore;
using Coati_Space_Project.Models;

namespace Coati_Space_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(connection));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "zoo_coati",
                pattern: "stav-zoo/coati",
                defaults: new { controller = "Zoo", action = "Coati" })
                .WithStaticAssets();

            app.MapControllerRoute(
                name: "zoo_main",
                pattern: "stav-zoo",
                defaults: new { controller = "Zoo", action = "Index" })
                .WithStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Zoo}/{action=Coati}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
