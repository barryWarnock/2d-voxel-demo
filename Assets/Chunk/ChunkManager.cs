using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChunkManager : MonoBehaviour {
    //indexed by lower left corner
    private Dictionary<Vector2, Chunk> chunks = new Dictionary<Vector2, Chunk>();
    private float leftmost;
    private float rightmost;
    public ChunkSettings chunkSettings;
    public GameObject chunkPrefab;
    public Camera mainCamera;

	// Use this for initialization
	void Start () {
        Vector2 chunkPos = new Vector2(0, 0);
        createChunkAt(chunkPos);
	}

    private void createChunkAt(Vector2 chunkPos) {
        Chunk chunk = GameObject.Instantiate(chunkPrefab, chunkPos, Quaternion.identity).GetComponent<Chunk>();
        chunks.Add(chunkPos, chunk);
        if (chunkPos.x < leftmost) {
            leftmost = chunkPos.x;
        } else if (chunkPos.x > rightmost) {
            rightmost = chunkPos.x;
        }
    }

    private Chunk worldPosToChunk(Vector2 pos) {
        float chunkWorldWidth = chunkSettings.chunkWidth * chunkSettings.blockSize;
        Vector2 chunkOffset = pos / chunkWorldWidth;
        float xOff = Mathf.Floor(chunkOffset.x);
        float yOff = Mathf.Floor(chunkOffset.y);
        Vector2 chunkOrigin = new Vector2(xOff, yOff) * chunkWorldWidth;
        Chunk chunk = null;
        chunks.TryGetValue(chunkOrigin, out chunk);

        return chunk;
    }

    private Vector2 worldPosToChunkLocal(Vector2 pos, Chunk chunk) {
        Vector2 offset = pos - (Vector2)chunk.transform.position;
        Debug.Log(offset);
        Vector2 chunkLocal = offset / chunkSettings.blockSize;
        float xOff = Mathf.Floor(chunkLocal.x);
        float yOff = Mathf.Floor(chunkLocal.y);
        return new Vector2(xOff, yOff);
    }

    public void SetBlock(Vector2 pos, BlockType type) {
        Chunk chunk = worldPosToChunk(pos);
        if (chunk != null) {
            Vector2 chunkLocalPos = worldPosToChunkLocal(pos, chunk);
            chunk.SetBlock(chunkLocalPos, type);
        }
    }

    public BlockType? GetBlock(Vector2 pos) {
        Chunk chunk = worldPosToChunk(pos);
        if (chunk == null) {
            return null;
        }
        Vector2 chunkLocalPos = worldPosToChunkLocal(pos, chunk);
        return chunk.GetBlock(chunkLocalPos);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
