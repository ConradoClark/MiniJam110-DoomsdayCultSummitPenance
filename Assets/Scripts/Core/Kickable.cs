using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Kickable : BaseGameObject
{
    public Collider2D HitBox;
    public LichtPhysicsObject PhysicsObject;
    public Faintable Faintable;
    public Kickable RedirectTo;
    public bool AlwaysKickable;
    public float KickStateDurationInSeconds = 2f;

    public bool WasKickedRecently { get; private set; }
    private bool _isResettingKick;

    protected override void OnAwake()
    {
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
    }

    public bool IsKickable()
    {
        return AlwaysKickable || Faintable != null && Faintable.IsFainted;
    }

    public void SetKicked()
    {
        WasKickedRecently = true;
        DefaultMachinery.AddBasicMachine(ResetKicked());
    }

    private IEnumerable<IEnumerable<Action>> ResetKicked()
    {
        if (_isResettingKick)
        {
            _isResettingKick = false;
            yield return TimeYields.WaitOneFrameX;
        }

        _isResettingKick = true;
        yield return TimeYields.WaitSeconds(GameTimer, KickStateDurationInSeconds);
        WasKickedRecently = false;
        _isResettingKick = false;
    }


}
