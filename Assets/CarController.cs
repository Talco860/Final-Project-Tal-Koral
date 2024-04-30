using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;
using System.IO;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class CarController : MonoBehaviour
{
    private Rigidbody2D car;
    private BoxCollider2D carCollider;
    public ParticleSystem dust;
    private Renderer carRenderer;
    private Renderer dustRenderer;
    public float speed = 3.2f; // Speed of the car
    private float moveDistance = 0;
    private Dictionary<string, float> mapErrors;
    private float[] errArray = new float[5];
    public int errorIndex;
    private bool changePos;
    void Start()
    {
        car = GetComponent<Rigidbody2D>();
        car.freezeRotation = true;
        carRenderer = GetComponent<Renderer>();
        carCollider = GetComponent<BoxCollider2D>();
        dustRenderer = dust.GetComponent<Renderer>();
        errorIndex = -1;
        mapErrors = new Dictionary<string, float>
        {
            { "Map1", 40f },
            { "Map2", 40f },
            { "Map3", 40f },
            { "Map4", 40f },
            { "Map5", 80f },
        };
        initAndShuffle(errArray, mapErrors);
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadSceneAsync(0);
        }


        HandleMovement();
        /*// Keyboard movement handling
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector2 moveVelocity = moveInput.normalized * speed;
        car.velocity = moveVelocity;

        // Rotate car based on movement direction if moving
        if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }*/

        if (errorIndex > -1)
        {
            distanceFromEdges();
        }
    }

    private void HandleMovement()
    {
        float rotationSpeed = 50f;

        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (moveInput.sqrMagnitude > 0.01f) // If there's significant input
        {
            Vector2 moveVelocity = moveInput.normalized * speed;
            car.velocity = moveVelocity;

            // Calculate target rotation based on movement direction
            float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

            // Smoothly rotate the car towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
        else
        {
            // Gradually decelerate the car if no input is detected
            car.velocity = Vector2.Lerp(car.velocity, Vector2.zero, 12 * Time.fixedDeltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BeforeError"))
        {
            errorIndex++;
        }
        if (collision.CompareTag("BeforeBridge"))
        {
            ChangeCarLayer(6);
            carRenderer.sortingOrder = carRenderer.sortingOrder == 1 ? 3 : 1;
            dustRenderer.sortingOrder = dustRenderer.sortingOrder == 1 ? 3 : 1;
        }
        if (collision.CompareTag("FinishLine"))
        {
            if(SceneManager.GetActiveScene().buildIndex == 5)
            {
                SceneManager.LoadScene(0);
            } else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("AfterBridge"))
        {
            ChangeCarLayer(0);
            carRenderer.sortingOrder = 1;
            dustRenderer.sortingOrder = 1;
        }
        if (collision.CompareTag("BeforeError"))
        {
            changePos = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Road") || collision.gameObject.CompareTag("TopBridge"))
        {
            car.AddForce(-car.velocity);
        }
    }

    private void ChangeCarLayer(int layerId)
    {
        gameObject.layer = layerId;
        dust.gameObject.layer = layerId;
    }

    private void distanceFromEdges()
    {
        float moveDistance = 0;
        // Define raycast directions relative to the car's orientation
        Vector2 left = -transform.right; // Left direction
        Vector2 right = transform.right; // Right direction

        // Define colors for each ray
        Dictionary<Vector2, Color> rayColorMap = new Dictionary<Vector2, Color>
    {
        { left, Color.green },
        { right, Color.yellow }
    };

        float leftDistance = -1f, rightDistance = -1f;

        // Iterate over each ray direction
        foreach (KeyValuePair<Vector2, Color> entry in rayColorMap)
        {
            Vector2 direction = entry.Key;
            Color color = entry.Value;

            // Calculate the position of the ray origin relative to the car's position
            Vector2 rayOrigin = (Vector2)transform.position + direction * carCollider.size.magnitude / 2;

            // Cast a ray in the current direction
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, direction);

            // Draw the ray with the assigned color for debugging purposes
            Debug.DrawRay(rayOrigin, direction * hit.distance, color);

            // If the ray hits a collider and the distance allows moving to the desired percentage
            if (hit.collider != null && hit.collider.CompareTag("HiddenSection"))
            {
                if (direction == left)
                {
                    leftDistance = hit.distance;
                    //print("Left ray: " + leftDistance);
                }
                else if (direction == right)
                {
                    rightDistance = hit.distance;
                    //print("Right ray: " + rightDistance);
                }
            }
        }

        moveDistance = errArray[errorIndex];

        // Move the car based on the chosen direction with the desired move percentage
        if (leftDistance != -1f && rightDistance != -1f && changePos)
        {
            float chosenDistance = leftDistance > rightDistance ? leftDistance : rightDistance;
            Vector2 chosenDirection = leftDistance > rightDistance ? -transform.right : transform.right;
            Vector2 newPosition = (Vector2)transform.position + chosenDirection * chosenDistance * moveDistance / 100f;
            transform.position = newPosition;
            changePos = false;

            if (moveDistance > 0) // log only the data of the error and not in every bridge
            {
                string moveDirection = leftDistance > rightDistance ? "Left" : "Right";
                logData(errorIndex + 1, moveDistance, moveDirection);
            }
        }
    }


    private void initAndShuffle(float[] arr, Dictionary<string, float> scenes)
    {
        float errSize = scenes.GetValueOrDefault(SceneManager.GetActiveScene().name);
        Random random = new Random();
        int rnd = random.Next(5);
        for (int i = 0; i < 5; i++) // Assuming you want to initialize it with 5 elements
        {
            if (i == rnd)
            {
                arr[i] = errSize;
                logData(i+1, errSize);
            }
            else
            {
                arr[i] = 0f;
            }
        }
    }

    private void logData(int bridge, float errSize, string direction = "")
    {
        string map = SceneManager.GetActiveScene().name;
        string fileName = "ErrorsSizes_" + map + ".csv";
        string filePath = Path.Combine(LogFolderManager.GetFolderPath(), fileName);

        // Check if the file already exists to decide whether to write headers
        bool fileExists = File.Exists(filePath);

        using (StreamWriter writer = new StreamWriter(filePath, true)) // Open the file in append mode
        {
            if (!fileExists)
            {
                // Write headers only if the file doesn't exist
                writer.WriteLine("Err_Bridge,Err_Size,Movement_Direction");
            }

            // Write the data; if direction is not provided, it will write an empty value for it
            string data = $"{bridge},{errSize}%,{direction}";
            writer.WriteLine(data);
        }
    }



}