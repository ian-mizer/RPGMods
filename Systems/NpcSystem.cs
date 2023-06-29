using VRising.Modding;
using UnityEngine;

public class UltraMerchant : ModNPC
{
    private Inventory merchantInventory; // Reference to the merchant's inventory

    public override void Initialize()
    {
        // Set the NPC's display name
        NPC.nameOverride = "Ultra Merchant";
        
        // Set the NPC's sprite (replace 'merchant_sprite' with the appropriate sprite name or path)
        NPC.sprite = Resources.Load<Sprite>("merchant_sprite");
        
        // Set the NPC's initial position
        NPC.position = new Vector2(100f, 200f); // Replace with the desired position coordinates

        // Initialize the merchant's inventory
        merchantInventory = new Inventory();

        // Add some items to the merchant's inventory (replace with your desired items)
        Item sword = new Item("Sword", 1);
        Item potion = new Item("Health Potion", 5);
        merchantInventory.AddItem(sword);
        merchantInventory.AddItem(potion);
    }

    public override void OnInteract()
    {
        // Display the merchant's inventory
        DisplayInventory();
    }

    private void DisplayInventory()
    {
        // Iterate through the items in the merchant's inventory and print their names
        foreach (Item item in merchantInventory.GetItems())
        {
            Debug.Log(item.Name);
        }
    }

    // Add any other necessary methods and functionality for the NPC
}
