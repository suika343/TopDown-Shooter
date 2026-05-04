 using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region HEADER GAME OBJECT REFERENCES
    [Space(10)]
    [Header("Game Object References")]
    #endregion

    #region Tooltip
    [Tooltip("The child healthBarContainer gameobject.")]
    #endregion
    [SerializeField] private GameObject healthBarContainer;

    public void EnableHealthBar()
    {
        healthBarContainer.SetActive(true);
    }

    public void DisableHealthBar()
    {
        healthBarContainer.SetActive(false);
    }

    public void SetHealthBarFillAmount(float fillAmount)
    {
        healthBarContainer.transform.localScale = new Vector3(fillAmount, healthBarContainer.transform.localScale.y, healthBarContainer.transform.localScale.z);
    }
}
