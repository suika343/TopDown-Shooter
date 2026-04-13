using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class HealthUI : MonoBehaviour
{
    private List<GameObject> healthHeartsList = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged += HealthEvent_OnHealthChanged;
    }

    private void OnDisable()
    {
        GameManager.Instance.GetPlayer().healthEvent.OnHealthChanged -= HealthEvent_OnHealthChanged;
    }

    private void HealthEvent_OnHealthChanged(HealthEvent healthEvent, HealthEventArgs healthEventArgs)
    {
        SetHealthBar(healthEventArgs);
    }

    private void ClearHealthBar()
    {
        foreach(GameObject heart in healthHeartsList)
        {
            Destroy(heart);
        }

        healthHeartsList.Clear();
    }
    private void SetHealthBar(HealthEventArgs healthEventArgs)
    {
        ClearHealthBar();

        //instantiate heart image prefabs (5 hearts total)
        int healthHearts = Mathf.CeilToInt(healthEventArgs.healthPercent * 100f / 20f);

        for(int i = 0; i < healthHearts; i++)
        {
            //Instantiate a heart prefab and add it to the health hearts list
            GameObject heart = Instantiate(GameResources.Instance.heartPrefab, transform);
            healthHeartsList.Add(heart);

            //Set Position
            heart.GetComponent<RectTransform>().anchoredPosition = new Vector2(Settings.uiHeartSpacing * i, 0);
        }
    }
}
