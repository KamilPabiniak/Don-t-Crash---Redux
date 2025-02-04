using UnityEngine;

public class VRGravityController : MonoBehaviour
{
    [Header("Ustawienia grawitacji")]
    public float gravity = -9.81f;       // Si³a grawitacji
    public float groundedGravity = -2f;  // Niewielka si³a, gdy jesteœmy na ziemi

    [Header("Opcjonalnie: ustawienia skoku")]
    public float jumpForce = 5.0f;       // Si³a skoku

    private CharacterController controller;
    private Vector3 velocity;           // Wektor prêdkoœci, g³ównie w osi Y (grawitacja)

    void Start()
    {
        // Pobieramy komponent CharacterController przypisany do obiektu
        controller = GetComponent<CharacterController>();

        if (controller == null)
        {
            Debug.LogError("Brak komponentu CharacterController na obiekcie!");
        }
    }

    void Update()
    {
        // Sprawdzenie, czy postaæ jest na ziemi
        if (controller.isGrounded)
        {
            // Gdy jesteœmy na ziemi, ustawiamy niewielk¹ prêdkoœæ opadania,
            // aby zapobiec unoszeniu siê postaci
            if (velocity.y < 0)
            {
                velocity.y = groundedGravity;
            }
        }


        // Nak³adamy grawitacjê na prêdkoœæ w osi Y
        velocity.y += gravity * Time.deltaTime;

        // Poruszamy postaci¹ – CharacterController zadba o kolizje, nie pozwalaj¹c przechodziæ przez obiekty
        controller.Move(velocity * Time.deltaTime);
    }

    // Opcjonalnie: Mo¿esz dodaæ metodê, która reaguje na kolizje z innymi obiektami
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Debug.Log("Kolizja z: " + hit.gameObject.name);
    
        // Sprawdzenie wektora prêdkoœci w stosunku do normalnej kolizji
        float dot = Vector3.Dot(velocity, hit.normal);
        Debug.Log("Dot: " + dot);

        // Jeœli prêdkoœæ wskazuje w kierunku obiektu, usuñ t¹ sk³adow¹ (chocia¿ domyœlnie CharacterController powinien zatrzymaæ ruch)
        if (dot < 0)
        {
            velocity = velocity - Vector3.Project(velocity, hit.normal);
        }
    }


}
