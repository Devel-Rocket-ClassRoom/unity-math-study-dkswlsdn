using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Indicator : MonoBehaviour
{
    public Transform[] targets;
    public float cameraMargin = 10f;
    private GameObject[] indicators;
    Camera cam;

    private Vector3 cameraPos = new Vector3(0.5f, 0.5f, 0f);
    private float indicatorPos = 0.1f;



    private void Start()
    {
        cam = Camera.main;
        indicators = new GameObject[targets.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            indicators[i] = targets[i].GetComponent<IndicatorContainer>().indicator;
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 pos = cam.WorldToScreenPoint(targets[i].position);

            if (pos.x < 0f || pos.x > Screen.width || pos.y < 0f || pos.y > Screen.height || pos.z < 0f) { ShowIndicator(pos, i); }
            else { indicators[i].SetActive(false); }
        }
    }

    void ShowIndicator(Vector3 pos, int i)
    {
        if (pos.z < 0)
        {
            pos.x = Screen.width - pos.x;
            pos.y = Screen.height - pos.y;
        }

        Vector3 newPos = new Vector3(Mathf.Clamp(pos.x, cameraMargin, Screen.width - cameraMargin), Mathf.Clamp(pos.y, cameraMargin, Screen.height - cameraMargin), 0);


        indicators[i].transform.position = newPos;
        indicators[i].SetActive(true);
    }
}
