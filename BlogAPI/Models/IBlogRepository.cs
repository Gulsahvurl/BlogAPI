using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    interface IBlogRepository
    {
        Task<List<Blog>> SearchAsync(string q);
        Task<Blog> FindOneAsync(int id);
        Task<List<Blog>> LatestBlogsAsync();
        Task DeleteAsync(int id);
        Task<List<Blog>> ReadAllAsync(DbDataReader reader);
    }
}
