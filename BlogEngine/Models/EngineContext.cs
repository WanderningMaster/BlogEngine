using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogEngine.Models
{
    public class EngineContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Post> Posts { get; set; }
        public EngineContext(DbContextOptions<EngineContext> options): base(options)
        {
            Database.EnsureCreated();
        }
    }
}
