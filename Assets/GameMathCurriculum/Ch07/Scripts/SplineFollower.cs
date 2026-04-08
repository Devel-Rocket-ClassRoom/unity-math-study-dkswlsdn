using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;

public class SplineFollower : MonoBehaviour
{
    public Transform mover;
    public float duration = 5f;
    private SplineContainer splineContainer;
    private float t;


    void Start()
    {
        splineContainer = GetComponent<SplineContainer>();
    }

    // Update is called once per frame
    void Update()
    {
        t += Time.deltaTime / duration;
        t = Mathf.Repeat(t, 1f);

        if (!splineContainer.Evaluate(splineContainer.Spline, t, out float3 position,out float3 tangent, out float3 up))
        {
            return;
        }

        mover.position = position;

        if (math.length(tangent) > 0.001f)
            mover.rotation = Quaternion.LookRotation(tangent, up);
    }
}
