using Photon.Pun;
using Photon.Realtime;
using SnakeGame;
using UnityEngine;

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
    }
    public override void OnDisable()
    {
        base.OnDisable();
        GlobalEventHandler.RemoveListener(EventID.REQUEST_PHOTON_TO_CONNECT_MASTER_SERVER, Callback_On_Connect_To_Master_Requested);
        GlobalEventHandler.RemoveListener(EventID.REQUEST_PHOTON_TO_CREATE_ROOM, Callback_On_Create_Room_Requested);
        GlobalEventHandler.RemoveListener(EventID.REQUEST_PHOTON_TO_JOIN_ROOM, Callback_On_Join_Room_Requested);
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
        PhotonNetwork.LoadLevel(Constants.LOBBY_SCENE);
    }
    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log($"Connected........");

    }
    public override void OnJoinedRoom()
    {

    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"New player entered room: {newPlayer.NickName}");
        CheckForMinimumPlayersToStartGame();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($" other Player exited room: {otherPlayer.NickName}");
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log($"On Joined Room Failed: {message}");
    }
    #endregion

    #region Private Methods

    private void CheckForMinimumPlayersToStartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            //Start the game
        }
        else
        {
            //Show waiting for other players popup.....

        }
    }
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