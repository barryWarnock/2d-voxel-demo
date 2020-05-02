using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ChunkSettings : ScriptableObject {
    public float blockSize = 1;
    public int chunkWidth = 100;
    //how many chunks (including "ground level") until you hit the ceiling
    public int maxHeight = 5;
    //number of chunks down before bedrock
    public int maxDepth = 10;
}
