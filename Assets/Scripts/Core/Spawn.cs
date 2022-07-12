using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Core;
using Assets.Scripts.Mechanics;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

[DefaultExecutionOrder(9999)]
public class Spawn : BaseGameObject
{
    public Animator Animator;
    public ScriptIdentifier SpawnIdentifier;
    public LichtPhysicsObject PhysicsObject;
    public SacrificeType SacrificeType;

    public bool Respawn = true;
    public bool HasBounds;
    public float MinY;
    public float MaxY;
    public float MinX;
    public float MaxX;

    public bool IsSacrificed { get; private set; }
    private Vector3 _initialPosition;
    private Resettable[] _resets;
    private Camera _camera;

    protected override void OnAwake()
    {
        _initialPosition = transform.position;
        _camera = Camera.main;
        base.OnAwake();
        PhysicsObject.AddCustomObject(this);
        _resets = GetComponents<Resettable>().Concat(GetComponentsInChildren<Resettable>(true)).Distinct().ToArray();

        gameObject.SetActive(false);

        SceneObject<SpawnCollection>.Instance().Add(this);

        DefaultMachinery.AddBasicMachine(HandleActivation());
    }

    private IEnumerable<IEnumerable<Action>> HandleActivation()
    {
        while (Vector2.Distance(_camera.transform.position, transform.position) > 7.5f)
        {
            yield return TimeYields.WaitOneFrameX;
        }
        gameObject.SetActive(true);
    }

    private void OnEnable()
    {
        if (Respawn) DefaultMachinery.AddBasicMachine(HandleRespawn());
    }

    public void MarkAsSacrifice()
    {
        IsSacrificed = true;
        if (Animator == null) return;
        DefaultMachinery.AddBasicMachine(DisablePhysicsObject().Combine(TriggerSacrificeAnim()));
    }

    private IEnumerable<IEnumerable<Action>> TriggerSacrificeAnim()
    {
        while (!Animator.isActiveAndEnabled)
        {
            yield return TimeYields.WaitOneFrameX;
            Animator.SetTrigger("Sacrifice");
        }
    }

    private IEnumerable<IEnumerable<Action>> DisablePhysicsObject()
    {
        yield return TimeYields.WaitMilliseconds(GameTimer, 200);
        PhysicsObject.enabled = false;
    }

    private IEnumerable<IEnumerable<Action>> HandleRespawn()
    {
        while (isActiveAndEnabled && !IsSacrificed) 
        {
            if ((Vector2.Distance(_camera.transform.position, transform.position) > 8f &&
                Vector2.Distance(_initialPosition, transform.position) > 8f && 
                Vector2.Distance(_initialPosition, _camera.transform.position) > 8f) ||
                HasBounds && (transform.position.y > MaxY || transform.position.y < MinY ||
                              transform.position.x < MinX || transform.position.x > MaxX))
            {
                transform.position = _initialPosition;
                foreach (var reset in _resets)
                {
                    reset.PerformReset();
                }
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }
}

