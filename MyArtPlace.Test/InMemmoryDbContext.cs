using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyArtPlace.Data;

namespace MyArtPlace.Test
{
    public class InMemoryDbContext
    {
        private SqliteConnection connection;
        private DbContextOptions<MyArtPlaceContext> dbContextOptions;

        public InMemoryDbContext()
        {
            connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            dbContextOptions = new DbContextOptionsBuilder<MyArtPlaceContext>()
                .UseSqlite(connection)
                .Options;

            using var context = new MyArtPlaceContext(dbContextOptions);

            context.Database.EnsureCreated();
        }

        public MyArtPlaceContext CreateContext() => new MyArtPlaceContext(dbContextOptions);

        public void Dispose() => connection.Dispose();
    }
}
