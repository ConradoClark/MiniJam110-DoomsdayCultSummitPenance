using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Physics;

public enum PickupEvents
{
    OnPickup
}

public class PickupEventArgs
{
    public Pickup Source;
    public Pickupable Target;
}
