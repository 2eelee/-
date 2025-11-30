using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory
{
    private List<InventorySlot> slots;
    private int maxSlots = 999;
    private int slotsPerPage;
    private int currentPage = 0;
    private int nextSortIndex = 0;

    public int CurrentPage => currentPage;
    public int SlotsPerPage => slotsPerPage;
    public int TotalPages => Mathf.CeilToInt((float)slots.Count(s => !s.isEmpty) / slotsPerPage);

    public Inventory(int slotsPerPage)
    {
        this.slotsPerPage = slotsPerPage;
        slots = new List<InventorySlot>();
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlot());
        }
    }
    public void AddItem(Item newItem)
    {
        int remainingQuantity = newItem.quantity;

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

                    if (remainingQuantity == 0) return;
                }
            }
        }

        // 새로운 슬롯에 추가
        while (remainingQuantity > 0)
        {
            int emptyIndex = slots.FindIndex(s => s.isEmpty);
            if (emptyIndex == -1) break; // 슬롯 가득참

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

    // 슬롯 목록 가져오기 (현재 페이지)
    public List<InventorySlot> GetCurrentPageSlots()
    {
        int startIndex = currentPage * slotsPerPage;
        return slots.GetRange(startIndex, Mathf.Min(slotsPerPage, slots.Count - startIndex));
    }
    public void NextPage()
    {
        int totalPages = TotalPages;
        if (totalPages == 0) return;

        currentPage = (currentPage + 1) % totalPages;
    }
    public void RemoveItemFromSlot(int slotIndex)
    {
        int actualIndex = currentPage * slotsPerPage + slotIndex;
        if (actualIndex < slots.Count)
        {
            slots[actualIndex].RemoveItem();
        }
    }
    public Item GetSlotItem(int slotIndex)
    {
        int actualIndex = currentPage * slotsPerPage + slotIndex;
        if (actualIndex < slots.Count)
        {
            return slots[actualIndex].item;
        }
        return null;
    }
}