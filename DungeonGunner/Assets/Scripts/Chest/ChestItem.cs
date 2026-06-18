using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// purely used for displaying chest items
/// </summary>
[RequireComponent(typeof(MaterializeEffect))]
public class ChestItem : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private TextMeshPro textTMP;
    private MaterializeEffect materializeEffect;
    [HideInInspector] public bool isItemMaterialized = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        textTMP = GetComponent<TextMeshPro>();
        materializeEffect = GetComponent<MaterializeEffect>();
    }

    public void Initialize(Sprite sprite, string text, Vector3 spawnPosition, Color materializeColor)
    {
        spriteRenderer.sprite = sprite;
        transform.position = spawnPosition;

        StartCoroutine(MaterializeItem(materializeColor, text));
    }

    private IEnumerator MaterializeItem(Color materializeColor, string text)
    {
        SpriteRenderer[] spriteRendererArray = new SpriteRenderer[] { spriteRenderer };

        yield return StartCoroutine(materializeEffect.MaterializeCoroutine(GameResources.Instance.materializeShader,
            materializeColor,
            0.75f,
            spriteRendererArray,
            GameResources.Instance.litMaterial));

        isItemMaterialized = true;
        textTMP.text = text;
    }

}
