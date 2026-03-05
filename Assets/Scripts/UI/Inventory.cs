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

    public bool isOpen = false;

    InventoryItem draggedItem;

    // Update is called once per frame
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

        if (slot.item)
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
                slot.item = item;
                item.slot = slot;
                item.transform.position = slot.transform.position;
            }
        }
        else
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
            if(slot.item== null)
            {
                if(targetSlot == null) targetSlot = slot;
                continue;
            }
            if (slot.item.itemName == type.ToString() && slot.item.ammount < 64)
            {
                slot.item.IncreaseAmmount(1);
                return;
            }
        }

        GameObject newItem = Instantiate(itemPrefeb, parent: itemParent);
        Image img=newItem.GetComponent<Image>();
        img.sprite = Resources.Load<Sprite>("Image/Imgs/Block/" + type.ToString());
        InventoryItem item = newItem.GetComponent<InventoryItem>();
        item.itemName = type.ToString();

        AddItemTriggers(item);
        item.SetAmmount(1);
        targetSlot.item = item;
        item.slot = targetSlot;
        item.transform.position = targetSlot.transform.position;
    }
}
