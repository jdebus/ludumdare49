using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform cameraTaget;

    [SerializeField]
    Cinemachine.CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    [Range(5,15)]
    float keyboardMovementSpeed = 10;

    [SerializeField]
    float ortographicZoomIncement = 1;

    [SerializeField, Min(1)]
    float zoomLerpSpeed = 1;

    public Vector2 cameraZoomRange = new Vector2(2, 10);

    float targetOrthographicSize = 0;

    Vector3 prevMouseWorldPos;

    [SerializeField]
    BoxCollider2D bounds;

    private void Start()
    {
        targetOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
    }

    private void Update()
    {
        HandleKeyboarMovement();
        HandleMouseWheelZoom();
        HandleDrag();

        KeepTargetInBounds();
    }

    private void KeepTargetInBounds()
    {
        if(bounds != null)
        {
            var pos = cameraTaget.position;
            pos.x = Mathf.Clamp(pos.x, bounds.bounds.min.x, bounds.bounds.max.x);
            pos.y = Mathf.Clamp(pos.y, bounds.bounds.min.y, bounds.bounds.max.y);
            cameraTaget.position = pos;
        }
    }

    private void HandleDrag() 
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mouseWorldDelta = prevMouseWorldPos - mouseWorldPos;
        if (Input.GetMouseButton(1))
            cameraTaget.Translate(mouseWorldDelta * virtualCamera.m_Lens.OrthographicSize);

        prevMouseWorldPos = mouseWorldPos;
    }

    private void HandleMouseWheelZoom()
    {
        var orthoSize = virtualCamera.m_Lens.OrthographicSize;
        var scrollDelta = Input.mouseScrollDelta.y * -1;
        if (scrollDelta != 0)
        {
            targetOrthographicSize += scrollDelta * ortographicZoomIncement;
            targetOrthographicSize = Mathf.Clamp(targetOrthographicSize, cameraZoomRange.x, cameraZoomRange.y);
        }

        if (Mathf.Abs(targetOrthographicSize - orthoSize) > 0.1f)
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(orthoSize, targetOrthographicSize, Time.deltaTime * zoomLerpSpeed);
    }

    private void HandleKeyboarMovement()
    {
        Vector3 moveInput = new Vector3();
        if (Input.GetKey(KeyCode.W)) moveInput.y += 1;
        if (Input.GetKey(KeyCode.S)) moveInput.y -= 1;
        if (Input.GetKey(KeyCode.D)) moveInput.x += 1;
        if (Input.GetKey(KeyCode.A)) moveInput.x -= 1;

        cameraTaget.Translate(moveInput.normalized * keyboardMovementSpeed * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Colors.CornflowerBlue;
        Gizmos.DrawWireCube(cameraTaget.position, Vector3.one * 0.2f);
        
        //Handles.color = Colors.CornflowerBlue;
        //Handles.Label(cameraTaget.position, "Camera target");
    }
}
