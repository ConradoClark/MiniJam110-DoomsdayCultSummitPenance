using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class CameraFollow : BaseGameObject
{
    public float MinX;
    public float MaxX;
    public float MinY;
    public float MaxY;

    public ScriptBasicMachinery PostUpdate;
    public Camera Camera;

    private Player _player;
    protected override void OnAwake()
    {
        base.OnAwake();
        _player = SceneObject<Player>.Instance();
    }

    private void OnEnable()
    {
        PostUpdate.Machinery.AddBasicMachine(HandleCameraFollow());
    }

    private IEnumerable<IEnumerable<Action>> HandleCameraFollow()
    {
        while (isActiveAndEnabled)
        {
            Camera.transform.position = new Vector3(
                Mathf.Clamp(_player.transform.position.x, MinX, MaxX),
                Mathf.Clamp(_player.transform.position.y, MinY, MaxY),
                Camera.transform.position.z);

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
