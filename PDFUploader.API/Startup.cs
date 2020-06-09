using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PDFUploader.API.Extensions;
using PDFUploader.API.Options;
using PDFUploader.Data.BlobStorage;
using PDFUploader.Data.Repositories;
using PDFUploader.Domain.Interfaces.Blobs;
using PDFUploader.Domain.Interfaces.Repositories;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PDFUploader.API
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
            var connectionStringsOptions =
                Configuration.GetSection("ConnectionStrings").Get<ConnectionStringsOptions>();
            var cosmosDbOptions = Configuration.GetSection("CosmosDb").Get<CosmosDbOptions>();
            var (serviceEndpoint, authKey, blobStorage) = connectionStringsOptions.ActiveConnectionStringOptions;
            //var (databaseName, collectionData) = cosmosDbOptions;
            //var collectionNames = collectionData.Select(c => c.Name).ToList();


            services.AddControllers();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PDF Uploader", Version = "v1" });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddSingleton<IBlobStorage, AzureBlobStorage>(serviceProvider => new AzureBlobStorage(blobStorage));

            //services.AddCosmosDb(serviceEndpoint, authKey, databaseName, collectionNames);
            //repos
            //services.AddScoped<IPdfMetadataRepository, PdfMetadataRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PDF Uploader API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseGlobalErrorHandler();
                app.UseHsts();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default", "{controller=Pdf}/{action=Get}/{id?}");
            });
        }
    }
}
