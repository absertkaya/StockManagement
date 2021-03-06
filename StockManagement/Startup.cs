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
using StockManagement.Data.Services;

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
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            services.AddStorage();
            services.AddBlazoredModal();
            services.AddBlazoredToast();
            services.AddScoped<IRepository, RepositoryBase>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddHttpClient<ProtectedApiCallHelper>();
            services.AddApplicationInsightsTelemetry();
            services.AddFileReaderService();
            services.AddRazorPages();
            services.AddServerSideBlazor().AddHubOptions(o =>
            {
                o.MaximumReceiveMessageSize = 20 * 1024 * 1024;
            });
            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)//, IItemRepository repo, IConfiguration config)
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
            //var excl = new ExcelReader(repo, config);
            //excl.ApiCall("https://graph.microsoft.com/v1.0/users?$top=999").Wait();
            //excl.ReadUsers();
            //excl.PersistUsers();
            //excl.ReadSubscriptions("C:\\Users\\Administrator\\Desktop\\GSM Nummers.xlsx");
            //excl.ReadUsers("C:\\Users\\Administrator\\Desktop\\VGDGebruikers.xlsx");
            //excl.ReadAndPopulateDatabase("C:\\Users\\Administrator\\Desktop\\Stock Overzicht.xlsx");
        } 
    }
}
