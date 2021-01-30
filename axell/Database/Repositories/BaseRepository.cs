using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Database.Attributes;

namespace Database.Repositories
{
    public class BaseRepository
    {
        private static string getTableName(object obj)
        {
            foreach (var attr in System.Attribute.GetCustomAttributes(obj.GetType()))
            {
                if (attr is ModelAttribute)
                {
                    var modelAttribute = (ModelAttribute)attr;
                    return modelAttribute.Table;
                }
            }

            return null;
        }

        private static IEnumerable<PropertyInfo> getProperties(object obj)
        {
            return obj.GetType().GetProperties().Where(
                prop => (
                    prop.IsDefined(typeof(ColumnAttribute), true)
                    && !prop.IsDefined(typeof(PkAttribute), true)
                )
            );
        }

        private static string getAttributes(object obj)
        {
            var attrs = "";
            var properties = getProperties(obj);

            foreach (var property in properties)
            {
                attrs += property.Name.ToLower() + ", ";
            }

            return attrs.Substring(0, attrs.Length - 2);
        }

        private static string getValues(object obj)
        {
            var values = "";
            var properties = getProperties(obj);

            foreach (var property in properties)
            {
                values += $"\"{property.GetValue(obj)}\", ";
            }

            return values.Substring(0, values.Length - 2);
        }

        public static void Save(object obj)
        {
            var tableName = getTableName(obj);
            var attrsStr = getAttributes(obj);
            var attrsValues = getValues(obj);
            var sql = $"INSERT INTO {tableName} ({attrsStr}) VALUES ({attrsValues});";
            Console.WriteLine(sql);
        }
    }
}
