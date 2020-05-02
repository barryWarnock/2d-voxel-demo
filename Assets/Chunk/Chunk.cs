using UnityEngine;

public enum BlockType {
    BLOCK_AIR,
    BLOCK_A,
    BLOCK_SAND,
    BLOCK_NOT_FOUND
}


public class Chunk : MonoBehaviour {
    public ChunkSettings chunkSettings;
    public Chunk leftChunk, rightChunk, aboveChunk, belowChunk;

    public Mesh mesh;
    public BlockType[,] blocks;

    private ChunkMesher chunkMesher;

    protected bool dirty = true;

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
                blocks[y,x] = initializeBlockAt(x, y);
            }
        }
    }

    private BlockType initializeBlockAt(int x, int y) {
        int width = chunkSettings.chunkWidth;
        if (transform.position.y > 0) {
            return BlockType.BLOCK_AIR;
        } else if (transform.position.y < 0) {
            return BlockType.BLOCK_A;
        } else {
            return y < width / 2 ? BlockType.BLOCK_A : BlockType.BLOCK_AIR;
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
            dirty = false;
            chunkMesher.meshChunk(this);
            updateDynamicBlocks();
        }
	}

    private void updateDynamicBlocks() {
        for (int y = 0; y < chunkSettings.chunkWidth; y++) {
            for (int x = 0; x < chunkSettings.chunkWidth; x++) {
                if (blocks[y,x] == BlockType.BLOCK_SAND) {
                    updateSand(x,y);
                }
            }
        }
    }

    private void updateSand(int x, int y) {
        BlockType below = GetBlock(x, y-1);
        if (below == BlockType.BLOCK_AIR) {
            blocks[y,x] = BlockType.BLOCK_AIR;
            SetBlock(x, y-1, BlockType.BLOCK_SAND);
        } else {
            bool sandCanFallLeft = sandCanFall(x, y, -1);
            bool sandCanFallRight = sandCanFall(x, y, 1);
            if (sandCanFallLeft || sandCanFallRight) {
                blocks[y,x] = BlockType.BLOCK_AIR;
                if (sandCanFallLeft && sandCanFallRight) {
                    if (Random.value < 0.5) {
                        SetBlock(x-1,y-1,BlockType.BLOCK_SAND);
                    } else {
                        SetBlock(x+1,y-1,BlockType.BLOCK_SAND);
                    }
                } else if (sandCanFallLeft) {
                    SetBlock(x-1,y-1,BlockType.BLOCK_SAND);
                } else {
                    SetBlock(x+1,y-1,BlockType.BLOCK_SAND);
                }
            }
        }
    }

    private bool sandCanFall(int x, int y, int xOffset) {
        for (int i = 0; i < 3; i++) {
            if (GetBlock(x+xOffset, y-i) != BlockType.BLOCK_AIR) {
                return false;
            } 
        }
        return true;
    }

    public void SetBlock(int x, int y, BlockType type) {
        if (y >= chunkSettings.chunkWidth && aboveChunk != null) {
            aboveChunk.SetBlock(x, y-chunkSettings.chunkWidth, type);
        } else if (y < 0 && belowChunk != null) {
            belowChunk.SetBlock(x, y+chunkSettings.chunkWidth, type);
        } else if (x >= chunkSettings.chunkWidth && rightChunk != null) {
            rightChunk.SetBlock(x-chunkSettings.chunkWidth, y, type);
        } else if (x < 0 && leftChunk != null) {
            leftChunk.SetBlock(x+chunkSettings.chunkWidth, y, type);
        } else {
            blocks[y,x] = type;
            dirty = true;
            dirtyChunk(aboveChunk);
            dirtyChunk(belowChunk);
            dirtyChunk(leftChunk);
            dirtyChunk(rightChunk);
        }
    }

    private void dirtyChunk(Chunk chunk) {
        if (chunk != null) {
            chunk.dirty = true;
        }
    }

    public BlockType GetBlock(int x, int y) {
        if (y >= chunkSettings.chunkWidth) {
            if (aboveChunk != null) {
                return aboveChunk.GetBlock(x, y-chunkSettings.chunkWidth);
            } else {
                return BlockType.BLOCK_NOT_FOUND;
            }
        } else if (y < 0) {
            if (belowChunk != null) {
                return belowChunk.GetBlock(x, y+chunkSettings.chunkWidth);
            } else {
                return BlockType.BLOCK_NOT_FOUND;
            }
        } else if (x >= chunkSettings.chunkWidth) {
            if (rightChunk != null) {
                return rightChunk.GetBlock(x-chunkSettings.chunkWidth, y);
            } else {
                return BlockType.BLOCK_NOT_FOUND;
            }
        } else if (x < 0) {
            if (leftChunk != null) {
                return leftChunk.GetBlock(x+chunkSettings.chunkWidth, y);
            } else {
                return BlockType.BLOCK_NOT_FOUND;
            }
        } else {
            return blocks[y,x];
        }
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
