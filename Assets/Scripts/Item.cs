using System;

[Serializable]
public enum Rarity
{
    Metal = 0,
    Bronze = 1,
    Silver = 2,
    Gold = 3,
    Diamond = 4
}

[Serializable]
public class Item
{
    public Rarity Rarity { get; private set; }
    public int SellValue { get; private set; }

    public Item(Rarity rarity)
    {
        Rarity = rarity;
        SellValue = GetSellValue(rarity);
    }

    private int GetSellValue(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Metal => 10,
            Rarity.Bronze => 20,
            Rarity.Silver => 30,
            Rarity.Gold => 40,
            Rarity.Diamond => 50,
            _ => 0
        };
    }
}