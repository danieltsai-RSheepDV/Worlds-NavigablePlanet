using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Modified from Sebastian Lague's First Person Controller: Spherical Worlds tutorial
 * Link: https://www.youtube.com/watch?v=TicipSVT-T8&t=1s
 */

public class PlayerController : MonoBehaviour
{
    private bool m_firstPerson = true;
    public bool firstPerson
    {
        get
        {
            return m_firstPerson;
        }
        set
        {
            if (value == m_firstPerson)
                return;
            m_firstPerson = value;
            TogglePlayerView(m_firstPerson);
        }
    }

    [SerializeField] private float playerHeight = 2f;

    public float mouseSensitivityX = 250f;
    public float mouseSensitivityY = 250f;
    public float walkSpeed = 8f;
    public float jumpForce = 220f;
    public LayerMask groundedMask;

    [SerializeField] private Vector3 firstPosition = new Vector3(0, 0.4f, 0);
    [SerializeField] private Vector3 firstRotation = Vector3.zero;
    [SerializeField] private Vector3 thirdPosition = new Vector3(0, 8f, 0);
    [SerializeField] private Vector3 thirdRotation = new Vector3(90f, 0, 0);

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
        // camera rotation
        transform.Rotate(Vector3.up * rotateX * Time.deltaTime * mouseSensitivityX);
        if (firstPerson)
        {
            verticalLookRotation += rotateY * Time.deltaTime * mouseSensitivityY;
            verticalLookRotation = Mathf.Clamp(verticalLookRotation, -40, 40);
            cameraT.localEulerAngles = Vector3.left * verticalLookRotation;
        }

        // player movement
        Vector3 targetMoveAmount = moveDir * walkSpeed;
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);

        // is player touching ground
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
        // player movement
        // GetComponent<Rigidbody>().MovePosition(GetComponent<Rigidbody>().position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        transform.position += (transform.TransformDirection(moveDir) * Time.deltaTime);
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

    private void OnViewChange(InputValue viewValue)
    {
        if (viewValue.Get<float>() > 0)
            firstPerson = !firstPerson;
    }

    private void TogglePlayerView(bool firstPersonView)
    {
        if (firstPersonView)
        {
            cameraT.localPosition = firstPosition;
            cameraT.localRotation = Quaternion.Euler(firstRotation);
        }
        else
        {
            cameraT.localPosition = thirdPosition;
            cameraT.localRotation = Quaternion.Euler(thirdRotation);
        }
    }
}
