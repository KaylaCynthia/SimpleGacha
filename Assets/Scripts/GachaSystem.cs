using UnityEngine;
using System;

public class GachaSystem : MonoBehaviour
{
    public static GachaSystem Instance { get; private set; }

    [SerializeField] private int[] defaultPercentages = new int[] { 20, 20, 20, 20, 20 };

    private int[] currentPercentages = new int[5];
    private float[] effectivePercentages = new float[5];

    public System.Action<Item> OnGachaResult;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            ResetPercentages();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetPercentages()
    {
        Array.Copy(defaultPercentages, currentPercentages, defaultPercentages.Length);
        CalculateEffectivePercentages();
    }

    public void SetPercentage(int index, int value)
    {
        if (index < 0 || index >= currentPercentages.Length) return;
        if (value < 0) value = 0;

        currentPercentages[index] = value;
        CalculateEffectivePercentages();
    }

    public int GetPercentage(int index)
    {
        if (index < 0 || index >= currentPercentages.Length) return 0;
        return currentPercentages[index];
    }

    private void CalculateEffectivePercentages()
    {
        int total = 0;
        foreach (int percentage in currentPercentages)
        {
            total += percentage;
        }

        if (total == 0)
        {
            for (int i = 0; i < effectivePercentages.Length; i++)
            {
                effectivePercentages[i] = 0f;
            }
            return;
        }

        for (int i = 0; i < currentPercentages.Length; i++)
        {
            effectivePercentages[i] = (float)currentPercentages[i] / total;
        }
    }

    public void PerformGacha()
    {
        if (GameManager.Instance.IsGameOver)
        {
            Debug.Log("Game Over - cannot perform gacha");
            return;
        }

        if (!GameManager.Instance.CanAffordGacha())
        {
            Debug.Log("Not enough coin");
            return;
        }

        int total = 0;
        foreach (int percentage in currentPercentages)
        {
            total += percentage;
        }

        if (total == 0)
        {
            Debug.Log("Invalid percentages: total cannot be zero");
            return;
        }

        GameManager.Instance.SpendCoins(GameManager.Instance.GachaCost);
        Item result = GetRandomItem();
        OnGachaResult?.Invoke(result);
    }

    private Item GetRandomItem()
    {
        float randomValue = UnityEngine.Random.value;
        float cumulative = 0f;

        for (int i = 0; i < effectivePercentages.Length; i++)
        {
            cumulative += effectivePercentages[i];
            if (randomValue <= cumulative)
            {
                return new Item((Rarity)i);
            }
        }

        return new Item(Rarity.Metal);
    }
}