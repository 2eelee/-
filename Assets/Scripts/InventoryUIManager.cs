using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private Transform materialSlotContainer;      // 왼쪽 인벤 슬롯 부모
    [SerializeField] private Transform potionSlotContainer;        // 오른쪽 인벤 슬롯 부모
    [SerializeField] private GameObject slotPrefab;                // 슬롯 프리팹
    [SerializeField] private Canvas canvas;
    [SerializeField] private Sprite testIcon;
    [SerializeField] private Text materialPageText;                // "페이지 1/5" 표시
    [SerializeField] private Text potionPageText;

    private Inventory materialInventory;
    private Inventory potionInventory;
    private float pageChangeSpeed = 0.05f;
    private bool isChangingPage = false;

    void Start()
    {
        materialInventory = new Inventory(5);
        potionInventory = new Inventory(5);

        RefreshUI();

        TestAddItems();
    }

    void Update()
    {
        // 임시: E키로 포션 인벤 페이지 넘기기
        if (Input.GetKeyDown(KeyCode.E) && !isChangingPage)
        {
            StartCoroutine(ChangePageWithDelay(potionInventory, potionSlotContainer, potionPageText));
        }
    }
    void FixedUpdate()
    {
        if (DialogueManager.DialogueActive == true)
        {
            if(gameObject.activeSelf)
            {
                gameObject.SetActive(false);
            }
        }
        else if (DialogueManager.DialogueActive == false)
        {
            if(gameObject.activeSelf == false)
                gameObject.SetActive(true);
        }
    }
    void RefreshUI()
    {
        RefreshInventoryUI(materialInventory, materialSlotContainer, materialPageText);
        RefreshInventoryUI(potionInventory, potionSlotContainer, potionPageText);
    }

    void RefreshInventoryUI(Inventory inventory, Transform slotContainer, Text pageText)
    {
        foreach (Transform child in slotContainer)
        {
            Destroy(child.gameObject);
        }
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
            }
        }
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
    void TestAddItems()
    {        
        Item herb = new Item("herb", "민트풀", 50, 99, testIcon, 0);
        materialInventory.AddItem(herb);

        Item potion = new Item("potion", "빨간 물약", 5, 20, testIcon, 0);
        potionInventory.AddItem(potion);

        RefreshUI();
    }
    public void OnClick_NextMaterialPage()
{
    if (!isChangingPage)
        StartCoroutine(ChangePageWithDelay(materialInventory, materialSlotContainer, materialPageText));
}

public void OnClick_NextPotionPage()
{
    if (!isChangingPage)
        StartCoroutine(ChangePageWithDelay(potionInventory, potionSlotContainer, potionPageText));
}
}