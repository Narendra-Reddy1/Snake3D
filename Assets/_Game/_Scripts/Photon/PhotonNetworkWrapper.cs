using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using SnakeGame;
using SnakeGame.Enums;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun.UtilityScripts;

public class PhotonNetworkWrapper : MonoBehaviourPunCallbacks
{
    #region Variables

    #endregion Variables

    #region Unity Methods

    public override void OnEnable()
    {
        base.OnEnable();
        GlobalEventHandler.AddListener(EventID.REQUEST_PHOTON_TO_CONNECT_MASTER_SERVER, Callback_On_Connect_To_Master_Requested);
        GlobalEventHandler.AddListener(EventID.REQUEST_PHOTON_TO_CREATE_ROOM, Callback_On_Create_Room_Requested);
        GlobalEventHandler.AddListener(EventID.REQUEST_PHOTON_TO_JOIN_ROOM, Callback_On_Join_Room_Requested);
        GlobalEventHandler.AddListener(EventID.REQUEST_TO_LOAD_MAIN_SCENE, Callback_On_Load_Main_Scene_Requested);
    }
    public override void OnDisable()
    {
        base.OnDisable();
        GlobalEventHandler.RemoveListener(EventID.REQUEST_PHOTON_TO_CONNECT_MASTER_SERVER, Callback_On_Connect_To_Master_Requested);
        GlobalEventHandler.RemoveListener(EventID.REQUEST_PHOTON_TO_CREATE_ROOM, Callback_On_Create_Room_Requested);
        GlobalEventHandler.RemoveListener(EventID.REQUEST_PHOTON_TO_JOIN_ROOM, Callback_On_Join_Room_Requested);
        GlobalEventHandler.RemoveListener(EventID.REQUEST_TO_LOAD_MAIN_SCENE, Callback_On_Load_Main_Scene_Requested);
    }
    private void OnDestroy()
    {
        PhotonNetwork.Disconnect();
    }
    #endregion Unity Methods

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log($"ConnectedToMaster");
        GlobalEventHandler.TriggerEvent(EventID.EVENT_PHOTON_CONNECTED_TO_MASTER_SERVER);
    }
    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log($"Connected........");
    }
    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined Room..");
        GlobalVariables.currentGameMode = PhotonNetwork.CurrentRoom.MaxPlayers > 1 ? GameMode.MultiPlayer : GameMode.SinglePlayer;
        GlobalEventHandler.TriggerEvent(EventID.EVENT_ON_PLAYER_JOINED_ROOM);
        switch (GlobalVariables.currentGameMode)
        {
            case GameMode.SinglePlayer:
            case GameMode.MultiPlayer:
                CheckForMinimumPlayersToStartGame();
                break;
        }
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"New player entered room: {newPlayer.NickName}");
        switch (GlobalVariables.currentGameMode)
        {
            case GameMode.SinglePlayer:
            case GameMode.MultiPlayer:
                CheckForMinimumPlayersToStartGame();
                break;
        }
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($" other Player exited room: {otherPlayer.NickName}");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"On Joined Room Failed: {message}");
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        if (targetPlayer.IsMasterClient)
            GlobalEventHandler.TriggerEvent(EventID.EVENT_ON_MASTER_CLIENT_PROPERTIES_UPDATED, changedProps);
        else
            GlobalEventHandler.TriggerEvent(EventID.EVENT_ON_OPPONENT_PLAYER_PROPERTIES_UPDATED, changedProps);
    }
    #endregion

    #region Private Methods

    private void ConnectToPhotonServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    private void CreateRoom(CreateRoomSettings roomSettings)
    {
        PhotonNetwork.JoinOrCreateRoom(roomSettings.roomID, roomSettings.roomOptions, TypedLobby.Default);
    }
    private void JoinRoom(string roomID)
    {
        PhotonNetwork.JoinRoom(roomID);
    }
    private void CheckForMinimumPlayersToStartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log($"JOINED REQUIRED players: {PhotonNetwork.CurrentRoom.PlayerCount}");
            //Start the game
            GlobalEventHandler.TriggerEvent(EventID.EVENT_ON_TOGGLE_WAITING_FOR_PLAYERS_PANEL, false);
            GlobalEventHandler.TriggerEvent(EventID.REQUEST_TO_LOAD_MAIN_SCENE);
        }
        else
        {
            GlobalEventHandler.TriggerEvent(EventID.EVENT_ON_TOGGLE_WAITING_FOR_PLAYERS_PANEL, true);
            //Show waiting for other players popup.....
        }
    }
    #endregion

    #region Callbacks
    private void Callback_On_Connect_To_Master_Requested(object args)
    {
        ConnectToPhotonServer();
    }
    private void Callback_On_Create_Room_Requested(object args)
    {
        CreateRoom((CreateRoomSettings)args);
    }
    private void Callback_On_Join_Room_Requested(object args)
    {
        JoinRoom((string)args);
    }
    private void Callback_On_Load_Main_Scene_Requested(object args)
    {
        PhotonNetwork.LoadLevel(Constants.MAIN_SCENE);
    }
    #endregion Callbacks
}
public struct CreateRoomSettings
{
    public string roomID;
    public RoomOptions roomOptions;

    public CreateRoomSettings(string id, RoomOptions options)
    {
        roomID = id;
        roomOptions = options;
    }
}