using UnityEngine;

[System.Serializable]
public class StockInfo
{
    public string Name;

    public enum StockType
    {
        milk,
        cereal,
        bigDrink,
        chipsTube,
        fruitsLarge,
    }

    public StockType typeOfStock;
}
