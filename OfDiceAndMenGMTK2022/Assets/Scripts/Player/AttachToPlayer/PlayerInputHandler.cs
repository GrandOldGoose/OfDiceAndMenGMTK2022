using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    #region Fields
    private float _rawMoveInput = 0f;

    private bool _jumpInput = false;
    private bool _fallThroughFloorInput = false;
    private bool _fireInput = false;
    #endregion



    #region Properties
    public float RawMoveInput { get => _rawMoveInput; }
    public bool JumpInput { get => _jumpInput; }
    public bool FallThroughFloorInput { get => _fallThroughFloorInput; }
    public bool FireInput { get => _fireInput; set => _fireInput = value; }
    #endregion


    #region Unity Event Callers
    public void OnMoveInput(InputAction.CallbackContext context)
    {
        /*
        if (!PauseMenu.GameIsPaused)
        {
        */
            _rawMoveInput = Mathf.RoundToInt(context.ReadValue<float>());
        /*
        }
        */
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        /*
        if (!PauseMenu.GameIsPaused)
        {
        */
            if (context.started)
            {
                _jumpInput = true;
            }
            /*
        }
            */
    }

    public void OnFallThroughFloorInput(InputAction.CallbackContext context)
    {
        /*
        if (!PauseMenu.GameIsPaused)
        {
        */
            if (context.performed)
            {
                _fallThroughFloorInput = true;
            }
            if (context.canceled)
            {
                _fallThroughFloorInput = false;
            }
            /*
        }
            */
    }
    #endregion
}
