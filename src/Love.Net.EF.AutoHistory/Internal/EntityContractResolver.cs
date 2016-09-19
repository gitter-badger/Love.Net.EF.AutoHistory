// Copyright (c) love.net team. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Love.Net.EF.AutoHistory.Internal {
    internal class EntityContractResolver : DefaultContractResolver {
        private readonly DbContext _dbContext;

        public EntityContractResolver(DbContext dbContext) {
            _dbContext = dbContext;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
            var list = base.CreateProperties(type, memberSerialization);

            var entry = _dbContext.ChangeTracker.Entries().FirstOrDefault(e => e.Entity.GetType() == type);
            if (entry == null) {
                return list;
            }

            var navigations = new List<string>();
            var properties = type.GetTypeInfo().GetProperties();
            foreach (var property in properties) {
                var navigation = entry.Metadata.FindNavigation(property);
                if (navigation != null) {
                    navigations.Add(property.Name);
                }
            }

            return list.Where(p => !navigations.Contains(p.PropertyName)).ToArray();
        }
    }
}
