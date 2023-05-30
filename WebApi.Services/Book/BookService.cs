using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Data.DataContext;
using WebApi.Entity;

namespace WebApi.Services.Book
{
    public class BookService:IBookService
    {
        private readonly BMSDbContext dataContext;

        public BookService(BMSDbContext dataContext)
        {
            this.dataContext = dataContext;
        }

        /// <summary>
        /// 获取图书列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<Entity.Book> GetAllBook()
        {
            var query = dataContext.Books.Select(c=>new Entity.Book{
                  Id = c.Id,
                  Name = c.Name,
                  Author = c.Author,
                  ISBN = c.ISBN,
                  Publisher = c.Publisher,
                 Inventory = c.Inventory
            });
            return query;
        }

        /// <summary>
        /// 通过ID查找图书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entity.Book GetBookById(int id)
        {
            var query = dataContext.Books.Select(c => new Entity.Book
            {
                Id = c.Id,
                Name = c.Name,
                Author = c.Author,
                ISBN = c.ISBN,
                Publisher = c.Publisher,
                Inventory = c.Inventory
            }).FirstOrDefault(c => c.Id == id);
            return query;
        }

        /// <summary>
        /// 通过书名查找图书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entity.Book GetBookByName(string name)
        {
            var query = dataContext.Books.Select(c => new Entity.Book
            {
                Id = c.Id,
                Name = c.Name,
                Author = c.Author,
                ISBN = c.ISBN,
                Publisher = c.Publisher,
                Inventory = c.Inventory
            }).FirstOrDefault(c => c.Name.Contains(name));
            return query;
        }

        /// <summary>
        /// 添加图书
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public bool AddBook(Entity.Book book)
        {
            dataContext.Books.Add(book);
            dataContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// 修改图书
        /// </summary>
        /// <param name="id"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        public bool UpdateBook(int id,Entity.Book book)
        {
            var item = dataContext.Books.Select(c => new Entity.Book
            {
                Id = c.Id,
                Name = c.Name,
                Author = c.Author,
                ISBN = c.ISBN,
                Publisher = c.Publisher,
                Inventory = c.Inventory
            }).FirstOrDefault(c => c.Id == id);

            if (item != null)
            {
                item.Name = book.Name;
                item.Publisher = book.Publisher;
                item.Author = book.Author;
                item.ISBN = book.ISBN;
                item.Inventory.Amount = book.Inventory.Amount;
                dataContext.SaveChanges();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 删除图书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteBook(int id)
        {
            var itemI = dataContext.Inventorys.FirstOrDefault(c => c.Bookid == id);
            var itemB = dataContext.Books.Find(id);
            if (itemB == null)
            {
                return false;
            }
            if (itemI != null){
                dataContext.Inventorys.Remove(itemI);
            }
            dataContext.Books.Remove(itemB);
            dataContext.SaveChanges();
            return true;
        }
    }
}
