using System;
using System.Collections.Generic;
using Assets.Scripts.Core;
using Licht.Impl.Orchestration;

public abstract class Killable : Resettable
{
    public virtual IEnumerable<IEnumerable<Action>> Kill()
    {
        yield return TimeYields.WaitOneFrameX;
    }
}

