﻿using System;
using System.Collections.Generic;
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

        public (SyntaxTree, List<Assembly>) Generate()
        {
            return GenerateControllers(_options.InterfacesToExpose);
        }

        private (SyntaxTree, List<Assembly>) GenerateControllers(IList<Type> types)
        {
            
            var namespaceName = _options.NamingConventionProvider.GetGeneratedTypesNamespace();

            var compilationUnit = CompilationUnit()
                .AddMembers(NamespaceDeclaration(ParseName(namespaceName))
                .AddMembers(types.Select(CreateControllerClass).ToArray()));
            
            return (compilationUnit.NormalizeWhitespace().SyntaxTree, types.Select(x => x.Assembly).Distinct().ToList());
        }

        private MemberDeclarationSyntax CreateControllerClass(Type targetInterface)
        {
            var name = _options.NamingConventionProvider.GetGeneratedControllerName(targetInterface);
            var methods = _options.ProxyContractProvider.GetMethodsToExpose(targetInterface).ToArray();

            return ClassDeclaration(Identifier(name))
                .AddModifiers(Token(SyntaxKind.PublicKeyword))
                .AddBaseListTypes(SimpleBaseType(_options.ProxyContractProvider.GetBaseControllerType(targetInterface).AsTypeSyntax()))
                .AddAttributeLists(AttributeList(SeparatedList<AttributeSyntax>()
                    .Add(GenerateAttribute<ApiControllerAttribute>())
                    .Add(GenerateRouteAttribute(name))))
                .AddMembers(CreateServiceField(targetInterface))
                .AddMembers(CreateControllerConstructor(targetInterface))
                .AddMembers(methods.Select(x => CreateApiEndpointMethod(x, targetInterface)).ToArray())
                .AddMembers(methods.Select(CreateParameterClassWrapper).Where(x => x != null).ToArray())
                ;
        }

        private MemberDeclarationSyntax CreateServiceField(Type targetInterface)
        {
            return FieldDeclaration(default,
                new SyntaxTokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)),
                VariableDeclaration(targetInterface.AsTypeSyntax(),
                    SeparatedList(new List<VariableDeclaratorSyntax>
                    {
                        VariableDeclarator(
                            ParseToken(_options.NamingConventionProvider.GetServiceFieldName(targetInterface)))
                    }))
            );
        }
        private MemberDeclarationSyntax CreateControllerConstructor(Type targetInterface)
        {
            var name = _options.NamingConventionProvider.GetGeneratedControllerName(targetInterface);

            var serviceParameterName = _options.NamingConventionProvider.GetServiceConstructorParameterName(targetInterface);

            return ConstructorDeclaration(default,
                new SyntaxTokenList(Token(SyntaxKind.PublicKeyword)),
                ParseToken(name),
                ParameterList(SeparatedList(new List<ParameterSyntax>()
                {
                    Parameter(default, default, targetInterface.AsTypeSyntax(), ParseToken(serviceParameterName), default)
                })),
                default, 
                Block(new List<StatementSyntax>()
                {
                    ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("this"),
                            IdentifierName(_options.NamingConventionProvider.GetServiceFieldName(targetInterface))
                        ).WithOperatorToken(Token(SyntaxKind.DotToken)),
                        IdentifierName(serviceParameterName)
                    ))
                })
            );
        }

        private MemberDeclarationSyntax CreateApiEndpointMethod(MethodInfo method, Type interfaceType)
        {
            const string resultVariableName = "result";
            string modelParameterName = _options.NamingConventionProvider.GetApiMethodModelParameterName(method);

            var attributeList = new SyntaxList<AttributeListSyntax>(AttributeList(SeparatedList<AttributeSyntax>()
                .Add(GenerateAttribute<HttpPostAttribute>())
                .Add(GenerateProducesResponseTypeAttribute(method.ReturnType))
                .Add(GenerateRouteAttribute(method.Name))
            ));

            var keywords = new SyntaxTokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AsyncKeyword));

            var returnType = typeof(Task<IActionResult>).AsTypeSyntax();

            var parameters = method.GetParameters();

            var bodyStatements = new List<StatementSyntax>();

            ExpressionSyntax serviceInvocation = AwaitExpression(InvocationExpression(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(_options.NamingConventionProvider.GetServiceFieldName(interfaceType)),
                    IdentifierName(method.Name)
                ).WithOperatorToken(Token(SyntaxKind.DotToken)),
                ArgumentList(SeparatedList(parameters.Select(x => Argument(MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName(modelParameterName),
                    IdentifierName(x.Name)
                ).WithOperatorToken(Token(SyntaxKind.DotToken))))))
            ));

            if (method.ReturnType.IsGenericType)
            {
                bodyStatements.Add(LocalDeclarationStatement(
                    VariableDeclaration(method.ReturnType.GetGenericArguments().Single().AsTypeSyntax())
                        .WithVariables(
                            SingletonSeparatedList(
                                VariableDeclarator(
                                        Identifier(resultVariableName))
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
                ParameterList(SeparatedList(new List<ParameterSyntax>()
                {
                    Parameter(default, default, ParseTypeName(_options.NamingConventionProvider.GetParameterModelTypeName(method)), ParseToken(modelParameterName), default)
                })),
                new SyntaxList<TypeParameterConstraintClauseSyntax>(),
                Block(bodyStatements.Concat(new List<StatementSyntax>()
                {
                    ReturnStatement(InvocationExpression(MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            IdentifierName("this"),
                            IdentifierName(nameof(ControllerBase.Ok))
                        ).WithOperatorToken(Token(SyntaxKind.DotToken)),
                        ArgumentList(method.ReturnType.IsGenericType ? SeparatedList(new List<ArgumentSyntax>()
                        {
                            Argument(IdentifierName(resultVariableName))
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

            return ClassDeclaration(Identifier(_options.NamingConventionProvider.GetParameterModelTypeName(method)))
                    .AddModifiers(Token(SyntaxKind.PublicKeyword))
                    .AddMembers(methodParameters.Select(CreatePropertyByParameter).ToArray())
                ;
        }

        private MemberDeclarationSyntax CreatePropertyByParameter(ParameterInfo parameter)
        {
            return PropertyDeclaration(new SyntaxList<AttributeListSyntax>(),
                SyntaxTokenList.Create(Token(SyntaxKind.PublicKeyword)),
                parameter.ParameterType.AsTypeSyntax(),
                explicitInterfaceSpecifier: default, 
                Identifier(parameter.Name), 
                AccessorList(new SyntaxList<AccessorDeclarationSyntax>(new[] {
                    AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken)),
                    AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(Token(SyntaxKind.SemicolonToken))
                })));
        }

        private AttributeSyntax GenerateProducesResponseTypeAttribute(Type returnType)
        {
            var parameters = new List<ExpressionSyntax>();

            //Type<TResult>
            if (returnType.IsGenericType)
            {
                var genericTypeArgument = returnType.GetGenericArguments().Single();
                parameters.Add(TypeOfExpression(genericTypeArgument.AsTypeSyntax()));
            }

            parameters.Add(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal((int)HttpStatusCode.OK)));

            return GenerateAttribute<ProducesResponseTypeAttribute>(parameters.ToArray());
        }

        private AttributeSyntax GenerateRouteAttribute(string routeValue)
        {
            return GenerateAttribute<RouteAttribute>(LiteralExpression(SyntaxKind.StringLiteralExpression, Literal(routeValue)));
        }

        private AttributeSyntax GenerateAttribute<TAttribute>(params ExpressionSyntax[] parameters)
        {
            var attributeName = ParseName(typeof(TAttribute).GetFullTypeName());

            var attributeParametersList = SeparatedList<AttributeArgumentSyntax>();

            foreach (var expressionSyntax in parameters)
            {
                attributeParametersList = attributeParametersList.Add(AttributeArgument(expressionSyntax));
            }

            return Attribute(attributeName, AttributeArgumentList(attributeParametersList));
        }

    }
}
