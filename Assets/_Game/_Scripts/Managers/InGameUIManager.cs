using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using SnakeGame;

public class InGameUIManager : MonoBehaviour
{
    #region Variables

    [SerializeField] private TextMeshProUGUI scoretxt;
    private short scoreCount = 0;
    #endregion Variables

    #region Unity Methods
    private void OnEnable()
    {
        GlobalEventHandler.AddListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Item_Collected);
    }
    private void OnDisable()
    {
        GlobalEventHandler.RemoveListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Item_Collected);
    }

    #endregion Unity Methods

    #region Public Methods

    #endregion Public Methods

    #region Private Methods
    public void OnFoodItemCollected()
    {
        scoreCount++;
        scoretxt.SetText(scoreCount.ToString());
    }
    #endregion Private Methods

    #region Callbacks
    private void Callback_On_Food_Item_Collected(object args)
    {
        OnFoodItemCollected();
    }
    #endregion Callbacks
}
