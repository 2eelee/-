using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private Transform materialSlotContainer;      // MaterialInventory > Slots
    [SerializeField] private Transform potionSlotContainer;        // PotionInventory > Slots
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private Text tooltipText;

    private Inventory materialInventory;
    private Inventory potionInventory;
    
    private List<InventorySlotUI> materialSlotUIs = new List<InventorySlotUI>();
    private List<InventorySlotUI> potionSlotUIs = new List<InventorySlotUI>();

    void Start()
    {
        // 인벤토리 초기화 (무한 슬롯)
        materialInventory = new Inventory(999);
        potionInventory = new Inventory(999);

        // 툴팁 초기화
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);

        // 기존 슬롯들을 InventorySlotUI로 등록
        RegisterSlotUIs();

        // 테스트 아이템 추가
        TestAddItems();
    }

    void RegisterSlotUIs()
    {
        // MaterialInventory의 기존 Slot들 등록
        foreach (Transform child in materialSlotContainer)
        {
            InventorySlotUI slotUI = new InventorySlotUI(child.gameObject, materialInventory, InventoryType.Material, materialSlotUIs.Count);
            materialSlotUIs.Add(slotUI);
            SetupSlotUIEvents(slotUI, InventoryType.Material);
        }

        // PotionInventory의 기존 Slot들 등록
        foreach (Transform child in potionSlotContainer)
        {
            InventorySlotUI slotUI = new InventorySlotUI(child.gameObject, potionInventory, InventoryType.Potion, potionSlotUIs.Count);
            potionSlotUIs.Add(slotUI);
            SetupSlotUIEvents(slotUI, InventoryType.Potion);
        }
    }

    void SetupSlotUIEvents(InventorySlotUI slotUI, InventoryType type)
    {
        Image image = slotUI.slotGameObject.GetComponent<Image>();
        EventTrigger trigger = slotUI.slotGameObject.AddComponent<EventTrigger>();

        // PointerEnter: 아이템이 있을 때만 툴팁 표시
        EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener(data =>
        {
            var item = slotUI.GetItem();
            if (item != null && type == InventoryType.Potion)  // 포션 인벤에만 툴팁 표시
            {
                ShowTooltip(item.itemName);
            }
        });
        trigger.triggers.Add(pointerEnter);

        // PointerExit: 툴팁 숨기기
        EventTrigger.Entry pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener(data => HideTooltip());
        trigger.triggers.Add(pointerExit);
    }

    public void AddItemToInventory(Item item, InventoryType type)
    {
        Inventory targetInventory = (type == InventoryType.Material) ? materialInventory : potionInventory;
        List<InventorySlotUI> slotUIs = (type == InventoryType.Material) ? materialSlotUIs : potionSlotUIs;

        // 인벤토리에 아이템 추가
        targetInventory.AddItem(item);

        // UI 업데이트: 아이템이 있는 순서대로 UI 갱신
        UpdateInventoryUI(targetInventory, slotUIs);
    }

    void UpdateInventoryUI(Inventory inventory, List<InventorySlotUI> slotUIs)
    {
        var items = inventory.GetAllItems();  // 아이템이 있는 것들만 반환

        // 기존 슬롯들 초기화
        foreach (var slotUI in slotUIs)
        {
            slotUI.Clear();
        }

        // 아이템을 순서대로 슬롯에 배치
        for (int i = 0; i < items.Count && i < slotUIs.Count; i++)
        {
            slotUIs[i].SetItem(items[i]);
        }
    }

    void ShowTooltip(string itemName)
    {
        if (tooltipPanel == null) return;

        tooltipPanel.SetActive(true);
        tooltipText.text = itemName;

        // 포션 인벤토리 위에 표시
        RectTransform tooltipRect = tooltipPanel.GetComponent<RectTransform>();
        RectTransform potionRect = potionSlotContainer.GetComponent<RectTransform>();
        
        if (potionRect != null)
        {
            Vector3 potionPos = potionRect.position;
            tooltipRect.position = potionPos + Vector3.up * 50;  // 포션 인벤 위쪽에 고정
        }
    }

    void HideTooltip()
    {
        if (tooltipPanel != null)
            tooltipPanel.SetActive(false);
    }

    void TestAddItems()
    {
        Sprite tempIcon = Resources.Load<Sprite>("Sprites/TestIcon");
        
        Item herb = new Item("herb", "민트풀", 50, 99, tempIcon, 0);
        AddItemToInventory(herb, InventoryType.Material);

        Item potion = new Item("potion", "빨간 물약", 5, 20, tempIcon, 0);
        AddItemToInventory(potion, InventoryType.Potion);
    }

    public enum InventoryType
    {
        Material,
        Potion
    }
}

/// <summary>
/// 개별 슬롯 UI 관리
/// </summary>
public class InventorySlotUI
{
    public GameObject slotGameObject;
    private Image slotImage;
    private Text itemCountText;
    private Item currentItem;
    private Inventory inventory;
    private InventoryUIManager.InventoryType inventoryType;
    private int slotIndex;

    public InventorySlotUI(GameObject slotObj, Inventory inv, InventoryUIManager.InventoryType type, int index)
    {
        slotGameObject = slotObj;
        inventory = inv;
        inventoryType = type;
        slotIndex = index;

        slotImage = slotObj.GetComponent<Image>();
        itemCountText = slotObj.transform.Find("ItemCount")?.GetComponent<Text>();
    }

    public void SetItem(Item item)
    {
        currentItem = item;
        
        if (item == null)
        {
            Clear();
            return;
        }

        // 아이콘 표시
        if (slotImage != null && item.icon != null)
        {
            slotImage.sprite = item.icon;
            slotImage.color = Color.white;
        }

        // 수량 표시
        if (itemCountText != null)
        {
            itemCountText.text = item.quantity > 1 ? item.quantity.ToString() : "";
        }
    }

    public void Clear()
    {
        currentItem = null;
        
        if (slotImage != null)
        {
            slotImage.sprite = null;
            slotImage.color = Color.gray;
        }

        if (itemCountText != null)
        {
            itemCountText.text = "";
        }
    }

    public Item GetItem()
    {
        return currentItem;
    }
}
