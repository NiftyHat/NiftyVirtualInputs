using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class EnhancedTouchFloatingJoystick : MonoBehaviour
{
    public delegate void OnMoved(Vector2 position);
    
    [SerializeField] [Range(100, 600)] private float _size;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _knob;

    protected float _halfSize;
    protected Vector2 _movementAmount;

    public event OnMoved OnMove;

    public void Start()
    {
        _halfSize = _size * 0.5f;
    }

    public void SetStartPosition(Finger touchedFinger)
    {
        _movementAmount = Vector2.zero;
        gameObject.SetActive(true);
        _rectTransform.anchoredPosition = ClampStartPosition(touchedFinger.screenPosition);
    }

    private Vector2 ClampStartPosition(Vector2 startPosition)
    {
        if (startPosition.x < _halfSize)
        {
            startPosition.x = _halfSize;
        }

        if (startPosition.y < _halfSize)
        {
            startPosition.y = _halfSize;
        }
        else if (startPosition.y > Screen.height - _halfSize)
        {
            startPosition.y = Screen.height - _halfSize;
        }

        return startPosition;
    }

    public void Clear()
    {
        _movementAmount = Vector2.zero;
        _knob.anchoredPosition = Vector2.zero;
        OnMove?.Invoke(Vector2.zero);
        gameObject.SetActive(false);
    }

    public void SetKnob(Touch currentTouch)
    {
        Vector2 nextJoystickPosition;
        Vector2 joystickCenter = _rectTransform.anchoredPosition;
        float maxMovement = _halfSize;
        if (Vector2.Distance(currentTouch.screenPosition, joystickCenter) > maxMovement)
        {
            nextJoystickPosition = (currentTouch.screenPosition - joystickCenter).normalized * maxMovement;
        }
        else
        {
            nextJoystickPosition = currentTouch.screenPosition - joystickCenter;
        }
        _knob.anchoredPosition = nextJoystickPosition;
        _movementAmount = nextJoystickPosition / maxMovement;
        OnMove?.Invoke(_movementAmount);
    }
}
