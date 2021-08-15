using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HttpProxyGenerator.Consumer.Extensions
{
    internal static class TypeExtensions
    {
        public static MethodInfo[] GetInterfaceMethods(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var baseInterfaces = type.GetInterfaces()
                
                //exclude system interfaces like IDisposable
                .Where(x => !(x.FullName?.StartsWith("System") ?? false));

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

        public static string GetUrlFriendlyName(this Type type)
        {
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int backtick = friendlyName.IndexOf('`');
                if (backtick > 0)
                {
                    friendlyName = friendlyName.Remove(backtick);
                }
                foreach (var typeParam in type.GetGenericArguments())
                {
                    friendlyName += typeParam.GetUrlFriendlyName();
                }
            }

            if (type.IsArray)
            {
                return type.GetElementType().GetUrlFriendlyName() + "Array";
            }

            return friendlyName;
        }
    }
}
