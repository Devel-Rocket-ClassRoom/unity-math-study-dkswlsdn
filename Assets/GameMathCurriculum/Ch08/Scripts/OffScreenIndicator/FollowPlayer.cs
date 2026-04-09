using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform Player;
    public float CameraXAngle = 30f;
    public float CameraDistance = 10f;
    Vector3 velocity = Vector3.zero;

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, Player.position + Player.forward * -10 + Player.up * 4, ref velocity, 0.3f);//Player.position + Player.forward * -10 + Player.up * 4;
        transform.forward = (Player.position - transform.position).normalized;
    }
}
