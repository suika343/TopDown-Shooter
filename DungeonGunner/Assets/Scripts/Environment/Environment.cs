using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[DisallowMultipleComponent]
public class Environment : MonoBehaviour
{
    [Space(10)]
    [Header("References")]
    public SpriteRenderer spriteRenderer;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(spriteRenderer), spriteRenderer);
    }
#endif
    #endregion
}
