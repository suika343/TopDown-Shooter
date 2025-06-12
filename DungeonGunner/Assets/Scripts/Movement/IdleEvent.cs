using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class IdleEvent : MonoBehaviour
{
    public event Action<IdleEvent> OnIdle;

    public void CallIdleEvent()
    {
        OnIdle?.Invoke(this);
    }

}
