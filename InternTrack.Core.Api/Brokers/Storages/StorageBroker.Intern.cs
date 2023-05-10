﻿// -------------------------------------------------------
// Copyright (c) Coalition of the Good-Hearted Engineers
// FREE TO USE FOR THE WORLD
// -------------------------------------------------------

using InternTrack.Core.Api.Models.Interns;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace InternTrack.Core.Api.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Intern> Interns { get; set; }

        public async ValueTask<Intern> DeleteInternsAsync(Intern intern)
        {
            using var broker =
                 new StorageBroker(this.configuration);

            EntityEntry<Intern> internEntityEntry =
                broker.Interns.Remove(intern);

            await broker.SaveChangesAsync();

            return internEntityEntry.Entity;
        }

        public async ValueTask<Intern> InsertAllInterns(Intern intern)
        {
            var broker = new StorageBroker(this.configuration);
            EntityEntry<Intern> internEntityEntry = await broker.AddAsync(intern);
            await broker.SaveChangesAsync();

            return internEntityEntry.Entity;
        }

        public IQueryable<Intern> SelectInternAsync()
        {
            using var broker =
               new StorageBroker(this.configuration);

            return broker.Interns;
        }
        public async ValueTask<Intern> SelectInternByIdAsync(Guid internId)
        {
            using var broker =
                   new StorageBroker(this.configuration);

            return await broker.Interns.FindAsync(internId);
        }

        public async ValueTask<Intern> UpdateInternAsync(Intern intern)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Intern> internEntityEntry =
                broker.Interns.Update(intern);

            await broker.SaveChangesAsync();

            return internEntityEntry.Entity;
        }
    }
}
