using System;
using System.Collections.Generic;
using System.Linq;

namespace Scratch
{
    public static class Extensions
    {
        public static IEnumerable<(Entity Entity, (string type, string name)[] Dependencies)> WhereRecursivley(this IEnumerable<(Entity Entity, (string type, string name)[] Dependencies)> items, Func<(Entity Entity, (string type, string name)[] Dependencies), bool> predicate)
        {
            var hashSet = items.Where(predicate).Select(t => (t.Entity.Type, t.Entity.Name)).ToHashSet();
            if (!hashSet.Any())
                return Enumerable.Empty<(Entity Entity, (string type, string name)[] Dependencies)>();
            var changed = true;
            while (changed)
            {
                changed = false;
                foreach (var item in items)
                {
                    var e = (item.Entity.Type, item.Entity.Name);
                    if (hashSet.Contains(e))
                        continue;
                    if (item.Dependencies.Any(d => hashSet.Contains(d)))
                    {
                        hashSet.Add(e);
                        changed = true;
                    }
                }
            }
            return items.Where(item => hashSet.Contains((item.Entity.Type, item.Entity.Name)));
        }
    }
}
