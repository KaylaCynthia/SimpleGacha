using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    private List<Item> items = new List<Item>();

    public System.Action<List<Item>> OnInventoryUpdated;
    public System.Action<int> OnTotalValueChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(Item item)
    {
        if (GameManager.Instance.IsGameOver) return;

        items.Add(item);
        SortInventory();
        OnInventoryUpdated?.Invoke(items);
        OnTotalValueChanged?.Invoke(GetTotalSellValue());
    }

    public void SellItem(Item item)
    {
        if (GameManager.Instance.IsGameOver) return;

        if (items.Contains(item))
        {
            items.Remove(item);
            GameManager.Instance.AddCoins(item.SellValue);
            OnInventoryUpdated?.Invoke(items);
            OnTotalValueChanged?.Invoke(GetTotalSellValue());

            GameManager.Instance.CheckGameOverAfterAction();
        }
    }

    public int GetTotalSellValue()
    {
        int total = 0;
        foreach (Item item in items)
        {
            total += item.SellValue;
        }
        return total;
    }

    private void SortInventory()
    {
        items.Sort((a, b) => b.Rarity.CompareTo(a.Rarity));
    }

    public void ClearInventory()
    {
        items.Clear();
        OnInventoryUpdated?.Invoke(items);
        OnTotalValueChanged?.Invoke(0);
    }
}