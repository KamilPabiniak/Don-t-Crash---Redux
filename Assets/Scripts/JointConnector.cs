using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JointConnector : MonoBehaviour
{
    [SerializeField] private Rigidbody rootRigidbody;
    [SerializeField] private float jointHealth = 100f; // Maksymalne życie jointa

    private InputAction connectAction;
    private bool isConnected = false;
    private List<FixedJoint> joints = new List<FixedJoint>(); // Lista jointów
    private GameObject vehicleParent; // Pusty obiekt Vehicle

    private void OnEnable()
    {
        connectAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/1");
        connectAction.performed += OnConnectActionPerformed;
        connectAction.Enable();
    }

    private void OnDisable()
    {
        connectAction.Disable();
        connectAction.performed -= OnConnectActionPerformed;
    }

    private void OnConnectActionPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Connect action triggered");
        ConnectAllSnappedObjects();
    }

    private void ConnectAllSnappedObjects()
    {
        if (isConnected) return;

        // Tworzymy nowy pusty obiekt Vehicle jako rodzic
        GameObject vehicleParent = new GameObject("Vehicle");
        vehicleParent.transform.position = rootRigidbody.transform.position;

        // Przenosimy rootRigidbody do Vehicle
        rootRigidbody.transform.SetParent(vehicleParent.transform);

        SnapSystem[] snapSystems = rootRigidbody.GetComponentsInChildren<SnapSystem>();
        Debug.Log(snapSystems);

        foreach (SnapSystem snapSystem in snapSystems)
        {
            Rigidbody rb = snapSystem.GetComponent<Rigidbody>();

            if (rb != null && rb != rootRigidbody)
            {
                Transform parentTransform = snapSystem.transform.parent;
                if (parentTransform != null)
                {
                    Rigidbody parentRigidbody = parentTransform.GetComponent<Rigidbody>();

                    if (parentRigidbody != null)
                    {
                        // Tworzymy joint między rodzicem a obiektem
                        FixedJoint joint = parentRigidbody.gameObject.AddComponent<FixedJoint>();
                        Debug.Log(joint);
                        joint.connectedBody = rb;
                        joints.Add(joint);

                        // Dodajemy komponent zdrowia jointa
                        JointHealth jointHealthComponent = joint.gameObject.AddComponent<JointHealth>();
                        jointHealthComponent.Initialize(joint, jointHealth);
                    }
                }
            }
        }

        // Przenosimy obiekty do Vehicle i wyłączamy SnapSystem
        foreach (SnapSystem snapSystem in snapSystems)
        {
            Rigidbody rb = snapSystem.GetComponent<Rigidbody>();

            if (rb != null)
            {
                snapSystem.transform.SetParent(vehicleParent.transform);
                snapSystem.enabled = false;
            }
        }

        // Ustawienia fizyki dla rootRigidbody
        rootRigidbody.useGravity = true;
        rootRigidbody.isKinematic = false;

        foreach (FixedJoint joint in joints)
        {
            Rigidbody rb = joint.connectedBody;
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }
        }

        isConnected = true;
    }



    // Metoda rekurencyjna do sprawdzania hierarchii
    private bool IsInHierarchy(Transform root, Transform target)
    {
        // Sprawdzamy bezpośrednie dopasowanie
        if (root == target) return true;

        // Rekurencyjnie przeszukujemy dzieci
        foreach (Transform child in root)
        {
            if (IsInHierarchy(child, target))
            {
                return true;
            }
        }

        return false; // Jeśli nie znaleziono w hierarchii
    }
}

public class JointHealth : MonoBehaviour
{
    private FixedJoint joint;
    private float currentHealth;

    public void Initialize(FixedJoint joint, float initialHealth)
    {
        this.joint = joint;
        currentHealth = initialHealth;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Oblicz siłę uderzenia na podstawie prędkości relatywnej
        float impactForce = collision.relativeVelocity.magnitude;

        // Zmniejsz życie jointa proporcjonalnie do siły uderzenia
        currentHealth -= impactForce;

        // Jeśli życie jointa spadnie do zera, niszczymy joint
        if (currentHealth <= 0)
        {
            Destroy(joint);
            Destroy(this); // Usuwamy również ten komponent
        }
    }
}
