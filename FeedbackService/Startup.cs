using CachingManager.Managers;
using FeedbackService.DataAccess.Context;
using FeedbackService.Facade;
using FeedbackService.Facade.Interfaces;
using FeedbackService.Factory;
using FeedbackService.Managers;
using FeedbackService.Managers.Interfaces;
using FeedbackService.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Reflection;

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
            services.AddDbContext<DataContext>(options => options.UseNpgsql(Configuration["DatabaseOptions:ConnectionString"]), ServiceLifetime.Transient);
            services.AddSingleton<IDistributedCacheManager>(provider => new DistributedCacheManager(provider.GetService<IDistributedCache>()));

            services.AddTransient<IRepository, RepositoryManager>(provider => new RepositoryManager(
                provider.GetService<DataContext>(),
                new FactoryManager()));

            services.AddTransient<IAuthenticationFacade, AuthenticationFacade>(provider => new AuthenticationFacade(
                provider.GetService<IRepository>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddTransient<IOrderFacade, OrderFacade>(provider => new OrderFacade(
                provider.GetService<IRepository>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddTransient<ICustomerFacade, CustomerFacade>(provider => new CustomerFacade(
                provider.GetService<IRepository>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddTransient<IProductFacade, ProductFacade>(provider => new ProductFacade(
                provider.GetService<IRepository>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddTransient<IOrderFeedbackFacade, OrderFeedbackFacade>(provider => new OrderFeedbackFacade(
                provider.GetService<IRepository>(),
                provider.GetService<IOrderFacade>(),
                provider.GetService<ICustomerFacade>(),
                provider.GetService<IProductFacade>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddTransient<IProductFeedbackFacade, ProductFeedbackFacade>(provider => new ProductFeedbackFacade(
                provider.GetService<IRepository>(),
                provider.GetService<IOrderFacade>(),
                provider.GetService<ICustomerFacade>(),
                provider.GetService<IProductFacade>(),
                provider.GetService<IDistributedCacheManager>(),
                provider.GetService<IOptions<CacheOptions>>()));

            services.AddControllers().AddNewtonsoftJson();
            services.AddSwaggerGen(options => 
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
                {
                    Title = "FeedbackService API v1", 
                    Version = "v1",
                    Description = "API that allow users to share feedback about their grocery orders and the items contained in the orders. Source code and details at https://github.com/albertxcastro/FeedbackService"
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "FeedbackService API v1");
            });

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
