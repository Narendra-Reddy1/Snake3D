using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SnakeGame.Enums;
using System;

namespace SnakeGame
{
    public class SnakeController : MonoBehaviour
    {


        #region Variables

        [SerializeField] private MoveDirection direction;
        [SerializeField] private List<Rigidbody> snakeNodes;
        [SerializeField] private GameObject snakeNodePrefab;
        [SerializeField] private float step_Length = 0.2f;
        [SerializeField] private float movementFrequency = 0.1f;
        [SerializeField] private bool canMove = false;
        [SerializeField] private Rigidbody mainRigidbody;
        [SerializeField] private Rigidbody headRigidbody;

        private Transform m_transform;
        private List<Vector3> deltaPositions;
        private bool isSnakeCollectedFood = false;
        private bool canCreateNewNodeAtrTail = false;
        private float counter = 0;

        private const byte sizeOfTheSnakeNodesPool = 25;
        private const string kSnakeNodesPool = "snakenodespool";
        #endregion Variables

        #region Unity Methods
        private void Start()
        {
            _Init();
            InitSnake();
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
            CheckForFrequencyToMove();
        }
        private void FixedUpdate()
        {
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
                case "Snake":
                    Debug.LogError($"GAME_OVER");
                    GlobalEventHandler.TriggerEvent(EventID.EVENT_COLLIDED_TO_OBSTACLE);
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
            PoolHandler.instance.CreatePool(kSnakeNodesPool, sizeOfTheSnakeNodesPool, snakeNodePrefab);
            deltaPositions = new List<Vector3>
            {
                new Vector3(0f,0f,step_Length),
                new Vector3(0f,0f,-step_Length),
                new Vector3(-step_Length,0f),
                new Vector3(step_Length,0f)
            };
            if (snakeNodes == null || snakeNodes.Count <= 0)
                for (int i = 0, count = m_transform.childCount; i < count; i++)
                    snakeNodes.Add(m_transform.GetChild(i).GetComponent<Rigidbody>());
        }
        private void InitSnake()
        {
            switch (direction)
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
            MoveDirection dir = direction;

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
                Debug.LogError($"curr: {direction} newDir: {dir} ");
                direction = dir;
            }
        }
        void ForceMove()
        {
            counter = 0;
            canMove = true;
        }
        private void OnFoodCollected()
        {
            GameObject snakeNode = PoolHandler.instance.SpawnElementFromPool(kSnakeNodesPool, snakeNodePrefab.name);
            snakeNode.transform.position = snakeNodes[snakeNodes.Count - 1].position;
            snakeNode.transform.SetParent(transform, true);
            snakeNode.SetActive(true);
            snakeNodes.Add(snakeNode.GetComponent<Rigidbody>());
            Debug.Log($"Added Node to the snake...");
        }
        private void MoveSnake()
        {
            Vector3 deltaPosition = deltaPositions[(int)direction];
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
                OnFoodCollected();
            }
        }
        private bool IsDirectionIsValid(MoveDirection newDirection)
        {
            return !((newDirection.Equals(MoveDirection.UP) && direction.Equals(MoveDirection.DOWN)) ||
                (newDirection.Equals(MoveDirection.DOWN) && direction.Equals(MoveDirection.UP)) ||
                (newDirection.Equals(MoveDirection.LEFT) && direction.Equals(MoveDirection.RIGHT)) ||
                (newDirection.Equals(MoveDirection.RIGHT) && direction.Equals(MoveDirection.LEFT)));
        }
        private void CheckForFrequencyToMove()
        {
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
            isSnakeCollectedFood = true;
        }
        #endregion Callbacks



    }
}