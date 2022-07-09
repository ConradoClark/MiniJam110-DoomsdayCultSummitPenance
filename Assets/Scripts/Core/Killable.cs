using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Core;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
public abstract class Killable : Resettable
{
    public virtual IEnumerable<IEnumerable<Action>> Kill()
    {
        yield return TimeYields.WaitOneFrameX;
    }
}

