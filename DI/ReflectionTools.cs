using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Libs.GameFramework.DI
{
    public static class ReflectionTools
    {
        public static IEnumerable<FieldInfo> GetFieldsWithAttributes(Type instanceType, Type attributeType)
        {
           return instanceType.GetFields(
                   BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
               .Where(prop => Attribute.IsDefined(prop, attributeType));

        }
        public static IEnumerable<PropertyInfo> GetPropsWithAttributes(Type instanceType, Type attributeType)
        {
           return instanceType.GetProperties(
                   BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
               .Where(prop => Attribute.IsDefined(prop, attributeType));

        }
    }
}