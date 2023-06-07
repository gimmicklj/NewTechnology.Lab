using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ocelot.Provider.Consul;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.GateWay
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
            services.AddCors(options => options.AddPolicy("CorsPolicy",
          builder =>
          {
              builder.AllowAnyMethod()
                  .SetIsOriginAllowed(_ => true)
                  .AllowAnyHeader()
                  .AllowCredentials();
          }));

            //获取appsettings.json文件中配置认证中密钥（Secret）跟受众（Aud）信息
            var audienceConfig = Configuration.GetSection("Audience");
            //获取安全秘钥
            var signingKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            //token要验证的参数集合
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true, //必须验证安全秘钥
                IssuerSigningKey = signingKey, //赋值安全秘钥
                ValidateIssuer = true, //必须验证签发人
                ValidIssuer = audienceConfig["Iss"], //赋值签发人
                ValidateAudience = true,//必须验证受众
                ValidAudience = audienceConfig["Aud"],//赋值受众
                ValidateLifetime = true,//是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                ClockSkew = TimeSpan.Zero,//允许的服务器时间偏移量
                RequireExpirationTime = true,//是否要求Token的Claims中必须包含Expires
            };

            //添加服务验证，方案为TestKey
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = "TestKey";
            })
            .AddJwtBearer("TestKey", x =>
            {
                x.RequireHttpsMetadata = false;
                //在JwtBearerOptions配置中，IssuerSigningKey(签名秘钥)、ValidIssuer(Token颁发机构)、ValidAudience(颁发给谁)三个参数是必须的。
                x.TokenValidationParameters = tokenValidationParameters;
            });

            //添加Ocelot网关服务时,包括Secret秘钥、Iss签发人、Aud受众

            services.AddOcelot(Configuration).AddConsul();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");

            // 使用认证服务
            app.UseAuthentication();

            // 使用Ocelot中间件
            app.UseOcelot().Wait();

        }
    }
}
