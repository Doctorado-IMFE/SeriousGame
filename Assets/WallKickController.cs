using UnityEngine;

public class WallKickController : MonoBehaviour
{
    public float wallKickForce = 5f; // Fuerza del impulso de Wall Kick
    public KeyCode wallKickKey = KeyCode.Space; // Tecla que activa el Wall Kick

    private bool isWallKickEnabled = false; // Indica si el personaje puede realizar Wall Kick
    private Rigidbody rb; // Referencia al componente Rigidbody

    private void Start()
    {
        // Obtener la referencia al componente Rigidbody
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Verificar si colisiona con una pared
        if (collision.gameObject.CompareTag("Ground"))
        {
            isWallKickEnabled = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // Desactivar Wall Kick al salir de la colisión con la pared
        if (collision.gameObject.CompareTag("Ground"))
        {
            isWallKickEnabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(wallKickKey) && isWallKickEnabled)
        {
            // Realizar Wall Kick
            PerformWallKick();
        }
    }

    private void PerformWallKick()
    {
        // Aplicar impulso en dirección opuesta a la pared utilizando el componente Rigidbody
        Vector3 kickDirection = -transform.right; // Cambiar a la dirección adecuada según el diseño de tu juego
        rb.AddForce(kickDirection * wallKickForce, ForceMode.Impulse);
    }
}
