using CatLog.Api.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace CatLog.Api.Data.Contexts
{
    public class DbCatLogContext : DbContext
    {
        public DbCatLogContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        public DbSet<Article> Articles { get; set; }
    }
}
