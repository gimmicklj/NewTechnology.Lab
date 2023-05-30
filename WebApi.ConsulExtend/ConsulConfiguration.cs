using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using static System.Net.WebRequestMethods;

namespace WebApi.ConsulExtend
{
    public static class ConsulConfiguration
    {
        /// <summary>
        /// 注册WebAPI服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        public static void RegisterService(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. 初始化Consul服务信息
            var consulClient = new Consul.ConsulClient(opt => {
                opt.Address = new Uri(configuration["ConsulConfig:ConsulServer"]);
                opt.Datacenter = configuration["ConsulConfig:DataCenter"];
            });

            // 2. 读取配置信息
            var ip = configuration["ConsulConfig:APIAddress"];
            int port = int.Parse(configuration["ConsulConfig:APIPort"]);
            var name = configuration["ConsulConfig:APIName"];
            var healthCheck = configuration["ConsulConfig:APICheckAddress"];
            var HTTP = $"http://{ip}:{port}/{healthCheck}";
            // 3. 注册服务
            consulClient.Agent.ServiceRegister(new Consul.AgentServiceRegistration()
            {
                ID = $"{name}-{Guid.NewGuid()}", //服务ID编号
                Name = name,    // WebAPI的名称
                Address = ip,   // WebAPI的IP地址
                Port = port,    // WebAPI的端口
                Check = new Consul.AgentServiceCheck() // 注册健康检查方式
                {
                    Interval = TimeSpan.FromSeconds(10), // 每隔10秒钟检查一次
                    HTTP = $"http://{ip}:{port}/{healthCheck}",
                    Timeout = TimeSpan.FromSeconds(5),   // 超时时长5秒钟
                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(10) //超时10秒后还不能连接到中心服务时则注销本服务
                }
            });

        }
    }
}
