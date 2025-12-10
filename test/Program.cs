using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using test.Data;
using test.Helpers;
using test.Hubs;
using test.Interfaces;
using test.Models;
using test.Repository;
using test.Services;


namespace test
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- 1. Add services to the container ---

            builder.Services.AddDbContext<DepiContext>
                (options => options
                .UseSqlServer(builder.Configuration.GetConnectionString("depiContextConnection")));
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(option =>
            {
                option.SignIn.RequireConfirmedEmail = true;
                option.User.RequireUniqueEmail = true;
            })
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<DepiContext>();
            
            // Configure Identity's cookie settings
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Home/Index";
            });
            builder.Services.AddSession(options =>
            {
                // You can set a timeout for the session
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            builder.Services.AddHttpContextAccessor();
            builder.Services.Configure<BraintreeSettings>(builder.Configuration.GetSection("Braintree"));
            builder.Services.AddScoped<BraintreeService>();
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddScoped<test.Services.PhotoServices>();
            builder.Services.AddScoped<IAnimal, AnimalRepository>();
            builder.Services.AddScoped<IAccounts, AccountRepository>();
            builder.Services.AddScoped<IRequests, RequestRepository>();
            builder.Services.AddScoped<IEmailSender, EmailSenderServcies>();
            builder.Services.AddScoped<IShelter, ShelterRepository>();
            builder.Services.AddScoped<IOrder, OrderRepository>();
            builder.Services.AddScoped<ITransaction, TransactionRepository>();
            builder.Services.AddScoped<IContact, ContactRepository>();
            builder.Services.AddScoped<IMedicalRecord, MedicalRecordRepository>();
            builder.Services.AddScoped<IVaccinationNeeded, VaccinationNeededRepository>();
            builder.Services.AddSignalR();




            var googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");
            builder.Services.AddAuthentication().AddGoogle(Services =>
            {
                Services.ClientId = googleAuthNSection["ClientId"];
                Services.ClientSecret = googleAuthNSection["ClientSecret"];
                Services.CallbackPath = "/signin-google";
                Services.Scope.Add("profile");
                Services.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
            });

            // Use AddControllersWithViews for MVC applications that use Views.
            builder.Services.AddControllersWithViews();


            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                // Pass the IServiceProvider to your seeder
                await RoleServices.SeedRolesAsync(scope.ServiceProvider);
                await RoleServices.SeedAdminUserAsync(scope.ServiceProvider);
            }

            // --- 2. Configure the HTTP request pipeline (Order is very important here) ---

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // This enables serving static files like CSS and JavaScript from the wwwroot folder.
            app.UseStaticFiles();

            // This sets up routing.
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.MapHub<ChatHub>("/chathub");


            // This is where you define your application's routes or "endpoints".
            // It should come at the end.
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();

        }
    }
}