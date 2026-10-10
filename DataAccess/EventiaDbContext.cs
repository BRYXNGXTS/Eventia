using System;
using Microsoft.EntityFrameworkCore;
using Eventia.Models;

namespace Eventia.DataAccess
{
    public class EventiaDbContext : DbContext
    {
        public DbSet<Evento> Eventos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "eventia.db");
            optionsBuilder.UseSqlite($"Filename={dbPath}");
        }
    }
}