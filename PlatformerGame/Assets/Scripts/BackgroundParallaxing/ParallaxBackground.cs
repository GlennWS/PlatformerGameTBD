using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    public enum TilingMode
    {
        HorizontalOnly,
        BothAxes
    }

    [Header("Tiling")]
    [SerializeField] private TilingMode tilingMode = TilingMode.BothAxes;

    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactorX = 0.2f;

    [Range(0f, 1f)]
    [SerializeField] private float parallaxFactorY = 0.1f;

    [Header("Coverage")]
    [SerializeField] private float coverageBuffer = 1.5f;

    private Transform cam;
    private Camera camComponent;
    private SpriteRenderer sr;
    private Material mat;
    private Vector2 startPos;
    private float textureUnitSizeX;
    private float textureUnitSizeY;
    private float originalSizeY;
    private float lastOrthoSize;

    private void Start()
    {
        camComponent = Camera.main;
        cam = camComponent.transform;

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("ParallaxBackground: No SpriteRenderer found.", this);
            enabled = false;
            return;
        }

        float ppu = sr.sprite.pixelsPerUnit;
        textureUnitSizeX = sr.sprite.texture.width / ppu;
        textureUnitSizeY = sr.sprite.texture.height / ppu;
        originalSizeY = sr.size.y;

        sr.drawMode = SpriteDrawMode.Tiled;
        UpdateTiling();

        mat = sr.material;
        startPos = transform.position;
    }

    private void UpdateTiling()
    {
        float viewportHeight = camComponent.orthographicSize * 2f;
        float viewportWidth = viewportHeight * camComponent.aspect;

        float tilingX = Mathf.Max(Mathf.Ceil(viewportWidth / textureUnitSizeX * coverageBuffer), 2f);

        if (tilingMode == TilingMode.BothAxes)
        {
            float tilingY = Mathf.Max(Mathf.Ceil(viewportHeight / textureUnitSizeY * coverageBuffer), 2f);
            sr.size = new Vector2(textureUnitSizeX * tilingX, textureUnitSizeY * tilingY);
        }
        else
        {
            sr.size = new Vector2(textureUnitSizeX * tilingX, originalSizeY);
        }

        lastOrthoSize = camComponent.orthographicSize;
    }

    private void LateUpdate()
    {
        if (!Mathf.Approximately(camComponent.orthographicSize, lastOrthoSize))
        {
            UpdateTiling();
        }

        Vector2 cameraDelta = (Vector2)cam.position - startPos;

        float posX = startPos.x + cameraDelta.x * parallaxFactorX;
        float posY = startPos.y + cameraDelta.y * parallaxFactorY;

        transform.position = new Vector3(posX, posY, transform.position.z);

        float offsetX = cameraDelta.x * (1f - parallaxFactorX) / textureUnitSizeX;

        if (tilingMode == TilingMode.BothAxes)
        {
            float offsetY = cameraDelta.y * (1f - parallaxFactorY) / textureUnitSizeY;
            mat.mainTextureOffset = new Vector2(offsetX, offsetY);
        }
        else
        {
            mat.mainTextureOffset = new Vector2(offsetX, 0f);
        }
    }

    private void OnDestroy()
    {
        if (mat != null)
        {
            Destroy(mat);
        }
    }
}