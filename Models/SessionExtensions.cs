﻿
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text.Json;
namespace Fiesta_Flavors.Models
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            if (string.IsNullOrEmpty(value))
            {
                return default(T);
            }
            else
            {
                return JsonSerializer.Deserialize<T>(value);
            }

        }
    }
}
