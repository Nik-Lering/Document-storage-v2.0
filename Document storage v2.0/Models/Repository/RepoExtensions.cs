using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Document_storage_v2.Models
{
    public static class RepoExtensions
    {
        public static bool IsDefault<T>(this T obj)
            => EqualityComparer<T>.Default.Equals(obj, default);

        public static void ApplyTo<T>(this T entity, Action<T> repoAction, Action<T> action = default)
        {
            if (!entity.IsDefault())
            {
                action?.Invoke(entity);
                repoAction?.Invoke(entity);
            }
        }
    }
}