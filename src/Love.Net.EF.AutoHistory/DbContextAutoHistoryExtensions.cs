// Copyright (c) love.net team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Love.Net.EF.AutoHistory.Internal;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Microsoft.EntityFrameworkCore {
    /// <summary>
    /// Represents a plugin for Microsoft.EntityFrameworkCore to support automatically recording data changes history.
    /// </summary>
    public static class DbContextAutoHistoryExtensions {
        /// <summary>
        /// Ensures the automatic history.
        /// </summary>
        /// <param name="context">The context.</param>
        public static void EnsureAutoHistory(this DbContext context) {
            var jsonSetting = new JsonSerializerSettings {
                ContractResolver = new EntityContractResolver(context),
            };

            var entries = context.ChangeTracker.Entries().Where(e => e.State != EntityState.Unchanged).ToArray();
            foreach (var entry in entries) {
                var history = new AutoHistory {
                    TypeName = entry.Entity.GetType().FullName,
                };

                switch (entry.State) {
                    case EntityState.Added:
                        // REVIEW: what's the best way to do this?
                        history.SourceId = "0";
                        history.Kind = EntityState.Added;
                        history.AfterJson = JsonConvert.SerializeObject(entry.Entity, Formatting.Indented, jsonSetting);
                        break;
                    case EntityState.Deleted:
                        history.SourceId = entry.PrimaryKey();
                        history.Kind = EntityState.Deleted;
                        history.BeforeJson = JsonConvert.SerializeObject(entry.Entity, Formatting.Indented, jsonSetting);
                        break;
                    case EntityState.Modified:
                        history.SourceId = entry.PrimaryKey();
                        history.Kind = EntityState.Modified;
                        history.BeforeJson = JsonConvert.SerializeObject(entry.Original(), Formatting.Indented, jsonSetting);
                        history.AfterJson = JsonConvert.SerializeObject(entry.Entity, Formatting.Indented, jsonSetting);
                        break;
                    default:
                        break;
                }

                context.Add(history);
            }
        }

        private static object Original(this EntityEntry entry) {
            var type = entry.Entity.GetType();
            var entity = Activator.CreateInstance(type, true);

            var properties = type.GetTypeInfo().GetProperties();
            foreach (var property in properties) {
                var nav = entry.Metadata.FindNavigation(property);
                if (nav != null) {
                    continue;
                }

                try {
                    // following code will throw exception if the property had been ignored.
                    var original = entry.Property(property.Name).OriginalValue;
                    property.SetValue(entity, original);
                }
                catch {
                    // what's the best way to get the ignored properties?
                    continue;
                }
            }

            return entity;
        }

        private static string PrimaryKey(this EntityEntry entry) {
            var key = entry.Metadata.FindPrimaryKey();

            var values = new List<object>();
            foreach (var property in key.Properties) {
                var value = entry.Property(property.Name).CurrentValue;
                if (value != null) {
                    values.Add(value);
                }
            }

            return string.Join(",", values);
        }
    }
}
