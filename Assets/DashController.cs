using UnityEngine;

public class DashController : MonoBehaviour
{
    public float dashDistance = 5f; // Distancia que recorre el personaje durante el dash
    public float dashDuration = 0.2f; // Duración del dash en segundos
    public KeyCode dashKey = KeyCode.Space; // Tecla que activa el dash

    public float dashStartDuration = 0.5f; // Duración de la animación Dash Start en segundos
    public float dashEndDuration = 0.5f; // Duración de la animación Dash End en segundos

    public bool isDashing = false; // Indica si el personaje está realizando un dash

    private void Update()
    {
        if (Input.GetKeyDown(dashKey) && !isDashing)
        {
            // Iniciar el dash
            StartCoroutine(PerformDash());
        }
    }

      private System.Collections.IEnumerator PerformDash()
    {
        isDashing = true;
        float elapsedTime = 0f;
        Vector3 originalPosition = transform.position;
        Vector3 dashTarget = originalPosition + transform.right * dashDistance; // Cambio de transform.forward a transform.right

        // Activar animación Dash Start
        GetComponent<Animator>().SetTrigger("DashStart");

        // Esperar la duración del Dash Start
        yield return new WaitForSeconds(dashStartDuration);

        // Activar animación Dash Loop
        GetComponent<Animator>().SetBool("ButtonPressed", true);

        while (elapsedTime < dashDuration || Input.GetKey(dashKey))
        {
            float dashPercentage = elapsedTime / dashDuration;
            transform.position = Vector3.Lerp(originalPosition, dashTarget, dashPercentage);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Desactivar animación Dash Loop
        GetComponent<Animator>().SetBool("ButtonPressed", false);

        // Activar animación Dash End
        GetComponent<Animator>().SetTrigger("DashEnd");

        // Esperar la duración del Dash End
        yield return new WaitForSeconds(dashEndDuration);

        // Finalizar el dash
        isDashing = false;
    }
}