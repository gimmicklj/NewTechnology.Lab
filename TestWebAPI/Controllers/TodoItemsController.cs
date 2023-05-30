using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Entity;
using WebApi.Data.DataContext;

namespace TestWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private static TodoContext m_DbContext;

        private static bool m_Init = false;
        private static void Init()
        {
            if (!m_Init)
            {
                m_DbContext.TodoItems.Add(new TodoItem
                {
                    Name = "计划工作A",
                    IsComplete = false
                });
                m_DbContext.SaveChanges();
            }
        }

        public TodoItemsController(TodoContext dbContext)
        {
            m_DbContext = dbContext;
            Init();
        }

        // GET: api/TodoItems
        //[HttpGet("items")]
        [HttpGet()]
        public ActionResult<IQueryable<TodoItem>> GetTodoItem()
        {
            var todoItems = m_DbContext.TodoItems;

            if (todoItems == null)
            {
                return NotFound();
            }

            return todoItems;
        }


        // POST: api/TodoItems
        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem([FromBody]TodoItem todoItem)
        {
            m_DbContext.TodoItems.Add(todoItem);
            await m_DbContext.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        
        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTodoItem(long id)
        {
            var todoItem = await m_DbContext.TodoItems.FindAsync(id);

            if (todoItem == null)
            {
                return NotFound();
            }

            return todoItem;
        }

        // PUT: api/TodoItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, [FromBody]TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return BadRequest();
            }

            m_DbContext.Entry(todoItem).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            try
            {
                await m_DbContext.SaveChangesAsync();
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// 数据库中是否存在该记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool TodoItemExists(long id)
        {
            return m_DbContext.TodoItems.Any(e => e.Id == id);
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await m_DbContext.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            m_DbContext.TodoItems.Remove(todoItem);
            await m_DbContext.SaveChangesAsync();

            return todoItem;
        }

    }
}
