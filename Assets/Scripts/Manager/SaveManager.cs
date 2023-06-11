using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    //private List<Item> inventory = new List<Item>();

    //// Function to save the inventory data
    //public void SaveInventory()
    //{
    //    string inventoryJson = JsonUtility.ToJson(inventory);
    //    PlayerPrefs.SetString("InventoryData", inventoryJson);
    //    PlayerPrefs.Save();

    //    Debug.Log("Inventory saved.");
    //}

    //// Function to load the inventory data
    //public void LoadInventory()
    //{
    //    string inventoryJson = PlayerPrefs.GetString("InventoryData", "");

    //    if (!string.IsNullOrEmpty(inventoryJson))
    //    {
    //        inventory = JsonUtility.FromJson<List<Item>>(inventoryJson);
    //    }
    //    else
    //    {
    //        inventory = new List<Item>();
    //    }

    //    Debug.Log("Inventory loaded.");
    //}
}
