using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAPI.Models
{
    public class BlogRepository: IBlogRepository
    {
        public ApplicationContext Db { get; }

        public BlogRepository(ApplicationContext db)
        {
            Db = db;
        }

        public async Task<Blog> FindOneAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM `Blog` WHERE `id` = @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<Blog>> SearchAsync(string q)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = $"SELECT * FROM `Blog` WHERE `title` like '%{q}%'  ORDER BY `id` DESC";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task<List<Blog>> LatestBlogsAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT * FROM `Blog` ORDER BY `id` DESC";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }

        public async Task DeleteAsync(int id)
        {
            using var txn = await Db.Connection.BeginTransactionAsync();
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `Blog` WHERE `id`= @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            await cmd.ExecuteNonQueryAsync();
            await txn.CommitAsync();
        }

        public async Task<List<Blog>> ReadAllAsync(DbDataReader reader)
        {
            var posts = new List<Blog>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new Blog(Db)
                    {
                        Id = reader.GetInt32(0),
                        Title = reader.GetString(1),
                        Content = reader.GetString(2),
                        Author=reader.GetString(3),
                        Create_time=reader.GetDateTime(4),
                        Update_time=reader.GetDateTime(5)
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }

    }
}
