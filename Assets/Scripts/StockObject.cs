using UnityEngine;

public class StockObject : MonoBehaviour
{
    public StockInfo info;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] public bool isPlaced;
    [SerializeField] public Rigidbody rig;
    [SerializeField] private Collider col;

    private Vector3 originalWorldScale;
    private Vector3 targetLocalPosition;

    private void Awake()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody>();

        if (col == null)
            col = GetComponent<Collider>();

        originalWorldScale = transform.lossyScale;
    }

    private void Update()
    {
        if (!isPlaced)
            return;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            targetLocalPosition,
            moveSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            Quaternion.identity,
            moveSpeed * Time.deltaTime
        );

        KeepOriginalScale();
    }

    public void SetTargetLocalPosition(Vector3 localPos)
    {
        targetLocalPosition = localPos;
    }

    public void PickUp()
    {
        isPlaced = false;
        if (col != null) col.enabled = true;
        if (rig != null) rig.isKinematic = true;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        KeepOriginalScale();
    }

    public void Release()
    {
        isPlaced = false;
        if (col != null) col.enabled = true;
        if (rig != null) rig.isKinematic = false;
        KeepOriginalScale();
    }

    public void MakePlaced()
    {
        isPlaced = true;
        if (rig != null) rig.isKinematic = true;
        if (col != null) col.enabled = false;

        KeepOriginalScale();
    }

    public void Throw()
    {
        isPlaced = false;
        if (col != null) col.enabled = true;
        if (rig != null) rig.isKinematic = false;
        KeepOriginalScale();
    }

    private void KeepOriginalScale()
    {
        if (transform.parent == null)
        {
            transform.localScale = originalWorldScale;
            return;
        }
        Vector3 parentScale = transform.parent.lossyScale;
        transform.localScale = new Vector3(
            originalWorldScale.x / parentScale.x,
            originalWorldScale.y / parentScale.y,
            originalWorldScale.z / parentScale.z
        );
    }
}