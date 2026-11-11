using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerInspector : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Скорость движения игрока")]
    public float speed = 5f;

    [Tooltip("Высота прыжка")]
    public float jumpHeight = 1.5f;

    [Tooltip("Гравитация")]
    public float gravity = -9.81f;

    [Header("Camera Settings")]
    [Tooltip("Камера от первого лица")]
    public Camera camera1st;

    [Tooltip("Камера от третьего лица")]
    public Camera camera3rd;

    [Tooltip("Смещение TPS камеры относительно игрока")]
    public Vector3 tpsOffset = new Vector3(0, 2, -4);

    [Tooltip("Плавность движения TPS камеры")]
    public float cameraSmoothSpeed = 5f;

    [Tooltip("Чувствительность мыши")]
    public float mouseSensitivity = 2f;

    [Tooltip("Максимальный угол вертикального обзора")]
    public float maxLookAngle = 80f;

    [Header("Control Settings")]
    [Tooltip("Можно ли управлять игроком")]
    public bool canControl = true;

    [Header("Scene Transition")]
    [SerializeField] private string spawnPointTag = "Respawn";

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private float xRotation = 0f;
    private bool isFirstPerson = true;
    private int jumpCount = 0;

    public int JumpCount => jumpCount;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Start()
    {
        if (camera1st == null || camera3rd == null)
        {
            enabled = false;
            return;
        }

        camera1st.enabled = true;
        camera3rd.enabled = false;
        camera3rd.transform.localPosition = tpsOffset;

        MoveToSpawnPoint();
        SetGameplayMode();

        GlobalPauseMenu pause = FindFirstObjectByType<GlobalPauseMenu>();
        if (pause != null)
            pause.UpdateJumpCounter(jumpCount);
    }

    void Update()
    {
        if (!canControl)
            return;

        HandleMovement();
        HandleMouseLook();
        HandleCameraSwitch();
        HandleTPSCamera();
    }

    private void MoveToSpawnPoint()
    {
        GameObject spawnPoint = GameObject.FindGameObjectWithTag(spawnPointTag);

        if (spawnPoint == null)
        {
            return;
        }

        if (controller != null)
            controller.enabled = false;

        transform.position = spawnPoint.transform.position;
        transform.rotation = spawnPoint.transform.rotation;

        if (controller != null)
            controller.enabled = true;
    }

    void HandleMovement()
    {
        if (controller == null)
            return;

        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        controller.Move(move * speed * Time.deltaTime);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpCount++;

            GlobalPauseMenu pause = FindFirstObjectByType<GlobalPauseMenu>();
            if (pause != null)
                pause.UpdateJumpCounter(jumpCount);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);

        if (isFirstPerson && camera1st != null)
            camera1st.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleCameraSwitch()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            isFirstPerson = !isFirstPerson;

            if (camera1st != null)
                camera1st.enabled = isFirstPerson;

            if (camera3rd != null)
                camera3rd.enabled = !isFirstPerson;

            if (!isFirstPerson && camera3rd != null)
                camera3rd.transform.localPosition = tpsOffset;
        }
    }

    void HandleTPSCamera()
    {
        if (!isFirstPerson && camera3rd != null)
        {
            Quaternion rotation = Quaternion.Euler(xRotation, transform.eulerAngles.y, 0f);
            Vector3 desiredPosition = transform.position + rotation * tpsOffset;

            camera3rd.transform.position = Vector3.Lerp(
                camera3rd.transform.position,
                desiredPosition,
                Time.deltaTime * cameraSmoothSpeed
            );

            camera3rd.transform.LookAt(transform.position + Vector3.up * 1.6f);
        }
    }

    public void SetGameplayMode()
    {
        canControl = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void SetUIMode()
    {
        canControl = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResetPlayerState()
    {
        canControl = true;
        velocity = Vector3.zero;
        xRotation = 0f;
        isFirstPerson = true;
        jumpCount = 0;

        if (camera1st != null)
        {
            camera1st.enabled = true;
            camera1st.transform.localRotation = Quaternion.identity;
        }

        if (camera3rd != null)
        {
            camera3rd.enabled = false;
            camera3rd.transform.localPosition = tpsOffset;
        }

        if (GlobalPauseMenu.Instance != null)
            GlobalPauseMenu.Instance.UpdateJumpCounter(jumpCount);
    }
}