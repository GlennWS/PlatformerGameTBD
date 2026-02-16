using UnityEngine;

public class PlayerAbility
{
    protected PlayerController pc;
    protected Rigidbody2D rb;
    public bool IsActive { get; protected set; } = false;
    [SerializeField] private bool isUnlockedField = true;

    public bool IsUnlocked
    {
        get { return isUnlockedField; }
        set { isUnlockedField = value; }
    }

    public virtual void Initialize(PlayerController playerController)
    {
        pc = playerController;
        rb = pc.rb;
    }

    protected void RestoreGravity()
    {
        rb.gravityScale = pc.baseGravity;
    }
}
