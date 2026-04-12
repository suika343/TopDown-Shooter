using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyedEvent : MonoBehaviour
{
    public event Action<DestroyedEvent> OnDestroyed;

    public void CallOnDestroyedEvent()
    {
        OnDestroyed?.Invoke(this);
    }
}
