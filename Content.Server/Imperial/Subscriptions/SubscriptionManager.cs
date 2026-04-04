using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Shared.Roles;
using Robust.Server.Player;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.Imperial.Subscriptions;

public sealed class SubscriptionManager : IPostInjectInit
{
    private const string SubscriptionPrefix = "subscription:";

    private static readonly Dictionary<SubscriptionTier, string> DbKeys = new()
    {
        [SubscriptionTier.Explorer] = SubscriptionPrefix + "explorer",
        [SubscriptionTier.Vanguard] = SubscriptionPrefix + "vanguard",
        [SubscriptionTier.Sovereign] = SubscriptionPrefix + "sovereign",
    };

    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly UserDbDataManager _userDb = default!;

    private readonly Dictionary<NetUserId, SubscriptionTier> _subscriptions = new();

    public SubscriptionTier GetTier(NetUserId userId)
    {
        return _subscriptions.GetValueOrDefault(userId, SubscriptionTier.None);
    }

    public int GetAntagSelectionWeight(NetUserId userId)
    {
        return GetTier(userId).AntagWeight();
    }

    public bool TryGetTier(NetUserId userId, [NotNullWhen(true)] out SubscriptionTier? tier)
    {
        if (_subscriptions.TryGetValue(userId, out var value) && value != SubscriptionTier.None)
        {
            tier = value;
            return true;
        }

        tier = null;
        return false;
    }

    public async Task SetTier(NetUserId userId, SubscriptionTier tier)
    {
        var allEntries = await _db.GetJobWhitelists(userId.UserId);
        foreach (var entry in allEntries)
        {
            if (!entry.StartsWith(SubscriptionPrefix, StringComparison.Ordinal))
                continue;

            await _db.RemoveJobWhitelist(userId.UserId, new ProtoId<JobPrototype>(entry));
        }

        if (tier != SubscriptionTier.None)
        {
            await _db.AddJobWhitelist(userId.UserId, new ProtoId<JobPrototype>(DbKeys[tier]));
            _subscriptions[userId] = tier;
        }
        else
        {
            _subscriptions.Remove(userId);
        }

        if (_player.TryGetSessionById(userId, out var session))
        {
            if (tier == SubscriptionTier.None)
                _subscriptions.Remove(session.UserId);
            else
                _subscriptions[session.UserId] = tier;
        }
    }

    private async Task LoadData(ICommonSession session, CancellationToken cancel)
    {
        var allEntries = await _db.GetJobWhitelists(session.UserId, cancel);
        cancel.ThrowIfCancellationRequested();

        var tier = SubscriptionTier.None;
        foreach (var entry in allEntries)
        {
            var parsed = ParseKey(entry);
            if (parsed > tier)
                tier = parsed;
        }

        if (tier == SubscriptionTier.None)
            _subscriptions.Remove(session.UserId);
        else
            _subscriptions[session.UserId] = tier;
    }

    private void ClientDisconnected(ICommonSession session)
    {
        _subscriptions.Remove(session.UserId);
    }

    private static SubscriptionTier ParseKey(string key)
    {
        foreach (var (tier, value) in DbKeys)
        {
            if (string.Equals(key, value, StringComparison.Ordinal))
                return tier;
        }

        return SubscriptionTier.None;
    }

    void IPostInjectInit.PostInject()
    {
        _userDb.AddOnLoadPlayer(LoadData);
        _userDb.AddOnPlayerDisconnect(ClientDisconnected);
    }
}
