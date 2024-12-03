using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoadReady.Models;

namespace RoadReady.Authentication
{
    public class RoadReadyContext : IdentityDbContext<ApplicationUser>
    {
        public RoadReadyContext(DbContextOptions<RoadReadyContext> options)
            : base(options) { }
        //public virtual DbSet<AdminActions> AdminActions { get; set; } = null!;
        public virtual DbSet<AdminDashboardData> AdminDashboardData { get; set; } = null!;
        public virtual DbSet<Car> Cars { get; set; } = null!;
        public virtual DbSet<CarExtra> CarExtras { get; set; } = null!;
        public virtual DbSet<PasswordReset> PasswordResets { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Reservation> Reservations { get; set; } = null!;
        public virtual DbSet<Review> Reviews { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        // public virtual DbSet<UserAudit> UserAudits { get; set; } = null!;
        //public virtual DbSet<ReservationExtra> ReservationExtras { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=LAPTOP-PV7LC260;Database=RoadReady;Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /*modelBuilder.Entity<AdminActions>(entity =>
            {
                entity.HasKey(e => e.ActionId)
                    .HasName("PK__AdminAct__74EFC21733B8F713");

                entity.Property(e => e.ActionId).HasColumnName("action_id");

                entity.Property(e => e.ActionDate)
                    .HasColumnName("action_date")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.ActionDescription).HasColumnName("action_description");

                entity.Property(e => e.ActionType)
                    .HasMaxLength(100)
                    .HasColumnName("action_type");

                entity.Property(e => e.AdminId).HasColumnName("admin_id");

                entity.HasOne(d => d.Admin)
                    .WithMany(p => p.AdminActions)
                    .HasForeignKey(d => d.AdminId)
                    .HasConstraintName("FK__AdminActi__admin__571DF1D5");
            });*/

            modelBuilder.Entity<AdminDashboardData>(entity =>
            {
                entity.HasKey(e => e.DashboardId)
                    .HasName("PK__AdminDas__5E2AEAE69B9D34E7");

                entity.Property(e => e.DashboardId).HasColumnName("dashboard_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.TotalCars).HasColumnName("total_cars");

                entity.Property(e => e.TotalReservations).HasColumnName("total_reservations");

                entity.Property(e => e.TotalRevenue)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_revenue");

                entity.Property(e => e.TotalReviews).HasColumnName("total_reviews");

                entity.Property(e => e.TotalUsers).HasColumnName("total_users");
            });

            modelBuilder.Entity<Car>(entity =>
            {
                entity.Property(e => e.CarId).HasColumnName("car_id");

                entity.Property(e => e.Availability)
                    .HasColumnName("availability")
                    .HasDefaultValueSql("((1))");

                entity.Property(e => e.CarType)
                    .HasMaxLength(50)
                    .HasColumnName("car_type");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.ImageUrl)
                    .HasMaxLength(255)
                    .HasColumnName("image_url");

                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .HasColumnName("location");

                entity.Property(e => e.Make)
                    .HasMaxLength(50)
                    .HasColumnName("make");

                entity.Property(e => e.Model)
                    .HasMaxLength(50)
                    .HasColumnName("model");

                entity.Property(e => e.PricePerDay)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price_per_day");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.Year).HasColumnName("year");
            });

            modelBuilder.Entity<CarExtra>(entity =>
            {
                entity.HasKey(e => e.ExtraId)
                    .HasName("PK__CarExtra__50512FA557C6053E");

                entity.Property(e => e.ExtraId).HasColumnName("extra_id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");
            });


            // Configuring the 'Payment' table without enums, using string values directly
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId)
                      .HasName("PK_Payments");

                entity.Property(e => e.PaymentId)
                      .HasColumnName("payment_id");

                entity.Property(e => e.ReservationId)
                      .HasColumnName("reservation_id");

                entity.Property(e => e.PaymentMethod)
                      .HasMaxLength(20)
                      .HasColumnName("payment_method")
                      .IsRequired()
                      .HasDefaultValue("credit_card") // Default value, if needed
                      .HasConversion(
                          v => v, // No conversion, just store as string
                          v => v); // No conversion, just map the string to the model's property

                entity.Property(e => e.PaymentDate)
                      .HasColumnType("datetime2")
                      .HasColumnName("payment_date")
                      .HasDefaultValueSql("SYSDATETIME()");

                entity.Property(e => e.Amount)
                      .HasColumnType("decimal(10, 2)")
                      .HasColumnName("amount");

                entity.Property(e => e.PaymentStatus)
                      .HasMaxLength(10)
                      .HasColumnName("payment_status")
                      .IsRequired()
                      .HasDefaultValue("pending") // Default value, if needed
                      .HasConversion(
                          v => v, // No conversion, just store as string
                          v => v); // No conversion, just map the string to the model's property

