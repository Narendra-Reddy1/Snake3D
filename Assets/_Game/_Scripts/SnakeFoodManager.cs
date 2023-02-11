using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.Random;

namespace SnakeGame
{
    public class SnakeFoodManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private GameObject m_foodPrefab;
        [SerializeField] private float minPoseX;
        [SerializeField] private float minPoseY;
        [SerializeField] private float maxPoseX;
        [SerializeField] private float maxPoseY;
        private bool isStaticFoodItem = true;
        private bool isFoodSpawned = false;
        private const string kStaticFoodPool = "staticFoodPool";
        private const string kDynamicFoodPool = "dynamicFoodPool";
        #endregion Variables

        #region Unity Methods
        private void Awake()
        {
            _Init();
        }
        private void Start()
        {
            _SpawnFoodItem();
        }
        private void OnEnable()
        {
            GlobalEventHandler.AddListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Collected);
        }
        private void OnDisable()
        {
            GlobalEventHandler.RemoveListener(EventID.EVENT_FOOD_COLLECTED, Callback_On_Food_Collected);
            StopAllCoroutines();
        }

        #endregion Unity Methods

        #region Public Methods

        #endregion Public Methods

        #region Private Methods

        private void _Init()
        {
            PoolHandler.instance.CreatePool(kStaticFoodPool, 3, m_foodPrefab, transform);
            PoolHandler.instance.CreatePool(kDynamicFoodPool, 3, m_foodPrefab, transform);
            Debug.Log($"Created pools");
        }
        private void _SpawnFoodItem()
        {
            if (isFoodSpawned) return;
            GameObject food = PoolHandler.instance.SpawnElementFromPool(kStaticFoodPool, m_foodPrefab.name);
            food.transform.position = GetPositionForFood();
            isFoodSpawned = food != null;
            //yield return new WaitForSeconds(0.25f);
            food.SetActive(true);
            Debug.Log($"!!!old is static: {isStaticFoodItem}");
            if (!isStaticFoodItem)
            {
                TweenFoodItem(food.transform);
            }
            Debug.Log($"!!!Food Item spawnned: {food != null} is static: {isStaticFoodItem}");
        }
        private void TweenFoodItem(Transform foodItem)
        {
            DOTween.Kill(foodItem);
            //o-Horizontal
            //1-Vertical
            Vector3 foodPose = foodItem.transform.position;
            Vector3 destPose = foodPose;
            int range = Range(1, 100);
            switch (Random.Range(0, 10))
            {
                case 0:
                case 2:
                case 4:
                case 6:
                case 8:
                    foodPose.z = range % 2 == 0 ? minPoseY : maxPoseY;
                    foodItem.transform.position = foodPose;
                    destPose.z = range % 2 == 0 ? maxPoseY : minPoseY;
                    break;
                case 1:
                case 3:
                case 5:
                case 7:
                case 9:

                    foodPose.x = range % 2 == 0 ? minPoseX : maxPoseX;
                    foodItem.transform.position = foodPose;
                    destPose.x = range % 2 == 0 ? maxPoseX : minPoseX;
                    break;
                default:
                    Debug.LogError($"Incorrent Orientation for FoodItem tweening...");
                    break;
            }
            foodItem.transform.DOMove(destPose, 4f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
        private Vector3 GetPositionForFood()
        {
            Vector3 foodPose = Vector3.zero;
            foodPose = new Vector3(Range(minPoseX, maxPoseX), 0.85f, Range(minPoseY, maxPoseY));// 0.85 is the offset Y value for current map. can use a variable to tweek it
            return foodPose;
        }
        #endregion Private Methods

        #region Callbacks
        private void Callback_On_Food_Collected(object args)
        {
            isFoodSpawned = false;
            isStaticFoodItem = !isStaticFoodItem;
            _SpawnFoodItem();
            Debug.Log($"!!! CAlbback on food collected");
        }
        #endregion Callbacks
    }
}