using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;

public class Logger : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 lastVelocity;
    private Vector2 velocity;
    private Vector2 acceleration;
    private string filePath;
    private StreamWriter writer;
    private bool beforeErr = false;
    private bool afterErr = false;
    public string folderName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        string map = SceneManager.GetActiveScene().name;
        string fileName = "DataLog_" + map + ".csv"; // You can change the file name format as needed
        filePath = Path.Combine(LogFolderManager.GetFolderPath(), fileName);

        writer = new StreamWriter(filePath);
        writer.WriteLine("Time,X,Y,Velocity_X,Velocity_Y,Vel_Magnitutde,Accel_X,Accel_Y,Accel_Magnitude,Before_Err,After_Err,Up,Down,Left,Right");
    }


    void FixedUpdate()
    {
        if (writer == null)
        {
            Debug.LogError("Writer is not initialized!");
            return;
        }

        velocity = rb.velocity;
        Vector2 deltaVelocity = velocity - lastVelocity;
        acceleration = deltaVelocity / Time.fixedDeltaTime;

        // Calculate the magnitudes (sizes) of the velocity and acceleration vectors
        float velocityMagnitude = velocity.magnitude;
        float accelerationMagnitude = acceleration.magnitude;

        // Check for arrow key inputs
        bool upPressed = Input.GetKey(KeyCode.UpArrow);
        bool downPressed = Input.GetKey(KeyCode.DownArrow);
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightPressed = Input.GetKey(KeyCode.RightArrow);

        // Convert boolean to integer (1 for true, 0 for false) for easier CSV parsing
        bool up = upPressed ? true : false;
        bool down = downPressed ? true : false;
        bool left = leftPressed ? true : false;
        bool right = rightPressed ? true : false;

        string data = $"{Time.time},{rb.position.x},{rb.position.y},{velocity.x},{velocity.y},{velocityMagnitude},{acceleration.x},{acceleration.y},{accelerationMagnitude},{beforeErr},{afterErr},{up},{down},{left},{right}";

        writer.WriteLine(data);

        lastVelocity = velocity;
    }


    void OnDestroy()
    {
        if (writer != null)
        {
            writer.Close();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("BeforeError"))
        {
            beforeErr = true;
        }
        if (collision.CompareTag("AfterError"))
        {
            afterErr = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("BeforeError"))
        {
            beforeErr = false;
        }

        if (collision.CompareTag("AfterError"))
        {
            afterErr = false;
        }
    }
}
