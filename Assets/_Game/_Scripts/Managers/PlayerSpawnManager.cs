using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnManager : MonoBehaviour
{
    #region Variables
    [SerializeField] private GameObject snakePrefab;
    [SerializeField] private Transform masterClientSpawnPoint;
    [SerializeField] private Transform newPlayerSpawnPoint;

    private bool isPlayerInstantiated = false;
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        //PhotonNetwork.AutomaticallySyncScene = true;
        SpawnPlayer();
    }
    #endregion Unity Methods

    #region Public Methods
    public void SpawnPlayer()
    {
        if (isPlayerInstantiated) return;
        PhotonNetwork.Instantiate(snakePrefab.name, PhotonNetwork.IsMasterClient ? masterClientSpawnPoint.position : newPlayerSpawnPoint.position, Quaternion.identity);
        isPlayerInstantiated = true;
    }
    #endregion Public Methods

    #region Private Methods

    #endregion Private Methods

    #region Callbacks

    #endregion Callbacks
}
