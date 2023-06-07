using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Redis;
using WebApi.Services.Book;
using WebApi.Services.Inventory;
using WebApi.RabbitMq;
using WebApi.Data.DataContext;
using Serilog;
using WebApi.ConsulExtend;

namespace WebApi.Demo
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
            var redisServerUrl = Configuration.GetConnectionString("RedisServerUrl");

            //配置jwt
            //获取appsettings.json文件中配置认证中密钥（Secret）跟受众（Aud）信息
            var audienceConfig = Configuration.GetSection("Audience");
            //获取安全秘钥
            var signingKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(audienceConfig["Secret"]));
            //token要验证的参数集合
            var tokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,//必须验证安全秘钥
                IssuerSigningKey = signingKey,//赋值安全秘钥
                ValidateIssuer = true,//必须验证签发人
                ValidIssuer = audienceConfig["Iss"],//赋值签发人
                ValidateAudience = true,//必须验证受众
                ValidAudience = audienceConfig["Aud"],//赋值受众
                ValidateLifetime = true,//是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                ClockSkew = TimeSpan.Zero,//允许的服务器时间偏移量
                RequireExpirationTime = true,//是否要求Token的Claims中必须包含Expires
            };
            //添加服务验证，方案为TestKey
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = "TestKey";
            })
            .AddJwtBearer("TestKey", x =>
            {
                x.RequireHttpsMetadata = false;
                //在JwtBearerOptions配置中，IssuerSigningKey(签名秘钥)、ValidIssuer(Token颁发机构)、ValidAudience(颁发给谁)三个参数是必须的。
                x.TokenValidationParameters = tokenValidationParameters;
            });

            //配置跨域
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin() //允许所有Origin策略

                           //允许所有请求方法：Get,Post,Put,Delete
                           .AllowAnyMethod()

                           //允许所有请求头:application/json
                           .AllowAnyHeader();
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("V1", new OpenApiInfo 
                { 
                    Title = "Swagger接口文档",
                    Version = "V1",
                    Description = "测试Web API",
                });
            });

            services.AddScoped<IBookService, BookService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddSingleton<RedisContext>();
            
            #region 配置RabbitMQ
            services.Configure<RabbitMQOptions>(Configuration.GetSection("RabbitMQOptions"));
            services.AddSingleton<RabbitMQClient>();
            services.AddSingleton<RabbitMQOptions>();
            services.RegisterService(this.Configuration);
            #endregion

            services.AddDbContext<BMSDbContext>(opt =>
            opt.UseSqlServer(Configuration.GetConnectionString("BMSDB")));

            services.AddLogging(logBuilder => {
                                 logBuilder.ClearProviders();});

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
           

            app.UseHttpsRedirection();
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseCors();

            app.UseAuthentication(); // 认证
            app.UseAuthorization();  // 授权

            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint($"/swagger/V1/swagger.json", "V1");
                opt.RoutePrefix = string.Empty;
            });
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
