using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Physics;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public enum HitEvents
    {
        OnHit
    }

    public class OnHitEventArgs
    {
        public LichtPhysicsObject Source;
        public LichtPhysicsObject Target;
    }
}
