using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Variaveis Comuns")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float lookSpeed = 120f;
    [SerializeField] private float minCameraAngle = -60f;
    [SerializeField] private float maxCameraAngle = 60f;

    [Header("Variaveis de Controle")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private InputActionReference interactAction;

    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform handTransform;
    [SerializeField] private LayerMask whatIsStock;
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private GameObject holdPickup;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float throwForce;

    private float ySpeed;
    private float cameraVerticalRotation;

    private GameObject heldObject;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
    }

    private void HandleLook()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        float horizontalLook = lookInput.x * lookSpeed * Time.deltaTime;
        float verticalLook = lookInput.y * lookSpeed * Time.deltaTime;

        transform.Rotate(0f, horizontalLook, 0f);

        cameraVerticalRotation -= verticalLook;

        cameraVerticalRotation = Mathf.Clamp(
            cameraVerticalRotation,
            minCameraAngle,
            maxCameraAngle
        );

        cameraTransform.localRotation =
            Quaternion.Euler(cameraVerticalRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();

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

        characterController.Move(moveAmount * Time.deltaTime);

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        RaycastHit hit;


        if(holdPickup == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStock))
                {
                    Debug.Log("I see a pickup!");
                    holdPickup = hit.collider.gameObject;
                    holdPickup.transform.SetParent(holdPoint);
                    holdPickup.transform.localPosition = Vector3.zero;
                    holdPickup.transform.localRotation = quaternion.identity;

                    holdPickup.GetComponent<Rigidbody>().isKinematic = true;
                }
            }
        }
        else
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                Rigidbody pickupRB = holdPickup.GetComponent<Rigidbody>();
                pickupRB.isKinematic = false;
                pickupRB.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);

                holdPickup.transform.SetParent(null);
                holdPickup = null;
            }
        }
    }
}