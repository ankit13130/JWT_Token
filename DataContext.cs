using JWT_Token.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT_Token;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    public DbSet<Users> Users { get; set; }
}