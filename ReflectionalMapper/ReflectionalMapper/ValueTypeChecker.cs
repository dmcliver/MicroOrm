using System;

namespace ReflectionalMapper
{
    public class ValueTypeChecker
    {
        public bool IsValueType(Type fieldType)
        {
            string fieldTypeName = fieldType.Name.ToLower();
            return (
                       fieldTypeName.Contains("int") ||
                       fieldTypeName.Contains("string") ||
                       fieldTypeName.Contains("double") ||
                       fieldTypeName.Contains("float") ||
                       fieldTypeName.Contains("char") ||
                       fieldTypeName.Contains("byte") ||
                       fieldTypeName.Contains("long")
                   );
        }
    }
}