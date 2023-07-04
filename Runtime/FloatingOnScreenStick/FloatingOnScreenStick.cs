using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

/// <summary>
/// A stick control displayed on screen and moved around by touch or other pointer
/// input. Floats to pointer down position.
/// </summary>
[AddComponentMenu("Input/Floating On-Screen Stick")]
public class FloatingOnScreenStick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField][Range(20,300)]
    private float _movementRange = 50;

    [InputControl(layout = "Vector2")]
    [SerializeField]
    private string _controlPath;

    [SerializeField]
    private RectTransform _joystickTransform;

    [SerializeField] 
    private RectTransform _joystickBackground;

    private Vector2 _startPos;
    private Vector2 _pointerDownPos;
    private Vector2 _dragPos;

    private void Awake()
    {
        if (_joystickBackground != null)
        {
            _joystickBackground.gameObject.SetActive(false);
        }

        if (_joystickTransform.gameObject != null)
        {
            _joystickTransform.gameObject.SetActive(false);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out _pointerDownPos);
        _joystickTransform.anchoredPosition = _pointerDownPos;
       
        _joystickTransform.gameObject.SetActive(true);
        if (_joystickBackground != null)
        {
            _joystickBackground.anchoredPosition = _pointerDownPos;
            _joystickBackground.sizeDelta = new Vector2(_movementRange + _joystickTransform.sizeDelta.x, _movementRange + _joystickTransform.sizeDelta.y);
            _joystickBackground.gameObject.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out _dragPos);
        var delta = _dragPos - _pointerDownPos;

        delta = Vector2.ClampMagnitude(delta, movementRange);
        _joystickTransform.anchoredPosition = _pointerDownPos + delta;

        var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
        SendValueToControl(newPos);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _joystickTransform.anchoredPosition = _startPos;
        SendValueToControl(Vector2.zero);
        _joystickTransform.gameObject.SetActive(false);
        if (_joystickBackground != null)
        {
            _joystickBackground.gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        _startPos = ((RectTransform) transform).anchoredPosition;
    }

    public float movementRange
    {
        get => _movementRange;
        set => _movementRange = value;
    }


    protected override string controlPathInternal
    {
        get => _controlPath;
        set => _controlPath = value;
    }
}