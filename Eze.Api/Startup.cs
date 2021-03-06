using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Eze.Api.Repositories;
using MongoDB.Driver;
using Eze.Api.Settings;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using System.Text.Json;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Eze.Api.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Eze.Api
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
            var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            services.AddSingleton<IMongoClient>(serviceProvide =>
            {
                return new MongoClient(mongoDbSettings.ConnectionString);
            });

            services.AddHealthChecks()
                .AddMongoDb(mongoDbSettings.ConnectionString, 
                            tags: new[]{"ready"},
                            name: "mongodb", 
                            timeout: TimeSpan.FromSeconds(5));

            services.AddSingleton<IEzeRepository, MongoDbRepository>();
            services.AddSingleton<IRequestRepository, InMemRequestRepository>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Eze", Version = "v1" });
            });

            var jwtSettings = Configuration.GetSection("JWTSettings");
            services.Configure<JWTSettings>(jwtSettings);

            var jwtSettingsObject = jwtSettings.Get<JWTSettings>();
            var key = Encoding.ASCII.GetBytes(jwtSettingsObject.SecretKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Eze v1"));
                app.UseHttpsRedirection();
            }


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health/live", new HealthCheckOptions
                {
                    Predicate = (_) => false,
                });

                endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions
                {
                    Predicate = (check) => check.Tags.Contains("ready"),
                    ResponseWriter = async(context,report) => 
                    {
                        var result = JsonSerializer.Serialize
                        (
                            new {
                                status = report.Status.ToString(),
                                checks = report.Entries.Select(entry => new {
                                    name = entry.Key,
                                    status = entry.Value.Status.ToString(),
                                    exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "None",
                                    duration = entry.Value.Duration.ToString()
                                })
                            }
                        );

                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        await context.Response.WriteAsync(result);

                    }
                });
            });
        }
    }
}
