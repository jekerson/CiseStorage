using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data;

public partial class SiceDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public SiceDbContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Domain.Entities.Attribute> Attributes { get; set; }

    public virtual DbSet<AttributeCategory> AttributeCategories { get; set; }

    public virtual DbSet<AttributeUnit> AttributeUnits { get; set; }

    public virtual DbSet<AttributeValueType> AttributeValueTypes { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<EmployeeAudit> EmployeeAudits { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemAttributeValue> ItemAttributeValues { get; set; }

    public virtual DbSet<ItemAudit> ItemAudits { get; set; }

    public virtual DbSet<ItemCategory> ItemCategories { get; set; }

    public virtual DbSet<ItemCategoryAttribute> ItemCategoryAttributes { get; set; }

    public virtual DbSet<ItemResponsibility> ItemResponsibilities { get; set; }

    public virtual DbSet<ItemResponsibilityAudit> ItemResponsibilityAudits { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<Region> Regions { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<UnitCategory> UnitCategories { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    public virtual DbSet<UserInfoAudit> UserInfoAudits { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Database"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("address_pkey");

            entity.ToTable("address");

            entity.HasIndex(e => e.RegionId, "idx_address_region_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Apartment)
                .HasMaxLength(50)
                .HasColumnName("apartment");
            entity.Property(e => e.Building)
                .HasMaxLength(50)
                .HasColumnName("building");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.House)
                .HasMaxLength(50)
                .HasColumnName("house");
            entity.Property(e => e.RegionId).HasColumnName("region_id");
            entity.Property(e => e.Street)
                .HasMaxLength(50)
                .HasColumnName("street");

            entity.HasOne(d => d.Region).WithMany(p => p.Addresses)
                .HasForeignKey(d => d.RegionId)
                .HasConstraintName("address_region_id_fkey");
        });

        modelBuilder.Entity<Domain.Entities.Attribute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attribute_pkey");

            entity.ToTable("attribute");

            entity.HasIndex(e => e.Name, "attribute_name_key").IsUnique();

            entity.HasIndex(e => e.AttributeCategoryId, "idx_attribute_attribute_category_id");

            entity.HasIndex(e => e.AttributeUnitId, "idx_attribute_attribute_unit_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttributeCategoryId).HasColumnName("attribute_category_id");
            entity.Property(e => e.AttributeUnitId).HasColumnName("attribute_unit_id");
            entity.Property(e => e.IsRequired).HasColumnName("is_required");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");

            entity.HasOne(d => d.AttributeCategory).WithMany(p => p.Attributes)
                .HasForeignKey(d => d.AttributeCategoryId)
                .HasConstraintName("attribute_attribute_category_id_fkey");

            entity.HasOne(d => d.AttributeUnit).WithMany(p => p.Attributes)
                .HasForeignKey(d => d.AttributeUnitId)
                .HasConstraintName("attribute_attribute_unit_id_fkey");
        });

        modelBuilder.Entity<AttributeCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attribute_category_pkey");

            entity.ToTable("attribute_category");

            entity.HasIndex(e => e.Name, "attribute_category_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<AttributeUnit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attribute_unit_pkey");

            entity.ToTable("attribute_unit");

            entity.HasIndex(e => e.Name, "attribute_unit_name_key").IsUnique();

            entity.HasIndex(e => e.Symbol, "attribute_unit_symbol_key").IsUnique();

            entity.HasIndex(e => e.AttributeValueTypeId, "idx_attribute_unit_attribute_value_type_id");

            entity.HasIndex(e => e.UnitCategoryId, "idx_attribute_unit_unit_category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttributeValueTypeId).HasColumnName("attribute_value_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Symbol)
                .HasMaxLength(10)
                .HasColumnName("symbol");
            entity.Property(e => e.UnitCategoryId).HasColumnName("unit_category_id");

            entity.HasOne(d => d.AttributeValueType).WithMany(p => p.AttributeUnits)
                .HasForeignKey(d => d.AttributeValueTypeId)
                .HasConstraintName("attribute_unit_attribute_value_type_id_fkey");

            entity.HasOne(d => d.UnitCategory).WithMany(p => p.AttributeUnits)
                .HasForeignKey(d => d.UnitCategoryId)
                .HasConstraintName("attribute_unit_unit_category_id_fkey");
        });

        modelBuilder.Entity<AttributeValueType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("attribute_value_type_pkey");

            entity.ToTable("attribute_value_type");

            entity.HasIndex(e => e.Name, "attribute_value_type_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pkey");

            entity.ToTable("employee");

            entity.HasIndex(e => e.EmailAddress, "employee_email_address_key").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "employee_phone_number_key").IsUnique();

            entity.HasIndex(e => e.AddressId, "idx_employee_address_id");

            entity.HasIndex(e => e.IsDeleted, "idx_employee_is_deleted");

            entity.HasIndex(e => e.PositionId, "idx_employee_position_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(50)
                .HasColumnName("email_address");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.Sex)
                .HasMaxLength(10)
                .HasColumnName("sex");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");

            entity.HasOne(d => d.Address).WithMany(p => p.Employees)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("employee_address_id_fkey");

            entity.HasOne(d => d.Position).WithMany(p => p.Employees)
                .HasForeignKey(d => d.PositionId)
                .HasConstraintName("employee_position_id_fkey");
        });

        modelBuilder.Entity<EmployeeAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_audit_pkey");

            entity.ToTable("employee_audit");

            entity.HasIndex(e => e.ChangedAt, "idx_employee_audit_changed_at");

            entity.HasIndex(e => e.ChangedBy, "idx_employee_audit_changed_by");

            entity.HasIndex(e => e.EmployeeId, "idx_employee_audit_employee_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.AddressId).HasColumnName("address_id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.EmailAddress)
                .HasMaxLength(50)
                .HasColumnName("email_address");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phone_number");
            entity.Property(e => e.PositionId).HasColumnName("position_id");
            entity.Property(e => e.Sex)
                .HasMaxLength(10)
                .HasColumnName("sex");
            entity.Property(e => e.Surname)
                .HasMaxLength(50)
                .HasColumnName("surname");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.EmployeeAudits)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("employee_audit_changed_by_fkey");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_pkey");

            entity.ToTable("item");

            entity.HasIndex(e => e.CategoryId, "idx_item_category_id");

            entity.HasIndex(e => e.IsDeleted, "idx_item_is_deleted");

            entity.HasIndex(e => e.Name, "item_name_key").IsUnique();

            entity.HasIndex(e => e.Number, "item_number_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .HasColumnName("number");

            entity.HasOne(d => d.Category).WithMany(p => p.Items)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("item_category_id_fkey");
        });

        modelBuilder.Entity<ItemAttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_attribute_value_pkey");

            entity.ToTable("item_attribute_value");

            entity.HasIndex(e => e.AttributeId, "idx_item_attribute_value_attribute_id");

            entity.HasIndex(e => e.ItemId, "idx_item_attribute_value_item_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttributeId).HasColumnName("attribute_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Value)
                .HasMaxLength(255)
                .HasColumnName("value");

            entity.HasOne(d => d.Attribute).WithMany(p => p.ItemAttributeValues)
                .HasForeignKey(d => d.AttributeId)
                .HasConstraintName("item_attribute_value_attribute_id_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemAttributeValues)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("item_attribute_value_item_id_fkey");
        });

        modelBuilder.Entity<ItemAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_audit_pkey");

            entity.ToTable("item_audit");

            entity.HasIndex(e => e.ChangedAt, "idx_item_audit_changed_at");

            entity.HasIndex(e => e.ChangedBy, "idx_item_audit_changed_by");

            entity.HasIndex(e => e.ItemId, "idx_item_audit_item_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Number)
                .HasMaxLength(50)
                .HasColumnName("number");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.ItemAudits)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("item_audit_changed_by_fkey");
        });

        modelBuilder.Entity<ItemCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_category_pkey");

            entity.ToTable("item_category");

            entity.HasIndex(e => e.ParentCategoryId, "idx_item_category_parent_category_id");

            entity.HasIndex(e => e.Name, "item_category_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.ParentCategoryId).HasColumnName("parent_category_id");

            entity.HasOne(d => d.ParentCategory).WithMany(p => p.InverseParentCategory)
                .HasForeignKey(d => d.ParentCategoryId)
                .HasConstraintName("item_category_parent_category_id_fkey");
        });

        modelBuilder.Entity<ItemCategoryAttribute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_category_attribute_pkey");

            entity.ToTable("item_category_attribute");

            entity.HasIndex(e => e.AttributeId, "idx_item_category_attribute_attribute_id");

            entity.HasIndex(e => e.ItemCategoryId, "idx_item_category_attribute_item_category_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AttributeId).HasColumnName("attribute_id");
            entity.Property(e => e.ItemCategoryId).HasColumnName("item_category_id");

            entity.HasOne(d => d.Attribute).WithMany(p => p.ItemCategoryAttributes)
                .HasForeignKey(d => d.AttributeId)
                .HasConstraintName("item_category_attribute_attribute_id_fkey");

            entity.HasOne(d => d.ItemCategory).WithMany(p => p.ItemCategoryAttributes)
                .HasForeignKey(d => d.ItemCategoryId)
                .HasConstraintName("item_category_attribute_item_category_id_fkey");
        });

        modelBuilder.Entity<ItemResponsibility>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_responsibility_pkey");

            entity.ToTable("item_responsibility");

            entity.HasIndex(e => e.EmployeeId, "idx_item_responsibility_employee_id");

            entity.HasIndex(e => e.ItemId, "idx_item_responsibility_item_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("assigned_at");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.UnassignedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("unassigned_at");

            entity.HasOne(d => d.Employee).WithMany(p => p.ItemResponsibilities)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("item_responsibility_employee_id_fkey");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemResponsibilities)
                .HasForeignKey(d => d.ItemId)
                .HasConstraintName("item_responsibility_item_id_fkey");
        });

        modelBuilder.Entity<ItemResponsibilityAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("item_responsibility_audit_pkey");

            entity.ToTable("item_responsibility_audit");

            entity.HasIndex(e => e.ChangedAt, "idx_item_responsibility_audit_changed_at");

            entity.HasIndex(e => e.ChangedBy, "idx_item_responsibility_audit_changed_by");

            entity.HasIndex(e => e.EmployeeId, "idx_item_responsibility_audit_employee_id");

            entity.HasIndex(e => e.ItemId, "idx_item_responsibility_audit_item_id");

            entity.HasIndex(e => e.ItemResponsibilityId, "idx_item_responsibility_audit_item_responsibility_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.AssignedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("assigned_at");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.ItemId).HasColumnName("item_id");
            entity.Property(e => e.ItemResponsibilityId).HasColumnName("item_responsibility_id");
            entity.Property(e => e.UnassignedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("unassigned_at");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.ItemResponsibilityAudits)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("item_responsibility_audit_changed_by_fkey");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("permission_pkey");

            entity.ToTable("permission");

            entity.HasIndex(e => e.Name, "permission_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Position>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("position_pkey");

            entity.ToTable("position");

            entity.HasIndex(e => e.Name, "position_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Region>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("region_pkey");

            entity.ToTable("region");

            entity.HasIndex(e => e.Name, "region_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(75)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.HasIndex(e => e.Name, "role_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_permission_pkey");

            entity.ToTable("role_permission");

            entity.HasIndex(e => e.PermissionId, "idx_role_permission_permission_id");

            entity.HasIndex(e => e.RoleId, "idx_role_permission_role_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PermissionId).HasColumnName("permission_id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.PermissionId)
                .HasConstraintName("role_permission_permission_id_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("role_permission_role_id_fkey");
        });

        modelBuilder.Entity<UnitCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("unit_category_pkey");

            entity.ToTable("unit_category");

            entity.HasIndex(e => e.Name, "unit_category_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_info_pkey");

            entity.ToTable("user_info");

            entity.HasIndex(e => e.EmployeeId, "idx_user_info_employee_id");

            entity.HasIndex(e => e.IsDeleted, "idx_user_info_is_deleted");

            entity.HasIndex(e => e.Salt, "user_info_salt_key").IsUnique();

            entity.HasIndex(e => e.Username, "user_info_username_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.HashedPassword)
                .HasMaxLength(255)
                .HasColumnName("hashed_password");
            entity.Property(e => e.IsDeleted)
                .HasDefaultValue(false)
                .HasColumnName("is_deleted");
            entity.Property(e => e.Salt)
                .HasMaxLength(1024)
                .HasColumnName("salt");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Employee).WithMany(p => p.UserInfos)
                .HasForeignKey(d => d.EmployeeId)
                .HasConstraintName("user_info_employee_id_fkey");
            entity.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.UserInfo)
                .HasForeignKey(rt => rt.UserInfoId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserInfoAudit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_info_audit_pkey");

            entity.ToTable("user_info_audit");

            entity.HasIndex(e => e.ChangedAt, "idx_user_info_audit_changed_at");

            entity.HasIndex(e => e.ChangedBy, "idx_user_info_audit_changed_by");

            entity.HasIndex(e => e.UserInfoId, "idx_user_info_audit_user_info_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Action)
                .HasMaxLength(50)
                .HasColumnName("action");
            entity.Property(e => e.ChangedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("changed_at");
            entity.Property(e => e.ChangedBy).HasColumnName("changed_by");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.HashedPassword)
                .HasMaxLength(255)
                .HasColumnName("hashed_password");
            entity.Property(e => e.Salt)
                .HasMaxLength(1024)
                .HasColumnName("salt");
            entity.Property(e => e.UserInfoId).HasColumnName("user_info_id");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.ChangedByNavigation).WithMany(p => p.UserInfoAudits)
                .HasForeignKey(d => d.ChangedBy)
                .HasConstraintName("user_info_audit_changed_by_fkey");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_role_pkey");

            entity.ToTable("user_role");

            entity.HasIndex(e => e.RoleId, "idx_user_role_role_id");

            entity.HasIndex(e => e.UserInfoId, "idx_user_role_user_info_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UserInfoId).HasColumnName("user_info_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("user_role_role_id_fkey");

            entity.HasOne(d => d.UserInfo).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserInfoId)
                .HasConstraintName("user_role_user_info_id_fkey");
        });
        modelBuilder.Entity<RefreshToken>(builder =>
        {
            builder.ToTable("refresh_token");
            builder.HasIndex(rt => rt.ExpiresAt);
            builder.HasIndex(rt => rt.Token);

            builder.HasKey(rt => rt.Id);

            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(rt => rt.ExpiresAt)
                .IsRequired();

            builder.Property(rt => rt.CreatedAt)
                .IsRequired();

            builder.HasOne(rt => rt.UserInfo)
                .WithMany()
                .HasForeignKey(rt => rt.UserInfoId)
                .OnDelete(DeleteBehavior.Cascade);
        });



        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
