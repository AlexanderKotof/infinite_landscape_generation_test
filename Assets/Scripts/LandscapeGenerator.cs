using System.Collections.Generic;
using System.Linq;
using Test.Noise;
using UnityEngine;

namespace Test
{
    public class LandscapeGenerator : MonoBehaviour
    {
        [SerializeField] private int _chunkSize = 16;
        [SerializeField] private int _heightMapResolution = 128;

        [SerializeField] private Transform _playerTransform;

        [SerializeField] private ChunkComponent _chunkPrefab;

        [SerializeField] private int _seed;

        private INoiseGenerator _noiseGenerator;
        private readonly Dictionary<Vector2Int, ChunkComponent> _chunksMap = new Dictionary<Vector2Int, ChunkComponent>();
        private readonly Vector2Int[] _chunksAroundPlayerOffsets = new Vector2Int[]
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
            _noiseGenerator = new PerlinNoiseGenerator(_heightMapResolution, _seed);
        }
        private void Update()
        {
            // Get chunk where player is
            var playerChunk = new Vector2Int(Mathf.FloorToInt(_playerTransform.position.x / _chunkSize), Mathf.FloorToInt(_playerTransform.position.z / _chunkSize));

            // Foreach chunks arround player
            foreach (var around in _chunksAroundPlayerOffsets)
            {
                var chunkCoord = playerChunk + around;
                if (_chunksMap.ContainsKey(chunkCoord))
                    continue;

                CreateChunk(chunkCoord);
            }

            // Remove far chunks
            var farChanks = _chunksMap.Keys.Where(x => (x - playerChunk).sqrMagnitude > 2).ToArray();
            foreach (var chunk in farChanks)
            {
                RemoveChunk(chunk);
            }
        }

        private void CreateChunk(Vector2Int coord)
        {
            // Calculate real chunk position
            var pos3d = new Vector3Int(coord.x * _chunkSize, 0, coord.y * _chunkSize);
            var chunk = Instantiate(_chunkPrefab, pos3d, Quaternion.identity, transform);

            // Generate heights map
            var heights = _noiseGenerator.GenerateNoiseMap(coord.x, coord.y);

            // Initialize Random for current chunk
            var rnd = new System.Random(_seed + coord.x + coord.y * 1000);

            chunk.Init(_chunkSize, _heightMapResolution, heights, rnd);

            _chunksMap[coord] = chunk;
        }

        private void RemoveChunk(Vector2Int coords)
        {
            var chank = _chunksMap[coords];

            Destroy(chank.gameObject);

            _chunksMap.Remove(coords);
        }
    }
}
