using UnityEngine;
using TMPro;

public class ScorePrefab : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(rankText), rankText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(nameText), nameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(levelText), levelText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(scoreText), scoreText);
    }
#endif
    #endregion

}
