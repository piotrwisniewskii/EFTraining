﻿using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using System;

namespace MyBoards.Entities
{
    public class MyBoardsContext : DbContext
    {
        public MyBoardsContext(DbContextOptions<MyBoardsContext> options) : base(options)
        {

        }
        public DbSet<WorkItem> WorkItems { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Epic> Epics { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Adress> Adresses { get; set; }

        public DbSet<WorkItemState> WorkItemsStates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Epic>()
                .Property(wi => wi.EndDate)
                .HasPrecision(3);

            modelBuilder.Entity<Task>()
                 .Property(wi => wi.Activity)
                 .HasMaxLength(200);

            modelBuilder.Entity<Task>()
                  .Property(wi => wi.RemainingWork)
                  
                  .HasPrecision(14, 2);
            modelBuilder.Entity<Issue>()
                .Property(wi => wi.Efford).HasColumnType("decimal(5,2)");


            modelBuilder.Entity<WorkItem>(eb =>
            {
                eb.Property(wi => wi.Area).HasColumnType("varchar(200)");
                eb.Property(wi => wi.IterationPath).HasColumnName("Iteration_Path");
                eb.Property(wi=>wi.Priority).HasDefaultValue(1);

                //Relation Configuration
                eb.HasMany(w => w.Comments)
                .WithOne(c => c.WorkItem)
                .HasForeignKey(c=>c.WorkItem.Id);

                eb.HasOne(w => w.Author)
                .WithMany(u => u.WorkItems)
                .HasForeignKey(w => w.AuthorId);

                eb.HasOne(w => w.State)
                .WithMany()
                .HasForeignKey(wis => wis.StateId);


                eb.HasMany(w => w.Tags)
                .WithMany(t => t.WorkItems)
                .UsingEntity<WorkItemTag>(
                    w => w.HasOne(wit => wit.Tag)    //WorkItemTag to Tag
                    .WithMany()
                    .HasForeignKey(wit => wit.TagId),

                    w => w.HasOne(wit => wit.WorkItem)    //WorkItemTag to WorkItem
                    .WithMany()
                    .HasForeignKey(wit => wit.WorkItemId),

                    wit =>
                    {
                        wit.HasKey(x => new { x.TagId, x.WorkItemId });
                        wit.Property(x => x.PublicationDate).HasDefaultValueSql("getutcdate()");
                    }

                    );
               
            });

            modelBuilder.Entity<WorkItemState>(eb =>
            {
                eb.Property(s => s.Value)
                .IsRequired()
                .HasColumnType("varchar(50)");
            });

            modelBuilder.Entity<Comment>(eb =>
            {
                eb.Property(x => x.CreatedDate).HasDefaultValueSql("getutcdate()");
                eb.Property(x => x.UpdatedDate).ValueGeneratedOnUpdate();
            });

            modelBuilder.Entity<User>()
                .HasOne(u => u.Adress)
                .WithOne(u => u.User)
                .HasForeignKey<Adress>(a => a.UserId);

            
        }
    }
}
