
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Update;
using Licht.Unity.Objects;

namespace Assets.Scripts.Core
{
    public class Resettable : BaseGameObject, IResettable
    {
        public virtual bool PerformReset()
        {
            return true;
        }
    }
}
