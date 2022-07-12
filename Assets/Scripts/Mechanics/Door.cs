using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

namespace Assets.Scripts.Mechanics
{
    public class Door : BaseGameObject
    {
        public float CameraMinX;
        public float CameraMaxX;
        public float CameraMinY;
        public float CameraMaxY;

        public Door TargetDoor;

        public LichtPhysicsObject PhysicsObject;
        public SpriteRenderer DoorSprite;

        private readonly Color _transparent = new Color(0, 0, 0, 0);

        protected override void OnAwake()
        {
            base.OnAwake();
            PhysicsObject.AddCustomObject(this);
        }

        public void Open()
        {
            DoorSprite.material.SetColor("_Colorize", Color.black);

            DefaultMachinery.AddBasicMachine(ChangeColor());
        }

        private IEnumerable<IEnumerable<Action>> ChangeColor()
        {
            yield return TimeYields.WaitSeconds(GameTimer, 0.4f);
            DoorSprite.material.SetColor("_Colorize", _transparent);
        }
    }
}
