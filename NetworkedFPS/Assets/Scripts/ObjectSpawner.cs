using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    public GameObject map;
    public GameObject objectToSpawnForTesting;
    public AnimationCurve animationCurve;

    public Vector3 FindSpawnPosition()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(241, 241, 1, 15.91f, 4, 0.368f, 1.4f, new Vector2(-24.5f, 3.6f));
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(noiseMap, 14.7f, animationCurve, 6);

        Vector3 vertice = meshData.vertices[Random.Range(0, meshData.vertices.Length)];

        Vector3 returnVector = new Vector3(vertice.x, vertice.y + 1.5f, vertice.z);

        return returnVector;

    }

    [Command]
    public void SpawnObjectAtPosition(GameObject objectToSpawn, Vector3 positionToSpawnAt)
    {
        Vector3 spawnPosition = new Vector3(positionToSpawnAt.x, positionToSpawnAt.y + 0.5f, positionToSpawnAt.z);

        Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
    }


}
