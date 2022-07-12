using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class Parallax : MonoBehaviour
{
    public Camera Camera;
    public float ParallaxSpeed;
    public ScriptBasicMachinery PostUpdate;
    public SpriteRenderer SpriteRenderer;

    private void OnEnable()
    {
        PostUpdate.Machinery.AddBasicMachine(HandleParallax());
    }

    private IEnumerable<IEnumerable<Action>> HandleParallax()
    {
        while (isActiveAndEnabled)
        {
            SpriteRenderer.material.SetFloat("_HScroll", Camera.transform.position.x * -0.01f * ParallaxSpeed);
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
