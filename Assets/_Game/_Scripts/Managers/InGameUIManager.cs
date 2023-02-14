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
using System.Text;
using UnityEngine.SceneManagement;

public class InGameUIManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject m_masterClientScorePanel;
    [SerializeField] private GameObject m_opponentScorePanel;
    [SerializeField] private GameObject m_timerPanel;
    [SerializeField] private TextMeshProUGUI m_masterScoretxt;
    [SerializeField] private TextMeshProUGUI m_opponentScoretxt;
    [SerializeField] private PhotonView uiView;
    private short m_scoreCount = 0;
    #endregion Variables

    #region Unity Methods

    private void Awake()
    {
        switch (GlobalVariables.currentGameMode)
        {
            case GameMode.SinglePlayer:
                m_opponentScorePanel.SetActive(false);
                m_timerPanel.SetActive(false);
                break;
            case GameMode.MultiPlayer:
                break;
            default:
                break;
        }
        if (uiView.ViewID == 0)
            uiView.ViewID = 99;
    }
    private void OnEnable()
    {
        GlobalEventHandler.AddListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Item_Collected);
        GlobalEventHandler.AddListener(EventID.REQUEST_NATIVE_ANDROID_ALERT, Callback_On_Android_Alert_Requested);
        GlobalEventHandler.AddListener(EventID.EVENT_ON_PLAYER_PROPERTIES_UPDATED, Callback_On_Player_Properties_Updated);
        GlobalEventHandler.AddListener(EventID.EVENT_ON_LEVEL_TIMER_COMPLETE, Callback_On_Level_Timer_Completed);
    }
    private void OnDisable()
    {
        GlobalEventHandler.RemoveListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Item_Collected);
        GlobalEventHandler.RemoveListener(EventID.REQUEST_NATIVE_ANDROID_ALERT, Callback_On_Android_Alert_Requested);
        GlobalEventHandler.RemoveListener(EventID.EVENT_ON_PLAYER_PROPERTIES_UPDATED, Callback_On_Player_Properties_Updated);
        GlobalEventHandler.RemoveListener(EventID.EVENT_ON_LEVEL_TIMER_COMPLETE, Callback_On_Level_Timer_Completed);
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

        if (uiView.IsMine)
        {
            uiView.Owner.AddScore(1);
        }
    }

    private void ShowAndroidNativeAlert(NativeAlertProperties alertProperties)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AlertDialog dialog = new AlertDialog();
            dialog.build(AlertDialog.THEME_DEVICE_DEFAULT_DARK).setTitle(alertProperties.title)
                .setMessage(alertProperties.message)
                .setNegativeButtion(alertProperties.cancelText, alertProperties.onCancel)
                .show();
        }
        else
        {
            alertProperties.onCancel?.Invoke();
        }


    }
    #endregion Private Methods

    #region Callbacks
    private void Callback_On_Food_Item_Collected(object args)
    {
        _OnFoodItemCollected();
    }
    StringBuilder score = new StringBuilder();
    private void Callback_On_Player_Properties_Updated(object args)
    {
        var player = (Player)args;
        uiView.RPC("UpdateToUI", RpcTarget.AllBuffered, player);
    }
    //private void Callback_On_Opponent_Player_Properties_Updated(object args)
    //{
    //    List<Player> playerList = PhotonNetwork.PlayerList.ToList();
    //    playerList.Remove(PhotonNetwork.MasterClient);
    //    m_opponentScoretxt.SetText(playerList[playerList.Count - 1].GetScore().ToString());
    //}
    [PunRPC]
    private void UpdateToUI(Player player)
    {

        score.Clear();
        score.Append(player.GetScore());
        if (player.IsLocal)
            m_masterScoretxt.SetText(score.ToString());
        else
            m_opponentScoretxt.text = score.ToString();
        Debug.Log($"UpdateUI:: isLocal: {player.IsLocal} {score}");
        //else if (GlobalVariables.opponentPlayer != null && !PhotonNetwork.IsMasterClient)
        //    m_opponentScoretxt.SetText(PhotonNetwork.MasterClient.GetScore().ToString());
    }

    private void Callback_On_Android_Alert_Requested(object args)
    {
        ShowAndroidNativeAlert((NativeAlertProperties)args);
    }
    private void Callback_On_Level_Timer_Completed(object args)
    {
        var playerList = PhotonNetwork.PlayerList;
        if (playerList.Length < 2) return;
        if (playerList[0].GetScore() > playerList[1].GetScore())
        {
            Debug.Log($"{playerList[0].ActorNumber} wins...");
            if (Application.platform == RuntimePlatform.Android)
                ShowAndroidNativeAlert(new NativeAlertProperties("TIME UP", playerList[0].IsLocal ? "You Win" : "You Lose", onCancel: () =>
                {
                    PhotonNetwork.LeaveRoom();
                    SceneManager.LoadSceneAsync(Constants.LOBBY_SCENE);
                }));
        }
        else if (playerList[1].GetScore() > playerList[0].GetScore())
        {
            Debug.Log($"{playerList[1].ActorNumber} wins...");
            if (Application.platform == RuntimePlatform.Android)
                ShowAndroidNativeAlert(new NativeAlertProperties("TIME UP", playerList[1].IsLocal ? "You Win" : "You Lose", onCancel: () =>
                {
                    PhotonNetwork.LeaveRoom();
                    SceneManager.LoadSceneAsync(Constants.LOBBY_SCENE);
                }));
        }
        else //same score
        {
            Debug.Log($"{playerList[1].ActorNumber} {playerList[0].ActorNumber} i.e both wins...");
            if (Application.platform == RuntimePlatform.Android)
                ShowAndroidNativeAlert(new NativeAlertProperties("TIME UP", " DRAW GAME Both Win", onCancel: () =>
          {
              PhotonNetwork.LeaveRoom();
              SceneManager.LoadSceneAsync(Constants.LOBBY_SCENE);
          }));

        }

    }
}
#endregion Callbacks
[Serializable]
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