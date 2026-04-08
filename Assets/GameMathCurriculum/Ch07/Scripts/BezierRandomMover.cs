using UnityEngine;

public class BezierRandomMover : MonoBehaviour
{
    public Transform Destination;

    public GameObject[] movers;
    private float lastCreateTime;
    public float CreateInterval;

    public float BulletSpeed = 1f;
    public float SpeedDeviation = 2f;

    public float SphereRadius;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time - lastCreateTime > CreateInterval)
        {
            Create(Random.Range(0, movers.Length));
            lastCreateTime = Time.time;
        }
    }

    void Create(int index)
    {
        Vector3 spherePoint = (Destination.position + transform.position) * 0.5f;

        Vector3 p1 = spherePoint + Random.insideUnitSphere * SphereRadius;
        Vector3 p2 = spherePoint + Random.insideUnitSphere * SphereRadius;

        GameObject mover = movers[index];
        BezierMover bezierMover = mover.GetComponent<BezierMover>();
        bezierMover.p0 = transform.position;
        bezierMover.p1 = p1;
        bezierMover.p2 = p2;
        bezierMover.p3 = Destination.position;
        bezierMover.LifeTime = Random.Range(1f, SpeedDeviation) / BulletSpeed;
        Instantiate(mover);
    }
}
