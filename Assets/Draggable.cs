using UnityEngine;

[RequireComponent (typeof(Collider2D))]
public class Draggable : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    Vector2 difference = Vector2.zero;

    void OnMouseDown()
    {
        // When the object is clicked, start dragging it
        isDragging = true;

        // Calculate the offset between the mouse position and the object's position
        //offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        difference = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position;
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // While dragging, update the object's position based on the mouse position
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - difference;
        }
    }

    void OnMouseUp()
    {
        // When the mouse button is released, stop dragging the object
        isDragging = false;
    }
}
