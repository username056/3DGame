using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float aimMoveSpeed = 2.5f;
    [SerializeField] private float sprintMultiplier = 1.5f;

    [Header("Look Settings")]
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float maxLookAngle = 70f;

    private CharacterController cc;
    private Animator animator;
    private Transform cam;
    private float xRotation = 0f;

    private void Start()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleLook();
        HandleMove();
        HandleActions();
    }

    private void HandleLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        xRotation = Mathf.Clamp(xRotation - mouseY, -maxLookAngle, maxLookAngle);
        cam.localEulerAngles = new Vector3(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleMove()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v);
        Vector3 direction = transform.TransformDirection(input).normalized;

        bool isAiming = Input.GetMouseButton(1);
        bool sprint = Input.GetKey(KeyCode.LeftShift) && !isAiming;
        float baseSpeed = isAiming ? aimMoveSpeed : moveSpeed;
        float currentSpeed = baseSpeed * (sprint ? sprintMultiplier : 1f);

        Vector3 velocity = direction * currentSpeed;
        cc.Move(velocity * Time.deltaTime);

        animator.SetFloat("Speed", input.magnitude);
        animator.SetFloat("Horizontal", h);
        animator.SetFloat("Vertical", v);
        animator.SetBool("isSprinting", sprint);
    }

    private void HandleActions()
    {
        bool isAiming = Input.GetMouseButton(1);
        animator.SetBool("isAiming", isAiming);
        if (Input.GetMouseButtonUp(1))
            animator.SetTrigger("FireArrow");

        if (Input.GetKeyDown(KeyCode.LeftControl))
            animator.SetTrigger("Roll");

        if (Input.GetKeyDown(KeyCode.Q))
            animator.SetTrigger("MeleeAttack");

        bool isBlocking = Input.GetKey(KeyCode.E);
        animator.SetBool("isBlocking", isBlocking);
    }
}
