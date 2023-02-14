using Photon.Realtime;
using SnakeGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private TMPro.TMP_InputField m_roomIdInputField;
    [SerializeField] private TMPro.TextMeshProUGUI m_roomIdTxt;
    [SerializeField] private GameObject m_gameModeSelectionPanel;
    [SerializeField] private GameObject m_roomCreateOrJoinPanel;
    [SerializeField] private GameObject m_waitingForOtherPlayersPanel;

    [SerializeField] private GameObject loadinScreen;
    #endregion Variables

    #region Unity Methods

    #endregion Unity Methods
    private void OnEnable()
    {
        ToggleLoadingScreen(!Photon.Pun.PhotonNetwork.IsConnectedAndReady);
        GlobalEventHandler.AddListener(EventID.EVENT_PHOTON_CONNECTED_TO_MASTER_SERVER, Callback_On_Connected_To_Master);
        GlobalEventHandler.AddListener(EventID.EVENT_ON_TOGGLE_WAITING_FOR_PLAYERS_PANEL, Callback_On_Waiting_For_Players_Panel_Toggle_Requested);
    }
    private void OnDisable()
    {
        GlobalEventHandler.RemoveListener(EventID.EVENT_ON_TOGGLE_WAITING_FOR_PLAYERS_PANEL, Callback_On_Waiting_For_Players_Panel_Toggle_Requested);
        GlobalEventHandler.RemoveListener(EventID.EVENT_PHOTON_CONNECTED_TO_MASTER_SERVER, Callback_On_Connected_To_Master);
    }
    #region Public Methods
    public void OnClickSinglePlayerBtn()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 1;
        options.IsVisible = true;
        options.IsOpen = true;
        GlobalEventHandler.TriggerEvent(EventID.REQUEST_PHOTON_TO_CREATE_ROOM, new CreateRoomSettings(Random.Range(999, 9999).ToString(), options));
    }
    public void OnClickMultiplayerBtn()
    {
        ToggleGameModeSelectionPanel(false);
        ToggleRoomCreateOrJoinPanel(true);
    }
    public void OnClickCreateRoomBtn()
    {
        if (string.IsNullOrEmpty(m_roomIdInputField.text))
        {
            //Throw some error
            return;
        }
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = Constants.MAX_PLAYERS_PER_ROOM;
        options.IsVisible = true;
        options.IsOpen = true;
        GlobalEventHandler.TriggerEvent(EventID.REQUEST_PHOTON_TO_CREATE_ROOM, new CreateRoomSettings(m_roomIdInputField.text, options));
    }
    public void OnClickJoinRoomBtn()
    {
        if (string.IsNullOrEmpty(m_roomIdInputField.text))
        {
            //Throw some error
            return;
        }
        GlobalEventHandler.TriggerEvent(EventID.REQUEST_PHOTON_TO_JOIN_ROOM, m_roomIdInputField.text);
    }
    #endregion Public Methods

    #region Private Methods
    private void ToggleRoomCreateOrJoinPanel(bool value)
    {
        m_roomCreateOrJoinPanel.SetActive(value);
    }
    private void ToggleGameModeSelectionPanel(bool value)
    {
        m_gameModeSelectionPanel.SetActive(value);
    }
    private void ToggleWaitingForOtherPlayersPanel(bool value)
    {
        m_roomIdTxt.SetText(Photon.Pun.PhotonNetwork.CurrentRoom.Name);
        m_waitingForOtherPlayersPanel.SetActive(value);
    }
    #endregion Private Methods

    #region Loading Screen

    private void ToggleLoadingScreen(bool value)
    {
        loadinScreen.SetActive(value);
    }
    #endregion Loading Screen

    #region Callbacks
    private void Callback_On_Waiting_For_Players_Panel_Toggle_Requested(object args)
    {
        ToggleWaitingForOtherPlayersPanel((bool)args);
    }
    private void Callback_On_Connected_To_Master(object args)
    {
        ToggleLoadingScreen(false);
    }
    #endregion Callbacks

}
