using System.Collections.Generic;
using UnityEngine;

public class CollectedItemsStore : MonoBehaviour
{
    public static CollectedItemsStore Instance { get; private set; }

    private HashSet<string> collectedIDs = new HashSet<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public bool IsCollected(string id)
    {
        return collectedIDs.Contains(id);
    }
    public void MarkCollected(string id)
    {
        collectedIDs.Add(id);
    }
    public void ResetAll()
    {
        collectedIDs.Clear();
    }
}