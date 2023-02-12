using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using SnakeGame;
using SnakeGame.Enums;
using Photon.Pun;
using Photon.Realtime;
using System;
using Photon.Pun.UtilityScripts;
using System.Linq;

public class InGameUIManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject m_masterClientScorePanel;
    [SerializeField] private GameObject m_opponentScorePanel;
    [SerializeField] private TextMeshProUGUI m_masterScoretxt;
    [SerializeField] private TextMeshProUGUI m_opponentScoretxt;
    private short m_scoreCount = 0;
    #endregion Variables

    #region Unity Methods
    private void OnEnable()
    {
        GlobalEventHandler.AddListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Item_Collected);
        GlobalEventHandler.AddListener(EventID.EVENT_ON_MASTER_CLIENT_PROPERTIES_UPDATED, Callback_On_Master_Properties_Updated);
        GlobalEventHandler.AddListener(EventID.EVENT_ON_OPPONENT_PLAYER_PROPERTIES_UPDATED, Callback_On_Opponent_Player_Properties_Updated);
    }
    private void OnDisable()
    {
        GlobalEventHandler.RemoveListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Item_Collected);
        GlobalEventHandler.RemoveListener(EventID.EVENT_ON_MASTER_CLIENT_PROPERTIES_UPDATED, Callback_On_Master_Properties_Updated);
        GlobalEventHandler.RemoveListener(EventID.EVENT_ON_OPPONENT_PLAYER_PROPERTIES_UPDATED, Callback_On_Opponent_Player_Properties_Updated);
    }
    #endregion Unity Methods
    private void Awake()
    {
        switch (GlobalVariables.currentGameMode)
        {
            case GameMode.SinglePlayer:
                m_opponentScorePanel.SetActive(false);
                break;
            case GameMode.MultiPlayer:
                break;
            default:
                break;
        }
    }
    private void Start()
    {
        _Init();
    }
    #region Public Methods

    #endregion Public Methods

    #region Private Methods
    private void _Init()
    {
        var playerList = PhotonNetwork.PlayerList;
        foreach (Player player in playerList)
        {
            player.SetScore(0);
        }
    }
    public void _OnFoodItemCollected()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.MasterClient.AddScore(1);
            m_masterScoretxt.SetText((PhotonNetwork.MasterClient.GetScore() + 1).ToString());
            Debug.Log($"score: {PhotonNetwork.MasterClient.GetScore()}");
        }
        else
        {
            //If it doesn't work try getting all players list and removing master client
            PhotonNetwork.LocalPlayer.AddScore(1);
            m_opponentScoretxt.SetText((PhotonNetwork.LocalPlayer.GetScore() + 1).ToString());
        }
    }
    private void Callback_On_Master_Properties_Updated(object args)
    {

    }
    private void Callback_On_Opponent_Player_Properties_Updated(object args)
    {

    }

    #endregion Private Methods

    #region Callbacks
    private void Callback_On_Food_Item_Collected(object args)
    {

        _OnFoodItemCollected();
    }
    #endregion Callbacks
}
