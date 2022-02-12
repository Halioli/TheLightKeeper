using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CustomLight : MonoBehaviour
{
    protected enum LightState { NONE, SHRINKING, PARTIAL_SHRINKING, EXPANDING, EXTRA_EXPANDING };


    [SerializeField] protected GameObject lightGameObject;
    protected LightState lightState = LightState.NONE;
    protected bool active = false;

    public virtual void Expand() { }
    public virtual void Shrink() { }
    public virtual void SetIntensity(float intensity) { }
    public virtual void SetDistance(float distance) { }
}
