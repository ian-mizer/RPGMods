using System.Collections.Generic;
using System.Linq;
using VampireCommandFramework;

public class NPC
{
    public string Name { get; set; }
    public List<string> Inventory { get; set; }

    public NPC(string name)
    {
        Name = name;
        Inventory = new List<string>();
    }

    public static NPC SpawnMerchant(string name, List<string> inventory)
    {
        var merchant = new NPC(name);
        merchant.Inventory.AddRange(inventory);
        return merchant;
    }
}

public class SpawnMerchantCommand : Command
{
    public SpawnMerchantCommand() : base("spawn_merchant", "/spawn_merchant [inventory]", "Spawns a NPC merchant with specific inventory.")
    {
    }

    public override void Execute(CommandArgs args)
    {
        var inventory = args.Parameters.Select(p => p.Raw).ToList();
        var npc = NPC.SpawnMerchant("Merchant", inventory);

        // Assume you have some way of getting the player's location
        var playerLocation = args.Player.GetLocation();

        // Assume you have some way of spawning the NPC at a specific location
        args.Game.SpawnNPC(npc, playerLocation);
    }
}
