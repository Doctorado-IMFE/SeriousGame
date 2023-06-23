/* using System.Collections;
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

    public KeyCode jumpKey = KeyCode.Space; // Tecla que activa el salto y el Wall Kick

    private bool canJump = true; // Indica si el personaje puede realizar un salto
    private bool isWallKickEnabled = false; // Indica si el personaje puede realizar Wall Kick

    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    private bool isGrounded;
    private bool isTouchingWall;

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
        CheckWallTouch();
        Movement();
        Jump();
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
        if (isGrounded && Input.GetKeyDown(jumpKey) && canJump)
        {
            myAnimator.SetTrigger("Jump");
            myRigidBody2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
            canJump = false;
        }
        else if (isWallKickEnabled && Input.GetKeyDown(jumpKey))
        {
            myAnimator.SetTrigger("Jump");
            myRigidBody2D.AddForce(new Vector2(0, jumpSpeed), ForceMode2D.Impulse);
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            myAnimator.SetBool("Land", false);
            canJump = true;
        }
    }

    void CheckWallTouch()
    {
        isTouchingWall = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, wallLayer);

        if (isTouchingWall)
        {
            isWallKickEnabled = true;
        }
        else
        {
            isWallKickEnabled = false;
        }
    }
}

 */



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
    [SerializeField] float groundDistance = 0.6f; // Puedes ajustar este valor según sea necesario

    //WALL KICK
    public float wallKickForce = 5f; // Fuerza del impulso de Wall Kick
    public KeyCode wallKickKey = KeyCode.Space; // Tecla que activa el Wall Kick
    public bool isWallKickEnabled = false; // Indica si el personaje puede realizar Wall Kick
    

    [SerializeField] Transform groundCheck;
    //[SerializeField] float groundCheckRadius = 0.2f;
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
    // Crea un raycast hacia abajo desde la posición del personaje
    RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, groundDistance, groundLayer);

    // Dibuja el raycast en el editor de Unity
    Debug.DrawRay(transform.position, -Vector2.up * groundDistance, Color.red);

    // Si el raycast golpea algo, entonces el personaje está en el suelo
    isGrounded = hit.collider != null;

    if (isGrounded)
    {
        myAnimator.SetBool("Land", false);
    }
    /* isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    if (isGrounded)
    {
        myAnimator.SetBool("Land", false);
    } */
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