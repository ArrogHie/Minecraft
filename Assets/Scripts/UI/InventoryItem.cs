using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public string itemName;
    public InventorySlot slot;
    public Text ammountText;
    public int ammount;

    [HideInInspector]
    public InventorySlot lastSlot;

    private void Update()
    {
        if (slot) { lastSlot = slot; }
    }

    public void SetAmmount(int newAmmount)
    {
        ammount = newAmmount;
        ammountText.text = ammount.ToString();
    }

    public void IncreaseAmmount(int increaseBy)
    {
        ammount += increaseBy;
        ammountText.text = ammount.ToString();
    }
}
