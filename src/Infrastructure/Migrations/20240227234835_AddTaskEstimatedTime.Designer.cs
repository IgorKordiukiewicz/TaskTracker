﻿// <auto-generated />
using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20240227234835_AddTaskEstimatedTime")]
    partial class AddTaskEstimatedTime
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Domain.Organizations.Organization", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("OwnerId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OwnerId");

                    b.ToTable("Organizations");
                });

            modelBuilder.Entity("Domain.Organizations.OrganizationInvitation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FinalizedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("State")
                        .HasColumnType("int");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("UserId");

                    b.ToTable("OrganizationInvitations");
                });

            modelBuilder.Entity("Domain.Organizations.OrganizationMember", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid?>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.HasIndex("UserId");

                    b.ToTable("OrganizationMembers");
                });

            modelBuilder.Entity("Domain.Organizations.OrganizationRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Permissions")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("OrganizationRoles");
                });

            modelBuilder.Entity("Domain.Projects.Project", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<Guid>("OrganizationId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OrganizationId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Domain.Projects.ProjectMember", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid?>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.HasIndex("UserId");

                    b.ToTable("ProjectMembers");
                });

            modelBuilder.Entity("Domain.Projects.ProjectRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Permissions")
                        .HasColumnType("int");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId");

                    b.ToTable("ProjectRoles");
                });

            modelBuilder.Entity("Domain.Tasks.Task", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AssigneeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("EstimatedTime")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<int>("Priority")
                        .HasColumnType("int");

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("ShortId")
                        .HasColumnType("int");

                    b.Property<Guid>("StatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AssigneeId");

                    b.HasIndex("ProjectId");

                    b.HasIndex("ShortId");

                    b.HasIndex("StatusId");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("Domain.Tasks.TaskActivity", b =>
                {
                    b.Property<int>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Key"));

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("NewValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OccurredAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("OldValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Property")
                        .HasColumnType("int");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Key");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskActivities");
                });

            modelBuilder.Entity("Domain.Tasks.TaskComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskComments");
                });

            modelBuilder.Entity("Domain.Tasks.TaskTimeLog", b =>
                {
                    b.Property<int>("Key")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Key"));

                    b.Property<DateTime>("Day")
                        .HasColumnType("date");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("LoggedBy")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Minutes")
                        .HasColumnType("int");

                    b.Property<Guid>("TaskId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Key");

                    b.HasIndex("LoggedBy");

                    b.HasIndex("TaskId");

                    b.ToTable("TaskTimeLogs");
                });

            modelBuilder.Entity("Domain.Users.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AuthenticationId")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.HasKey("Id");

                    b.HasIndex("AuthenticationId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Workflows.TaskStatus", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<bool>("Initial")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("WorkflowId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("WorkflowId");

                    b.ToTable("TaskStatuses");
                });

            modelBuilder.Entity("Domain.Workflows.TaskStatusTransition", b =>
                {
                    b.Property<Guid>("FromStatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("ToStatusId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid?>("WorkflowId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("FromStatusId", "ToStatusId");

                    b.HasIndex("WorkflowId");

                    b.ToTable("TaskStatusTransitions");
                });

            modelBuilder.Entity("Domain.Workflows.Workflow", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<Guid>("ProjectId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("ProjectId")
                        .IsUnique();

                    b.ToTable("Workflows");
                });

            modelBuilder.Entity("Infrastructure.Models.UserPresentationData", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("AvatarColor")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("nvarchar(7)");

                    b.Property<bool>("IsDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.HasKey("UserId");

                    b.ToTable("UsersPresentationData");
                });

            modelBuilder.Entity("Domain.Organizations.Organization", b =>
                {
                    b.HasOne("Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Organizations.OrganizationInvitation", b =>
                {
                    b.HasOne("Domain.Organizations.Organization", null)
                        .WithMany("Invitations")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Organizations.OrganizationMember", b =>
                {
                    b.HasOne("Domain.Organizations.Organization", null)
                        .WithMany("Members")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Organizations.OrganizationRole", b =>
                {
                    b.HasOne("Domain.Organizations.Organization", null)
                        .WithMany("Roles")
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Projects.Project", b =>
                {
                    b.HasOne("Domain.Organizations.Organization", null)
                        .WithMany()
                        .HasForeignKey("OrganizationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Projects.ProjectMember", b =>
                {
                    b.HasOne("Domain.Projects.Project", null)
                        .WithMany("Members")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Projects.ProjectRole", b =>
                {
                    b.HasOne("Domain.Projects.Project", null)
                        .WithMany("Roles")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Tasks.Task", b =>
                {
                    b.HasOne("Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("AssigneeId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Domain.Projects.Project", null)
                        .WithMany()
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Domain.Workflows.TaskStatus", null)
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Tasks.TaskActivity", b =>
                {
                    b.HasOne("Domain.Tasks.Task", null)
                        .WithMany("Activities")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Tasks.TaskComment", b =>
                {
                    b.HasOne("Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Tasks.Task", null)
                        .WithMany("Comments")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Tasks.TaskTimeLog", b =>
                {
                    b.HasOne("Domain.Users.User", null)
                        .WithMany()
                        .HasForeignKey("LoggedBy")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Domain.Tasks.Task", null)
                        .WithMany("TimeLogs")
                        .HasForeignKey("TaskId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Workflows.TaskStatus", b =>
                {
                    b.HasOne("Domain.Workflows.Workflow", null)
                        .WithMany("Statuses")
                        .HasForeignKey("WorkflowId");
                });

            modelBuilder.Entity("Domain.Workflows.TaskStatusTransition", b =>
                {
                    b.HasOne("Domain.Workflows.Workflow", null)
                        .WithMany("Transitions")
                        .HasForeignKey("WorkflowId");
                });

            modelBuilder.Entity("Domain.Workflows.Workflow", b =>
                {
                    b.HasOne("Domain.Projects.Project", null)
                        .WithOne()
                        .HasForeignKey("Domain.Workflows.Workflow", "ProjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Infrastructure.Models.UserPresentationData", b =>
                {
                    b.HasOne("Domain.Users.User", null)
                        .WithOne()
                        .HasForeignKey("Infrastructure.Models.UserPresentationData", "UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Domain.Organizations.Organization", b =>
                {
                    b.Navigation("Invitations");

                    b.Navigation("Members");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Domain.Projects.Project", b =>
                {
                    b.Navigation("Members");

                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Domain.Tasks.Task", b =>
                {
                    b.Navigation("Activities");

                    b.Navigation("Comments");

                    b.Navigation("TimeLogs");
                });

            modelBuilder.Entity("Domain.Workflows.Workflow", b =>
                {
                    b.Navigation("Statuses");

                    b.Navigation("Transitions");
                });
#pragma warning restore 612, 618
        }
    }
}
