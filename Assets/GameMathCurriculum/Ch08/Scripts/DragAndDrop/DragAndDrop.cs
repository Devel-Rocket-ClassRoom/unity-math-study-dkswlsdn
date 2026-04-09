using Unity.VisualScripting;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    public float RayDistance = 10000f;
    private GameObject selectedObject;
    private SelectableObject selectable;
    public LayerMask layerMask;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, RayDistance) && hit.collider.CompareTag("Selectable"))
                SelectObject(hit);
        }
        else if (Input.GetMouseButtonUp(0) && selectedObject != null)
        {
            putObject();
        }

        if (Physics.Raycast(ray, out hit, RayDistance, layerMask))
        {
            if (selectedObject != null)
            {
                MoveSelectedObject(hit);
            }

        }
    }

    void MoveSelectedObject(RaycastHit hit)
    {
        selectedObject.transform.position = hit.point + Vector3.up * 10;
    }

    void SelectObject(RaycastHit hit)
    {
        if (selectedObject != null) return;

        selectable = hit.collider.GetComponent<SelectableObject>();
        if (selectable == null) return;

        selectedObject = hit.collider.gameObject;
        selectable.Selected = true;
    }

    void putObject()
    {
        selectedObject = null;
        selectable.Selected = false;
        selectable = null;
    }
}
