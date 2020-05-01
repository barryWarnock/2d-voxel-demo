using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private BoxCollider2D scrollBox;
    //units/second
    public float baseCameraSpeed = 1;
    public float mouseDistanceMultiplier = 1.5f;
    private Vector3 velocity = Vector3.zero;
    Transform cameraTransform;

	// Use this for initialization
	void Start () {
        scrollBox = GetComponent<BoxCollider2D>();
        cameraTransform = GetComponentInParent<Transform>();
	}
	
    private void FixedUpdate() {
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
