using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

[DisallowMultipleComponent]
public class CharacterSelectorUI : MonoBehaviour
{
    [SerializeField] private Transform characterSelector;
    [SerializeField] private TMP_InputField playerNameInputField;

    private List<PlayerDetailsSO> playerDetailsList;
    private GameObject playerSelectionPrefab;
    private CurrentPlayerSO currentPlayerSO;
    private List<GameObject> playerCharacterObjectList = new List<GameObject>();
    private Coroutine moveCharacterSelectorCoroutine;
    private int selectedPlayerIndex = 0;
    private float characterPosOffset = 4f;

    private void Awake()
    {
        //Load Resources
        playerSelectionPrefab = GameResources.Instance.playerSelectionPrefab;
        playerDetailsList = GameResources.Instance.playerDetailsList;
        currentPlayerSO = GameResources.Instance.currentPlayer;
    }

    private void Start()
    {
        for (int i = 0; i < playerDetailsList.Count; i++)
        {
            GameObject playerSelectionObject = Instantiate(playerSelectionPrefab, characterSelector);
            playerCharacterObjectList.Add(playerSelectionObject);
            playerSelectionObject.transform.localPosition = new Vector3(i * characterPosOffset, 0, 0);
            PopulatePlayerDetails(playerSelectionObject.GetComponent<PlayerSelectionUI>(), playerDetailsList[i]);
        }

        //playerNameInputField.text = currentPlayerSO.name;

        //Initialize the current player
        currentPlayerSO.playerDetails = playerDetailsList[selectedPlayerIndex];
    }

    private void PopulatePlayerDetails(PlayerSelectionUI playerSelectionUI, PlayerDetailsSO playerDetails)
    {
        playerSelectionUI.playerHandSpriteRenderer.sprite = playerDetails.playerHandSprite;
        playerSelectionUI.playerHandNoWeaponSpriteRenderer.sprite = playerDetails.playerHandSprite;
        playerSelectionUI.playerWeaponSpriteRenderer.sprite = playerDetails.startingWeapon.weaponSprite;
        playerSelectionUI.animator.runtimeAnimatorController = playerDetails.runtimeAnimatorController;
    }

    public void NextCharacter()
    {
        if (selectedPlayerIndex >= playerDetailsList.Count - 1)
            return;

        selectedPlayerIndex++;

        currentPlayerSO.playerDetails = playerDetailsList[selectedPlayerIndex];
        MoveToSelectedPlayer(selectedPlayerIndex);
    }

    public void PreviousCharacter()
    {
        if (selectedPlayerIndex <= 0)
            return;

        selectedPlayerIndex--;

        currentPlayerSO.playerDetails = playerDetailsList[selectedPlayerIndex];
        MoveToSelectedPlayer(selectedPlayerIndex);
    }

    private void MoveToSelectedPlayer(int index)
    {
        if(moveCharacterSelectorCoroutine != null)
        {
            StopCoroutine(moveCharacterSelectorCoroutine);
        }

        moveCharacterSelectorCoroutine = StartCoroutine(MoveToSelectedCharacterRoutine(index));
    }

    private IEnumerator MoveToSelectedCharacterRoutine(int index)
    {
        float currentLocalXPosition = characterSelector.localPosition.x;
        float tartLocalXPosition = index * characterPosOffset * characterSelector.localScale.x * -1f;

        while(Mathf.Abs(currentLocalXPosition - tartLocalXPosition) > 0.01f)
        {
            currentLocalXPosition = Mathf.Lerp(currentLocalXPosition, tartLocalXPosition, Time.deltaTime * 10f);

            characterSelector.localPosition = new Vector3(currentLocalXPosition, characterSelector.localPosition.y, 0f);

            yield return null;
        }

        characterSelector.localPosition = new Vector3(tartLocalXPosition, characterSelector.localPosition.y, 0f);
    }

    public void UpdatePlayerName()
    {
        playerNameInputField.text = playerNameInputField.text.ToUpper();

        currentPlayerSO.playerName = playerNameInputField.text;
    }

    #region VALIDATION
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(characterSelector), characterSelector);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerNameInputField), playerNameInputField);
    }
#endif
    #endregion
}
