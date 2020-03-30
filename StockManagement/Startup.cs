using Azure.Storage.Blobs;
using Blazor.FileReader;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StockManagement.Data.Services;
using StockManagement.Data.Repositories;
using StockManagement.Domain.IRepositories;
using StockManagement.Graph;
using StockManagement.Domain.IServices;
using Blazor.Extensions.Storage;
using Blazor.Extensions.Storage.Interfaces;
using Blazored.Modal;
using StockManagement.Data;
using Blazored.Toast;
using Blazored.Toast.Services;

namespace StockManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                
                options.Filters.Add(new AuthorizeFilter(policy));  
            });

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            services.AddStorage();
            services.AddBlazoredModal();
            services.AddBlazoredToast();
            services.AddScoped<IRepository, RepositoryBase>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<MailService, GmailService>();
            services.AddHttpClient<ProtectedApiCallHelper>();
            services.AddApplicationInsightsTelemetry();
            services.AddFileReaderService();
            services.AddRazorPages();
            services.AddServerSideBlazor().AddHubOptions(o =>
            {
                o.MaximumReceiveMessageSize = 20 * 1024 * 1024; // 20MB
            });
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

            //new ExcelReader().ReadAndPopulateDatabase("C:\\Users\\bse\\Desktop\\Stock Overzicht.xlsx");
        }
    }
}
