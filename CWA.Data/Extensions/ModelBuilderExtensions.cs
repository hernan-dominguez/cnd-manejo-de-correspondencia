using CWA.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CWA.Data.Extensions
{
    public static class ModelBuilderExtensions
    {
        public static void SetAuditingDateSQL<T>(this ModelBuilder builder) where T : class
        {
            // Auditoría
            builder.Entity<T>()
                .Property("RegFecha")
                .HasDefaultValueSql("SYSDATE");

            builder.Entity<T>()
                .Property("ModFecha")
                .HasDefaultValueSql("SYSDATE");
        }

        public static void SetNamesUppercase(this ModelBuilder builder)
        {
            // This is recommended when working with Oracle database
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                var schema = entity.GetSchema();
                var tableName = entity.GetTableName().ToUpper();
                var soi = StoreObjectIdentifier.Table(tableName, schema);

                entity.SetTableName(tableName);

                foreach (var property in entity.GetProperties()) property.SetColumnName(property.GetColumnName(soi).ToUpper());

                foreach (var key in entity.GetKeys()) key.SetName(key.GetName().ToUpper());

                foreach (var fkey in entity.GetForeignKeys()) fkey.SetConstraintName(fkey.GetConstraintName().ToUpper());

                foreach (var index in entity.GetIndexes()) index.SetDatabaseName(index.GetDatabaseName().ToUpper());
            }
        }

        public static void SetStringPropertiesAutoTrim(this ModelBuilder builder)
        {
            // Prevent reading and/or writing string values with leading and/or trailing blanks
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                foreach (var property in entity.GetProperties())
                {
                    if (property.ClrType.Name == "String") property.SetValueConverter(new ValueConverter<string, string>(v => v.Trim(), v => v.Trim()));
                }
            }
        }
    }
}
