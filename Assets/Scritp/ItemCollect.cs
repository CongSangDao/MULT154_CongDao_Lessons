using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ItemCollect : NetworkBehaviour
{
    private Dictionary<Item.VegetableType, int> ItemInventory = new Dictionary<Item.VegetableType, int>();

    public delegate void CollecItem(Item.VegetableType item);
    public static event CollecItem ItemCollected;

    Collider itemCollider = null;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Item.VegetableType item in System.Enum.GetValues(typeof(Item.VegetableType)))
        {
            ItemInventory.Add(item, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }
        if (itemCollider && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Space bar and item collected");
            Item item = itemCollider.gameObject.GetComponent<Item>();
            AddToInventory(item);
            ItemCollected?.Invoke(item.typeOfVeggie);
            PrintInventory();

            CmdItemCollected(item.typeOfVeggie);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (other.CompareTag("Item"))
        {
            itemCollider = other;

        }
    }


    [Command]
    void CmdItemCollected(Item.VegetableType itemType)
    {
        Debug.Log("CommandItemCollecte:" + itemType);
        RpcItemCollected(itemType);
    }

    [ClientRpc]
    void RpcItemCollected(Item.VegetableType itemType)
    {
        ItemCollected?.Invoke(itemType);
    }
    private void OnTriggerExit(Collider other)
    {
        if (!isLocalPlayer) { return; }

        if (other.CompareTag("Item"))
        {
            itemCollider = null;
        }
    }

    private void AddToInventory(Item item)
    {
        ItemInventory[item.typeOfVeggie]++;
    }

    private void PrintInventory()
    {
        string output = "";

        foreach (KeyValuePair<Item.VegetableType, int> kvp in ItemInventory)
        {
            output += string.Format("{0}: {1}", kvp.Key, kvp.Value);
        }

        Debug.Log(output);
    }
}
