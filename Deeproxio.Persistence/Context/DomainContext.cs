﻿using System;
using System.Linq;
using Deeproxio.Domain.Models;
using Deeproxio.Persistence.Configuration.Context;
using Deeproxio.Persistence.Configuration.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Deeproxio.Persistence.Context
{
    public class DomainContext : DbContext, IUnitOfWork
    {
        private string _connectionString;
        public virtual DbSet<Media> Media { get; set; }
        public virtual DbSet<MediaAttachment> MediaAttachment { get; set; }
        public virtual DbSet<MediaProcessingNode> MeMediaProcessingNodedia { get; set; }
        public virtual DbSet<MediaProcessOperation> MediaProcessOperation { get; set; }
        public virtual DbSet<MediaProcessRequest> MediaProcessRequest { get; set; }
        public virtual DbSet<MediaProcessResult> MediaProcessResult { get; set; }
        public virtual DbSet<MediaSource> MediaSource { get; set; }
        public virtual DbSet<MediaType> MediaType { get; set; }

        public DbContext Context => this;

        // When used with ASP.net core, add these lines to Startup.cs
        //   var connectionString = Configuration.GetConnectionString("BlogContext");
        //   services.AddEntityFrameworkNpgsql().AddDbContext<BlogContext>(options => options.UseNpgsql(connectionString));
        // and add this to appSettings.json
        // "ConnectionStrings": { "BlogContext": "Server=localhost;Database=blog" }

        public DomainContext()
        {
            
        }

        public DomainContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DomainContext(DbContextOptions<DomainContext> options) : base(options)
        { 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("dbo");

            var mapTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .Where(x => x.BaseType != null && x.BaseType.IsGenericType && x.BaseType.GetGenericTypeDefinition() == typeof(BaseMap<>))
                .ToList();

            foreach (var type in mapTypes)
            {
                dynamic instance = Activator.CreateInstance(type);

                instance.Map(modelBuilder);
            }
        }
    }
}
