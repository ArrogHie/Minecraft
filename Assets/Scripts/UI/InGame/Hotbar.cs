using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public Inventory inventory;
    public List<InventorySlot> slots;
    public List<GameObject> selectionSlots;
    public GameObject itemPrefeb;
    public PlayerControl playerControl;

    public int selectedSlot = 0;

    void Update()
    {
        if (inventory == null || inventory.isOpen) return;

        SyncHotbar();
        CheckHotbarInput();
        UpdateHotbarSelection();
    }

    /// <summary>
    /// 检查数字键1-9输入，切换快捷栏选中的槽位
    /// </summary>
    void CheckHotbarInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlot = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlot = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlot = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlot = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5)) selectedSlot = 4;
        else if (Input.GetKeyDown(KeyCode.Alpha6)) selectedSlot = 5;
        else if (Input.GetKeyDown(KeyCode.Alpha7)) selectedSlot = 6;
        else if (Input.GetKeyDown(KeyCode.Alpha8)) selectedSlot = 7;
        else if (Input.GetKeyDown(KeyCode.Alpha9)) selectedSlot = 8;

        UpdateActiveBlock();
    }

    /// <summary>
    /// 根据当前快捷栏选中的槽位，更新PlayerControl的activeBlock
    /// </summary>
    public void UpdateActiveBlock()
    {
        InventorySlot hotbarSlot = inventory.slots[selectedSlot];
        if (playerControl != null && hotbarSlot != null && hotbarSlot.item != null)
        {
            System.Enum.TryParse(hotbarSlot.item.itemName, out playerControl.activeBlock);
        }
        else if (playerControl != null)
        {
            playerControl.activeBlock = BlockType.Air;
        }
    }

    /// <summary>
    /// 同步快捷栏与背包最下一行的物品（图标和数量）
    /// </summary>
    void SyncHotbar()
    {
        if (inventory == null || inventory.slots == null || inventory.slots.Count < 36 || slots == null || slots.Count < 9) return;

        for (int i = 0; i < 9; i++)
        {
            InventorySlot mainSlot = inventory.slots[i];
            InventorySlot hotbarSlot = slots[i];

            if (mainSlot.item != null)
            {
                if (hotbarSlot.item == null)
                {
                    GameObject newItem = Instantiate(itemPrefeb, hotbarSlot.transform);
                    newItem.transform.position = hotbarSlot.transform.position;
                    InventoryItem item = newItem.GetComponent<InventoryItem>();
                    item.itemName = mainSlot.item.itemName;
                    item.SetAmmount(mainSlot.item.ammount);
                    Image img = newItem.GetComponent<Image>();
                    Sprite dynamicIcon = BlockIconGenerator.Instance != null ? BlockIconGenerator.Instance.GetIcon(item.itemName) : null;
                    if (dynamicIcon != null)
                    {
                        img.sprite = dynamicIcon;
                    }
                    else
                    {
                        img.sprite = Resources.Load<Sprite>("Image/Imgs/Block/" + item.itemName);
                    }
                    hotbarSlot.item = item;
                    item.slot = hotbarSlot;
                }
                else
                {
                    hotbarSlot.item.SetAmmount(mainSlot.item.ammount);
                }
            }
            else
            {
                if (hotbarSlot.item != null)
                {
                    Destroy(hotbarSlot.item.gameObject);
                    hotbarSlot.item = null;
                }
            }
        }
    }

    /// <summary>
    /// 更新快捷栏选中高亮显示
    /// </summary>
    void UpdateHotbarSelection()
    {
        if (selectionSlots == null) return;
        for (int i = 0; i < selectionSlots.Count; i++)
        {
            selectionSlots[i].gameObject.SetActive(i == selectedSlot);
        }
    }

    /// <summary>
    /// 获取当前选中的快捷栏槽位索引
    /// </summary>
    public int GetSelectedSlotIndex()
    {
        return selectedSlot;
    }

    /// <summary>
    /// 减少当前选中快捷栏槽位的物品数量（放置方块时调用）
    /// </summary>
    public void DecreaseSelectedItem()
    {
        if (inventory == null || inventory.slots == null || inventory.slots.Count < 36) return;

        InventorySlot mainSlot = inventory.slots[selectedSlot];
        if (mainSlot.item != null)
        {
            mainSlot.item.IncreaseAmmount(-1);
            if (mainSlot.item.ammount <= 0)
            {
                Destroy(mainSlot.item.gameObject);
                mainSlot.item = null;
            }
        }
        UpdateActiveBlock();
    }
}
