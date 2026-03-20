using System.Collections;
using UnityEngine;

public class MaterializeEffect : MonoBehaviour
{
    public IEnumerator MaterializeCoroutine(Shader materializeShader, Color materializeColor, float materializeEffectTime,
         SpriteRenderer[] spriteRendererArray, Material normalMaterial)
    {
        Material materializeMaterial = new Material(materializeShader);
        materializeMaterial.SetColor("_EmissionColor", materializeColor);

        //set the material of all the sprite renderers to the materialize material
        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = materializeMaterial;
        }

        float dissolveAmount = 0;

        //materialize effect
        while (dissolveAmount < 1)
        {
            dissolveAmount += Time.deltaTime / materializeEffectTime;
            materializeMaterial.SetFloat("_DissolveAmount", dissolveAmount);
            yield return null;
        }

        //set the material of all the sprite renderers to the normal material
        foreach (SpriteRenderer spriteRenderer in spriteRendererArray)
        {
            spriteRenderer.material = normalMaterial;
        }

    }
}