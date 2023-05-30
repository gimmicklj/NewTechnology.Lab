using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using Consumer;
using WebApi.Dtos;
using WebApi.Entity;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            string connString = "Server=127.0.0.1,14333;Database=BMSDB;User ID=sa;Password=Aa123456.;Integrated Security=True;Trusted_Connection=false;";
            var factory = new RabbitMQ.Client.ConnectionFactory
            {
                HostName = "127.0.0.1",
                UserName = "admin",
                Password = "123456",
                VirtualHost = "my_vhost"
            };


            ///第一步：连接RabbitMQ
            using var connection = factory.CreateConnection();
            ///第二步：打开信道
            using var channel = connection.CreateModel();
            //定义队列
            var queue = channel.QueueDeclare("Book", false, false, false, null);

            //同一时刻服务器只发送一条消息给消费者
            channel.BasicQos(0, 1, false);

            // 定义消费者
            var consumer = new RabbitMQ.Client.Events.EventingBasicConsumer(channel);

            consumer.Received += (sender, e) =>
            {
                try
                {
                    var result = System.Text.Encoding.UTF8.GetString(e.Body.ToArray());
                    PurchaseBook u = (PurchaseBook)JsonConvert.DeserializeObject(result, typeof(PurchaseBook));
                    UpdateInventory(u, connString);

                    channel.BasicAck(e.DeliveryTag, false);

                    Console.WriteLine("消费消息：" + result);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("消费者错误：" + ex.Message);
                }
            };

            channel.BasicConsume(queue.QueueName, false, "UpdateInventory", false, false, null, consumer);
            Console.ReadLine();

        }
        /// <summary>
        /// 修改库存
        /// </summary>
        /// <param name="purchaseBook"></param>
        private static void UpdateInventory(PurchaseBook purchaseBook, string connString)
        {
            using var db = new BMSDbContext(connString);
            var item = db.Books.Select(c => new Book
            {
                Id = c.Id,
                Name = c.Name,
                Author = c.Author,
                ISBN = c.ISBN,
                Publisher = c.Publisher,
                Inventory = c.Inventory
            }).FirstOrDefault(c => c.Id == purchaseBook.Id);
            if (item != null && item.Inventory.Amount != 0)
            {
                item.Inventory.Amount -= purchaseBook.Quantity;
            }
            db.Books.Update(item);
            db.SaveChanges();
        }
    }
}
