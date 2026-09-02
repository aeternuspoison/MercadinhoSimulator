using UnityEngine;

public class ShelfSpaceController : MonoBehaviour
{
    public StockInfo info;
    public int amountOnShelf;

    public void PlaceStock(StockObject objectToPlace)
    {
        if (objectToPlace == null)
            return;

        if (amountOnShelf == 0)
        {
            info = objectToPlace.info;
        }
        else if (info.Name != objectToPlace.info.Name)
        {
            return;
        }

        objectToPlace.transform.SetParent(transform);

        objectToPlace.MakePlaced();

        amountOnShelf++;
    }

    public StockObject GetStock()
    {
        return null;
    }
}