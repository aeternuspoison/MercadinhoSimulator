using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Camera")]
    [SerializeField] private float lookSpeed = 120f;
    [SerializeField] private float minCameraAngle = -60f;
    [SerializeField] private float maxCameraAngle = 60f;
    [SerializeField] private Transform cameraTransform;

    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference interactAction;

    [Header("Player")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform handTransform;

    [Header("Interacao")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask whatIsStock;
    [SerializeField] private LayerMask whatIsShelf;

    [Header("Objeto Segurado")]
    [SerializeField] private StockObject holdPickup;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float throwForce = 5f;

    private float ySpeed;
    private float cameraVerticalRotation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleInteraction();
    }

    private void HandleLook()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        float horizontalLook =
            lookInput.x * lookSpeed * Time.deltaTime;

        float verticalLook =
            lookInput.y * lookSpeed * Time.deltaTime;

        transform.Rotate(0f, horizontalLook, 0f);

        cameraVerticalRotation -= verticalLook;

        cameraVerticalRotation = Mathf.Clamp(
            cameraVerticalRotation,
            minCameraAngle,
            maxCameraAngle
        );

        cameraTransform.localRotation =
            Quaternion.Euler(
                cameraVerticalRotation,
                0f,
                0f
            );
    }

    private void HandleMovement()
    {
        Vector2 moveInput =
            moveAction.action.ReadValue<Vector2>();

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 moveAmount =
            right * moveInput.x +
            forward * moveInput.y;

        if (moveAmount.magnitude > 1f)
            moveAmount.Normalize();

        moveAmount *= moveSpeed;

        HandleGravityAndJump(ref moveAmount);

        characterController.Move(
            moveAmount * Time.deltaTime
        );
    }

    private void HandleGravityAndJump(ref Vector3 moveAmount)
    {
        if (characterController.isGrounded)
        {
            if (ySpeed < 0f)
                ySpeed = -2f;

            if (jumpAction.action.WasPressedThisFrame())
                ySpeed = jumpForce;
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        moveAmount.y = ySpeed;
    }

    private void HandleInteraction()
    {
        if (Camera.main == null)
            return;

        Ray ray = Camera.main.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (holdPickup == null)
        {
            HandlePickup(ray);
            return;
        }

        HandlePlace(ray);
        HandleThrow();
    }

    private void HandlePickup(Ray ray)
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionRange,
            whatIsStock))
        {
            StockObject stock =
                hit.collider.GetComponentInParent<StockObject>();

            if (stock == null)
                return;

            holdPickup = stock;

            holdPickup.transform.SetParent(
                holdPoint,
                true
            );

            holdPickup.PickUp();
        }
    }

    private void HandlePlace(Ray ray)
    {
        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            interactionRange,
            whatIsShelf))
        {
            ShelfSpaceController shelf =
                hit.collider.GetComponentInParent<
                    ShelfSpaceController>();

            if (shelf == null)
                return;

            shelf.PlaceStock(holdPickup);

            if (holdPickup.isPlaced)
                holdPickup = null;
        }
    }

    private void HandleThrow()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame)
            return;


        StockObject stock = holdPickup;

        stock.transform.SetParent(null, true);

        stock.Throw();

        if (stock.rig != null)
        {
            stock.rig.AddForce(
                cameraTransform.forward * throwForce,
                ForceMode.Impulse
            );
        }

        holdPickup = null;
    }
}