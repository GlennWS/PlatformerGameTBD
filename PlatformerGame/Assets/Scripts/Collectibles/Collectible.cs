using UnityEngine;
using UnityEngine.SceneManagement;
public class Collectible : MonoBehaviour
{
    [Header("Value")]
    [SerializeField] private int value = 1;

    [Header("ID")]
    [SerializeField] private string overrideID = "";

    [Header("Hover Animation")]
    [SerializeField] private bool enableHover = true;
    [SerializeField] private float hoverAmplitude = 0.15f;
    [SerializeField] private float hoverFrequency = 2f;

    [Header("Collection Effect")]
    [SerializeField] private float collectScaleSpeed = 8f;
    [SerializeField] private float collectRiseSpeed = 3f;

    private string uniqueID;
    private Vector3 startPosition;
    private float hoverTimer;
    private bool isCollected = false;

    private void Start()
    {
        uniqueID = string.IsNullOrEmpty(overrideID)
            ? SceneManager.GetActiveScene().name + "/" + gameObject.name
            : overrideID;

        if (LevelTracker.Instance != null)
        {
            LevelTracker.Instance.RegisterCollectible(value);
        }

        if (CollectedItemsStore.Instance != null && CollectedItemsStore.Instance.IsCollected(uniqueID))
        {
            if (LevelTracker.Instance != null)
            {
                LevelTracker.Instance.CollectItem(value);
            }

            Destroy(gameObject);
            return;
        }

        startPosition = transform.position;
        hoverTimer = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        if (isCollected)
        {
            transform.localScale *= 1f - (collectScaleSpeed * Time.deltaTime);
            transform.position += Vector3.up * collectRiseSpeed * Time.deltaTime;

            if (transform.localScale.x < 0.05f)
            {
                Destroy(gameObject);
            }
            return;
        }

        if (enableHover)
        {
            hoverTimer += Time.deltaTime;
            float yOffset = Mathf.Sin(hoverTimer * hoverFrequency * Mathf.PI * 2f) * hoverAmplitude;
            transform.position = startPosition + new Vector3(0f, yOffset, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        if (!other.CompareTag("Player")) return;

        isCollected = true;
        GetComponent<Collider2D>().enabled = false;

        if (CollectedItemsStore.Instance != null)
        {
            CollectedItemsStore.Instance.MarkCollected(uniqueID);
        }

        if (LevelTracker.Instance != null)
        {
            LevelTracker.Instance.CollectItem(value);
        }
    }
}