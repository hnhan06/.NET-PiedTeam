using Microsoft.EntityFrameworkCore;
using piedteam_hocmienphi.repository.entity;

namespace piedteam_hocmienphi.repository;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<MentorCategory> MentorCategories { get; set; }
    public DbSet<Mentor> Mentors { get; set; }
    public DbSet<MentorFreeTime> MentorFreeTimes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<ApplyRequest> ApplyRequests { get; set; }
    public DbSet<ApplyRequestCategory> ApplyRequestCategories { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(builder =>
        {
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            
            //kỹ thuật Indexing
            builder.HasIndex(u => u.Email)
                .IsUnique();
                
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);
            
            builder.HasOne(u => u.Mentor)
                .WithOne(m => m.User)
                .HasForeignKey<Mentor>(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(u => u.ApplyRequests)
                .WithOne(m => m.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(u => u.Bookings)
                .WithOne(m => m.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Khi mà mình chỉ định việc khai báo các mối quan hệ ở đây,
                // các mqh sẽ không bao giờ bị tạo thiếu
            // Mình có thể config các behavior (hành động)
                // khi có sự thay đổi dữ liệu giữa các mqh
            // VD: 1 User thì có nhiều Meeting, vậy khi User đó bị xóa thì các Meeting sẽ xử lí ntn - sống chết ra sao
            //** ==> OnDelete sẽ xử lí
            
            /*
             DeleteBehavior.Cascade: Khi 1 User bị xóa thì Meeting sẽ bị xóa theo
             DeleteBehavior.Restrict: Ngăn chặn việc xóa 1 User nếu có Meeting liên quan
                    (Tham chiếu tới PK tồn tại)
                    1 Meeting thuộc về 1 User thì k xóa được
            DeleteBehavior.NoAction: K thực hiện hành động gì đặc biệt khi User bị xóa (gần giống Restrict, xử lí ở DB)
            DeleteBehavior.SetNull: Khi 1 User bị xóa thì trường UserId trong bảng Meeting sẽ được đặt là NULL
                    (Áp dụng khi trường PK cho phép NULL)
             */
            
            // Seed Data
            List<User> list = new List<User>()
            {};

            for (int i = 0; i <= 1000; i++)
            {
                var newUser = new User()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "John" + i,
                    LastName = "Doe" + i,
                    Email = "ahihi" + i + "@gmail.com",
                    Password = "123456",
                    Phone = "01301320",
                    Role = "Student",
                    Age = "20",
                    IsDeleted = false
                };
                
                list.Add(newUser);
            }
            builder.HasData(list);
        });

        modelBuilder.Entity<Mentor>(builder =>
        {
            builder.HasMany(u => u.MentorCategories)
                .WithOne(m => m.Mentor)
                .HasForeignKey(u => u.MentorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(u => u.Bookings)
                .WithOne(m => m.Mentor)
                .HasForeignKey(u => u.MentorId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(u => u.MentorFreeTimes)
                .WithOne(m => m.Mentor)
                .HasForeignKey(u => u.MentorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<Category>(builder =>
        {
            builder.HasMany(u => u.MentorCategories)
                .WithOne(m => m.Category)
                .HasForeignKey(u => u.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasMany(u => u.ApplyRequestCategories)
                .WithOne(m => m.Category)
                .HasForeignKey(u => u.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.Entity<ApplyRequest>(builder =>
        {
            builder.HasMany(u => u.ApplyRequestCategories)
                .WithOne(m => m.ApplyRequest)
                .HasForeignKey(u => u.ApplyRequestId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

// TechLead kêu tạo mới table đi
// Em hiểu ngay mô hình 3 lớp (trong file program): đi vào repo
    // tìm ngay folder Entity: tạo mới class Category
    // khai báo các field: Id, name
    // khai báo các field liên quan tới mqh: ParentId, Parent
    // đi vào DbContext khai báo
    // chạy Migration để tạo file migration (các phiên bản của DB)
    // update DB