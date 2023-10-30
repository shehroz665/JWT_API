using JWT_API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace JWT_API.Data
{
    public class ApplicationContext:DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options):base(options) { 
        
        }
        public DbSet<Users> User { get; set; }

        public DbSet<Students> Student { get; set; }

        public DbSet<Categories> Category { get; set; }

        public DbSet<Authors> Author { get; set; }

        public DbSet<Books> Book { get; set; }

        public DbSet<BookDto> Bookdto { get; set; }

        public DbSet<Transactions> Transaction { get; set; }

        public DbSet<TransactionsDto> TransactionDto { get; set; }

        public DbSet<AuthorDto> AuthorDto { get; set; }

    }
}
