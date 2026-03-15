using Microsoft.EntityFrameworkCore;
using StudioFlow.Models;

namespace StudioFlow.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Department> Departments { get; set; }
    public DbSet<User> Users { get; set; }
}