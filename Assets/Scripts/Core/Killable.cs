using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
public abstract class Killable : BaseGameObject
{
    public virtual IEnumerable<IEnumerable<Action>> Kill()
    {
        yield return TimeYields.WaitOneFrameX;
    }
}

