using ProceduralDungeon.Settings;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private Camera mainCamera;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mainCamera = GetComponent<Camera>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        
    }

    public void SetPlayerTransform(Transform playerTrans)
    {
        playerTransform = playerTrans;
    }

    void LateUpdate()
    {
        if (playerTransform == null) return;
        
        Vector3 targetPos = playerTransform.position;
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }    
    
}
