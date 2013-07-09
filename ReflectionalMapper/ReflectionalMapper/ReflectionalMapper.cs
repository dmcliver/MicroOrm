using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;

namespace ReflectionalMapper
{
    public class ReflectionalMapper
    {
        private readonly ValueTypeChecker _valueTypeChecker = new ValueTypeChecker();

        public IEnumerable<T> Map<T>(SqlDataReader reader)
        {
            List<T> entities = new List<T>();
            using (reader)
            {
                while (reader.Read())
                {
                    T entity = Activator.CreateInstance<T>();
                    MapToFields<T>(reader, entity, entity.GetType().GetProperties());
                    entities.Add(entity);
                }
            }

            return entities;
        }

        private void MapToFields<T>(SqlDataReader reader, object o, IEnumerable<PropertyInfo> props)
        {
            foreach (PropertyInfo prop in props)
            {
                if (!_valueTypeChecker.IsValueType(prop.PropertyType))
                {
                    object entity = Activator.CreateInstance(prop.PropertyType);
                    MapToFields<T>(reader, entity, entity.GetType().GetProperties());
                    prop.SetValue(o,entity);
                }
                else
                {
                    try
                    {
                        prop.SetValue(o, reader[prop.Name]);
                    }
                    catch (Exception)
                    {
                        string name = o.GetType().Name;
                        prop.SetValue(o, reader[name + prop.Name]);
                    }
                }
            }
        }
    }
}

