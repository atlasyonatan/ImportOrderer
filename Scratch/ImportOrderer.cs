using System;
using System.Collections.Generic;
using System.Linq;

namespace Scratch
{
    //Assumptions:
    // - E1 don't depend on anything
    // - E3 don't depend on an entity that depends on E3 (recurisivley)
    // - No circular dependencies

    //Solutions:
    //All imports:
    //A) make all E1 (guarenteed)
    //B) make all E2,E4 who don't depend on an E3
    //C) make all E3 (guarenteed)
    //D) make all the E2, E4

    //Import step B, D (difference is the E2 and E4 which will have names in the dictionary as keys):
    //dictionary <name,hashset<name>> (hashsets contain names of other E2 and E4 which the name(key) depends on)
    public class ImportOrderer
    {
        private readonly HashSet<(string type, string name)> resolved = new();
        private readonly Dictionary<Entity, (string type, string name)[]> _entities;

        public ImportOrderer(Dictionary<Entity, (string type, string name)[]> entities) => 
            _entities = entities;


        public IEnumerable<Entity> GetImportOrder() => StepA().Concat(StepB()).Concat(StepC()).Concat(StepD());

        private IEnumerable<Entity> StepA()
        {
            foreach (var E1 in _entities.Keys.Where(e => e.Type == "E1"))
            {
                resolved.Add((E1.Type, E1.Name));
                yield return E1;
            }
        }

        private IEnumerable<Entity> StepB()
        {
            var E2AndE4s = _entities.Keys.Where(e => e.Type == "E2" || e.Type == "E4")
                .Where(e => !resolved.Contains((e.Type, e.Name)))
                .ToArray();

            var dependsOnE3Recursivley = E2AndE4s
                .Select(e => (Entity: e, Dependencies: _entities[e]))
                .WhereRecursivley(tuple => tuple.Dependencies.Any(d => d.type == "E3"))
                .Select(tuple => tuple.Entity)
                .ToHashSet();

            var notDependsOnE3Recursivley = E2AndE4s
                .Where(e => !dependsOnE3Recursivley.Contains(e))
                .ToHashSet();
            var unresolvedDependencies = notDependsOnE3Recursivley.ToDictionary(
                e => e,
                e => _entities[e].Where(tuple => tuple.type == "E2" || tuple.type == "E4")
                    .Where(tuple => !resolved.Contains(tuple))
                    .ToHashSet());

            var changed = true;
            while (changed)
            {
                changed = false;
                foreach (var entity in unresolvedDependencies.Keys)
                {
                    if (unresolvedDependencies[entity].Count == 0)
                    {
                        unresolvedDependencies.Remove(entity);
                        resolved.Add((entity.Type,entity.Name));
                        yield return entity;
                        foreach (var e in unresolvedDependencies.Keys)
                            unresolvedDependencies[e].Remove((entity.Type, entity.Name));
                        changed = true;
                    }
                }
            }
            if (unresolvedDependencies.Any())
                throw new ArgumentException("unresolved");
        }

        private IEnumerable<Entity> StepC()
        {
            foreach (var E3 in _entities.Keys.Where(e => e.Type == "E3"))
            {
                resolved.Add((E3.Type, E3.Name));
                yield return E3;
            }
        }

        private IEnumerable<Entity> StepD()
        {
            var unresolvedE2AndE4s = _entities.Keys.Where(e => e.Type == "E2" || e.Type == "E4")
                .Where(e => !resolved.Contains((e.Type, e.Name)))
                .ToArray();

            var dependsOnE3Recursivley = unresolvedE2AndE4s
                .Select(e => (Entity: e, Dependencies: _entities[e]))
                .WhereRecursivley(tuple => tuple.Dependencies.Any(d => d.type == "E3"))
                .Select(tuple => tuple.Entity)
                .ToHashSet();

            var unresolvedDependencies = dependsOnE3Recursivley
                .ToDictionary(
                e => e,
                e => _entities[e].Where(tuple => tuple.type == "E2" || tuple.type == "E4")
                    .Where(tuple => !resolved.Contains(tuple))
                    .ToHashSet());

            var changed = true;
            while (changed)
            {
                changed = false;
                foreach (var entity in unresolvedDependencies.Keys)
                {
                    if (unresolvedDependencies[entity].Count == 0)
                    {
                        unresolvedDependencies.Remove(entity);
                        resolved.Add((entity.Type, entity.Name));
                        yield return entity;
                        foreach (var e in unresolvedDependencies.Keys)
                            unresolvedDependencies[e].Remove((entity.Type, entity.Name));
                        changed = true;
                    }
                }
            }
            if (unresolvedDependencies.Any())
                throw new ArgumentException("unresolved");
        }
    }
}
