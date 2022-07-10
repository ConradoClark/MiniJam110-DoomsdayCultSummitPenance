using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;

namespace Assets.Scripts.UI
{
    public abstract class MenuButtonFunction : BaseUIObject
    {
        public abstract IEnumerable<IEnumerable<Action>> Execute();
    }
}
