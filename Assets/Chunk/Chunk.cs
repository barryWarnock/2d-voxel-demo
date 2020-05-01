using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chunk : MonoBehaviour {
    public ChunkSettings chunkSettings;

    public Mesh mesh;
    public BlockType[,] blocks;

    private ChunkMesher chunkMesher;

    private bool dirty = true;

	// Use this for initialization
	void Start () {
        initializeMesh();
        chunkMesher = ChunkMesher.GetInstance();
        Initialize();
	}

    private void Initialize() {
        initializeBlocks();
        chunkMesher.meshChunk(this);
    }

    private void initializeBlocks() {
        int width = chunkSettings.chunkWidth;
        blocks = new BlockType[width,width];
       for (int y = 0; y < width; y++) {
            for (int x = 0; x < width; x++) {
                blocks[y,x] = y < width / 2 ? BlockType.BLOCK_A : BlockType.BLOCK_AIR;
            }
        }
    }

    private void initializeMesh() {
        mesh = new Mesh();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
	
	// Update is called once per frame
	void Update () {
        if (dirty) {
            chunkMesher.meshChunk(this);
            dirty = false;
        }
	}

    public void SetBlock(Vector2 pos, BlockType type) {
        blocks[(int)pos.y, (int)pos.x] = type;
        dirty = true;
    }

    public BlockType GetBlock(Vector2 pos) {
        return blocks[(int)pos.y, (int)pos.x];
    }

    void OnDrawGizmosSelected() {
        int width = chunkSettings.chunkWidth;
        float blockSize = chunkSettings.blockSize;
        Gizmos.color = Color.green;
        float worldWidth = width * blockSize;
        float x = this.transform.position.x + worldWidth/2;
        float y = this.transform.position.y + worldWidth/2;

        Gizmos.DrawWireCube(new Vector3(x, y, 0), new Vector3(worldWidth, worldWidth, 1));
    }

    void OnDrawGizmos() {
        int width = chunkSettings.chunkWidth;
        float blockSize = chunkSettings.blockSize;
        Gizmos.color = Color.red;
        float worldWidth = width * blockSize;
        float x = this.transform.position.x + worldWidth/2;
        float y = this.transform.position.y + worldWidth/2;

        Gizmos.DrawWireCube(new Vector3(x, y, 0), new Vector3(worldWidth, worldWidth, 1));
    }
}
