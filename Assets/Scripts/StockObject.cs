using UnityEngine;

public class StockObject : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private bool isPlaced;
    [SerializeField] public Rigidbody rig;

    private Vector3 originalScale;

    private void Awake()
    {
        if (rig == null)
            rig = GetComponent<Rigidbody>();

        originalScale = transform.lossyScale;
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
    }

    public void PickUp()
    {
        isPlaced = false;

        if (rig != null)
            rig.isKinematic = true;

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        RestoreScale();
    }

    public void MakePlace()
    {
        isPlaced = true;

        if (rig != null)
            rig.isKinematic = true;

        RestoreScale();
    }

    private void RestoreScale()
    {
        transform.localScale = new Vector3(
            originalScale.x / transform.parent.lossyScale.x,
            originalScale.y / transform.parent.lossyScale.y,
            originalScale.z / transform.parent.lossyScale.z
        );
    }

    public void Release()
    {

    }
}