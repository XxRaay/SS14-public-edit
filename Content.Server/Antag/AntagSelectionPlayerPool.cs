using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Antag;

public readonly record struct AntagSelectionPoolEntry(ICommonSession Session, int Weight = 100);

public sealed class AntagSelectionPlayerPool
{
    private readonly List<List<AntagSelectionPoolEntry>> _orderedPools;

    public AntagSelectionPlayerPool(List<List<ICommonSession>> orderedPools)
    {
        _orderedPools = orderedPools
            .Select(pool => pool.Select(session => new AntagSelectionPoolEntry(session)).ToList())
            .ToList();
    }

    public AntagSelectionPlayerPool(List<List<AntagSelectionPoolEntry>> orderedPools)
    {
        _orderedPools = orderedPools;
    }

    public bool TryPickAndTake(IRobustRandom random, [NotNullWhen(true)] out ICommonSession? session)
    {
        session = null;

        foreach (var pool in _orderedPools)
        {
            if (pool.Count == 0)
                continue;

            var totalWeight = pool.Sum(entry => Math.Max(entry.Weight, 1));
            var roll = random.Next(1, totalWeight + 1);
            var cumulative = 0;

            for (var i = 0; i < pool.Count; i++)
            {
                var entry = pool[i];
                cumulative += Math.Max(entry.Weight, 1);
                if (roll > cumulative)
                    continue;

                session = entry.Session;
                pool.RemoveAt(i);
                break;
            }

            break;
        }

        return session != null;
    }

    public int Count => _orderedPools.Sum(p => p.Count);
}
