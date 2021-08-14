using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using HttpProxyGenerator.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace HttpProxyGenerator
{
    public class ControllerGenerator
    {
        private readonly ControllerGeneratorOptions _options;

        public ControllerGenerator(ControllerGeneratorOptions options)
        {
            _options = options;
        }

        public (string, List<Assembly>) Generate()
        {
            return GenerateControllers(_options.InterfacesToExpose);
        }

        private (string, List<Assembly>) GenerateControllers(IList<Type> types)
        {
            var namespaceParts = new List<string>()
            {
                "Test",
                "Controllers",
                "CodeGen"
            }.Where(x => !string.IsNullOrEmpty(x));
            var nsName = string.Join(".", namespaceParts);

            var ns = NamespaceDeclaration(ParseName(nsName))
                .AddMembers(types.Select(x => CreateClass($"{x.Name}Controller", x.GetInterfaceMethods(), x)).ToArray());
            
            var strWriter = new StringWriter();

            ns.NormalizeWhitespace().WriteTo(strWriter);
            return (strWriter.ToString(), types.Select(x => x.Assembly).Distinct().ToList());
        }

        private MemberDeclarationSyntax CreateClass(string name, MethodInfo[] methods, Type targetInterface)
        {
            return ClassDeclaration(Identifier(name))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(SimpleBaseType(typeof(ControllerBase).AsTypeSyntax()))
                .AddAttributeLists(AttributeList(SeparatedList<AttributeSyntax>().Add(
                    Attribute(ParseName(typeof(ApiControllerAttribute).FullName)))
                    .Add(GenerateRouteAttribute(name))))
                .AddMembers(CreateServiceField(targetInterface))
                .AddMembers(CreateControllerConstructor(name, targetInterface))
                .AddMembers(methods.Select(CreateMethod).ToArray())
                .AddMembers(methods.Select(CreateParameterClassWrapper).Where(x => x != null).ToArray())
                ;
        }

        private MemberDeclarationSyntax CreateServiceField(Type targetInterface)
        {
            return FieldDeclaration(default,
                new SyntaxTokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)),
                VariableDeclaration(targetInterface.AsTypeSyntax(),
                    SeparatedList(
                        new List<VariableDeclaratorSyntax>()
                        {
                            VariableDeclarator(ParseToken("_service"))
                        }))
            );
        }
        private MemberDeclarationSyntax CreateControllerConstructor(string name, Type targetInterface)
        {
            return ConstructorDeclaration(default,
                new SyntaxTokenList(Token(SyntaxKind.PublicKeyword)),
                ParseToken(name),
                ParameterList(SeparatedList<ParameterSyntax>(new List<ParameterSyntax>()
                {
                    Parameter(default, default, targetInterface.AsTypeSyntax(), ParseToken("service"), default)
                })),
                default,
                Block(new List<StatementSyntax>()
                {
                    ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("this"),
                            IdentifierName("_service")
                        ).WithOperatorToken(Token(SyntaxKind.DotToken)),
                        IdentifierName("service")
                        ))
                })
            );
        }

        private MemberDeclarationSyntax CreateMethod(MethodInfo method)
        {
            var attributeList = new SyntaxList<AttributeListSyntax>(AttributeList(SeparatedList<AttributeSyntax>()
                .Add(Attribute(ParseName(typeof(HttpPostAttribute).FullName)))
                .Add(GenerateProducesResponseTypeAttribute(method.Name, method.ReturnType))
                .Add(GenerateRouteAttribute(method.Name))
            ));

            var keywords = new SyntaxTokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword));

            var returnType = typeof(Task<IActionResult>).AsTypeSyntax();

            var parameters = method.GetParameters();

            var bodyStatements = new List<StatementSyntax>();

            ExpressionSyntax serviceInvocation = AwaitExpression(InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("_service"),
                    IdentifierName(method.Name)
                ).WithOperatorToken(Token(SyntaxKind.DotToken)),
                ArgumentList(SeparatedList<ArgumentSyntax>(parameters.Select(x => Argument(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("model"),
                    IdentifierName(x.Name)
                ).WithOperatorToken(Token(SyntaxKind.DotToken))))))
            ));

            if (method.ReturnType.IsGenericType)
            {
                bodyStatements.Add(LocalDeclarationStatement(
                    VariableDeclaration(method.ReturnType.GetGenericArguments().Single().AsTypeSyntax())
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                        Identifier("result"))
                                    .WithInitializer(
                                        EqualsValueClause(
                                            serviceInvocation
                                        ))))));
            }
            else
            {
                bodyStatements.Add(ExpressionStatement(
                    serviceInvocation
                    ));
            }


            return MethodDeclaration(
                attributeList,
                keywords,
                returnType,
                default,
                ParseToken(method.Name),
                default,
                ParameterList(SeparatedList<ParameterSyntax>(new List<ParameterSyntax>()
                {
                    Parameter(default, default, ParseTypeName($"{method.Name}ParameterModel"), ParseToken("model"), default)
                })),
                new SyntaxList<TypeParameterConstraintClauseSyntax>(),
                Block(bodyStatements.Concat(new List<StatementSyntax>()
                {
                    ReturnStatement(InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("this"),
                            IdentifierName(nameof(ControllerBase.Ok))
                        ).WithOperatorToken(Token(SyntaxKind.DotToken)),
                        ArgumentList(method.ReturnType.IsGenericType ? SeparatedList<ArgumentSyntax>(new List<ArgumentSyntax>()
                        {
                            Argument(IdentifierName("result"))
                        }) : default)
                    ))
                })),
                null);
        }

        private MemberDeclarationSyntax CreateParameterClassWrapper(MethodInfo method)
        {
            var methodParameters = method.GetParameters();
            if (methodParameters.Length == 0)
            {
                return null;
            }

            return ClassDeclaration(Identifier($"{method.Name}ParameterModel"))
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddMembers(methodParameters.Select(CreatePropertyByParameter).ToArray())
                ;
        }

        private MemberDeclarationSyntax CreatePropertyByParameter(ParameterInfo parameter)
        {
            return PropertyDeclaration(new SyntaxList<AttributeListSyntax>(),
                SyntaxTokenList.Create(Token(SyntaxKind.PublicKeyword)),
                parameter.ParameterType.AsTypeSyntax(),
                explicitInterfaceSpecifier: null, 
                Identifier(parameter.Name), 
                AccessorList(new SyntaxList<AccessorDeclarationSyntax>(new[] {
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                })));
        }

        private AttributeSyntax GenerateProducesResponseTypeAttribute(string methodName, Type returnType)
        {
            var attributeName = ParseName(typeof(ProducesResponseTypeAttribute).FullName);

            if (!typeof(Task).IsAssignableFrom(returnType))
            {
                throw new ArgumentException($"Return type of the {methodName} method should be {nameof(Task)} or {nameof(Task)}<TType>");
            }


            var attributeParametersList = SeparatedList<AttributeArgumentSyntax>();

            if (returnType.IsGenericType)
            {
                var genericTypeArgument = returnType.GetGenericArguments().Single();

                attributeParametersList = attributeParametersList.Add(AttributeArgument(TypeOfExpression(genericTypeArgument.AsTypeSyntax())));
            }

            attributeParametersList = attributeParametersList.Add(AttributeArgument(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)HttpStatusCode.OK))));


            return Attribute(attributeName, AttributeArgumentList(attributeParametersList));
        }

        private AttributeSyntax GenerateRouteAttribute(string routeValue)
        {
            var attributeName = ParseName(typeof(RouteAttribute).FullName);
            
            var attributeParametersList = SeparatedList<AttributeArgumentSyntax>();
            

            attributeParametersList = attributeParametersList.Add(AttributeArgument(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(routeValue))));


            return Attribute(attributeName, AttributeArgumentList(attributeParametersList));
        }
    }
}
