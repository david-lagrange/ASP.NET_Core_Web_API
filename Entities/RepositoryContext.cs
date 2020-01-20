using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options)
            :base(options)
        {

        }
        DbSet<Company> Companies { get; set; }
        DbSet<Employee> Employees { get; set; }
    }
}
