using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Core;
using Assets.Scripts.Mechanics;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Update;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

public class Spawn : BaseGameObject
{
    public ScriptIdentifier SpawnIdentifier;
    public LichtPhysicsObject PhysicsObject;
    public SacrificeType SacrificeType;

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
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleRespawn());
    }

    public void MarkAsSacrifice()
    {
        IsSacrificed = true;
    }

    private IEnumerable<IEnumerable<Action>> HandleRespawn()
    {
        while (isActiveAndEnabled && !IsSacrificed)
        {
            if (Vector2.Distance(_camera.transform.position, transform.position) > 10f &&
                Vector2.Distance(_initialPosition, transform.position) > 10f)
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

