using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpProxyGenerator.Extensions
{
    internal static class TypeExtensions
    {
        public static MethodInfo[] GetInterfaceMethods(this Type type)
        {
            return type.GetMethods();
        }

        public static TypeSyntax AsTypeSyntax(this Type type)
        {
            if (type.FullName is null)
            {
                throw new ArgumentException($"Type {type.Name} has no {type.FullName} value");
            }

            string name = type.FullName.Replace('+', '.');

            if (type.IsGenericType)
            {
                // Get the C# representation of the generic type minus its type arguments.
                name = name.Substring(0, name.IndexOf("`", StringComparison.InvariantCulture));

                // Generate the name of the generic type.
                var genericArgs = type.GetGenericArguments();
                return SyntaxFactory.GenericName(SyntaxFactory.Identifier(name),
                    SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(genericArgs.Select(AsTypeSyntax)))
                );
            }
            else
                return SyntaxFactory.ParseTypeName(name);
        }
    }
}
