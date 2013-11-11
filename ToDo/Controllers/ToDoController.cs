using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ToDo.Models;

namespace ToDo.Controllers
{
    public class ToDoController : ApiController
    {
        private ToDoContext db = new ToDoContext();

        // //GET api/ToDo
        //public IQueryable<TodoItem> GetTodoItems()
        //{
        //    return db.TodoItems;
        //}

        public IEnumerable<TodoItem> GetTodoItems(string q = null, string sort = null, bool desc = false,
                                                        int? limit = null, int offset = 0)
        {
            var list = ((IObjectContextAdapter)db).ObjectContext.CreateObjectSet<TodoItem>();

            IQueryable<TodoItem> items = string.IsNullOrEmpty(sort) ? list.OrderBy(o => o.Priority)
                : list.OrderBy(String.Format("it.{0} {1}", sort, desc ? "DESC" : "ASC"));

            if (!string.IsNullOrEmpty(q) && q != "undefined") items = items.Where(t => t.Todo.Contains(q));
            if (offset > 0) items = items.Skip(offset);
            if (limit.HasValue) items = items.Take(limit.Value);

            return items;
        }

        // GET api/ToDo/5
        [ResponseType(typeof(TodoItem))]
        public IHttpActionResult GetTodoItem(int id)
        {
            TodoItem todoitem = db.TodoItems.Find(id);
            if (todoitem == null)
            {
                return NotFound();
            }

            return Ok(todoitem);
        }

        // PUT api/ToDo/5
        public IHttpActionResult PutTodoItem(int id, TodoItem todoitem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != todoitem.TodoItemId)
            {
                return BadRequest();
            }

            db.Entry(todoitem).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST api/ToDo
        [ResponseType(typeof(TodoItem))]
        public IHttpActionResult PostTodoItem(TodoItem todoitem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TodoItems.Add(todoitem);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = todoitem.TodoItemId }, todoitem);
        }

        // DELETE api/ToDo/5
        [ResponseType(typeof(TodoItem))]
        public IHttpActionResult DeleteTodoItem(int id)
        {
            TodoItem todoitem = db.TodoItems.Find(id);
            if (todoitem == null)
            {
                return NotFound();
            }

            db.TodoItems.Remove(todoitem);
            db.SaveChanges();

            return Ok(todoitem);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TodoItemExists(int id)
        {
            return db.TodoItems.Count(e => e.TodoItemId == id) > 0;
        }
    }
}