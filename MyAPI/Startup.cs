using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Reflection;
using AutoMapper;
using FluentValidation.AspNetCore;
using IdentityServer4.AccessTokenValidation;
using Infrastructure.AutoMapper;
using Infrastructure.Data;
using Infrastructure.Fluentvalidation;
using Infrastructure.Repository;
using Infrastructure.Tools;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace MyAPI
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
            #region jwt验证配置

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme).
                AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme,o=> {
                    o.Audience = "api1";
                    o.Authority = "http://39.99.217.82:5001";
                    o.RequireHttpsMetadata = false;
                });

            //去除token解析的默认映射
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            #endregion

            #region 跨域策略

            services.AddCors(o =>
            {
                o.AddPolicy("all", option => {
                    option.WithOrigins("http://localhost:4200/", "http://39.99.217.82:4200");
                    option.AllowAnyHeader();
                    option.AllowAnyMethod();
                    option.AllowCredentials();
                });
            });

            #endregion

            #region mvc配置(检查器配置,json序列化器)

            services.AddControllers(o => {

            })
                .AddFluentValidation(fu =>
                {
                    fu.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    //验证由前端进行
                    //fu.RegisterValidatorsFromAssemblyContaining<MeetingDtoValidation>();
                }).AddNewtonsoftJson(o => {
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;                  
                });
                

            #endregion

            #region dbContext配置

            services.AddDbContext<ApplicationDbContext>(o=> {
                o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            #endregion

            #region 工具框架类配置

            services.AddAutoMapper(typeof(MapperSet));

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "MyApi", Version = "v1", Description ="第一个版本"});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });

            #endregion

            #region 仓储类注入

            services.AddScoped<MeetingRepository>();
            services.AddScoped<UnitOfWork>();
            services.AddScoped<UserRepository>();
            services.AddScoped<User_MeetingRepository>();
            services.AddScoped<StatisticsRepository>();

            #endregion

            services.AddScoped<InviteCodeHelper>();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //代理头 本地测试不用

            //if (env.IsProduction())
            //{
            //    app.UseForwardedHeaders(new ForwardedHeadersOptions
            //    {
            //        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //    });
            //}

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseCors("all");
           

            app.UseRouting();
            
            app.UseAuthentication();

            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
