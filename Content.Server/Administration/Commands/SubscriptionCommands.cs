using System.Linq;
using Content.Server.Imperial.Subscriptions;
using Content.Shared.Administration;
using Robust.Server.Player;
using Robust.Shared.Console;

namespace Content.Server.Administration.Commands;

[AdminCommand(AdminFlags.Admin)]
public sealed class SubscriptionSetCommand : LocalizedCommands
{
    [Dependency] private readonly SubscriptionManager _subscriptions = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;
    [Dependency] private readonly IPlayerManager _players = default!;

    public override string Command => "subscriptionset";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 2)
        {
            shell.WriteLine($"Usage: {Command} <playerName> <none|1|2|3>");
            return;
        }

        var playerName = args[0].Trim();
        var player = await _playerLocator.LookupIdByNameAsync(playerName);
        if (player == null)
        {
            shell.WriteError($"Player '{playerName}' not found.");
            return;
        }

        if (!TryParseTier(args[1], out var tier))
        {
            shell.WriteError("Invalid tier. Use: none, 1, 2, or 3.");
            return;
        }

        await _subscriptions.SetTier(player.UserId, tier);
        shell.WriteLine($"Set subscription for {player.Username} to {tier.DisplayName()}.");
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
            return CompletionResult.FromHintOptions(_players.Sessions.Select(x => x.Name), "Player name");

        if (args.Length == 2)
            return CompletionResult.FromHintOptions(new[] { "none", "1", "2", "3" }, "Subscription tier");

        return CompletionResult.Empty;
    }

    private static bool TryParseTier(string value, out SubscriptionTier tier)
    {
        tier = value.Trim().ToLowerInvariant() switch
        {
            "none" => SubscriptionTier.None,
            "0" => SubscriptionTier.None,
            "1" => SubscriptionTier.Explorer,
            "2" => SubscriptionTier.Vanguard,
            "3" => SubscriptionTier.Sovereign,
            _ => (SubscriptionTier)(-1)
        };

        return tier != (SubscriptionTier)(-1);
    }
}

[AdminCommand(AdminFlags.Admin)]
public sealed class SubscriptionGetCommand : LocalizedCommands
{
    [Dependency] private readonly SubscriptionManager _subscriptions = default!;
    [Dependency] private readonly IPlayerLocator _playerLocator = default!;
    [Dependency] private readonly IPlayerManager _players = default!;

    public override string Command => "subscriptionget";

    public override async void Execute(IConsoleShell shell, string argStr, string[] args)
    {
        if (args.Length != 1)
        {
            shell.WriteLine($"Usage: {Command} <playerName>");
            return;
        }

        var playerName = args[0].Trim();
        var player = await _playerLocator.LookupIdByNameAsync(playerName);
        if (player == null)
        {
            shell.WriteError($"Player '{playerName}' not found.");
            return;
        }

        var tier = _subscriptions.GetTier(player.UserId);
        shell.WriteLine($"{player.Username}: {tier.DisplayName()}");
    }

    public override CompletionResult GetCompletion(IConsoleShell shell, string[] args)
    {
        if (args.Length == 1)
            return CompletionResult.FromHintOptions(_players.Sessions.Select(x => x.Name), "Player name");

        return CompletionResult.Empty;
    }
}
