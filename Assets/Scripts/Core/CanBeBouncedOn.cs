using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class CanBeBouncedOn : BaseGameObject
    {
        public LichtPhysicsObject PhysicsObject;

        protected override void OnAwake()
        {
            base.OnAwake();
            PhysicsObject.AddCustomObject(this);
        }
    }
}
