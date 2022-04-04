using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace TuanBuy.Models.Entities
{
    public partial class TuanBuyContext : DbContext
    {

        public TuanBuyContext(DbContextOptions<TuanBuyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<ProductMessage> ProductMessages { get; set; }

        public virtual DbSet<ProductPic> ProductPics { get; set; }
        public virtual DbSet<TestProduct> TestProducts { get; set; }

        public virtual DbSet<ChatRoom> ChatRooms { get; set; }

        public virtual DbSet<ChatRoomMember> Member_Chats { get; set; }

        public virtual DbSet<ChatMessages> ChatMessages { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatRoomMember>().HasKey(s => new { s.MemberId, s.ChatRoomId });
            base.OnModelCreating(modelBuilder);
        }
    }
}
