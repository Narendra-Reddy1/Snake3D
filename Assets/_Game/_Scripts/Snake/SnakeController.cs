using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SnakeGame.Enums;
using System;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace SnakeGame
{
    public class SnakeController : MonoBehaviour
    {
        #region Variables

        [SerializeField] private MoveDirection m_direction;

        [SerializeField] private List<Rigidbody> snakeNodes;
        [SerializeField] private GameObject snakeNodePrefab;
        [SerializeField] private float step_Length = 0.2f;
        [SerializeField] private float movementFrequency = 0.1f;
        [SerializeField] private bool canMove = false;
        [SerializeField] private Rigidbody mainRigidbody;
        [SerializeField] private Rigidbody headRigidbody;
        [SerializeField] private PhotonView photonView;
        [SerializeField] private Transform m_transform;
        private List<Vector3> deltaPositions;
        private bool isSnakeCollectedFood = false;
        private float counter = 0;
        #endregion Variables

        #region Unity Methods
        private void Start()
        {
            if (photonView.IsMine)
            {
                _Init();
                InitSnake();
            }
        }
        private void OnEnable()
        {
            GlobalEventHandler.AddListener(EventID.EVENT_ON_SWIPE_DETECTED, Callback_On_Swipe_Detected);
            GlobalEventHandler.AddListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Snake_Collected_Food_Item);
        }
        private void OnDisable()
        {
            GlobalEventHandler.RemoveListener(EventID.EVENT_ON_SWIPE_DETECTED, Callback_On_Swipe_Detected);
            GlobalEventHandler.RemoveListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Snake_Collected_Food_Item);
        }
        private void Update()
        {
            if (photonView.IsMine)
                CheckForFrequencyToMove();
        }
        private void FixedUpdate()
        {
            if (!photonView.IsMine) return;
            if (canMove)
            {
                canMove = false;
                MoveSnake();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "Wall":
                    movementFrequency = -1;
#if UNITY_ANDROID
                    if (GlobalVariables.currentGameMode == GameMode.SinglePlayer)
                    {
                        GlobalEventHandler.TriggerEvent(EventID.REQUEST_NATIVE_ANDROID_ALERT, new NativeAlertProperties("GAME OVER", "Obstacle Hit!!", onCancel: () =>
                        {
                            PhotonNetwork.LeaveRoom();
                            SceneManager.LoadSceneAsync(Constants.LOBBY_SCENE);
                        }));
                    }
                    else
                    {
                        GlobalEventHandler.TriggerEvent(EventID.REQUEST_NATIVE_ANDROID_ALERT, new NativeAlertProperties(photonView.IsMine ? "YOU LOST" : "YOU WIN", photonView.IsMine ? "You Hit Obstacle!!" : "Opponent hit obstacle!!", onCancel: () =>
                                {
                                    PhotonNetwork.LeaveRoom();
                                    SceneManager.LoadSceneAsync(Constants.LOBBY_SCENE);
                                }));

                    }
#endif
                    GlobalEventHandler.TriggerEvent(EventID.EVENT_COLLIDED_TO_OBSTACLE);
                    break;
                case "Snake":
                    movementFrequency = -1;
#if UNITY_ANDROID
                    if (GlobalVariables.currentGameMode == GameMode.SinglePlayer)
                    {
                        GlobalEventHandler.TriggerEvent(EventID.REQUEST_NATIVE_ANDROID_ALERT, new NativeAlertProperties("GAME OVER", "You ate yourself!!", onCancel: () =>
                        {
                            PhotonNetwork.LeaveRoom();
                            SceneManager.LoadSceneAsync(Constants.LOBBY_SCENE);
                        }));
                    }
                    else
                    {
                        GlobalEventHandler.TriggerEvent(EventID.REQUEST_NATIVE_ANDROID_ALERT, new NativeAlertProperties(photonView.IsMine ? "YOU LOST" : "YOU WIN", photonView.IsMine ? "You ate yourself!!" : "Opponent snake ate itself!!", onCancel: () =>
                        {
                            PhotonNetwork.LeaveRoom();
                            SceneManager.LoadSceneAsync(Constants.LOBBY_SCENE);
                        }));

                    }
#endif
                    GlobalEventHandler.TriggerEvent(EventID.EVENT_COLLIDED_TO_OBSTACLE);
                    break;
                case "Food":
                    if (photonView.IsMine)
                    {
                        photonView.RPC("_OnFoodCollected", RpcTarget.All, m_transform.name);
                        Debug.Log($"!!!Food Collected..");
                    }
                    GlobalEventHandler.TriggerEvent(EventID.EVENT_FOOD_COLLECTED);
                    other.gameObject.SetActive(false);
                    break;
            }
        }
        #endregion Unity Methods

        #region Public Methods

        #endregion Public Methods

        #region Private Methods
        private void _Init()
        {
            m_transform = transform;
            deltaPositions = new List<Vector3>
            {
                new Vector3(0f,0f,step_Length),
                new Vector3(0f,0f,-step_Length),
                new Vector3(-step_Length,0f),
                new Vector3(step_Length,0f)
            };
            if (photonView == null) TryGetComponent(out photonView);
            if (snakeNodes == null || snakeNodes.Count <= 0)
                for (int i = 0, count = m_transform.childCount; i < count; i++)
                    snakeNodes.Add(m_transform.GetChild(i).GetComponent<Rigidbody>());
            m_transform.name += photonView.ViewID;
        }
        private void InitSnake()
        {
            switch (m_direction)
            {
                case MoveDirection.DOWN:
                    snakeNodes[1].position = snakeNodes[0].position + new Vector3(0f, 0f, step_Length);
                    snakeNodes[2].position = snakeNodes[0].position + new Vector3(0f, 0f, step_Length * 2f);
                    break;
                case MoveDirection.UP:
                    snakeNodes[1].position = snakeNodes[0].position + new Vector3(0f, 0f, -step_Length);
                    snakeNodes[2].position = snakeNodes[0].position + new Vector3(0f, 0f, -step_Length * 2f);
                    break;
                case MoveDirection.RIGHT:
                    snakeNodes[1].position = snakeNodes[0].position + new Vector3(-step_Length, 0f, 0f);
                    snakeNodes[2].position = snakeNodes[0].position + new Vector3(-step_Length * 2f, 0f, 0f);
                    break;
                case MoveDirection.LEFT:
                    snakeNodes[1].position = snakeNodes[0].position + new Vector3(step_Length, 0f, 0f);
                    snakeNodes[2].position = snakeNodes[0].position + new Vector3(step_Length * 2f, 0f, 0f);
                    break;
            }
        }
        private void SetDirectionBasedOnSwipeDirection(Vector2 swipeVector)
        {
            MoveDirection dir = m_direction;

            if (swipeVector.x > 0)
            {
                dir = MoveDirection.RIGHT;
            }
            else if (swipeVector.x < 0)
            {
                dir = MoveDirection.LEFT;
            }
            else if (swipeVector.y > 0)
            {
                dir = MoveDirection.UP;
            }
            else if (swipeVector.y < 0)
            {
                dir = MoveDirection.DOWN;
            }
            if (IsDirectionIsValid(dir))
            {
                Debug.Log($"curr: {m_direction} newDir: {dir} ");
                m_direction = dir;
            }
        }

        [PunRPC]
        private void _OnFoodCollected(string name)
        {
            //if (!photonView.IsMine) return;
            GameObject snakeNode = PhotonNetwork.InstantiateRoomObject(snakeNodePrefab.name, snakeNodes[snakeNodes.Count - 1].position, Quaternion.identity);
            if (snakeNode == null) return;
            snakeNode.GetComponent<PhotonView>().TransferOwnership(photonView.Owner);
            snakeNode.transform.SetParent(GameObject.Find(name).transform, true);
            snakeNode.SetActive(true);
            snakeNodes.Add(snakeNode.GetComponent<Rigidbody>());
            Debug.Log($"!! Added Node to the snake...{snakeNodes.Count}");
        }
        private void MoveSnake()
        {
            Vector3 deltaPosition = deltaPositions[(int)m_direction];
            Vector3 parentpostion = mainRigidbody.position;
            Vector3 prevPosition;
            mainRigidbody.position += deltaPosition;
            headRigidbody.position += deltaPosition;
            for (int i = 1, count = snakeNodes.Count; i < count; i++)
            {
                prevPosition = snakeNodes[i].position;
                snakeNodes[i].position = parentpostion;
                parentpostion = prevPosition;
            }
            //SnakeGrowing Logic here
            if (isSnakeCollectedFood)
            {
                isSnakeCollectedFood = false;
                // _OnFoodCollected();
            }
        }
        private bool IsDirectionIsValid(MoveDirection newDirection)
        {
            return !((newDirection.Equals(MoveDirection.UP) && m_direction.Equals(MoveDirection.DOWN)) ||
                (newDirection.Equals(MoveDirection.DOWN) && m_direction.Equals(MoveDirection.UP)) ||
                (newDirection.Equals(MoveDirection.LEFT) && m_direction.Equals(MoveDirection.RIGHT)) ||
                (newDirection.Equals(MoveDirection.RIGHT) && m_direction.Equals(MoveDirection.LEFT)));
        }
        private void CheckForFrequencyToMove()
        {
            if (movementFrequency <= -1)
            {
                canMove = false;
                return;
            }
            counter += Time.deltaTime;
            if (counter >= movementFrequency)
            {
                counter = 0;
                canMove = true;
            }
        }
        #endregion Private Methods

        #region Callbacks
        private void Callback_On_Swipe_Detected(object args)
        {
            Vector2 input = (Vector2)args;
            SetDirectionBasedOnSwipeDirection(input);
        }
        private void Callback_On_Snake_Collected_Food_Item(object args)
        {
            // if (!photonView.IsMine) return;
            isSnakeCollectedFood = true;
        }

        #endregion Callbacks



    }
}