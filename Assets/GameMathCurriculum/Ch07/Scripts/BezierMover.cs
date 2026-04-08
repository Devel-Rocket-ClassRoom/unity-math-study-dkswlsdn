using UnityEngine;

public class BezierMover : MonoBehaviour
{
    public Vector3 p0;
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;

    public float LifeTime;
    private float currentLife;
    private Renderer Renderer;


    private void Start()
    {
        transform.position = p0;
        Destroy(gameObject, LifeTime);
        Renderer = GetComponent<Renderer>();
        Renderer.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    }

    void Update()
    {
        currentLife += Time.deltaTime;
        transform.position = CubicBezier(p0, p1, p2, p3, currentLife / LifeTime);
    }

    Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        Vector3 a = Vector3.Lerp(p0, p1, t);
        Vector3 b = Vector3.Lerp(p1, p2, t);
        Vector3 c = Vector3.Lerp(p2, p3, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(d, e, t);
    }
}
