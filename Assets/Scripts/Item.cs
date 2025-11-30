using UnityEngine;

[System.Serializable]
public class Item
{
    public string itemID;        // 아이템 고유 ID (예: "herb", "potion")
    public string itemName;      // 아이템 이름
    public int quantity;         // 현재 수량
    public int maxStackSize;     // 최대 스택 크기 (재료: 99, 완성물약: 20)
    public Sprite icon;          // 슬롯에 표시할 아이콘
    public int sortIndex;        // 획득 순서 기록용 인덱스

    public Item(string id, string name, int quantity, int maxStack, Sprite icon, int sortIndex)
    {
        itemID = id;
        itemName = name;
        this.quantity = quantity;
        maxStackSize = maxStack;
        this.icon = icon;
        this.sortIndex = sortIndex;
    }
}