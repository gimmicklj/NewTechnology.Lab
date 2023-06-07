using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApi.Dtos;
using WebApi.Entity;
using WebApi.RabbitMq;

namespace WebApi.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")] //允许跨域
    public class InventoryController : ControllerBase
    {
        private readonly RabbitMQClient m_rabbitMQClient;

        public InventoryController(RabbitMQClient rabbitMQClient)
        {
            m_rabbitMQClient = rabbitMQClient;
        }

        [HttpPost()]
        public void Purchase(PurchaseBook purchaseBook)
        {
            using (var channel = m_rabbitMQClient.CreateChannel())
            {
                var queue = channel.QueueDeclare("Book", false, false, false, null);

                var json = JsonConvert.SerializeObject(purchaseBook);
                var body = Encoding.UTF8.GetBytes(json);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish("", queue.QueueName, false, properties, body);
            }
           
        }
    }
}
