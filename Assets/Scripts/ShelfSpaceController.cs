using UnityEngine;

public class ShelfSpaceController : MonoBehaviour
{
    public StockInfo info;
    public int amountOnShelf;

    [SerializeField] private float itemSpacingX = 0.4f;
    [SerializeField] private float itemSpacingZ = 0.4f;

    private BoxCollider shelfCollider;

    private void Awake()
    {
        shelfCollider = GetComponent<BoxCollider>();
        if (shelfCollider == null)
        {
            shelfCollider = GetComponentInChildren<BoxCollider>();
        }
    }

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

        Vector3 colCenter = shelfCollider != null ? shelfCollider.center : Vector3.zero;
        Vector3 colSize = shelfCollider != null ? shelfCollider.size : Vector3.one;

        int maxItemsPerRow = Mathf.Max(1, Mathf.FloorToInt(colSize.x / itemSpacingX));

        int rowIndex = amountOnShelf / maxItemsPerRow;
        int columnIndex = amountOnShelf % maxItemsPerRow;

        float startX = colCenter.x - (colSize.x * 0.5f) + (itemSpacingX * 0.5f);
        float startZ = colCenter.z - (colSize.z * 0.5f) + (itemSpacingZ * 0.5f);
        float baseY = colCenter.y - (colSize.y * 0.5f);

        float posX = startX + (columnIndex * itemSpacingX);
        float posZ = startZ + (rowIndex * itemSpacingZ);

        float itemBottomOffsetY = 0f;
        Collider itemCol = objectToPlace.GetComponent<Collider>();
        if (itemCol is BoxCollider itemBox)
        {
            itemBottomOffsetY = itemBox.center.y - (itemBox.size.y * 0.5f);
        }

        float posY = baseY - itemBottomOffsetY;

        Vector3 targetLocalPos = new Vector3(posX, posY, posZ);

        objectToPlace.SetTargetLocalPosition(targetLocalPos);
        objectToPlace.MakePlaced();

        amountOnShelf++;
    }

    public StockObject GetStock()
    {
        return null;
    }
}