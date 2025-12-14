using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private Transform materialSlotContainer;      // 왼쪽 인벤 슬롯 부모
    [SerializeField] private Transform potionSlotContainer;        // 오른쪽 인벤 슬롯 부모
    [SerializeField] private GameObject slotPrefab;                // 슬롯 프리팹
    [SerializeField] private Canvas canvas;
    [SerializeField] private Text materialPageText;                // "페이지 1/5" 표시
    [SerializeField] private Text potionPageText;
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private Text tooltipText;

    private Inventory materialInventory;
    private Inventory potionInventory;
    private float pageChangeSpeed = 0.05f;
    private bool isChangingPage = false;

    void Start()
    {
        // 인벤토리 초기화
        materialInventory = new Inventory(6);  // 왼쪽: 6칸/페이지
        potionInventory = new Inventory(5);    // 오른쪽: 5칸/페이지

        // 툴팁 생성
        CreateTooltip();

        // UI 초기 렌더링
        RefreshUI();

        // 테스트 아이템 추가
        TestAddItems();
    }

    void Update()
    {
        // 임시: 스페이스바로 왼쪽 페이지 넘기기
        if (Input.GetKeyDown(KeyCode.Space) && !isChangingPage)
        {
            StartCoroutine(ChangePageWithDelay(materialInventory, materialSlotContainer, materialPageText));
        }

        // 임시: E키로 오른쪽 페이지 넘기기
        if (Input.GetKeyDown(KeyCode.E) && !isChangingPage)
        {
            StartCoroutine(ChangePageWithDelay(potionInventory, potionSlotContainer, potionPageText));
        }
    }

    void CreateTooltip()
    {
        RectTransform rect = tooltipPanel.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 40);
        tooltipPanel.SetActive(false);
    }

    void RefreshUI()
    {
        RefreshInventoryUI(materialInventory, materialSlotContainer, materialPageText);
        RefreshInventoryUI(potionInventory, potionSlotContainer, potionPageText);
    }

    void RefreshInventoryUI(Inventory inventory, Transform slotContainer, Text pageText)
    {
        // ✅ FIX 1: 기존 슬롯 모두 삭제 (중복 방지)
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }

        // 현재 페이지의 슬롯 표시
        var currentSlots = inventory.GetCurrentPageSlots();
        foreach (var slot in currentSlots)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotContainer);
            Image slotImage = slotObj.GetComponent<Image>();
            Text itemNameText = slotObj.transform.Find("ItemCount")?.GetComponent<Text>();

            if (slot.isEmpty)
            {
                slotImage.color = Color.gray;
                if (itemNameText) itemNameText.text = "";
            }
            else
            {
                slotImage.color = Color.white;
                slotImage.sprite = slot.item.icon;
                if (itemNameText && slot.item.quantity > 1)
                {
                    itemNameText.text = slot.item.quantity.ToString();
                }

                // 툴팁 이벤트
                EventTrigger trigger = slotObj.AddComponent<EventTrigger>();
                
                EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
                pointerEnter.eventID = EventTriggerType.PointerEnter;
                pointerEnter.callback.AddListener(data => ShowTooltip(slot.item.itemName, ((PointerEventData)data).position));
                trigger.triggers.Add(pointerEnter);

                EventTrigger.Entry pointerExit = new EventTrigger.Entry();
                pointerExit.eventID = EventTriggerType.PointerExit;
                pointerExit.callback.AddListener(data => HideTooltip());
                trigger.triggers.Add(pointerExit);
            }
        }

        // 페이지 텍스트 업데이트
        int totalPages = inventory.TotalPages > 0 ? inventory.TotalPages : 1;
        pageText.text = $"페이지 {inventory.CurrentPage + 1}/{totalPages}";
    }

    IEnumerator ChangePageWithDelay(Inventory inventory, Transform slotContainer, Text pageText)
    {
        isChangingPage = true;
        inventory.NextPage();
        RefreshInventoryUI(inventory, slotContainer, pageText);
        yield return new WaitForSeconds(pageChangeSpeed);
        isChangingPage = false;
    }

    void ShowTooltip(string itemName, Vector2 mousePos)
    {
        tooltipPanel.SetActive(true);
        tooltipText.text = itemName;
        RectTransform rect = tooltipPanel.GetComponent<RectTransform>();
        rect.position = mousePos + Vector2.right * 10;
    }

    void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }

    void TestAddItems()
    {
        // 테스트용 아이템 추가
        Sprite tempIcon = Resources.Load<Sprite>("Sprites/TestIcon"); // 임시 아이콘
        
        Item herb = new Item("herb", "민트풀", 50, 99, tempIcon, 0);
        materialInventory.AddItem(herb);

        Item potion = new Item("potion", "빨간 물약", 5, 20, tempIcon, 0);
        potionInventory.AddItem(potion);

        RefreshUI();
    }
}
