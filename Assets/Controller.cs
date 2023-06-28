using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float speedRampUpTime = 1f;
    [SerializeField] private float groundDistance = 0.6f;
    [SerializeField] private float wallCheckDistance = 0.5f;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    public float WallKickForce { get; private set; } = 2f;
    public KeyCode WallKickKey { get; private set; } = KeyCode.Space;
    public bool IsWallKickEnabled { get; private set; }
    public bool IsTouchingWall { get; private set; }
    public bool IsGrounded { get; private set; }

    private Animator myAnimator;
    private Rigidbody2D myRigidBody2D;
    private SpriteRenderer mySpriteRenderer;
    private float speedRampTimer = 0f;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidBody2D = GetComponent<Rigidbody2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        CheckGrounded();
        CheckIfTouchingWall();
        Movement();
        Jump();

        if (Input.GetKey(WallKickKey) && IsTouchingWall)
        {
            PerformWallKick();
        }        
    }

    private void Movement()
    {
        float currentMovement = Input.GetAxis("Horizontal");

        if (currentMovement != 0 && !myAnimator.GetCurrentAnimatorStateInfo(0).IsName("WallKick"))
        {
            speedRampTimer += Time.deltaTime;
            float currentSpeed = Mathf.Lerp(walkSpeed, runSpeed, speedRampTimer / speedRampUpTime);
            myAnimator.SetFloat("Speed", currentSpeed);
            transform.Translate(new Vector2(currentMovement * currentSpeed * Time.deltaTime, 0));

            // Flip the sprite in the direction of movement
            mySpriteRenderer.flipX = currentMovement < 0;
        }
        else
        {
            speedRampTimer = 0f;
            myAnimator.SetFloat("Speed", 0f);
        }
    }

    private void Jump()
    {
        if (IsGrounded && Input.GetKeyDown(KeyCode.Space))
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
        if (!IsGrounded && IsTouchingWall && Input.GetKey(WallKickKey) && ((Input.GetAxis("Horizontal") > 0 && Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer)) || (Input.GetAxis("Horizontal") < 0 && Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer))))
        {
            // Check which side the wall is on
            if (Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer))
            {
                // Apply force in opposite direction to wall (left) using Rigidbody2D
                myRigidBody2D.AddForce(new Vector2(-0.5f, 1.5f));

                // Trigger Wall Kick animation
                myAnimator.SetTrigger("WallKick");
                // Stop the jump animation
                myAnimator.SetBool("Jump", false);
                myAnimator.SetBool("Land", false);
                myAnimator.SetFloat("Speed", 0f); // Stop run animation
            }
            else if (Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer))
            {
                // Apply force in opposite direction to wall (right) using Rigidbody2D
                myRigidBody2D.AddForce(new Vector2(0.5f, 1.5f));

                // Trigger Wall Kick animation
                myAnimator.SetTrigger("WallKick");
                // Stop the jump animation
                myAnimator.SetBool("Jump", false);
                myAnimator.SetBool("Land", false);
                myAnimator.SetFloat("Speed", 0f); // Stop run animation
            } else  myRigidBody2D.AddForce(new Vector2(0.0f, 0.0f));
        }
        else  myRigidBody2D.AddForce(new Vector2(0.0f, 0.0f));
    }


    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, groundDistance, groundLayer);
        Debug.DrawRay(transform.position, -Vector2.up * groundDistance, Color.red);
        IsGrounded = hit.collider != null;

        if (IsGrounded)
        {
            myAnimator.SetBool("Land", false);
        }
    }

    private void CheckIfTouchingWall()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);
        Debug.DrawRay(transform.position, Vector2.right * wallCheckDistance, Color.blue);
        Debug.DrawRay(transform.position, Vector2.left * wallCheckDistance, Color.blue);
        IsTouchingWall = hitRight.collider != null || hitLeft.collider != null;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            IsWallKickEnabled = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            IsWallKickEnabled = false;
        }
    }
}



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
    [SerializeField] float groundDistance = 0.6f; // Puedes ajustar este valor según sea necesario

    //WALL KICK
    public float wallKickForce = 5f; // Fuerza del impulso de Wall Kick
    public KeyCode wallKickKey = KeyCode.Space; // Tecla que activa el Wall Kick
    public bool isWallKickEnabled = false; // Indica si el personaje puede realizar Wall Kick
    public bool isTouchingWall = false; // Indica si el personaje está tocando una pared
    [SerializeField] float wallCheckDistance = 0.5f; // Puedes ajustar este valor según sea necesario
    [SerializeField] LayerMask wallLayer; // Capa de las paredes

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
        CheckIfTouchingWall(); // Don't forget to check if touching wall
        Movement();
        Jump();
        //WALL KICK
        if (Input.GetKeyDown(wallKickKey) && isTouchingWall)
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
        // Comprobar en qué lado está la pared
    if (Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer))
    {
        // Aplicar impulso en la dirección opuesta a la pared (izquierda) utilizando el componente Rigidbody2D
        myRigidBody2D.AddForce(new Vector2(-wallKickForce, wallKickForce));

        // Activar la animación de Wall Kick
        myAnimator.SetTrigger("WallKick");
        // Desactivar la animación de salto
        myAnimator.SetBool("Jump", false);
    }
    else if (Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer))
    {
        // Aplicar impulso en la dirección opuesta a la pared (derecha) utilizando el componente Rigidbody2D
        myRigidBody2D.AddForce(new Vector2(wallKickForce, wallKickForce));

        // Activar la animación de Wall Kick
        myAnimator.SetTrigger("WallKick");
        // Desactivar la animación de salto
        myAnimator.SetBool("Jump", false);
    }
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
}

private void CheckIfTouchingWall()
    {
        // Crea un raycast hacia la derecha desde la posición del personaje
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDistance, wallLayer);
        // Crea un raycast hacia la izquierda desde la posición del personaje
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, Vector2.left, wallCheckDistance, wallLayer);

        // Dibuja los raycasts en el editor de Unity
        Debug.DrawRay(transform.position, Vector2.right * wallCheckDistance, Color.blue);
        Debug.DrawRay(transform.position, Vector2.left * wallCheckDistance, Color.blue);

        // Si alguno de los raycasts golpea una pared, entonces el personaje está tocando una pared
        isTouchingWall = hitRight.collider != null || hitLeft.collider != null;
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

} */