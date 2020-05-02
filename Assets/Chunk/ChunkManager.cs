using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ChunkManager : MonoBehaviour {
    //indexed by lower left corner
    private Dictionary<Vector2, Chunk> chunks = new Dictionary<Vector2, Chunk>();
    private float leftmost;
    private float rightmost;
    private float chunkWorldWidth;
    public ChunkSettings chunkSettings;
    public GameObject chunkPrefab;
    public Camera mainCamera;

	// Use this for initialization
	void Start () {
        chunkWorldWidth = chunkSettings.blockSize * chunkSettings.chunkWidth;
        Vector2 chunkPos = new Vector2(0, 0);
        createChunkAt(chunkPos);
	}

    private void createChunkAt(Vector2 chunkPos) {
        Chunk chunk = GameObject.Instantiate(chunkPrefab, chunkPos, Quaternion.identity).GetComponent<Chunk>();
        chunk.transform.parent = transform;
        chunks.Add(chunkPos, chunk);
        if (chunkPos.x < leftmost) {
            leftmost = chunkPos.x;
        } else if (chunkPos.x > rightmost) {
            rightmost = chunkPos.x;
        }
    }

    private void addNeighbours(Chunk chunk) {
        Vector2 chunkXOffset = new Vector2(chunkWorldWidth, 0);
        Vector2 chunkYOffset = new Vector2(0, chunkWorldWidth);
        Vector2 chunkPos = chunk.transform.position;
        Vector2 leftNeighbourPos = chunkPos - chunkXOffset;
        Vector2 rightNeighbourPos = chunkPos + chunkXOffset;
        Vector2 aboveNeighbourPos = chunkPos + chunkYOffset;
        Vector2 belowNeighbourPos = chunkPos - chunkYOffset;

        Chunk leftNeighbour = worldPosToChunk(leftNeighbourPos);
        Chunk rightNeighbour = worldPosToChunk(rightNeighbourPos);
        Chunk aboveNeighbour = worldPosToChunk(aboveNeighbourPos);
        Chunk belowNeighbour = worldPosToChunk(belowNeighbourPos);

        //chunk to the left
        if (leftNeighbour )
        chunk.leftChunk = leftNeighbour;
        leftNeighbour.rightChunk = chunk;
        //chunk to the right
        chunk.rightChunk = rightNeighbour;
        rightNeighbour.leftChunk = chunk;
        //chunk above
        chunk.aboveChunk = aboveNeighbour;
        aboveNeighbour.belowChunk = chunk;
        //chunk below
        chunk.belowChunk = belowNeighbour;
        belowNeighbour.aboveChunk = chunk;
    }

    private void pairNeighbours() {

    }

    private Chunk worldPosToChunk(Vector2 pos) {
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
        Vector2 cameraBounds = new Vector2(mainCamera.orthographicSize * Screen.width / Screen.height, mainCamera.orthographicSize);
        if (mainCamera.transform.position.x + cameraBounds.x > rightmost) {
            createChunkAt(new Vector2(rightmost+chunkWorldWidth,0));
        }
        if (mainCamera.transform.position.x - cameraBounds.x < leftmost) {
            createChunkAt(new Vector2(leftmost-chunkWorldWidth,0));
        }
	}
}
