using System;

namespace HttpProxyGenerator.Common.Extensions
{
    internal static class TypeExtensions
    {
        public static string GetUrlFriendlyName(this Type type)
        {
            const char backTick = '`';
            string friendlyName = type.Name;
            if (type.IsGenericType)
            {
                int backtick = friendlyName.IndexOf(backTick);
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
