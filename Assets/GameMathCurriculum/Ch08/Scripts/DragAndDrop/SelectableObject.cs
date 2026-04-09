using UnityEngine;

public class SelectableObject : MonoBehaviour
{
    public float RayDistance = 100f;
    public float cubeHeight = 10;
    public LayerMask layerMask;
    public float returnSpeed;


    private bool selected;
    private bool putted = false;
    private Vector3 originPosition;
    private Terrain terrain;

    public bool Selected
    {
        get { return selected; }
        set
        {
            Ray ray = new Ray(transform.position, -transform.up);

            if (Physics.Raycast(ray, out RaycastHit hit, RayDistance, layerMask))
            {
                putted = hit.collider.CompareTag("DropZone");
            }

            selected = value;
        }
    }


    

    private void Start()
    {
        terrain = Terrain.activeTerrain;

        Ray ray = new Ray(transform.position, -transform.up);

        if (Physics.Raycast(ray, out RaycastHit hit, RayDistance, layerMask))
        {
            originPosition = new Vector3(transform.position.x, hit.point.y + cubeHeight, transform.position.z);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (putted)
        {
            StayOnDropZone();
        }
        else if (!Selected)
        {
            ReturnToOriginPosition();
        }
    }

    void ReturnToOriginPosition()
    {
        Vector3 nextPos = Vector3.Lerp(transform.position, originPosition, returnSpeed * Time.deltaTime);

        float nextY = terrain.SampleHeight(nextPos) + terrain.transform.position.y;
        nextPos.y = nextY + cubeHeight;

        transform.position = nextPos;


        //Ray ray = new Ray(nextPos + Vector3.up * 30f, Vector3.down);

        //if (Physics.Raycast(ray, out RaycastHit hit, RayDistance, layerMask))
        //{
        //    nextPos.y = hit.point.y + cubeHeight;
        //    transform.position = nextPos;
        //}
        //else
        //{
        //    transform.position = originPosition;
        //}
    }

    void StayOnDropZone()
    {

    }
}
