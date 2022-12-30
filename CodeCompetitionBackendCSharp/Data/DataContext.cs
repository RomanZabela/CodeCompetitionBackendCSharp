using System;
using System.Collections.Generic;
using CodeCompetitionBackendCSharp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CodeCompetitionBackendCSharp.Data
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Competition> Competitions { get; set; } = null!;
        public virtual DbSet<CompetitionStatus> CompetitionStatuses { get; set; } = null!;
        public virtual DbSet<Participant> Participants { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<Status> Statuses { get; set; } = null!;
        public virtual DbSet<Models.Task> Tasks { get; set; } = null!;
        public virtual DbSet<TaskCategory> TaskCategories { get; set; } = null!;
        public virtual DbSet<Models.TaskStatus> TaskStatuses { get; set; } = null!;
        public virtual DbSet<TaskToCompetition> TaskToCompetitions { get; set; } = null!;
        public virtual DbSet<TaskToTeam> TaskToTeams { get; set; } = null!;
        public virtual DbSet<Team> Teams { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<UserToRole> UserToRoles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Competition>(entity =>
            {
                entity.ToTable("competition");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompetitionAdministratorId).HasColumnName("competition_administrator_id");

                entity.Property(e => e.CompetitionName)
                    .HasMaxLength(50)
                    .HasColumnName("competition_name");

                entity.Property(e => e.CompetitionStatusId).HasColumnName("competition_status_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.HashCode)
                    .HasMaxLength(255)
                    .HasColumnName("hash_code");

                entity.Property(e => e.MaxTasksPerGroups).HasColumnName("max_tasks_per_groups");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.CompetitionAdministrator)
                    .WithMany(p => p.CompetitionCompetitionAdministrators)
                    .HasForeignKey(d => d.CompetitionAdministratorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_competition_administrator_user");

                entity.HasOne(d => d.CompetitionStatus)
                    .WithMany(p => p.Competitions)
                    .HasForeignKey(d => d.CompetitionStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_competition_competition_status");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.CompetitionCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_competition_create_user");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Competitions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_competition_status");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.CompetitionUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_competition_update_user");
            });

            modelBuilder.Entity<CompetitionStatus>(entity =>
            {
                entity.ToTable("competition_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(50)
                    .HasColumnName("status_name");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.CompetitionStatusCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_create_user");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.CompetitionStatuses)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_competition_status_status_id");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.CompetitionStatusUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_update_user");
            });

            modelBuilder.Entity<Participant>(entity =>
            {
                entity.ToTable("participant");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .HasColumnName("email");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TeamId).HasColumnName("team_id");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.ParticipantCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_participant_user");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Participants)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_participant_status");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.ParticipantUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_participant_user1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ParticipantUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_participant_user2");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .HasColumnName("role_name");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_role_status");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.Roles)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_role_update_user");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.ToTable("status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<Models.Task>(entity =>
            {
                entity.ToTable("task");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.Points).HasColumnName("points");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TaskCategoryId).HasColumnName("task_category_id");

                entity.Property(e => e.TaskContent).HasColumnName("task_content");

                entity.Property(e => e.TaskDescription).HasColumnName("task_description");

                entity.Property(e => e.TaskName)
                    .HasMaxLength(100)
                    .HasColumnName("task_name");

                entity.Property(e => e.Timeframe).HasColumnName("timeframe");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.TaskCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_create_user");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_status");

                entity.HasOne(d => d.TaskCategory)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.TaskCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_task_category");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.TaskUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_update_user");
            });

            modelBuilder.Entity<TaskCategory>(entity =>
            {
                entity.ToTable("task_category");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryDescription).HasColumnName("category_description");

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(50)
                    .HasColumnName("category_name");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.TaskCategoryCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_category_create_user");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TaskCategories)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_category_status");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.TaskCategoryUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_category_update_user");
            });

            modelBuilder.Entity<Models.TaskStatus>(entity =>
            {
                entity.ToTable("task_status");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.StatusName)
                    .HasMaxLength(50)
                    .HasColumnName("status_name");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.TaskStatusCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_status_create_user");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TaskStatuses)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_status_status");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.TaskStatusUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_status_update_user");
            });

            modelBuilder.Entity<TaskToCompetition>(entity =>
            {
                entity.ToTable("task_to_competition");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CompetitionId).HasColumnName("competition_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.TaskToCompetitions)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_competition_competition");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.TaskToCompetitionCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_competition_create_user");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TaskToCompetitions)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_competition_status");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskToCompetitions)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_competition_task");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.TaskToCompetitionUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_competition_update_user");
            });

            modelBuilder.Entity<TaskToTeam>(entity =>
            {
                entity.ToTable("task_to_team");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.ParticipantIdForTask).HasColumnName("participant_id_for_task");

                entity.Property(e => e.ReachedScore).HasColumnName("reached_score");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TaskId).HasColumnName("task_id");

                entity.Property(e => e.TaskStatusId).HasColumnName("task_status_id");

                entity.Property(e => e.TeamId).HasColumnName("team_id");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.TaskToTeamCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_team_create_user");

                entity.HasOne(d => d.ParticipantIdForTaskNavigation)
                    .WithMany(p => p.TaskToTeams)
                    .HasForeignKey(d => d.ParticipantIdForTask)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_team_participant");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.TaskToTeams)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_team_status");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.TaskToTeams)
                    .HasForeignKey(d => d.TaskId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_team_task");

                entity.HasOne(d => d.TaskStatus)
                    .WithMany(p => p.TaskToTeams)
                    .HasForeignKey(d => d.TaskStatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_team_task_status");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TaskToTeams)
                    .HasForeignKey(d => d.TeamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_team_team");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.TaskToTeamUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_task_to_team_update_user");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.ToTable("team");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CompetitionId).HasColumnName("competition_id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.IconImage).HasColumnName("icon_image");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TeamLeaderId).HasColumnName("team_leader_id");

                entity.Property(e => e.TeamName)
                    .HasMaxLength(100)
                    .HasColumnName("team_name");

                entity.Property(e => e.TotalPoints).HasColumnName("total_points");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_team_competition");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.TeamCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_team_create_user");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_team_status");

                entity.HasOne(d => d.TeamLeader)
                    .WithMany(p => p.Teams)
                    .HasForeignKey(d => d.TeamLeaderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_team_leader_participant");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.TeamUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_team_update_user");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.FirstName)
                    .HasMaxLength(50)
                    .HasColumnName("first_name");

                entity.Property(e => e.LastName)
                    .HasMaxLength(50)
                    .HasColumnName("last_name");

                entity.Property(e => e.Login)
                    .HasMaxLength(100)
                    .HasColumnName("login");

                entity.Property(e => e.Password)
                    .HasMaxLength(100)
                    .HasColumnName("password");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_status");
            });

            modelBuilder.Entity<UserToRole>(entity =>
            {
                entity.ToTable("user_to_role");

                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("create_date");

                entity.Property(e => e.CreateUserId).HasColumnName("create_user_id");

                entity.Property(e => e.RoleId).HasColumnName("role_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.UpdateDate)
                    .HasColumnType("datetime")
                    .HasColumnName("update_date");

                entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.CreateUser)
                    .WithMany(p => p.UserToRoleCreateUsers)
                    .HasForeignKey(d => d.CreateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_create_user_id");

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserToRoles)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_to_role_role");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.UserToRoles)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_to_role_status");

                entity.HasOne(d => d.UpdateUser)
                    .WithMany(p => p.UserToRoleUpdateUsers)
                    .HasForeignKey(d => d.UpdateUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_update_user_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserToRoleUsers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_user_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
