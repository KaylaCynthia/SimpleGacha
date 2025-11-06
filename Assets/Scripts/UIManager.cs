using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("Coin UI")]
    [SerializeField] private Text coinText;

    [Header("Gacha UI")]
    [SerializeField] private InputField[] percentageInputFields;
    [SerializeField] private Button gachaButton;

    [Header("Storage UI")]
    [SerializeField] private Text totalValueText;
    [SerializeField] private Transform itemsContainer;
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private ScrollRect inventoryScrollRect;

    private List<GameObject> currentItemSlots = new List<GameObject>();

    private void Start()
    {
        InitializeUI();
        RegisterEvents();
    }

    private void InitializeUI()
    {
        UpdateCoinText(GameManager.Instance.PlayerCoins);
        UpdatePercentageInputFields();
        UpdateTotalValueText(0);
    }

    private void RegisterEvents()
    {
        GameManager.Instance.OnCoinsChanged += UpdateCoinText;
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;

        GachaSystem.Instance.OnGachaResult += OnGachaResult;

        InventorySystem.Instance.OnInventoryUpdated += UpdateInventoryUI;
        InventorySystem.Instance.OnTotalValueChanged += UpdateTotalValueText;

        gachaButton.onClick.AddListener(OnGachaButtonClick);

        for (int i = 0; i < percentageInputFields.Length; i++)
        {
            int index = i;
            percentageInputFields[i].onEndEdit.AddListener((value) => OnPercentageInputChanged(index, value));
        }
    }

    private void OnGameStateChanged(bool isGameOver)
    {
        gachaButton.interactable = !isGameOver;

        foreach (InputField inputField in percentageInputFields)
        {
            inputField.interactable = !isGameOver;
        }

        foreach (GameObject slot in currentItemSlots)
        {
            Button slotButton = slot.GetComponent<Button>();
            if (slotButton != null)
            {
                slotButton.interactable = !isGameOver;
            }
        }
    }

    private void UpdateCoinText(int coins)
    {
        coinText.text = $"Coins: {coins}";
    }

    private void UpdateTotalValueText(int totalValue)
    {
        totalValueText.text = $"Storage Value: {totalValue}";
    }

    private void UpdatePercentageInputFields()
    {
        for (int i = 0; i < percentageInputFields.Length; i++)
        {
            percentageInputFields[i].text = GachaSystem.Instance.GetPercentage(i).ToString();
        }
    }

    private void OnPercentageInputChanged(int index, string value)
    {
        if (GameManager.Instance.IsGameOver) return;

        if (int.TryParse(value, out int percentage))
        {
            GachaSystem.Instance.SetPercentage(index, percentage);
        }
        else
        {
            percentageInputFields[index].text = "0";
            GachaSystem.Instance.SetPercentage(index, 0);
        }
    }

    private void OnGachaButtonClick()
    {
        GachaSystem.Instance.PerformGacha();
    }

    private void OnGachaResult(Item item)
    {
        InventorySystem.Instance.AddItem(item);
        Debug.Log($"Obtained: {item.Rarity} (Value: {item.SellValue})");

        GameManager.Instance.CheckGameOverAfterAction();
    }

    private void UpdateInventoryUI(List<Item> items)
    {
        foreach (GameObject slot in currentItemSlots)
        {
            Destroy(slot);
        }
        currentItemSlots.Clear();

        foreach (Item item in items)
        {
            GameObject slot = Instantiate(itemSlotPrefab, itemsContainer);
            InventorySlot inventorySlot = slot.GetComponent<InventorySlot>();
            inventorySlot.Initialize(item);
            currentItemSlots.Add(slot);

            if (GameManager.Instance.IsGameOver)
            {
                Button slotButton = slot.GetComponent<Button>();
                if (slotButton != null)
                {
                    slotButton.interactable = false;
                }
            }
        }
    }
}