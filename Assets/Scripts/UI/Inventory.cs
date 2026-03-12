using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public Canvas canvas;
    public Image window;
    public List<InventorySlot> slots;
    public Transform itemParent;
    public GameObject itemPrefeb;

    [Header("Crafting - 合成系统")]
    public List<InventorySlot> craftingInputSlots;
    public InventorySlot craftingOutputSlot;
    public Image craftingOutputImage;
    private CraftingRecipe currentRecipe;

    public bool isOpen = false;

    InventoryItem draggedItem;
    private string[] lastCraftingInputs = new string[4];
    private int[] lastCraftingAmounts = new int[4];

    void Start()
    {
        InitializeCraftingSlots();
    }

    void InitializeCraftingSlots()
    {
        for (int i = 0; i < 4; i++)
        {
            lastCraftingInputs[i] = null;
            lastCraftingAmounts[i] = 0;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) { ToggleInventory(); }

        if (draggedItem)
        {
            draggedItem.transform.SetSiblingIndex(draggedItem.transform.parent.childCount - 1);
            if (Input.GetButtonDown("Fire2"))
            {
                if(draggedItem.ammount <= 1)
                {
                    Drop(draggedItem);
                    return;
                }

                GameObject newItem = Instantiate(draggedItem.gameObject, parent: draggedItem.transform.parent);
                AddItemTriggers(newItem.GetComponent<InventoryItem>());
                newItem.GetComponent<InventoryItem>().SetAmmount(1);
                draggedItem.IncreaseAmmount(-1);
                Drop(newItem.GetComponent<InventoryItem>());
            }
            if (Input.GetButtonUp("Fire1")) { Drop(draggedItem); }
        }

        if (isOpen)
        {
            CheckCraftingInput();
        }
    }

    /// <summary>
    /// 检查合成输入是否有变化
    /// </summary>
    void CheckCraftingInput()
    {
        bool changed = false;
        string[] currentInputs = new string[4];
        int[] currentAmounts = new int[4];

        for (int i = 0; i < craftingInputSlots.Count; i++)
        {
            InventorySlot slot = craftingInputSlots[i];
            if (slot != null && slot.item != null)
            {
                currentInputs[i] = slot.item.itemName;
                currentAmounts[i] = slot.item.ammount;
            }
            else
            {
                currentInputs[i] = null;
                currentAmounts[i] = 0;
            }

            if (currentInputs[i] != lastCraftingInputs[i] || currentAmounts[i] != lastCraftingAmounts[i])
            {
                changed = true;
            }
        }

        if (changed)
        {
            for (int i = 0; i < 4; i++)
            {
                lastCraftingInputs[i] = currentInputs[i];
                lastCraftingAmounts[i] = currentAmounts[i];
            }
            CheckCrafting();
        }
    }

    /// <summary>
    /// 检查是否有匹配的配方
    /// </summary>
    void CheckCrafting()
    {
        currentRecipe = RecipeManager.instance.FindRecipe(lastCraftingInputs, lastCraftingAmounts);

        if (currentRecipe != null)
        {
            craftingOutputImage.sprite = Resources.Load<Sprite>("Image/Imgs/Block/" + currentRecipe.resultItem);
            craftingOutputImage.gameObject.SetActive(true);
        }
        else
        {
            craftingOutputImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 点击合成输出槽时调用
    /// </summary>
    public void OnCraftingOutputClick()
    {
        if (currentRecipe == null) return;
        if (craftingOutputImage == null || !craftingOutputImage.gameObject.activeSelf) return;

        Craft();
    }

    /// <summary>
    /// 执行合成
    /// </summary>
    void Craft()
    {
        // 消耗输入物品
        for (int i = 0; i < craftingInputSlots.Count; i++)
        {
            InventorySlot slot = craftingInputSlots[i];
            if (slot != null && slot.item != null)
            {
                string required = currentRecipe.inputItems[i];
                int requiredAmount = currentRecipe.inputAmounts[i];

                if (required != null && slot.item.itemName == required)
                {
                    slot.item.IncreaseAmmount(-requiredAmount);
                    if (slot.item.ammount <= 0)
                    {
                        Destroy(slot.item.gameObject);
                        slot.item = null;
                    }
                }
            }
        }

        // 生成产物
        InventoryItem item = CreateInventoryItem(currentRecipe.resultItem, currentRecipe.resultAmount, Vector3.zero, null);
        
        // 直接将产物设为拖拽状态
        draggedItem = item;
        if (item.slot) { item.slot.item = null; }
        item.slot = null;

        CheckCrafting();
    }

    InventoryItem CreateInventoryItem(string itemName, int amount, Vector3 position, InventorySlot slot)
    {
        GameObject newItem = Instantiate(itemPrefeb, parent: itemParent);
        Image img = newItem.GetComponent<Image>();
        Sprite dynamicIcon = BlockIconGenerator.Instance != null ? BlockIconGenerator.Instance.GetIcon(itemName) : null;
        if (dynamicIcon != null)
        {
            img.sprite = dynamicIcon;
        }
        else
        {
            img.sprite = Resources.Load<Sprite>("Image/Imgs/Block/" + itemName);
        }
        
        InventoryItem item = newItem.GetComponent<InventoryItem>();
        item.itemName = itemName;
        
        AddItemTriggers(item);
        item.SetAmmount(amount);
        
        if (slot != null)
        {
            slot.item = item;
            item.slot = slot;
            item.transform.position = slot.transform.position;
        }
        else if (position != Vector3.zero)
        {
            newItem.transform.position = position;
        }
        
        return item;
    }

    void ToggleInventory()
    {
        bool enable = !window.gameObject.activeSelf;
        window.gameObject.SetActive(enable);
        Cursor.visible = enable;
        isOpen = enable;
        if (enable)
        {
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void StartDrag(InventoryItem item, BaseEventData data)
    {
        PointerEventData pointerData = data as PointerEventData;
        if(pointerData.button == PointerEventData.InputButton.Right)
        {
            return;
        }

        draggedItem = item;
        if (item.slot) { item.slot.item = null; }
        item.slot = null;
    }

    /// <summary>
    /// 为物品添加拖拽事件触发器
    /// </summary>
    void AddItemTriggers(InventoryItem item)
    {
        EventTrigger trigger = item.gameObject.GetComponent<EventTrigger>();
        trigger.triggers.Clear();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { StartDrag(item.GetComponent<InventoryItem>(),eventData); });
        trigger.triggers.Add(entry);
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.Drag;
        entry.callback.AddListener((eventData) => { Drag(eventData); });
        trigger.triggers.Add(entry);
    }

    public void Drag(BaseEventData data)
    {
        PointerEventData pointData = (PointerEventData)data;
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            pointData.position,
            canvas.worldCamera,
            out position);
        if (draggedItem)
        {
            draggedItem.transform.position = canvas.transform.TransformPoint(position);
        }
    }

    public void Drop(InventoryItem item)
    {
        if (draggedItem == item) { draggedItem = null; }

        // 查找最近的普通背包槽
        float minDistance = 1000f;
        InventorySlot slot = null;
        foreach (InventorySlot s in slots)
        {
            float distance = Vector3.Distance(item.transform.position, s.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                slot = s;
            }
        }

        if (slot == null)
        {
            foreach (InventorySlot s in craftingInputSlots)
            {
                float distance = Vector3.Distance(item.transform.position, s.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    slot = s;
                }
            }
        }

        if (slot != null && slot.item)
        {
            if (slot.item.itemName == item.itemName)
            {
                if (slot.item.ammount + item.ammount <= 64)
                {
                    slot.item.IncreaseAmmount(item.ammount);
                    Destroy(item.gameObject);
                }
                else
                {
                    item.IncreaseAmmount(-(64 - slot.item.ammount));
                    slot.item.SetAmmount(64);
                }
            }
            else
            {
                slot = item.lastSlot;
                if (slot != null)
                {
                    slot.item = item;
                    item.slot = slot;
                    item.transform.position = slot.transform.position;
                }
            }
        }
        else if (slot != null)
        {
            slot.item = item;
            item.slot = slot;
            item.transform.position = slot.transform.position;
        }
    }

    public void Pickup(BlockType type)
    {
        InventorySlot targetSlot = null;
        
        foreach(InventorySlot slot in slots)
        {
            if (slot.item != null && slot.item.itemName == type.ToString() && slot.item.ammount < 64)
            {
                slot.item.IncreaseAmmount(1);
                return;
            }
            if (slot.item == null && targetSlot == null)
            {
                targetSlot = slot;
            }
        }

        CreateInventoryItem(type.ToString(), 1, Vector3.zero, targetSlot);
    }
}
