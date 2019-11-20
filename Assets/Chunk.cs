using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Array;

public class Chunk : MonoBehaviour
{
    //the height and width of the chunk
    public int chunk_size = 10;
    //the height and width of an individual block
    public float block_size = 1;
    //the lower left of the chunk in world coords
    public float x_offset = 0;
    public float y_offset = 0;

    protected bool[] blocks;
    bool dirty = true;
    protected Mesh mesh;
    protected Vector3[] vertices;
    protected int[] triangles;
    // Start is called before the first frame update
    void Start()
    {
        int num_blocks = chunk_size * chunk_size;
        blocks = new bool[num_blocks];
        for (int y = 0; y < chunk_size; y++) {
            for (int x = 0; x < chunk_size; x++) {
                set(x, y, in_circle(x,y));
            }
        }
        mesh = GetComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        //size vertices and triangles for the worst case
        vertices = new Vector3[num_blocks*4];
        triangles = new int[num_blocks*6];
    }

    bool in_circle(int x, int y) {
        float r = chunk_size/2;
        float height = Mathf.Abs(r-y);
        float width = Mathf.Abs(r-x);
        return Mathf.Sqrt(Mathf.Pow(height, 2)+Mathf.Pow(width, 2)) < r;
    }

    // Update is called once per frame
    void Update()
    {
        if (dirty) {
            dirty=false;
            generate_mesh();
        }

        Vector2 pos = world_to_local(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetMouseButton(0)) {
            if (pos != Vector2.negativeInfinity) {
                set((int)pos.x, (int)pos.y, false);
            }
        }
        if (Input.GetMouseButton(1)) {
            if (pos != Vector2.negativeInfinity) {
                set((int)pos.x, (int)pos.y, true);
            }
        }
    }

    protected int xy_to_i(int x, int y) {
        return (y * chunk_size) + x;
    }
    protected void set(int x, int y, bool value) {
        int i = xy_to_i(x, y);
        dirty = dirty || (value != blocks[i]);
        blocks[i] = value;
    }
    protected bool get(int x, int y) {
        int i = xy_to_i(x, y);
        return blocks[i];
    }
    protected Vector2 world_to_local(Vector2 location) {
        if (location.x < x_offset ||
            location.y < y_offset ||
            location.x > x_offset+block_size*chunk_size ||
            location.y > y_offset+block_size*chunk_size) {
                return Vector2.negativeInfinity;
            }

        int local_x = Mathf.FloorToInt((location.x-x_offset)/block_size);
        int local_y = Mathf.FloorToInt((location.y-y_offset)/block_size);

        return new Vector2(local_x, local_y);
    }

    protected void generate_mesh() {
        int vert_i = 0;
        int tri_i = 0;
        //lower and upper left and reight vert indices
        int ll, lr, ul, ur;
        Vector2 block_pos = new Vector2();
        for (int y = 0; y < chunk_size; y++) {
            for (int x = 0; x < chunk_size; x++) {
                if (get(x,y)) {
                    block_pos.Set((x+x_offset)*block_size, (y+y_offset)*block_size);
                    ll = vert_i++;
                    ul = vert_i++;
                    ur = vert_i++;
                    lr = vert_i++;
                    vertices[ll].Set(block_pos.x,block_pos.y,0);
                    vertices[ul].Set(block_pos.x,block_pos.y+block_size,0);
                    vertices[ur].Set(block_pos.x+block_size,block_pos.y+block_size,0);
                    vertices[lr].Set(block_pos.x+block_size,block_pos.y,0);

                    triangles[tri_i++] = ll;
                    triangles[tri_i++] = ul;
                    triangles[tri_i++] = ur;

                    triangles[tri_i++] = ur;
                    triangles[tri_i++] = lr;
                    triangles[tri_i++] = ll;
                }
            }
        }

        Vector3[] new_vertices = new Vector3[vert_i];
        System.Array.Copy(vertices, new_vertices, vert_i);
        int[] new_triangles = new int[tri_i];
        System.Array.Copy(triangles, new_triangles, tri_i);
        mesh.Clear();
        mesh.vertices = new_vertices;
        mesh.triangles = new_triangles;
    }

}
