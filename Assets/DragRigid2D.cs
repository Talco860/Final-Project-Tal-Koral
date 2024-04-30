using UnityEngine;

public class DragRigidbody2D : MonoBehaviour
{
    public float forceAmount = 500f;

    Rigidbody2D selectedRigidbody;
    Camera targetCamera;
    Vector3 originalScreenTargetPosition;
    Vector2 originalRigidbodyPos;
    float selectionDistance;

    void Start()
    {
        targetCamera = Camera.main; // Assuming the main camera is used for input
    }

    void Update()
    {
        if (!targetCamera)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            // Check if we are hovering over Rigidbody2D, if so, select it
            selectedRigidbody = GetRigidbodyFromMouseClick();
        }
        if (Input.GetMouseButtonUp(0) && selectedRigidbody)
        {
            // Release selected Rigidbody2D if there is any
            selectedRigidbody = null;
        }
    }

    void FixedUpdate()
    {
        if (selectedRigidbody)
        {
            Vector3 mousePositionOffset = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance)) - originalScreenTargetPosition;
            selectedRigidbody.velocity = (originalRigidbodyPos + (Vector2)mousePositionOffset - (Vector2)selectedRigidbody.transform.position) * forceAmount * Time.deltaTime;
        }
    }

    Rigidbody2D GetRigidbodyFromMouseClick()
    {
        RaycastHit2D hitInfo = Physics2D.GetRayIntersection(targetCamera.ScreenPointToRay(Input.mousePosition));
        if (hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.GetComponent<Rigidbody2D>())
            {
                selectionDistance = Vector3.Distance(targetCamera.transform.position, hitInfo.point);
                originalScreenTargetPosition = targetCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selectionDistance));
                originalRigidbodyPos = hitInfo.collider.transform.position;
                return hitInfo.collider.gameObject.GetComponent<Rigidbody2D>();
            }
        }
        return null;
    }
}
