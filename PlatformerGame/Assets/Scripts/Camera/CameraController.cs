using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Speed")]
    private Transform target;
    [SerializeField] private float smoothSpeed = 0.15f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

    private Vector3 currentVelocity;

    private void Awake()
    {
        CameraController[] cameras = FindObjectsByType<CameraController>(FindObjectsSortMode.None);

        if (cameras.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            smoothSpeed
        );
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target == null) return;

        transform.position = target.position + offset;
        currentVelocity = Vector3.zero;
    }
}