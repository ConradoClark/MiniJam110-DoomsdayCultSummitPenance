using System;
using System.Collections.Generic;
using Licht.Unity.Objects;

namespace Assets.Scripts.UI
{
    public abstract class MenuButtonFunction : BaseUIObject
    {
        public abstract IEnumerable<IEnumerable<Action>> Execute();
    }
}
