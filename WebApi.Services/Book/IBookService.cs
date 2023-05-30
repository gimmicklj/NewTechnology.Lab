using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services.Book
{
    public interface IBookService
    {
        /// <summary>
        /// 获取图书列表
        /// </summary>
        /// <returns></returns>
        public IQueryable<Entity.Book> GetAllBook();

        /// <summary>
        /// 通过Id查询图书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entity.Book GetBookById(int id);

        /// <summary>
        /// 通过书名查找图书
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Entity.Book GetBookByName(string name);

        /// <summary>
        /// 添加图书
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public bool AddBook(Entity.Book book);

        /// <summary>
        /// 修改图书
        /// </summary>
        /// <param name="id"></param>
        /// <param name="book"></param>
        /// <returns></returns>
        public bool UpdateBook(int id, Entity.Book book);

        /// <summary>
        /// 删除图书
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeleteBook(int id);
    }
}
