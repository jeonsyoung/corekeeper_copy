using System;
using UnityEngine;

[Serializable]
public class DropItem
{
    public ItemData item;

    [Min(1)]
    public int minAmount = 1;

    [Min(1)]
    public int maxAmount = 2;

    [Range(0, 100)]
    public int chance = 100;
}