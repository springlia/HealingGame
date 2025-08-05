using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI countText;
    
    public void AddItem(Sprite sprite, int count)
    {
        icon.sprite = sprite;
        icon.enabled = true;
        countText.text = countText.text = count.ToString();
    }

    public void RemoveItem()
    {
        icon.sprite = null;
        icon.enabled = false;
        countText.text = "";
    }

}
