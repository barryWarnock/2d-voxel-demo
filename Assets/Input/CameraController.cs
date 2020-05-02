using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private BoxCollider2D scrollBox;
    public ChunkSettings chunkSettings;
    //units/second
    public float baseCameraSpeed = 1;
    public float mouseDistanceMultiplier = 1.5f;
    public float maxZoom = 100;
    public float minZoom = 1;
    public float zoomMultiplier = 1.1f;
    private Vector3 velocity = Vector3.zero;
    Transform cameraTransform;
    Camera mainCamera;
    bool doScroll = true;

	// Use this for initialization
	void Start () {
        scrollBox = GetComponent<BoxCollider2D>();
        mainCamera = GetComponent<Camera>();
        cameraTransform = GetComponentInParent<Transform>();

        float chunkWorldWidth = chunkSettings.chunkWidth * chunkSettings.blockSize;
        cameraTransform.position = new Vector3(5, chunkWorldWidth * chunkSettings.maxDepth + chunkWorldWidth / 2, -10);
	}

    private void Update() {
        checkEnableScroll();
        zoom();
    }

    private void checkEnableScroll() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            doScroll = !doScroll;
        }
    }

    private void zoom() {
        if (Input.GetAxis("Mouse ScrollWheel") > 0) {
            scaleCamera(1 / zoomMultiplier);
        } else if (Input.GetAxis("Mouse ScrollWheel") < 0) {
            scaleCamera(zoomMultiplier);
        }
    }

    private void scaleCamera(float scale) {
        float currentSize = mainCamera.orthographicSize;
        float newSize = Mathf.Clamp(currentSize * scale, minZoom, maxZoom);
        float actualScale = newSize / currentSize;
        mainCamera.orthographicSize *= actualScale;
        scrollBox.size *= actualScale;
    }

    private void LateUpdate() {
        if (doScroll) {
            Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!scrollBox.OverlapPoint(mouse)) {
                Vector2 closestPoint = scrollBox.bounds.ClosestPoint(mouse);
                Vector2 mouseDistance = (Vector2)mouse - closestPoint;
                float cameraSpeed = baseCameraSpeed * Mathf.Ceil(mouseDistance.magnitude * mouseDistanceMultiplier);
                float travelTime = mouseDistance.magnitude / cameraSpeed;
                cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, mouse, ref velocity, travelTime, cameraSpeed);
            }
        }
    }
}
