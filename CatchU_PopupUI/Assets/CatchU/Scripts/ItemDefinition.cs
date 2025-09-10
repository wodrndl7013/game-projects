using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CatchU/Item Definition")]
public class ItemDefinition : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public Rarity rarity;
    public int price;
}
