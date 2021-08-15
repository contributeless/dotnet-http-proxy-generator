﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HttpProxyGenerator.Extensions;

namespace HttpProxyGenerator.Abstractions
{
    public class DefaultProxyNamingConventionProvider: IProxyNamingConventionProvider
    {
        public string GetControllerNamespace(Type interfaceType)
        {
            return "Test.Controllers.CodeGen";
        }

        public string GetGeneratedControllerName(Type interfaceType)
        {
            if (interfaceType is null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            return $"{interfaceType.Name}Controller";
        }

        public string GetServiceFieldName(Type interfaceType)
        {
            return "_service";
        }

        public string GetServiceConstructorParameterName(Type interfaceType)
        {
            return "service";
        }

        public string GetApiMethodModelParameterName(MethodInfo method)
        {
            return "model";
        }

        public string GetParameterModelTypeName(MethodInfo method)
        {
            return $"{method.Name}ParameterModel";
        }

        public string GetControllerRoute(Type targetInterface)
        {
            return targetInterface.Name.ToKebabCase();
        }

        public string GetEndpointRoute(MethodInfo method)
        {
            return method.Name.ToKebabCase();
        }
    }
}