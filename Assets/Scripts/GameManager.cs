using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int initialCoins = 200;
    [SerializeField] private int gachaCost = 30;

    public int PlayerCoins { get; private set; }
    public int GachaCost => gachaCost;
    public bool IsGameOver { get; private set; }

    public System.Action<int> OnCoinsChanged;
    public System.Action<bool> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        PlayerCoins = initialCoins;
        IsGameOver = false;
        OnCoinsChanged?.Invoke(PlayerCoins);
        OnGameStateChanged?.Invoke(false);
    }

    public bool CanAffordGacha()
    {
        return PlayerCoins >= gachaCost && !IsGameOver;
    }

    public void SpendCoins(int amount)
    {
        if (amount <= 0 || IsGameOver) return;

        PlayerCoins -= amount;
        OnCoinsChanged?.Invoke(PlayerCoins);
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0 || IsGameOver) return;

        PlayerCoins += amount;
        OnCoinsChanged?.Invoke(PlayerCoins);
    }

    public void CheckGameOverAfterAction()
    {
        if (IsGameOver) return;

        int totalValue = PlayerCoins + InventorySystem.Instance.GetTotalSellValue();
        if (totalValue < gachaCost)
        {
            IsGameOver = true;
            Debug.Log("Game Over");
            OnGameStateChanged?.Invoke(true);
        }
    }

    public void RestartGame()
    {
        InitializeGame();
        InventorySystem.Instance.ClearInventory();
        GachaSystem.Instance.ResetPercentages();
    }
}