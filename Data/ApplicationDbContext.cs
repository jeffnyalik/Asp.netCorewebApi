using System.Runtime.CompilerServices;
using System.Net.Mime;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {     
        }

        //Creating Roles
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new {Id="1", Name="Admin", NormalizedName="ADMIN"},
                new {Id="2", Name="Moderator", NormalizedName="MODERATOR"},
                new {Id="3", Name="Customer", NormalizedName="CUSTOMER"}
            );
        }

        internal Task<ProductModel> SaveChangesAsync(ValueTask<EntityEntry<ProductModel>> results)
        {
            throw new NotImplementedException();
        }

        public DbSet<ProductModel> Products {get;set;}
    }
}