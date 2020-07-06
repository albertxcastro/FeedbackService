using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FeedbackService.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CachingManager.Managers;
using Microsoft.Extensions.Caching.Distributed;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Managers;
using Microsoft.Extensions.Options;
using FeedbackService.DataAccess.Context;

namespace FeedbackService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.Configure<CacheOptions>(Configuration.GetSection("CacheOptions"));
            services.AddStackExchangeRedisCache(options => options.Configuration = Configuration["CacheOptions:Configuration"]);
            services.AddEntityFrameworkNpgsql().AddDbContext<DataContext>(options => options.UseNpgsql(Configuration["DatabaseOptions:ConnectionString"]));
            services.AddSingleton<IDistributedCacheManager>(provider => new DistributedCacheManager(provider.GetService<IDistributedCache>()));

            services.AddTransient<IRepository, RepositoryManager>(provider => new RepositoryManager(
                provider.GetService<DataContext>()));

            services.AddTransient<IAuthenticationManager, AuthenticationManager>(provider => new AuthenticationManager(
                provider.GetService<IRepository>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddTransient<IOrderManager, OrderManager>(provider => new OrderManager(
                provider.GetService<IRepository>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddTransient<ICustomerManager, CustomerManager>(provider => new CustomerManager(
                provider.GetService<IRepository>(),
                provider.GetService<IOrderManager>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddTransient<IFeedbackManager, FeedbackManager>(provider => new FeedbackManager(
                provider.GetService<IRepository>(),
                provider.GetService<IOrderManager>(),
                provider.GetService<ICustomerManager>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddControllers().AddNewtonsoftJson();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
