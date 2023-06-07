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
                if (string.Compare(item.Value.Service, "api/todoitems", true) == 0)
                {
                    ConsulService service = new ConsulService
                    {
                        Id = item.Value.ID,
                        Address = item.Value.Address,
                        Port = item.Value.Port,
                        Name = item.Value.Service,
                        Url = $"http://localhost:5000/ocelot-1/todoitems"
                    };
                    list.Add(service);
                }
                if (string.Compare(item.Value.Service, "api/Books", true) == 0)
                {
                    ConsulService service = new ConsulService
                    {
                        Id = item.Value.ID,
                        Address = item.Value.Address,
                        Port = item.Value.Port,
                        Name = item.Value.Service,
                        Url = $"http://localhost:5000/ocelot-2/Books"
                    };
                    list.Add(service);
                }
            }
            return View(list);
        }

         public IActionResult GetWebAPI(string Url)
        {
            var result =ClientPolicy.Instance.WrapPolicy.Execute(delegate ()
            {
                return CallWebAPI(Url);
            });

            ViewBag.result = result;
            return View();
        }

        public static string CallWebAPI(string Url)
        {
            var url = Url;
            var client = new System.Net.WebClient();
            var result = client.DownloadString(url);
            Console.WriteLine("执行WebAPI请求：" + url);
            return result;
        }
    }
}
