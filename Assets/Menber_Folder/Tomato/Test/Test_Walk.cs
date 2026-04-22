using UnityEngine;

public class Test_Walk : MonoBehaviour
{
    [Header("移動")]
    public float moveSpeed = 5f;
    public float gravity = -9.8f;

    [Header("マウス視点")]
    public float mouseSensitivity = 100f;
    public Transform cameraTransform;

    float xRotation = 0f;
    Vector3 velocity;
    CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        // マウス固定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
        Move();
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal"); // A,D
        float z = Input.GetAxis("Vertical");   // W,S

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 重力
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
