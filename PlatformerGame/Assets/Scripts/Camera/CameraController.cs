using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target & Speed")]
    private Transform target;
    [SerializeField] private float smoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2f, -10f);

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
        if (target == null)
        {
            Debug.LogError("Camera target not set.");
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime * 50f
        );

        transform.position = smoothedPosition;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            Vector3 desiredPosition = target.position;
            desiredPosition.z = transform.position.z;
            transform.position = desiredPosition;
        }
    }
}