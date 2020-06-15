using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BlogAPI.Models;
using MySql.Data.MySqlClient;

namespace BlogAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        public BlogController(ApplicationContext db)
        {
            Db = db;
        }
        public ApplicationContext Db { get; }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            await Db.Connection.OpenAsync();
            var query = new BlogRepository(Db);
            var result = await query.LatestBlogsAsync();
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new BlogRepository(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }

        [HttpGet("search/{q}")]
        public async Task<IActionResult> Search(string q)
        {
            await Db.Connection.OpenAsync();
            var query = new BlogRepository(Db);
            var result = await query.SearchAsync(q);
            if (result is null)
                return new NotFoundResult();
            return new OkObjectResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Blog body)
        {
            await Db.Connection.OpenAsync();
            body.Db = Db;
            await body.InsertAsync();
            return new OkObjectResult(body);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Blog body)
        {
            await Db.Connection.OpenAsync();
            var query = new BlogRepository(Db);
            var result = await query.FindOneAsync(id);
            if (result is null)
                return new NotFoundResult();
            result.Title = body.Title;
            result.Content = body.Content;
            result.Author = body.Author;
            result.Update_time = body.Update_time;
            await result.UpdateAsync();
            return new OkObjectResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Db.Connection.OpenAsync();
            var query = new BlogRepository(Db);
            await query.DeleteAsync(id);
            return new OkResult();
        }

    }
}
