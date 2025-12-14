using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    private List<InventorySlot> slots;
    private int maxSlots;
    private int nextSortIndex = 0;

    public Inventory(int maxSlots)
    {
        this.maxSlots = maxSlots;
        slots = new List<InventorySlot>();
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public void AddItem(Item newItem)
    {
        int remainingQuantity = newItem.quantity;

        // 단계 1: 기존 아이템 스택 찜단
        foreach (var slot in slots)
        {
            if (!slot.isEmpty && slot.item.itemID == newItem.itemID)
            {
                int canAdd = newItem.maxStackSize - slot.item.quantity;
                if (canAdd > 0)
                {
                    int addAmount = Mathf.Min(remainingQuantity, canAdd);
                    slot.item.quantity += addAmount;
                    remainingQuantity -= addAmount;

                    if (remainingQuantity == 0)
                        return;
                }
            }
        }

        // 단계 2: 비어있는 슬롯을 맨 앞에서부터 챌브담은 아이템 넘기기
        while (remainingQuantity > 0)
        {
            int emptyIndex = slots.FindIndex(s => s.isEmpty);
            if (emptyIndex == -1)
            {
                Debug.LogWarning("인벤날리 가득!");
                break;
            }

            int addAmount = Mathf.Min(remainingQuantity, newItem.maxStackSize);
            slots[emptyIndex].item = new Item(
                newItem.itemID,
                newItem.itemName,
                addAmount,
                newItem.maxStackSize,
                newItem.icon,
                nextSortIndex++
            );
            remainingQuantity -= addAmount;
        }
    }

    /// <summary>
    /// 비어있는 아이템들을 순서대로 반환 (븋당슰리로 줨 아이템들을 맨앞에)
    /// </summary>
    public List<Item> GetAllItems()
    {
        // 1. 비어있는 아이템만 추출
        var items = slots
            .Where(s => !s.isEmpty)
            .Select(s => s.item)
            .ToList();

        // 2. sortIndex로 정렬 (비어있는 아이템을 맨앞에 배치하기 위해)
        items = items.OrderBy(item => item.sortIndex).ToList();

        return items;
    }

    public void RemoveItem(int slotIndex)
    {
        var items = GetAllItems();
        if (slotIndex < items.Count)
        {
            // 비어있는 아이템만 기냉으로 데이터 비기
            int actualIndex = slots.FindIndex(s => !s.isEmpty && s.item == items[slotIndex]);
            if (actualIndex != -1)
            {
                slots[actualIndex].RemoveItem();
            }
        }
    }

    public Item GetItem(int slotIndex)
    {
        var items = GetAllItems();
        if (slotIndex < items.Count)
        {
            return items[slotIndex];
        }
        return null;
    }

    public int GetItemCount()
    {
        return slots.Count(s => !s.isEmpty);
    }
}
