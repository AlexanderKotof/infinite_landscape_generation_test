using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LandscapeGenerator : MonoBehaviour
{
    public int chunkSize = 16;
    public int noiseResolution = 128;

    public Transform player;

    public ChunkComponent chunkPrefab;

    public int seed;

    private INoiseGenerator _generator;

    private Dictionary<Vector2Int, ChunkComponent> _chunks = new Dictionary<Vector2Int, ChunkComponent>();

    private readonly Vector2Int[] _checkChunksAround = new Vector2Int[]
    {
       Vector2Int.zero,
       Vector2Int.up,
       Vector2Int.down,
       Vector2Int.right,
       Vector2Int.left,
       Vector2Int.one,
       -Vector2Int.one,
        new Vector2Int(-1, 1),
        new Vector2Int(1, -1),
    };

    private void Awake()
    {
        _generator = new PerlinNoiseGenerator(noiseResolution, seed);
    }

    public void CreateChunk(Vector2Int coord)
    {
        var pos3d = new Vector3Int(coord.x * chunkSize, 0, coord.y * chunkSize);
        var chunk = Instantiate(chunkPrefab, pos3d, Quaternion.identity, transform);

        var noise = _generator.GenerateNoiseMap(coord.x, coord.y);

        var rnd = new System.Random(seed + coord.x + coord.y * 1000);

        chunk.Init(chunkSize, noiseResolution, noise, rnd);

        _chunks[coord] = chunk;
    }

    private void RemoveChunk(Vector2Int coords)
    {
        var chank = _chunks[coords];

        Destroy(chank.gameObject);

        _chunks.Remove(coords);
    }

    private void Update()
    {
        var playerChunk = new Vector2Int(Mathf.FloorToInt(player.position.x / chunkSize), Mathf.FloorToInt(player.position.z / chunkSize));

        foreach (var around in _checkChunksAround)
        {
            if (_chunks.ContainsKey(playerChunk + around))
                continue;    

            CreateChunk(playerChunk + around);
        }

        var farChanks = _chunks.Keys.Where(x => (x - playerChunk).sqrMagnitude > 2).ToArray();
        foreach(var chunk in farChanks)
        {
            RemoveChunk(chunk);
        }
    }
}

public interface INoiseGenerator
{
    float[,] GenerateNoiseMap(int x, int y);
}

public class PerlinNoiseGenerator : INoiseGenerator
{
    private readonly int _resolution;
    private readonly int _rnd;

    public PerlinNoiseGenerator(int resolution, int seed)
    {
        _resolution = resolution;
        _rnd = new System.Random(seed).Next(100);
    }

    public float[,] GenerateNoiseMap(int x, int y)
    {
        var noiseMap = new float[_resolution + 1, _resolution + 1];

        for (int i = 0; i <= _resolution; i++)
        {
            for (int j = 0; j <= _resolution; j++)
            {
                noiseMap[j, i] = Mathf.PerlinNoise(
                    x + ((float)i / _resolution) + _rnd,
                    y + ((float)j / _resolution) + _rnd
                    );
            }
        }

        return noiseMap;
    }
}
