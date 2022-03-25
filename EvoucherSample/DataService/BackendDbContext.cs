using EvoucherSample.Models;
using EvoucherSample.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvoucherSample.DataService
{
    public class BackendDbContext : IdentityDbContext<AuthenticationUser, AuthenticationRole, string>
    {

        public BackendDbContext(DbContextOptions<BackendDbContext> options) : base(options)
        {

        }

        public DbSet<TblEvoucherInfo> eVouchers { get; set; }
        public DbSet<TblEvoucherHistory> eVoucherHistory { get; set; }
        public DbSet<TblPaymentType> tblPaymentType { get; set; }
        public DbSet<TblBuyingType> tblBuyingType { get; set; }
        public DbSet<RefreshToken> refreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthenticationUser>(entity => entity.Property(m => m.Id).HasMaxLength(200));
            modelBuilder.Entity<AuthenticationUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(200));
            modelBuilder.Entity<AuthenticationUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(200));

            modelBuilder.Entity<AuthenticationRole>(entity => entity.Property(m => m.Id).HasMaxLength(200));
            modelBuilder.Entity<AuthenticationRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(200));

            modelBuilder.Entity<IdentityUserLogin<String>>(entity => entity.Property(m => m.UserId).HasMaxLength(200));
            modelBuilder.Entity<IdentityUserLogin<String>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(200));
            modelBuilder.Entity<IdentityUserLogin<String>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(200));

            modelBuilder.Entity<IdentityUserRole<String>>(entity => entity.Property(m => m.UserId).HasMaxLength(200));
            modelBuilder.Entity<IdentityUserRole<String>>(entity => entity.Property(m => m.RoleId).HasMaxLength(200));

            modelBuilder.Entity<IdentityUserToken<String>>(entity => entity.Property(m => m.UserId).HasMaxLength(200));
            modelBuilder.Entity<IdentityUserToken<String>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(200));
            modelBuilder.Entity<IdentityUserToken<String>>(entity => entity.Property(m => m.Name).HasMaxLength(200));

            modelBuilder.Entity<IdentityUserClaim<String>>(entity => entity.Property(m => m.UserId).HasMaxLength(200));
            modelBuilder.Entity<IdentityRoleClaim<String>>(entity => entity.Property(m => m.RoleId).HasMaxLength(200));

            base.OnModelCreating(modelBuilder);
        }

    }
}
