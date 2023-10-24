using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private float cameraSpeed = 8f;
    [SerializeField]
    private CinemachineVirtualCamera followCamera;
    private float minZoomDistance = 4f;
    private float maxZoomDistance = 12f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputMoveDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDirection.y = +1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDirection.x = -1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDirection.y = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDirection.x = +1f;
        }

        //Allows zooming in/out with the mousewheel. Negative so that up = in, down = out.
        followCamera.m_Lens.OrthographicSize = Mathf.Clamp(followCamera.m_Lens.OrthographicSize - Input.mouseScrollDelta.y, minZoomDistance, maxZoomDistance);
        //Rounds the camera movement to the nearest whole number.
        transform.position += inputMoveDirection * cameraSpeed * Time.deltaTime;
    }
}
