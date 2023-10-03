using System.Collections.Generic;

[System.Serializable]
public class RawInventoryItem
{
    public string name;
    public string quantity;
}

[System.Serializable]
public class RawSaveData
{
    public List<RawInventoryItem> inventoryItems;
}