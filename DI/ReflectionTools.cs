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
           return instanceType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
               .Where(prop => Attribute.IsDefined(prop, attributeType));

        }
    }
}