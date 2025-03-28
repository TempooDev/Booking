using System.Reflection;

using Booking.Booking.Application.TodoItems.Domain;
using Booking.Booking.Application.TodoItems.Domain.ValueObjects;
using Booking.Booking.Application.TodoLists.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using Shared.Common;
using Shared.Common.Interfaces;

namespace Booking.Booking.Application.Common.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IDateTime _dateTime;
    private readonly IDomainEventService _domainEventService;

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUserService currentUserService,
        IDomainEventService domainEventService,
        IDateTime dateTime)
        : base(options)
    {
        _currentUserService = currentUserService;
        _domainEventService = domainEventService;
        _dateTime = dateTime;
    }

    public DbSet<TodoList> TodoLists => Set<TodoList>();

    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = _currentUserService.UserId;
                    entry.Entity.Created = _dateTime.Now;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModified = _dateTime.Now;
                    break;
                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    break;
            }
        }

        var events = ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .Where(domainEvent => !domainEvent.IsPublished)
                .ToArray();

        var result = await base.SaveChangesAsync(cancellationToken);

        await DispatchEvents(events);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Ignorar DomainEvent y sus colecciones
        builder.Ignore<DomainEvent>();
        builder.Ignore<List<DomainEvent>>();

        builder.Entity<TodoList>(entity =>
        {
            entity.ToTable("TodoLists");

            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsRequired();

            // Configuración correcta del Value Object Colour
            entity.OwnsOne(e => e.Colour, colourBuilder =>
            {
                colourBuilder.Property(c => c.Code)
                    .HasColumnName("ColourCode")
                    .HasMaxLength(7)
                    .IsRequired(false);  // Hacerlo opcional ya que Colour es nullable
            });
        });

        builder.Entity<TodoItem>(entity =>
        {
            entity.ToTable("TodoItems");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .IsRequired();

            entity.HasOne(d => d.List)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.ListId);
        });

        // Configurar todas las propiedades DateTime para usar UTC
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Utc ? v : DateTime.SpecifyKind(v, DateTimeKind.Utc),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)));
                }
            }
        }

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .LogTo(Console.WriteLine);
        }

        base.OnConfiguring(optionsBuilder);
    }

    private async Task DispatchEvents(DomainEvent[] events)
    {
        foreach (var @event in events)
        {
            @event.IsPublished = true;
            await _domainEventService.Publish(@event);
        }
    }
}