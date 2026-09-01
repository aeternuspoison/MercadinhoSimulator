using UnityEngine;

public class StockObject : MonoBehaviour
{
     public StockInfo info;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private bool isPlaced;
    [SerializeField] public Rigidbody rig;

    private Vector3 originalWorldScale;

    private void Awake()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody>();

        // Guarda o tamanho REAL do objeto no mundo
        originalWorldScale = transform.lossyScale;
    }

    private void Update()
    {
        if (!isPlaced)
            return;

        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            Vector3.zero,
            moveSpeed * Time.deltaTime
        );

        transform.localRotation = Quaternion.Slerp(
            transform.localRotation,
            Quaternion.identity,
            moveSpeed * Time.deltaTime
        );

        KeepOriginalScale();
    }

    public void PickUp()
    {
        isPlaced = false;

        if (rig != null)
            rig.isKinematic = true;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        KeepOriginalScale();
    }

    public void Release()
    {
        isPlaced = false;

        if (rig != null)
            rig.isKinematic = false;

        KeepOriginalScale();
    }

    public void MakePlace()
    {
        isPlaced = true;

        if (rig != null)
            rig.isKinematic = true;

        KeepOriginalScale();
    }

    public void Throw()
    {
        isPlaced = false;

        if (rig != null)
            rig.isKinematic = false;

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