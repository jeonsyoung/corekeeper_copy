using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    [Header("테스트용 아이템")]
    [SerializeField] private ItemData wood;
    [SerializeField] private ItemData stone;
    [SerializeField] private ItemData iron;

    [Header("현재 인벤토리")]
    [SerializeField] private List<InventorySlotData> slots = new();

    public IReadOnlyList<InventorySlotData> Slots => slots;

    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        // 테스트용 단축키
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddItem(wood, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddItem(stone, 1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AddItem(iron, 1);
        }
    }

    public void AddItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
        {
            return;
        }

        InventorySlotData existingSlot =
            slots.Find(slot => slot.item == item);

        if (existingSlot != null)
        {
            existingSlot.amount += amount;
        }
        else
        {
            slots.Add(new InventorySlotData(item, amount));
        }

        OnInventoryChanged?.Invoke();
    }

    public int GetItemAmount(ItemData item)
    {
        InventorySlotData slot =
            slots.Find(currentSlot => currentSlot.item == item);

        return slot == null ? 0 : slot.amount;
    }

    public bool RemoveItem(ItemData item, int amount)
    {
        InventorySlotData slot =
            slots.Find(currentSlot => currentSlot.item == item);

        if (slot == null || slot.amount < amount)
        {
            return false;
        }

        slot.amount -= amount;

        if (slot.amount <= 0)
        {
            slots.Remove(slot);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }
}