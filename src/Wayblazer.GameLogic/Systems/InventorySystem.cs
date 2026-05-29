using Wayblazer.GameLogic.Models;

namespace Wayblazer.GameLogic.Systems;

/// <summary>
/// Manages the player's inventory of resources.
/// Sprint 3: Core inventory operations.
/// </summary>
public class InventorySystem
{
    private readonly Dictionary<string, int> _items = new();

    public event Action? OnInventoryChanged;

    public IReadOnlyDictionary<string, int> Items => _items;

    public void AddItem(string resourceName, int amount = 1)
    {
        if (string.IsNullOrEmpty(resourceName))
            throw new ArgumentException("Resource name cannot be null or empty.", nameof(resourceName));
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive.", nameof(amount));

        if (_items.ContainsKey(resourceName))
            _items[resourceName] += amount;
        else
            _items[resourceName] = amount;

        OnInventoryChanged?.Invoke();
    }

    public bool TryRemoveItem(string resourceName, int amount = 1)
    {
        if (string.IsNullOrEmpty(resourceName))
            return false;
        if (amount <= 0)
            return false;
        if (!_items.ContainsKey(resourceName) || _items[resourceName] < amount)
            return false;

        _items[resourceName] -= amount;
        if (_items[resourceName] <= 0)
            _items.Remove(resourceName);

        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetItemCount(string resourceName)
    {
        return _items.GetValueOrDefault(resourceName, 0);
    }

    public bool HasItem(string resourceName, int amount = 1)
    {
        return GetItemCount(resourceName) >= amount;
    }

    public bool HasItems(IEnumerable<RecipeIngredient> ingredients)
    {
        return ingredients.All(i => HasItem(i.ResourceName, i.Amount));
    }

    public void Clear()
    {
        _items.Clear();
        OnInventoryChanged?.Invoke();
    }
}
