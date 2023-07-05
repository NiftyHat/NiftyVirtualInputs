using UnityEngine;
using ETouch = UnityEngine.InputSystem.EnhancedTouch;

public class EnhancedTouchStickInput : MonoBehaviour
{
    [SerializeField] private EnhancedTouchFloatingJoystick _joystick;

    private ETouch.Finger _trackedFinger;
    
    void OnEnable()
    {
        ETouch.EnhancedTouchSupport.Enable();
        ETouch.Touch.onFingerDown += HandleFingerDown;
        ETouch.Touch.onFingerUp += HandleFingerUp;
        ETouch.Touch.onFingerMove += HandleFingerMove;
        _joystick.OnMove += HandleJoystickMoved;
    }

    private void HandleJoystickMoved(Vector2 inputVector)
    {
        //_joystickControllable.SetInput(inputVector);
    }

    private void OnDisable()
    {         
        ETouch.EnhancedTouchSupport.Disable();
        ETouch.Touch.onFingerDown -= HandleFingerDown;
        ETouch.Touch.onFingerUp -= HandleFingerUp;
        ETouch.Touch.onFingerMove -= HandleFingerMove;
        _joystick.OnMove -= HandleJoystickMoved;
    }

    private bool IsOnLeftSideOfScreen(ETouch.Finger touchedFinger)
    {
        return touchedFinger.screenPosition.x <= Screen.width / 2f;
    }
    
    
    private void HandleFingerDown(ETouch.Finger touchedFinger)
    {
        //don't process additional inputs
        if (_trackedFinger == null && IsOnLeftSideOfScreen(touchedFinger))
        {
            _trackedFinger = touchedFinger;
            _joystick.SetStartPosition(touchedFinger);
        }
    }
    
    private void HandleFingerMove(ETouch.Finger movedFinger)
    {
        
        if (_trackedFinger == movedFinger)
        {
            ETouch.Touch currentTouch = _trackedFinger.currentTouch;
            _joystick.SetKnob(currentTouch);
        }
    }


    private void HandleFingerUp(ETouch.Finger touchedFinger)
    {
        //throw new System.NotImplementedException();
        if (_trackedFinger == touchedFinger)
        {
            _trackedFinger = null;
        }
        _joystick.Clear();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
