using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpProxyGenerator.Extensions
{
    internal static class TypeExtensions
    {
        public static string GetFullTypeName(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.FullName ?? type.Name;
        }

        public static MethodInfo[] GetInterfaceMethods(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var baseInterfaces = type.GetInterfaces();
            return type.GetMethods()
                .Concat(baseInterfaces.SelectMany(x => x.GetMethods()))
                .ToArray();
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
                var typeNameWithoutGenericPart = type.Name.Substring(0, type.Name.IndexOf("`", StringComparison.InvariantCulture));

                // Generate the name of the generic type.
                var genericArgs = type.GetGenericArguments();
                return SyntaxFactory.QualifiedName(SyntaxFactory.ParseName(type.Namespace),
                    SyntaxFactory.GenericName(SyntaxFactory.Identifier(typeNameWithoutGenericPart),
                        SyntaxFactory.TypeArgumentList(SyntaxFactory.SeparatedList(genericArgs.Select(AsTypeSyntax)))
                    )
                );
            }
            else
                return SyntaxFactory.ParseTypeName(name);
        }
    }
}
