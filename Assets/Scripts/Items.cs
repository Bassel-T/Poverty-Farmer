using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items {

    public string itemName;
    public int index;
    public int price;
    public string type;
    public GameObject prefab;
    public double multiplier;

    public Items(string itemName, int index, int price, string type, GameObject prefab, double multiplier) {
        this.itemName = itemName;
        this.index = index;
        this.price = price;
        this.type = type;
        this.prefab = prefab;
        this.multiplier = multiplier;
    }

}
