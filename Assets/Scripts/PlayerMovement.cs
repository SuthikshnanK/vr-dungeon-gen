using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CharacterController))]
[RequireComponent (typeof(GunController))]
public class PlayerMovement : MonoBehaviour
{

    public CharacterController controller;
    public Animator anim;
    public Transform groundCheck;
    public float groundRadius = 0.4f;
    public LayerMask groundMask;

    public float speed = 12f;
    public float gravity = -9.81f;
    public float gravityMult = 2f;
    public float jumpHeight = 3f;

    GunController gunController;

    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        gunController = GetComponent<GunController>();
    }


    // Update is called once per frame
    void Update()
    {

        // Grounded Check - Jumping
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        if (move.magnitude> 0.5f)
        {
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
        }

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity * gravityMult);
        }

        velocity.y += gravity * gravityMult * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        // GunController
        if (Input.GetMouseButton(0))
        {
            gunController.Shoot();
        }

        
    }
}
