using AutoMapper;
using CatLog.Api.Data.Contexts;
using CatLog.Api.Data.Implements;
using CatLog.Api.Data.Interfaces;
using CatLog.Api.Services.Implements;
using CatLog.Api.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;

namespace CatLog.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment WebHostEnvironment { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            WebHostEnvironment = webHostEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (WebHostEnvironment.IsDevelopment())
            {
                // 添加 Swagger 支持
                services.AddSwaggerGen(options =>
                {
                    // 添加 Swagger 文档信息
                    options.SwaggerDoc(Configuration["Swagger:Doc:Name"], new OpenApiInfo
                    {
                        Title = Configuration["Swagger:Doc:Title"],
                        Version = Configuration["Swagger:Doc:Version"],
                        Description = Configuration["Swagger:Doc:Name:Description"],
                        Contact = new OpenApiContact
                        {
                            Name = Configuration["Swagger:Doc:Contact:Name"],
                            Email = Configuration["Swagger:Doc:Contact:Email"],
                            Url = new Uri(Configuration["Swagger:Doc:Contact:Url"])
                        }
                    });

                    // 为 Swagger JSON and UI 设置 xml 文档注释路径
                    var basePath = Path.GetDirectoryName(AppContext.BaseDirectory);
                    var xmlPath = Path.Combine(basePath, "Catlog.Api.xml");
                    options.IncludeXmlComments(xmlPath);
                });
            }

            services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true; //启用 406 状态码

            })
                //使用 AddNewtonsoftJson 作为 json 序列化工具
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                })
                .ConfigureApiBehaviorOptions(options =>
                {
                    //自定义 422 错误（InvalidModel）的返回信息
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetails = new ValidationProblemDetails(context.ModelState)
                        {
                            Type = Configuration["Status422UnprocessableInfo:Type"],
                            Title = Configuration["Status422UnprocessableInfo:Title"],
                            Status = StatusCodes.Status422UnprocessableEntity,
                            Detail = Configuration["Status422UnprocessableInfo:Detail"],
                            Instance = context.HttpContext.Request.Path
                        };
                        //problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };
                    };
                });

            services.AddAuthorization();

            services.AddAuthentication("GitHub");
                    //.AddOpenIdConnect("GitHub", options =>
                    //{
                    //    options.Authority = "https://api.github.com/user";
                    //    options.SaveTokens = true;
                    //});
                    //.AddJwtBearer("Bearer", options =>
                    //{
                    //    options.Authority = "https://api.github.com/user";
                    //    options.RequireHttpsMetadata = false;
                    //    options.SaveToken = true;
                    //    //options.Authority = Configuration["JwtBearerOptions:Authority"];
                    //    //options.RequireHttpsMetadata = bool.Parse(Configuration["JwtBearerOptions:RequireHttpsMetadata"]);
                    //    //options.Audience = Configuration["JwtBearerOptions:Audience"];
                    //});
                    

            services.Configure<MvcOptions>(options =>
            {
                // 配置 NewtonsoftJsonOutputFormatter 的 SupportedMediaTypes
                var newtonSoftJsonOutputFormatter = options.OutputFormatters
                                                    .OfType<NewtonsoftJsonOutputFormatter>()
                                                    ?.FirstOrDefault();
                newtonSoftJsonOutputFormatter?.SupportedMediaTypes.Add("application/vnd.hateoas+json");
            });

            // 添加 Mapper 服务，扫描当前应用域的所有 Assemblies 寻找 AutoMapper 的映射关系
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddDbContext<CatLogContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DbCatLogConnection"));
            });

            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<IColumnRepository, ColumnRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();

            //属性映射服务，用于 OrderBy、Select
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // 启用 Swagger 中间件
                app.UseSwagger();
                // 配置 SwaggerUI
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint(Configuration["Swagger:Endpoint:Url"], Configuration["Swagger:Endpoint:Name"]);
                    options.RoutePrefix = string.Empty;
                });
            }
            else
            {
                app.UseHttpsRedirection();
            }

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
