using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ParallaxBackground : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Parallax")]
    [Range(0f, 1f)]
    [SerializeField] private float parallaxEffectX = 0.5f;
    [Range(0f, 1f)]
    [SerializeField] private float parallaxEffectY = 0f;

    [Header("Looping")]
    [SerializeField] private bool loopHorizontal = true;
    [SerializeField] private bool loopVertical = false;

    [Header("Auto Setup")]
    [SerializeField] private bool autoSetupChildren = true;

    private float startPosX;
    private float startPosY;
    private float spriteWidth;
    private float spriteHeight;

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        startPosX = transform.position.x;
        startPosY = transform.position.y;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        spriteWidth = sr.bounds.size.x;
        spriteHeight = sr.bounds.size.y;

        if (autoSetupChildren)
        {
            CreateDuplicates(sr);
        }
    }

    private void Update()
    {
        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null) return;
        }

        float distX = cam.transform.position.x * parallaxEffectX;
        float tempX = cam.transform.position.x * (1 - parallaxEffectX);

        transform.position = new Vector3(
            startPosX + distX,
            startPosY + cam.transform.position.y * parallaxEffectY,
            transform.position.z
        );

        if (loopHorizontal)
        {
            if (tempX > startPosX + spriteWidth)
            {
                startPosX += spriteWidth;
            }
            else if (tempX < startPosX - spriteWidth)
            {
                startPosX -= spriteWidth;
            }
        }

        if (loopVertical)
        {
            float tempY = cam.transform.position.y * (1 - parallaxEffectY);

            if (tempY > startPosY + spriteHeight)
            {
                startPosY += spriteHeight;
            }
            else if (tempY < startPosY - spriteHeight)
            {
                startPosY -= spriteHeight;
            }
        }
    }

    private void CreateDuplicates(SpriteRenderer sr)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        if (loopHorizontal)
        {
            CreateCopy(sr, new Vector3(-spriteWidth, 0f, 0f), "Left");
            CreateCopy(sr, new Vector3(spriteWidth, 0f, 0f), "Right");
        }

        if (loopVertical)
        {
            CreateCopy(sr, new Vector3(0f, spriteHeight, 0f), "Top");
            CreateCopy(sr, new Vector3(0f, -spriteHeight, 0f), "Bottom");
        }

        if (loopHorizontal && loopVertical)
        {
            CreateCopy(sr, new Vector3(-spriteWidth, spriteHeight, 0f), "TopLeft");
            CreateCopy(sr, new Vector3(spriteWidth, spriteHeight, 0f), "TopRight");
            CreateCopy(sr, new Vector3(-spriteWidth, -spriteHeight, 0f), "BottomLeft");
            CreateCopy(sr, new Vector3(spriteWidth, -spriteHeight, 0f), "BottomRight");
        }
    }

    private void CreateCopy(SpriteRenderer original, Vector3 localOffset, string suffix)
    {
        GameObject copy = new GameObject(original.name + "_" + suffix);
        copy.transform.SetParent(transform, false);
        copy.transform.localPosition = localOffset;

        SpriteRenderer copySr = copy.AddComponent<SpriteRenderer>();
        copySr.sprite = original.sprite;
        copySr.sortingLayerID = original.sortingLayerID;
        copySr.sortingOrder = original.sortingOrder;
        copySr.color = original.color;
        copySr.flipX = original.flipX;
        copySr.flipY = original.flipY;
    }
}