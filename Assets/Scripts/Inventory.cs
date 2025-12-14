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
    public int TotalPages
    {
        get
        {
            var nonEmptySlots = slots.Count(s => !s.isEmpty);
            return nonEmptySlots == 0 ? 1 : Mathf.CeilToInt((float)nonEmptySlots / slotsPerPage);
        }
    }

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

        // 단계 1: 기존 아이템 스택을 찜다
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
                    {
                        // ✅ FIX 2: 아이템 추가 후 UI 갱메 업데이트
                        return;
                    }
                }
            }
        }

        // 단계 2: 새로운 슬롯에 추가
        while (remainingQuantity > 0)
        {
            // ✅ FIX 3: 비어있는 슬롯 공간을 단상에서 찬다
            int emptyIndex = -1;
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i].isEmpty)
                {
                    emptyIndex = i;
                    break;  // 찬 맨나 비어있는 슬롯 스킬
                }
            }

            if (emptyIndex == -1) 
            {
                Debug.LogWarning("인벤날리 가득 가득!);
                break;  // 더 이상 추가할 슬롯 없음
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

    // 슬롯 목록 가져오기 (현재 페이지)
    public List<InventorySlot> GetCurrentPageSlots()
    {
        // ✅ FIX 4: 비어있는 아이템만 필터링
        var nonEmptySlots = slots.Where(s => !s.isEmpty).ToList();
        
        // ✅ 페이지에 맨는 슬롯만 추출
        int startIndex = currentPage * slotsPerPage;
        int count = Mathf.Min(slotsPerPage, nonEmptySlots.Count - startIndex);
        
        if (startIndex >= nonEmptySlots.Count)
            return new List<InventorySlot>();
        
        return nonEmptySlots.GetRange(startIndex, count);
    }

    public void NextPage()
    {
        int totalPages = TotalPages;
        if (totalPages <= 1) return;

        currentPage = (currentPage + 1) % totalPages;
    }

    // ✅ FIX 5: 비어있는 아이템만 사용당으로 제거
    public void RemoveItem(int slotIndex)
    {
        var nonEmptySlots = slots.Where(s => !s.isEmpty).ToList();
        int actualIndex = currentPage * slotsPerPage + slotIndex;

        if (actualIndex < nonEmptySlots.Count)
        {
            // 비어있는 아이템만 진짹 인벤렉스 찾기
            for (int i = 0; i < slots.Count; i++)
            {
                if (!slots[i].isEmpty)
                {
                    actualIndex--;
                    if (actualIndex == 0)
                    {
                        slots[i].RemoveItem();
                        return;
                    }
                }
            }
        }
    }

    public Item GetSlotItem(int slotIndex)
    {
        var nonEmptySlots = slots.Where(s => !s.isEmpty).ToList();
        int actualIndex = currentPage * slotsPerPage + slotIndex;
        
        if (actualIndex < nonEmptySlots.Count)
        {
            return nonEmptySlots[actualIndex].item;
        }
        return null;
    }
}
