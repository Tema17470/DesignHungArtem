using Microsoft.EntityFrameworkCore;
using SmartGreenhouse.Domain.Entities;

namespace SmartGreenhouse.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Device> Devices => Set<Device>();
    public DbSet<SensorReading> Readings => Set<SensorReading>();
    public DbSet<AlertRule> AlertRules => Set<AlertRule>();
    public DbSet<AlertNotification> AlertNotifications => Set<AlertNotification>();
    public DbSet<ControlProfile> ControlProfiles => Set<ControlProfile>();
    public DbSet<DeviceStateSnapshot> DeviceStates { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Config for Device
        modelBuilder.Entity<Device>().HasKey(d => d.Id);
        modelBuilder.Entity<Device>().Property(d => d.DeviceName).HasMaxLength(120);
        modelBuilder.Entity<Device>().HasIndex(d => d.DeviceName);
        modelBuilder.Entity<Device>().HasIndex(d => d.DeviceType); 
        
        // Config for SendsorReading
        modelBuilder.Entity<SensorReading>()
            .HasKey(r => r.Id);
        
        modelBuilder.Entity<SensorReading>()
            .HasOne(r => r.Device)
            .WithMany(d => d.Readings)
            .HasForeignKey(r => r.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SensorReading>()
            .HasIndex(r => new { r.DeviceId, r.SensorType, r.Timestamp });
        

        // Config for AlertRUle
        modelBuilder.Entity<AlertRule>()
            .HasKey(ar => ar.Id);

        modelBuilder.Entity<AlertRule>()
            .Property(ar => ar.OperatorSymbol)
            .HasMaxLength(8);

        modelBuilder.Entity<AlertRule>()
            .HasIndex(ar => new { ar.DeviceId, ar.SensorType });

        modelBuilder.Entity<AlertRule>()
            .HasOne(ar => ar.Device)
            .WithMany()
            .HasForeignKey(ar => ar.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);
            
        //Config for AlerTnotification
        modelBuilder.Entity<AlertNotification>()
            .HasKey(an => an.Id);

        modelBuilder.Entity<AlertNotification>()
            .Property(an => an.OperatorSymbol)
            .HasMaxLength(8);

        modelBuilder.Entity<AlertNotification>()
            .HasIndex(an => new { an.DeviceId, an.SensorType, an.Timestamp });

        modelBuilder.Entity<AlertNotification>()
            .HasOne(an => an.AlertRule)
            .WithMany()
            .HasForeignKey(an => an.AlertRuleId)
            .OnDelete(DeleteBehavior.SetNull);
            
        //Config for ControlProfile
        modelBuilder.Entity<ControlProfile>()
            .HasKey(cp => cp.Id);

        modelBuilder.Entity<ControlProfile>()
            .Property(cp => cp.StrategyKey)
            .HasMaxLength(80);

        modelBuilder.Entity<ControlProfile>()
            .HasIndex(cp => cp.DeviceId)
            .IsUnique();
        
        modelBuilder.Entity<ControlProfile>()
            .HasOne(cp => cp.Device)
            .WithMany() // no Device.ControlProfiles collection
            .HasForeignKey(cp => cp.DeviceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeviceStateSnapshot>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.DeviceId, e.EnteredAt });
            entity.Property(e => e.StateName)
            .IsRequired()
            .HasMaxLength(64);
            entity.Property(e => e.EnteredAt)
            .IsRequired();
        });


    }
}