using Photon.Pun;
using SnakeGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


///Even though there is no much fucntionality for 
///collectable having a separate script will give more control on it .
public class Collectable : MonoBehaviour
{
    #region Variables
    #endregion Variables

    #region Unity Methods

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "SnakeHead")
    //    {
    //        var view = other.GetComponent<PhotonView>();
    //        if (view.IsMine)
    //        {
    //            view.RPC("_OnFoodCollected", RpcTarget.All);
    //            Debug.Log($"!!!Food Collected..");
    //        }
    //        GlobalEventHandler.TriggerEvent(EventID.EVENT_FOOD_COLLECTED);
    //        gameObject.SetActive(false);
    //    }
    //}

    #endregion Unity Methods

    #region Public Methods

    #endregion Public Methods

    #region Private Methods

    #endregion Private Methods

    #region Callbacks

    #endregion Callbacks
}
