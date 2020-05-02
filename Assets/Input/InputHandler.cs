using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    public ChunkManager chunkManager;
    public BlockType blockType;

    // Use this for initialization
    void Start () {
        blockType = BlockType.BLOCK_A;
	}

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.A)) blockType = BlockType.BLOCK_A;
        if (Input.GetKeyDown(KeyCode.B)) blockType = BlockType.BLOCK_SAND;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
            BlockType placeBlockType = Input.GetMouseButton(0) ? blockType : BlockType.BLOCK_AIR;
            chunkManager.SetBlock(mouse, placeBlockType);
        }
    }
}
