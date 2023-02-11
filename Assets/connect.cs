using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class connect : MonoBehaviourPunCallbacks
{
    void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();

    }
    public override void OnConnected()
    {
        base.OnConnected();
        Debug.Log($"Connected....");
    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log($"Connected to master server....");

    }
}
