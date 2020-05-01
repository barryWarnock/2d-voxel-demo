using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    public ChunkManager chunkManager;
    private BoxCollider2D scrollBox;
    //units/second
    public float baseCameraSpeed = 1;
    public float mouseDistanceMultiplier = 1.5f;
    private Vector3 velocity = Vector3.zero;

    // Use this for initialization
    void Start () {
        scrollBox = GetComponent<BoxCollider2D>();
	}

    // Update is called once per frame
    void Update() {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
            BlockType placeBlockType = Input.GetMouseButton(0) ? BlockType.BLOCK_A : BlockType.BLOCK_AIR;

            chunkManager.SetBlock(mouse, placeBlockType);
        }
    }

    private void FixedUpdate() {
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!scrollBox.OverlapPoint(mouse)) {
            Vector2 closestPoint = scrollBox.bounds.ClosestPoint(mouse);
            Vector2 mouseDistance = (Vector2)mouse - closestPoint;
            float cameraSpeed = baseCameraSpeed * Mathf.Ceil(mouseDistance.magnitude * mouseDistanceMultiplier);
            float travelTime = mouseDistance.magnitude / cameraSpeed;
            Transform cameraTransform = GetComponentInParent<Transform>();
            cameraTransform.position = Vector3.SmoothDamp(cameraTransform.position, mouse, ref velocity, travelTime, cameraSpeed);
        }
    }
}
