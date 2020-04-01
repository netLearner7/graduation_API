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
            #region jwt��֤����

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme).
                AddJwtBearer(IdentityServerAuthenticationDefaults.AuthenticationScheme,o=> {
                    o.Audience = "api1";
                    o.Authority = "http://192.168.43.131:5001";
                    o.RequireHttpsMetadata = false;
                });

            //ȥ��token������Ĭ��ӳ��
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            #endregion

            #region �������

            services.AddCors(o =>
            {
                o.AddPolicy("all", option => {
                    option.WithOrigins("http://192.168.43.131:4200/", "http://10.84.34.19");
                    option.AllowAnyHeader();
                    option.AllowAnyMethod();
                    option.AllowCredentials();
                });
            });

            #endregion

            #region mvc����(���������,json���л���)

            services.AddControllers(o => {

            })
                .AddFluentValidation(fu =>
                {
                    fu.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                    //��֤��ǰ�˽���
                    //fu.RegisterValidatorsFromAssemblyContaining<MeetingDtoValidation>();
                }).AddNewtonsoftJson(o => {
                    o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;                  
                });
                

            #endregion

            #region dbContext����

            services.AddDbContext<ApplicationDbContext>(o=> {
                o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            #endregion

            #region ���߿��������

            services.AddAutoMapper(typeof(MapperSet));

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo() { Title = "MyApi", Version = "v1", Description ="��һ���汾"});

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });

            #endregion

            #region �ִ���ע��

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