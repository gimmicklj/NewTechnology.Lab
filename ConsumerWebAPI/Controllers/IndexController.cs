
using ConsumerWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Configuration;
using ConsumerWebAPI.PollyExtend;

namespace ConsumerWebAPI.Controllers
{
    public class IndexController : Controller
    {
        private readonly IConfiguration m_Configuration;

        public IndexController(IConfiguration configuration)
        {
            m_Configuration = configuration;
        }

        public IActionResult ServiceIndex()
        {
            // 1. 初始化Consul服务信息
            var client = new Consul.ConsulClient(opt => {
                opt.Address = new Uri(m_Configuration["ConsulConfig:ConsulServer"]);
                opt.Datacenter = m_Configuration["ConsulConfig:DataCenter"];
            });

            // 2. 获取服务
            var services = client.Agent.Services().Result;

            var list = new List<ConsulService>();
            foreach (var item in services.Response)
            {
                ConsulService service = new ConsulService
                {
                    Id = item.Value.ID,
                    Address = item.Value.Address,
                    Port = item.Value.Port,
                    Name = item.Value.Service,
                    Url = $"http://{item.Value.Address}:{item.Value.Port}/{item.Value.Service}"
                };
                list.Add(service);
            }

            return View(list);
        }      
    }
}
