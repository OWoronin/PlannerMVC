using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Pz_Proj_11_12.Data;
using Pz_Proj_11_12.Handlers;
using Pz_Proj_11_12.Utils;

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

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            builder.Services.AddAuthentication("Cookie").AddCookie("Cookie", options =>
            {
                options.LoginPath = "/Users/Login";
                options.AccessDeniedPath = "/Users/AccessDenied";
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(HanderNames.Planner, policy => policy.Requirements.Add(new IsPlannerOwnerRequirement()));
                options.AddPolicy(HanderNames.Task, policy => policy.Requirements.Add(new IsTaskOwnerRequirement()));
                options.AddPolicy(HanderNames.Reminder, policy => policy.Requirements.Add(new IsReminderOwnerRequirement()));
                options.AddPolicy(HanderNames.Meeting, policy => policy.Requirements.Add(new IsMeetingOwnerRequirement()));
                options.AddPolicy(HanderNames.Day, policy => policy.Requirements.Add(new IsDayOwnerRequirement()));
            });

            builder.Services.AddScoped<IAuthorizationHandler, IsPlannerOwnerHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, IsTaskOwnerHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, IsReminderOwnerHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, IsMeetingOwnerHandler>();
            builder.Services.AddScoped<IAuthorizationHandler, IsDayOwnerHandler>();


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

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
