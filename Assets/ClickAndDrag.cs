using UnityEngine;

public class ClickAndDrag : MonoBehaviour
{
    public Rigidbody2D selectedObject;
    Vector3 offset;
    Vector3 mousePosition;
    private bool isDragging = false;

    void OnMouseDown()
    {
        isDragging = true;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
            newPosition.z = 0f; // Keep the object on the 2D plane
            transform.position = newPosition;
        }
    }

    void OnMouseUp()
    {
        isDragging = false;
    }
}