                entity.HasOne(d => d.Reservation)
                      .WithMany(p => p.Payments)
                      .HasForeignKey(d => d.ReservationId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_Payments_Reservations");
            });

            modelBuilder.Entity<PasswordReset>(entity =>
            {
                // Define the primary key with a meaningful name
                entity.HasKey(e => e.ResetId)
                    .HasName("PK_PasswordReset");

                // Specify the table name
                entity.ToTable("PasswordReset");

                // Configure the ExpirationDate column with datetime type
                entity.Property(e => e.ExpirationDate)
                    .HasColumnType("datetime")
                    .HasColumnName("expiration_date");

                // Configure the ResetToken column with a maximum length of 100
                entity.Property(e => e.ResetToken)
                    .HasMaxLength(100)
                    .HasColumnName("reset_token");

                // Define the foreign key relationship with User and give a meaningful name
                entity.HasOne(d => d.User)
                    .WithMany(p => p.PasswordResetRequests)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_PasswordReset_User");
            });





            /*modelBuilder.Entity<PasswordReset>(entity =>
            {
                entity.HasKey(e => e.ResetId)
                    .HasName("PK__Password__40FB0520CEEC07F9");

                entity.Property(e => e.ResetId).HasColumnName("reset_id");

                entity.Property(e => e.RequestTime)
                    .HasColumnName("request_time")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.ResetTime).HasColumnName("reset_time");

                entity.Property(e => e.ResetToken)
                    .HasMaxLength(255)
                    .HasColumnName("token");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PasswordResetRequests)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__PasswordR__user___6754599E");
            });*/

            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.Property(e => e.ReservationId).HasColumnName("reservation_id");

                entity.Property(e => e.CarId).HasColumnName("car_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.DropoffDate).HasColumnName("dropoff_date");

                entity.Property(e => e.PickupDate).HasColumnName("pickup_date");

                entity.Property(e => e.Status)
                    .HasMaxLength(10)
                    .HasColumnName("status")
                    .HasDefaultValueSql("('pending')");

                entity.Property(e => e.TotalPrice)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("total_price");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                // Relationship with Car (one-to-many)
                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__Reservati__car_i__46E78A0C");

                // Relationship with User (one-to-many)
                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reservations)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Reservati__user___45F365D3");

                // Many-to-many relationship with CarExtras (direct relationship)
                entity.HasMany(d => d.Extras)
                    .WithMany(p => p.Reservation)
                    .UsingEntity<Dictionary<string, object>>(
                        "ReservationCarExtras", // Join table name
                        j => j.HasOne<CarExtra>().WithMany().HasForeignKey("ExtraId"),
                        j => j.HasOne<Reservation>().WithMany().HasForeignKey("ReservationId"),
                        j =>
                        {
                            j.ToTable("ReservationCarExtras");
                            j.Property<int>("ReservationId").HasColumnName("reservation_id");
                            j.Property<int>("ExtraId").HasColumnName("extra_id");
                            j.HasKey("ReservationId", "ExtraId");
                        });
                entity.Ignore(e => e.CarExtraIds);
            });



            modelBuilder.Entity<Review>(entity =>
            {
                entity.Property(e => e.ReviewId).HasColumnName("review_id");

                entity.Property(e => e.CarId).HasColumnName("car_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.ReviewText).HasColumnName("review_text");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.Car)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.CarId)
                    .HasConstraintName("FK__Reviews__car_id__534D60F1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Reviews__user_id__52593CB8");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164C3748354")
                    .IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.UserName)
                  .HasMaxLength(50)
                  .HasColumnName("User_name");

                entity.Property(e => e.Password)
                    .HasMaxLength(255)
                    .HasColumnName("password");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(15)
                    .HasColumnName("phone_number");

                entity.Property(e => e.Role)
                    .HasMaxLength(10)
                    .HasColumnName("role");

                entity.Property(e => e.UpdatedAt)
                    .HasColumnName("updated_at")
                    .HasDefaultValueSql("(sysdatetime())");
            });

            /*modelBuilder.Entity<UserAudit>(entity =>
            {
                entity.HasKey(e => e.AuditId)
                    .HasName("PK__UserAudi__5AF33E33E1A9B2CA");

                entity.ToTable("UserAudit");

                entity.Property(e => e.AuditId).HasColumnName("audit_id");

                entity.Property(e => e.Action)
                    .HasMaxLength(100)
                    .HasColumnName("action");

                entity.Property(e => e.ActionTime)
                    .HasColumnName("action_time")
                    .HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.Details).HasColumnName("details");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAudits)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__UserAudit__user___5AEE82B9");
            });*/

            /*modelBuilder.Entity<ReservationExtra>()
     .HasKey(re => new { re.ReservationId, re.ExtraId });

            modelBuilder.Entity<ReservationExtra>()
                .HasOne(re => re.Reservation)
                .WithMany(r => r.ReservationExtra)  // Use ReservationExtras instead of Extras
                .HasForeignKey(re => re.ReservationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReservationExtra_Reservation");

            modelBuilder.Entity<ReservationExtra>()
                .HasOne(re => re.CarExtra)
                .WithMany(ce => ce.ReservationExtra)
                .HasForeignKey(re => re.ExtraId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ReservationExtra_CarExtra");*/

            // Configure the many-to-many relationship between Reservation and CarExtra



        }


    }
}