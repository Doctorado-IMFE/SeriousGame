using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float jumpSpeed;
    [SerializeField] float walkSpeed = 3f;
    [SerializeField] float runSpeed = 6f;
    [SerializeField] float speedRampUpTime = 1f;
    private float speedRampTimer = 0f;

    //WALL KICK
    public float wallKickForce = 5f; // Fuerza del impulso de Wall Kick
    public KeyCode wallKickKey = KeyCode.Space; // Tecla que activa el Wall Kick
    public bool isWallKickEnabled = false; // Indica si el personaje puede realizar Wall Kick
    

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;
    public bool isGrounded;



    Animator myAnimator;
    Rigidbody2D myRigidBody2D;

    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidBody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckGrounded();
        Movement();
        Jump();
        //WALL KICK
        if (Input.GetKeyDown(wallKickKey) && isWallKickEnabled)
        {
            // Realizar Wall Kick
            PerformWallKick();
        }        
    }

    void Movement()
    {
        float CurrentMovement = Input.GetAxis("Horizontal");

        if (CurrentMovement != 0)
        {
            speedRampTimer += Time.deltaTime;
            float currentSpeed = Mathf.Lerp(walkSpeed, runSpeed, speedRampTimer / speedRampUpTime);

            // Set your speed parameter here.
            myAnimator.SetFloat("Speed", currentSpeed);

            transform.Translate(new Vector2(CurrentMovement * currentSpeed * Time.deltaTime, 0));
        }
        else
        {
            speedRampTimer = 0f;
            myAnimator.SetFloat("Speed", 0f);
        }
    }


void Jump()
{
    if (isGrounded && Input.GetKeyDown(KeyCode.Space))
    {
        myAnimator.SetTrigger("Jump");
        myRigidBody2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
    }

    if (myRigidBody2D.velocity.y < 0)
    {
        myAnimator.SetBool("Land", true);
    }
    else
    {
        myAnimator.SetBool("Land", false);
    }
}

private void PerformWallKick()
    {
        // Aplicar impulso en dirección opuesta a la pared utilizando el componente Rigidbody2D
        Vector2 kickDirection = -transform.right; // Cambiar a la dirección adecuada según el diseño de tu juego
        myRigidBody2D.velocity = kickDirection * wallKickForce;
    }

void CheckGrounded()
{
    isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    if (isGrounded)
    {
        myAnimator.SetBool("Land", false);
    }
}

 private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar si colisiona con una pared
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWallKickEnabled = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Desactivar Wall Kick al salir de la colisión con la pared
        if (collision.gameObject.CompareTag("Wall"))
        {
            isWallKickEnabled = false;
        }
    }

}
