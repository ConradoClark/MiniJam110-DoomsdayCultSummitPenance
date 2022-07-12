using Licht.Unity.Objects;
using UnityEngine;

public class SacrificeDoor : BaseGameObject
{
    public LockedBehindSacrifice Demands;
    public SpriteRenderer DoorSprite;
    public Collider2D DoorCollider;

    private void OnEnable()
    {
        if (Demands.IsComplete) gameObject.SetActive(false);
         // DoorSprite.enabled = DoorCollider.enabled = false;

        Demands.OnComplete += Demands_OnComplete;
    }

    private void Demands_OnComplete()
    {
        gameObject.SetActive(false);
     //   DoorSprite.enabled = DoorCollider.enabled =  false;
    }
}
