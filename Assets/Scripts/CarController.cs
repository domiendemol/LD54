using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float turnSpeed = 5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get the horizontal and vertical input from the keyboard.
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Move the car forward or backward based on the vertical input.
        if( Mathf.Approximately(verticalInput, 0f) )
            rb.velocity = Vector2.zero;
        else
            rb.AddForce(transform.up * verticalInput * speed * Time.deltaTime);

        // Rotate the car left or right based on the horizontal input.
        if( Mathf.Approximately(verticalInput, 0f) )
            rb.angularVelocity = 0f;
        else
            rb.AddTorque(-horizontalInput * turnSpeed * rb.velocity.magnitude * Time.deltaTime);
    }
} 
