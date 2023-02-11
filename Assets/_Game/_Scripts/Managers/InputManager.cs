using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SnakeGame
{
    public class InputManager : MonoBehaviour
    {
        #region Variables
        private ControlHub m_controlHub;
        #endregion Variables

        #region Unity Methods
        private void Awake()
        {
            m_controlHub = new ControlHub();
        }
        private void OnEnable()
        {
            m_controlHub.Enable();
            m_controlHub.Player.Movement.performed += _ => ReadInput();
            m_controlHub.Player.Movement.canceled += _ => ReadInput();
        }
        private void OnDisable()
        {
            m_controlHub.Disable();
            m_controlHub.Player.Movement.performed -= _ => ReadInput();
            m_controlHub.Player.Movement.canceled -= _ => ReadInput();

        }
        #endregion Unity Methods

        #region Public Methods

        #endregion Public Methods

        #region Private Methods
        private void ReadInput()
        {
            Vector2 inputVector = (m_controlHub.Player.Movement.ReadValue<Vector2>());
            Vector2 roundedInput = new Vector2(Mathf.RoundToInt(inputVector.x), Mathf.RoundToInt(inputVector.y));
            GlobalEventHandler.TriggerEvent(EventID.EVENT_ON_SWIPE_DETECTED, roundedInput);
            //Debug.Log($"Input Vector: {roundedInput}");
        }
        #endregion Private Methods

        #region Callbacks

        #endregion Callbacks
    }
}