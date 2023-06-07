using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entity;
using WebApi.Services.Book;
using StackExchange.Redis;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;
using System.Collections;
using WebApi.Redis;
using WebApi.Services.Inventory;
using WebApi.RabbitMq;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")] //允许跨域
    public class BooksController : ControllerBase
    {
        private readonly IBookService m_bookService;
        private readonly ILogger<BooksController> m_logger;
        private readonly RedisContext m_context;
        private readonly RabbitMQClient m_rabbitMQClient;
        public BooksController(IBookService bookService,
            ILogger<BooksController> logger,
            RedisContext context,        
            RabbitMQClient rabbitMQClient)
        {
            m_bookService = bookService;
            m_logger = logger;
            m_context = context;
            m_rabbitMQClient = rabbitMQClient;
        }

        ///<summary>
        /// 获取图书信息
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        [Authorize]
        public ActionResult<IQueryable<Book>> GetAllBook()
        {
            if (m_context.GetList("Books").Count() == 0)
            {
                var booksDb = m_bookService.GetAllBook();
                foreach (var item in booksDb)
                {
                    var json = JsonConvert.SerializeObject(item);
                    m_context.SetList("Books", json);
                }
            }
            var BooksCache = m_context.GetList("Books");
            //设置缓存的有效时间
            m_context.SetTimeout("Books", new TimeSpan(0, 1, 0));

            List<Book> books = new List<Book>();
            foreach (var item in BooksCache)
            {
                Book book = (Book)JsonConvert.DeserializeObject(item, typeof(Book));
                books.Add(book);
            }

            return Ok(books);
        }

        /// <summary>
        /// 通过id获取图书信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public ActionResult<Book> GetSingleBook(int id)
        {
            var json = m_context.GetList("Books");
            foreach (var item in json)
            {
                Book book = (Book)JsonConvert.DeserializeObject(item, typeof(Book));
                if(book.Id == id)
                {
                    return Ok(book);
                }
            }
            var singleBook = m_bookService.GetBookById(id);
            var singleBookjson = JsonConvert.SerializeObject(singleBook);
            m_context.SetList("Books", singleBookjson);
            //设置缓存的有效时间
            m_context.SetTimeout("Books", new TimeSpan(0, 1, 0));
            return Ok(singleBook);
        }

        /// <summary>
        /// 添加图书
        /// </summary>
        /// <param name = "book" ></ param >
        /// < returns ></ returns >
        [HttpPost()]
        public void AddBook([FromBody] Book book)
        {
            m_bookService.AddBook(book);
        }

        /// <summary>
        /// 修改图书
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public void UpdateBook(int id,[FromBody] Book book)
        {
            m_bookService.UpdateBook(id, book);
            var json = m_context.GetList("BooksId");
            foreach(var item in json)
            {
                Book oldBook = (Book)JsonConvert.DeserializeObject(item, typeof(Book));
                if(oldBook.Id == id)
                {
                    var newBook = JsonConvert.SerializeObject(book);
                    m_context.ListSetByIndex("BooksId",item,newBook);
                }
            }
        }

        /// <summary>
        /// 删除图书
        /// </summary>
        /// <param name="id"></param>
        [HttpDelete("{id}")]
        public void DeleteBook(int id)
        {
            m_bookService.DeleteBook(id);
            //清除缓存
            var json = m_context.GetList("Books");
            foreach (var item in json)
            {
                Book book = (Book)JsonConvert.DeserializeObject(item, typeof(Book));
                if(book.Id == id)
                {
                    m_context.ListRemove("Books",item);
                }
            }      
        }

    }
}
