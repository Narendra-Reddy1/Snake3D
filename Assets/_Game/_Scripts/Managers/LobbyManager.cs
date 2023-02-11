using Photon.Realtime;
using SnakeGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private TMPro.TMP_InputField roomIdInputField;
    #endregion Variables

    #region Unity Methods

    #endregion Unity Methods

    #region Public Methods

    public void OnClickSinglePlayerBtn()
    {

    }
    public void OnClickMultiplayerBtn()
    {

    }
    public void OnClickCreateRoomBtn()
    {
        if (string.IsNullOrEmpty(roomIdInputField.text))
        {
            //Throw some error
            return;
        }
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = Constants.MAX_PLAYERS_PER_ROOM;
        GlobalEventHandler.TriggerEvent(EventID.REQUEST_PHOTON_TO_CREATE_ROOM, new CreateRoomSettings(roomIdInputField.text, options));
    }
    public void OnClickJoinRoomBtn()
    {
        if (string.IsNullOrEmpty(roomIdInputField.text))
        {
            //Throw some error
            return;
        }
        GlobalEventHandler.TriggerEvent(EventID.REQUEST_PHOTON_TO_JOIN_ROOM, roomIdInputField.text);
    }
    #endregion Public Methods

    #region Private Methods

    #endregion Private Methods

    #region Callbacks

    #endregion Callbacks
}
