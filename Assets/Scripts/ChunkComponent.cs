using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Test
{
    public class ChunkComponent : MonoBehaviour
    {
        [SerializeField] private Terrain terrain;
        [SerializeField] private float height;

        [SerializeField] private GameObject[] environmentPrefabs;

        public void Init(int chunkSize, int resolution, float[,] heighMap, System.Random rnd)
        {
            // Create terrain data
            var terrainData = CreateTerrainData(chunkSize, resolution, heighMap);

            terrain.terrainData = terrainData;
            terrain.GetComponent<TerrainCollider>().terrainData = terrainData;

            //TODO set textures

            InstantiateEnvironment(chunkSize, resolution, heighMap, rnd);
        }

        private TerrainData CreateTerrainData(int chunkSize, int resolution, float[,] heighMap)
        {
            var terrainData = new TerrainData
            {
                heightmapResolution = resolution,
                size = new Vector3(chunkSize, height, chunkSize),
                terrainLayers = terrain.terrainData.terrainLayers,
            };

            terrainData.SetHeights(0, 0, heighMap);
            return terrainData;
        }

        private void InstantiateEnvironment(int chunkSize, int resolution, float[,] heighMap, System.Random rnd)
        {
            var environmentsCount = rnd.Next(5, 20);

            for (int i = 0; i < environmentsCount; i++)
            {
                var index = rnd.Next(0, environmentPrefabs.Length);
                var go = Instantiate(environmentPrefabs[index], transform);

                var x = rnd.Next(0, chunkSize);
                var z = rnd.Next(0, chunkSize);
                var y = heighMap[(int)((float)z / chunkSize * resolution), (int)((float)x / chunkSize * resolution)] * height;

                go.transform.localPosition = new Vector3(x, y, z);

                go.transform.rotation = Quaternion.AngleAxis(rnd.Next(0, 360), Vector3.up);
            }
        }
    }
}