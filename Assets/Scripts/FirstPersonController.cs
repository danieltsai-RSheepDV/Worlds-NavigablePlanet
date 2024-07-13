using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField] private float playerHeight = 2f;

    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float walkSpeed = 8f;
    public float jumpForce = 220f;
    public LayerMask groundedMask;

    Transform cameraT;
    float verticalLookRotation;

    Vector3 moveDir;
    Vector3 moveAmount;
    Vector3 smoothMoveVelocity;

    float rotateX = 0f;
    float rotateY = 0f;

    bool grounded;

    // Start is called before the first frame update
    void Start()
    {
        cameraT = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up * rotateX * Time.deltaTime * mouseSensitivityX);
        verticalLookRotation += rotateY * Time.deltaTime * mouseSensitivityY;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -40, 40);
        cameraT.localEulerAngles = Vector3.left * verticalLookRotation;

        Vector3 targetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

        grounded = false;
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, (playerHeight / 2) + 0.1f, groundedMask))
        {
            grounded = true;
        }
    }

    void FixedUpdate()
    {
        GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        float xMove = movementVector.x;
        float zMove = movementVector.y;

        moveDir = new Vector3(xMove, 0, zMove).normalized;
    }

    private void OnRotate(InputValue rotateValue)
    {
        rotateX = rotateValue.Get<Vector2>().x;
        rotateY = rotateValue.Get<Vector2>().y;
    }

    private void OnJump(InputValue jumpValue)
    {
        if (jumpValue.Get<float>() > 0)
        {
            if (grounded)
            {
                GetComponent<Rigidbody>().AddForce(transform.up * jumpForce);
            }
        }
    }
}
