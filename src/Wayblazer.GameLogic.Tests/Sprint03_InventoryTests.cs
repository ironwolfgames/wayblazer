using Wayblazer.GameLogic.Models;
using Wayblazer.GameLogic.Systems;

namespace Wayblazer.GameLogic.Tests;

/// <summary>
/// Inventory Manager — deterministic acceptance tests.
/// These tests define "done" for the inventory system.
/// </summary>
public class Sprint03_InventoryTests
{
    [Fact]
    public void AddItem_IncreasesCount()
    {
        var inventory = new InventorySystem();

        inventory.AddItem("Base Ore", 3);

        Assert.Equal(3, inventory.GetItemCount("Base Ore"));
    }

    [Fact]
    public void AddItem_MultipleAdds_Accumulate()
    {
        var inventory = new InventorySystem();

        inventory.AddItem("Soft Wood", 2);
        inventory.AddItem("Soft Wood", 3);

        Assert.Equal(5, inventory.GetItemCount("Soft Wood"));
    }

    [Fact]
    public void AddItem_DifferentResources_TrackedSeparately()
    {
        var inventory = new InventorySystem();

        inventory.AddItem("Base Ore", 5);
        inventory.AddItem("Catalyst Ore", 2);
        inventory.AddItem("Soft Wood", 10);

        Assert.Equal(5, inventory.GetItemCount("Base Ore"));
        Assert.Equal(2, inventory.GetItemCount("Catalyst Ore"));
        Assert.Equal(10, inventory.GetItemCount("Soft Wood"));
    }

    [Fact]
    public void TryRemoveItem_Success_DecreasesCount()
    {
        var inventory = new InventorySystem();
        inventory.AddItem("Base Ore", 5);

        bool removed = inventory.TryRemoveItem("Base Ore", 3);

        Assert.True(removed);
        Assert.Equal(2, inventory.GetItemCount("Base Ore"));
    }

    [Fact]
    public void TryRemoveItem_InsufficientQuantity_ReturnsFalse()
    {
        var inventory = new InventorySystem();
        inventory.AddItem("Base Ore", 2);

        bool removed = inventory.TryRemoveItem("Base Ore", 5);

        Assert.False(removed);
        Assert.Equal(2, inventory.GetItemCount("Base Ore")); // Unchanged
    }

    [Fact]
    public void TryRemoveItem_ExactQuantity_RemovesEntry()
    {
        var inventory = new InventorySystem();
        inventory.AddItem("Catalyst Ore", 3);

        inventory.TryRemoveItem("Catalyst Ore", 3);

        Assert.Equal(0, inventory.GetItemCount("Catalyst Ore"));
        Assert.False(inventory.HasItem("Catalyst Ore"));
    }

    [Fact]
    public void TryRemoveItem_NonExistentResource_ReturnsFalse()
    {
        var inventory = new InventorySystem();

        bool removed = inventory.TryRemoveItem("Unobtainium");

        Assert.False(removed);
    }

    [Fact]
    public void HasItem_ReturnsTrue_WhenSufficientQuantity()
    {
        var inventory = new InventorySystem();
        inventory.AddItem("Soft Wood", 10);

        Assert.True(inventory.HasItem("Soft Wood", 5));
        Assert.True(inventory.HasItem("Soft Wood", 10));
        Assert.False(inventory.HasItem("Soft Wood", 11));
    }

    [Fact]
    public void HasItems_ChecksMultipleIngredients()
    {
        var inventory = new InventorySystem();
        inventory.AddItem("Base Ore", 2);
        inventory.AddItem("Soft Wood", 1);

        var recipe = new List<RecipeIngredient>
        {
            new("Base Ore", 1),
            new("Soft Wood", 1)
        };

        Assert.True(inventory.HasItems(recipe));

        var expensiveRecipe = new List<RecipeIngredient>
        {
            new("Base Ore", 5),
            new("Soft Wood", 1)
        };

        Assert.False(inventory.HasItems(expensiveRecipe));
    }

    [Fact]
    public void AddItem_NullOrEmpty_ThrowsArgumentException()
    {
        var inventory = new InventorySystem();

        Assert.Throws<ArgumentException>(() => inventory.AddItem(""));
        Assert.Throws<ArgumentException>(() => inventory.AddItem(null!));
    }

    [Fact]
    public void AddItem_ZeroOrNegativeAmount_ThrowsArgumentException()
    {
        var inventory = new InventorySystem();

        Assert.Throws<ArgumentException>(() => inventory.AddItem("Ore", 0));
        Assert.Throws<ArgumentException>(() => inventory.AddItem("Ore", -1));
    }

    [Fact]
    public void OnInventoryChanged_FiresOnAddAndRemove()
    {
        var inventory = new InventorySystem();
        int eventCount = 0;
        inventory.OnInventoryChanged += () => eventCount++;

        inventory.AddItem("Ore", 1);
        inventory.TryRemoveItem("Ore", 1);

        Assert.Equal(2, eventCount);
    }

    [Fact]
    public void GetItemCount_UnknownResource_ReturnsZero()
    {
        var inventory = new InventorySystem();

        Assert.Equal(0, inventory.GetItemCount("Unknown Resource"));
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        var inventory = new InventorySystem();
        inventory.AddItem("Ore", 5);
        inventory.AddItem("Wood", 3);

        inventory.Clear();

        Assert.Equal(0, inventory.GetItemCount("Ore"));
        Assert.Equal(0, inventory.GetItemCount("Wood"));
    }
}
