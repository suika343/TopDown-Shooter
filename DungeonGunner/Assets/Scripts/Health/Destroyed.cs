using UnityEngine;

[RequireComponent(typeof(DestroyedEvent))]
[DisallowMultipleComponent]
public class Destroyed : MonoBehaviour
{
    private DestroyedEvent destroyedEvent;
    private void Awake()
    {
        destroyedEvent = GetComponent<DestroyedEvent>();
    }

    private void OnEnable()
    {
        destroyedEvent.OnDestroyed += DestroyedEvent_OnDestroy;
    }

    private void OnDisable()
    {
        destroyedEvent.OnDestroyed -= DestroyedEvent_OnDestroy;
    }

    private void DestroyedEvent_OnDestroy(DestroyedEvent destroyedEvent)
    {
        Destroy(gameObject);
    }
}
