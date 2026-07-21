using UnityEngine;

[CreateAssetMenu(
    fileName = "NewItem",
    menuName = "Inventory/Item Data"
)]
public class ItemData : ScriptableObject
{
    [Header("아이템 정보")]
    public string itemName;

    [TextArea]
    public string description;

    public Sprite icon;
}