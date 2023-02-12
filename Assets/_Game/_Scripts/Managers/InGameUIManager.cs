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
using AndroidNativeCore;

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
    private void OnEnable()
    {
        GlobalEventHandler.AddListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Item_Collected);
        GlobalEventHandler.AddListener(EventID.REQUEST_NATIVE_ANDROID_ALERT, Callback_On_Android_Alert_Requested);
        GlobalEventHandler.AddListener(EventID.EVENT_ON_MASTER_CLIENT_PROPERTIES_UPDATED, Callback_On_Master_Properties_Updated);
        GlobalEventHandler.AddListener(EventID.EVENT_ON_OPPONENT_PLAYER_PROPERTIES_UPDATED, Callback_On_Opponent_Player_Properties_Updated);
    }
    private void OnDisable()
    {
        GlobalEventHandler.RemoveListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Item_Collected);
        GlobalEventHandler.RemoveListener(EventID.REQUEST_NATIVE_ANDROID_ALERT, Callback_On_Android_Alert_Requested);
        GlobalEventHandler.RemoveListener(EventID.EVENT_ON_MASTER_CLIENT_PROPERTIES_UPDATED, Callback_On_Master_Properties_Updated);
        GlobalEventHandler.RemoveListener(EventID.EVENT_ON_OPPONENT_PLAYER_PROPERTIES_UPDATED, Callback_On_Opponent_Player_Properties_Updated);
    }

    private void Start()
    {
        _Init();
    }

    #endregion Unity Methods

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
    private void _OnFoodItemCollected()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.MasterClient.AddScore(1);
        }
        else
        {
            //If it doesn't work try getting all players list and removing master client
            PhotonNetwork.LocalPlayer.AddScore(1);
        }
    }

    private void ShowAndroidNativeAlert(NativeAlertProperties alertProperties)
    {
        AlertDialog dialog = new AlertDialog();
        dialog.build(AlertDialog.THEME_DEVICE_DEFAULT_DARK).setTitle(alertProperties.title)
            .setMessage(alertProperties.message)
            .setNegativeButtion(alertProperties.cancelText, alertProperties.onCancel)
            .show();
    }

    #endregion Private Methods

    #region Callbacks
    private void Callback_On_Food_Item_Collected(object args)
    {

        _OnFoodItemCollected();
    }
    private void Callback_On_Master_Properties_Updated(object args)
    {
        m_masterScoretxt.SetText((PhotonNetwork.MasterClient.GetScore()).ToString());
    }
    private void Callback_On_Opponent_Player_Properties_Updated(object args)
    {
        List<Player> playerList = PhotonNetwork.PlayerList.ToList();
        playerList.Remove(PhotonNetwork.MasterClient);
        m_opponentScoretxt.SetText(playerList[playerList.Count - 1].GetScore().ToString());
    }

    private void Callback_On_Android_Alert_Requested(object args)
    {
        ShowAndroidNativeAlert((NativeAlertProperties)args);
    }

    #endregion Callbacks
}
public struct NativeAlertProperties
{
    public string title;
    public string message;
    public string cancelText;
    public AlertDialog.dialogOnClick onCancel;

    public NativeAlertProperties(string title, string message, string cancelTxt = "Okay", AlertDialog.dialogOnClick onCancel = null)
    {
        this.title = title;
        this.message = message;
        cancelText = cancelTxt;
        this.onCancel = onCancel;
    }
}