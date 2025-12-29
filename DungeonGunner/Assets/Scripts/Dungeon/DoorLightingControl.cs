using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class DoorLightingControl : MonoBehaviour
{
    private bool isLit = false;
    private Door door;

    private void Awake()
    {
        door = GetComponentInParent<Door>();
    }

    /// <summary>
    /// Fade In Door
    /// </summary>
    /// <param name="door"></param>
    public void FadeInDoor(Door door)
    {
        //Create a new material to fade in
        Material material = new Material(GameResources.Instance.variableLitShader);

        if (!isLit)
        {
            SpriteRenderer[] spriteRenderers = GetComponentsInParent<SpriteRenderer>();

            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                StartCoroutine(FadeInDoorRoutine(spriteRenderer, material));
            }
            isLit = true;
        }
    }

    /// <summary>
    /// Fade in Door Coroutine
    /// </summary>
    /// <param name="spriteRenderer"></param>
    /// <param name="material"></param>
    /// <returns></returns>
    private IEnumerator FadeInDoorRoutine(SpriteRenderer spriteRenderer, Material material)
    {
        spriteRenderer.material = material;

        for(float i = 0.05f; i <= 1f; i += Time.deltaTime / Settings.fadeInTime)
        {
            material.SetFloat("Alpha_Slider", i);
            yield return null;
        }

        spriteRenderer.material = GameResources.Instance.litMaterial;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        FadeInDoor(door);
    }
}
 