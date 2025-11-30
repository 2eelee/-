using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public Item item;       // null이면 빈 슬롯
    public bool isEmpty => item == null;

    public InventorySlot()
    {
        item = null;
    }

    public bool AddItem(Item newItem, int maxStackSize)
    {
        if (isEmpty)
        {
            // 빈 슬롯에 아이템 추가
            item = newItem;
            return true;
        }
        else if (item.itemID == newItem.itemID && item.quantity < maxStackSize)
        {
            // 같은 아이템이고 스택 여유가 있으면 수량 증가
            int addAmount = Mathf.Min(newItem.quantity, maxStackSize - item.quantity);
            item.quantity += addAmount;
            return addAmount == newItem.quantity; // 모두 추가되었으면 true
        }
        return false; // 추가 실패
    }

    public void RemoveItem()
    {
        item = null;
    }
}