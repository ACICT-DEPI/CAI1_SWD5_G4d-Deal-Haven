using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Implementation;
using ECommerce.Entities.Repositories;
using ECommerce.Utilities;
using ECommerce.Web.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));

            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(4);
            })
                .AddDefaultTokenProviders()
                .AddDefaultUI()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddAutoMapper(typeof(MappingProfiles));

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            var app = builder.Build();

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

            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Customer",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");
            app.Run();
        }
    }
}
