using ClinicService.Data;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Net;

namespace ClinicServiceV2
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.Listen(IPAddress.Any, 5501, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http2;
                });

                options.Listen(IPAddress.Any, 5500, listenOptions =>
                {
                    listenOptions.Protocols = HttpProtocols.Http1;
                });
            });

            builder.Services.AddGrpc().AddJsonTranscoding();
            builder.Services.AddGrpcSwagger();
            builder.Services.AddSwaggerGen( conf =>
            {
                conf.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ClinicService",
                });

                var file = Path.Combine(System.AppContext.BaseDirectory, "ClinicServiceV2.xml");
                conf.IncludeXmlComments(file);
                conf.IncludeGrpcXmlComments(file, includeControllerXmlComments: true);
            });


            #region Config EFCore
            builder.Services.AddDbContext<ClinicServiceDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration["Settings:DatabaseOptons:ConnectionString"]);
            });
            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
            }

            // Configure the HTTP request pipeline.

            app.UseRouting();
            app.UseGrpcWeb(new GrpcWebOptions
            {
                DefaultEnabled = true,
            });
            app.MapGrpcService<Services.ClinicService>().EnableGrpcWeb();
            app.MapGet("/", () => "Testing gRPC in progress...");

            app.Run();
        }
    }
}