using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ReflectionalMapper
{
    public class SqlBuilder
    {
        private readonly ValueTypeChecker _valueTypeChecker = new ValueTypeChecker();

        public string ColumnName { get; private set; }

        public string BuildFromExpression<T>(Expression<Func<T, object>> field,object val)
        {
            MemberExpression expr = GetMemberInfo(field);
            ColumnName = expr.Member.Name;
            return "SELECT * FROM " + typeof(T).Name + " WHERE " + ColumnName + " = @" + ColumnName;
        }

        private static MemberExpression GetMemberInfo(Expression method)
        {
            LambdaExpression lambda = method as LambdaExpression;
            if (lambda == null) throw new ArgumentNullException("method");

            MemberExpression memberExpr = null;

            if (lambda.Body.NodeType == ExpressionType.Convert)
            {
                memberExpr = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
            }
            else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpr = lambda.Body as MemberExpression;
            }

            if (memberExpr == null) throw new ArgumentException("method");

            return memberExpr;
        }

        public string BuildInsertStatement<T>(T entity, Expression<Func<T, object>> autoIncrementIdToExclude)
        {
            string sql = "INSERT INTO " + typeof (T).Name + "(";
            foreach (PropertyInfo prop in typeof (T).GetProperties())
            {
                if (IsFieldToExclude(autoIncrementIdToExclude, prop))
                    continue;

                sql += prop.Name + ",";
            }
            sql = StripCommaAndAddBracket(sql) + " VALUES(";

            foreach (PropertyInfo prop in typeof (T).GetProperties())
            {
                if (IsFieldToExclude(autoIncrementIdToExclude, prop))
                    continue;

                sql += ApplyQuotes(prop) + prop.GetValue(entity) + ApplyQuotes(prop) + ",";
            }
            sql = StripCommaAndAddBracket(sql);
            return sql;
        }

        private bool IsFieldToExclude<T>(Expression<Func<T, object>> autoIncrementIdToExclude, PropertyInfo prop)
        {
            return (autoIncrementIdToExclude != null && GetMemberInfo(autoIncrementIdToExclude).Member.Name == prop.Name) || !_valueTypeChecker.IsValueType(prop.PropertyType);
        }

        private static string StripCommaAndAddBracket(string sql)
        {
            return sql.Trim(',') + ")";
        }

        private string ApplyQuotes(PropertyInfo prop)
        {
            if (prop.PropertyType == typeof (DateTime) || prop.PropertyType == typeof (string) || prop.PropertyType == typeof(Guid))
                return "'";
            return string.Empty;
        }

        public string BuildUpdateStatement<T>(T entity,params Expression<Func<T, object>>[] ids)
        {
            Type entityType = typeof (T);
            
            if(ids.Length == entityType.GetProperties().Length)
                throw new InvalidOperationException("Cannot update an entity whose fields only contain ids");

            if(ids == null || ids.Length == 0)
                throw new InvalidOperationException("Must specify identifier");

            string sql = "UPDATE " + entityType.Name + " SET ";
            foreach (var prop in entityType.GetProperties())
            {
                if(PropIsId<T>(prop, ids)||!_valueTypeChecker.IsValueType(prop.PropertyType))
                    continue;
                sql += prop.Name + "=" + prop.GetValue(entity) + ",";
            }
            sql = sql.Trim(',');
            sql += " WHERE ";
            return BuildWhereClauseConditions(entity, ids, sql, entityType);
        }

        private bool PropIsId<T>(PropertyInfo prop, IEnumerable<Expression<Func<T, object>>> ids)
        {
            return ids.Any(expression => GetMemberInfo(expression).Member.Name == prop.Name);
        }

        public string BuildDeleteStatement<T>(T entity, params Expression<Func<T, object>>[] ids)
        {
            Type entityType = typeof(T);

            if (ids == null || ids.Length == 0)
                throw new InvalidOperationException("Must specify identifier");

            string sql = "DELETE FROM " + entityType.Name + " WHERE ";

            return BuildWhereClauseConditions(entity, ids, sql, entityType); 
        }

        private string BuildWhereClauseConditions<T>(T entity, Expression<Func<T, object>>[] ids, string sql, Type entityType)
        {
            foreach (Expression<Func<T, object>> id in ids)
            {
                string name = GetMemberInfo(id).Member.Name;
                PropertyInfo propertyInfo = entityType.GetProperty(name);
                sql += name + "=" + ApplyQuotes(propertyInfo) + propertyInfo.GetValue(entity) + ApplyQuotes(propertyInfo) + " AND ";
            }
            sql = sql.TrimEnd(' ', 'A', 'N', 'D');
            return sql;
        }
    }
}

