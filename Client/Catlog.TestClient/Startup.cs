using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Catlog.TestClient
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
            services.AddControllersWithViews();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            // AddAuthentication 将身份认证服务添加到 DI
            services.AddAuthentication(options =>
            {
                // 使用 cookie 来本地登录用户
                options.DefaultScheme = "idsr.oidc";
                // 将 DefaultChallengeScheme 设置为 "oidc"，因为当我们需要用户登录时，我们将使用 OpenID Connect 协议
                options.DefaultChallengeScheme = "oidc";
            })
                // 使用 AddCookie 添加可以处理 cookie 的处理程序
                .AddCookie("idsr.oidc")
                .AddOpenIdConnect("oidc", options =>
                {
                    options.Authority = "http://localhost:5004";
                    options.RequireHttpsMetadata = false;

                    options.ClientId = "catlog.client";
                    options.ClientSecret = "69C1A7E1-AC79-4A89-A308-C46998FB86B2";
                    // SaveTokens 用于在 cookie 中保留来自 IdentityServer 的令牌（稍后将需要它们）
                    options.SaveTokens = true;
                    options.ResponseType = "code";

                    options.Scope.Clear();
                    options.Scope.Add("catlog.api");
                    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
                    options.Scope.Add(OidcConstants.StandardScopes.Profile);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
