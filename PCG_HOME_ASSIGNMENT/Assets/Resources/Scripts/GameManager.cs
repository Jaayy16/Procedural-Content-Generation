using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform endPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        getSpawnPoint();
        getEndPoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 getSpawnPoint()
    {
        if (spawnPoint != null) return spawnPoint.position;
        
        return Vector3.zero;
    }

    public Vector3 getEndPoint()
    {
        if (spawnPoint != null) return spawnPoint.position;
        return Vector3.zero;
    }
}
