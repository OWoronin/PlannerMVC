using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;

namespace Pz_Proj_11_12
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSqlServer<PlannerContext>(builder.Configuration.GetConnectionString("Connection"),
                options => options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)); 



            var app = builder.Build();

            //here scope 
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                SeedData.Initialize(services);
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
