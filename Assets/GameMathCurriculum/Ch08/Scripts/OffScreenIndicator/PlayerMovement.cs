using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float RotateSpeed = 90f;

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxisRaw("Vertical");
        float inputY = Input.GetAxisRaw("Horizontal");

        Vector3 dir = (transform.forward * inputX + transform.right * inputY).normalized;

        transform.position += dir * MoveSpeed * Time.deltaTime;

        if (Input.GetMouseButton(0))
            transform.rotation *= Quaternion.AngleAxis(-RotateSpeed * Time.deltaTime, Vector3.up);
        if (Input.GetMouseButton(1))
            transform.rotation *= Quaternion.AngleAxis(RotateSpeed * Time.deltaTime, Vector3.up);
    }
}
