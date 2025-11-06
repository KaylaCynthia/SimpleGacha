using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Button sellButton;

    [Header("Rarity Sprites")]
    [SerializeField] private Sprite metalSprite;
    [SerializeField] private Sprite bronzeSprite;
    [SerializeField] private Sprite silverSprite;
    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite diamondSprite;

    private Item item;

    private void Start()
    {
        sellButton.onClick.AddListener(OnSellButtonClick);

        if (GameManager.Instance.IsGameOver)
        {
            sellButton.interactable = false;
        }
    }

    public void Initialize(Item item)
    {
        this.item = item;
        SetRaritySprite(item.Rarity);
    }

    private void SetRaritySprite(Rarity rarity)
    {
        if (itemImage == null) return;

        itemImage.sprite = rarity switch
        {
            Rarity.Metal => metalSprite,
            Rarity.Bronze => bronzeSprite,
            Rarity.Silver => silverSprite,
            Rarity.Gold => goldSprite,
            Rarity.Diamond => diamondSprite,
            _ => metalSprite
        };
    }

    private void OnSellButtonClick()
    {
        InventorySystem.Instance.SellItem(item);
        Destroy(gameObject);
    }
}