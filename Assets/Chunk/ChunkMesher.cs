using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMesher : MonoBehaviour {
    public static ChunkMesher instance = null;
    public ChunkSettings chunkSettings;

    public static ChunkMesher GetInstance() {
        return instance;
    }

    private ChunkMesher() {
        instance = this;
    }

    public void meshChunk(Chunk chunk) {

        int width = chunkSettings.chunkWidth;
        float blockSize = chunkSettings.blockSize;
        List<Vector3> vertices = new List<Vector3>(chunk.blocks.Length * 4);
        List<Vector2> uvs = new List<Vector2>(chunk.blocks.Length * 4);
        List<int> triangles = new List<int>(chunk.blocks.Length * 2);

        int vertexIndex = 0;
        for (int y = 0; y < width; y++) {
            for (int x = 0; x < width; x++) {
                if (chunk.blocks[y,x] != BlockType.BLOCK_AIR) {
                    //create corner vertices
                    Vector3 llVert = new Vector3(x, y, 0) * blockSize;
                    vertices.Add(llVert);
                    int ll = vertexIndex++;
                    vertices.Add(llVert + Vector3.right*blockSize);
                    int lr = vertexIndex++;
                    vertices.Add(llVert + Vector3.up*blockSize);
                    int ul = vertexIndex++;
                    vertices.Add(llVert + Vector3.up*blockSize + Vector3.right*blockSize);
                    int ur = vertexIndex++;

                    //calculate uvs
                    uvs.AddRange(getUvsForType(chunk.blocks[y, x]));
                    
                    //create clockwise triangles
                    triangles.Add(ll); triangles.Add(ul); triangles.Add(ur);
                    triangles.Add(ll); triangles.Add(ur); triangles.Add(lr);
                }
            }
        }

        chunk.mesh.Clear();
        chunk.mesh.vertices = vertices.ToArray();
        chunk.mesh.uv = uvs.ToArray();
        chunk.mesh.triangles = triangles.ToArray();
    }

    //return uvs for ll,lr,ul,ur
    private Vector2[] getUvsForType(BlockType type) {
        switch (type) {
            case BlockType.BLOCK_A:
                return new Vector2[] {new Vector2(0, 0), new Vector2(0.1f, 0), new Vector2(0, 1), new Vector2(0.1f, 1)};
            case BlockType.BLOCK_SAND:
                return new Vector2[] {new Vector2(0.9f, 0), new Vector2(1, 0), new Vector2(0.9f, 1), new Vector2(1, 1)};
            default:
                throw new System.Exception("trying to access the uv coords for a block type that isn't defined");
        }
    }
}
