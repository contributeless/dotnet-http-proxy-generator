using System;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpProxyGenerator.Consumer.Extensions
{
    internal static class TypeExtensions
    {
        public static TypeSyntax AsTypeSyntax(this Type type)
        {
            const char backTick = '`';
            if (type.FullName is null)
            {
                throw new ArgumentException($"Type {type.Name} has no {type.FullName} value");
            }

            string name = type.FullName.Replace('+', '.');

            if (type.IsGenericType)
            {
                // Get the C# representation of the generic type minus its type arguments.
                var typeNameWithoutGenericPart = type.Name.Substring(0, type.Name.IndexOf(backTick, StringComparison.InvariantCulture));

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
