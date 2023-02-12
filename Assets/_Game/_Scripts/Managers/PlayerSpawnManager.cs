using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private Transform masterClientSpawnPoint;
    [SerializeField] private Transform newPlayerSpawnPoint;

    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        Photon.Pun.PhotonNetwork.AutomaticallySyncScene = true;
        SpawnPlayer();
    }
    #endregion Unity Methods

    #region Public Methods
    public void SpawnPlayer()
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
            Photon.Pun.PhotonNetwork.Instantiate(snakePrefab.name, masterClientSpawnPoint.position, Quaternion.identity);
        else
            Photon.Pun.PhotonNetwork.Instantiate(snakePrefab.name, newPlayerSpawnPoint.position, Quaternion.identity);
    }
    #endregion Public Methods

    #region Private Methods

    #endregion Private Methods

    #region Callbacks

    #endregion Callbacks
}
