using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ExampleInputActionControlled : MonoBehaviour
{
    [SerializeField] private InputActionReference _moveInputAction;
    [SerializeField] private InputActionReference _resetButton;
    
    private StickInputHandler _movementInputHandler;
    
    private Vector3 _velocity;
    private Vector3 _spawnPosition;
    [SerializeField][Range(10,100)] private float _speed;

    public class AnalogueInputHandler<TInputValue> : IDisposable where TInputValue : struct
    {
        private InputAction _action;
        private TInputValue _lastInputValue;
        private bool _isActive;

        public event OnInputUpdated OnStart;
        public event OnInputUpdated OnChange;
        public event OnInputUpdated OnEnd;

        public delegate void OnInputUpdated(TInputValue vector);

        public TInputValue LastValue => _lastInputValue;
        public bool IsActive => _isActive;

        public AnalogueInputHandler(InputAction inputAction)
        {
            _action = inputAction;
            _action.Enable();
            _lastInputValue = default;
            
            _action.canceled += HandleCancel;
            _action.performed += HandlePerform;
            _action.started += HandleStart;
        }

        private void HandleStart(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.action == _action)
            {
                _lastInputValue = callbackContext.ReadValue<TInputValue>();
                _isActive = true;
                OnStart?.Invoke(_lastInputValue);
                OnChange?.Invoke(_lastInputValue);
            }
        }

        private void HandlePerform(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.action == _action)
            {
                _lastInputValue = callbackContext.ReadValue<TInputValue>();
                _isActive = true;
                OnChange?.Invoke(_lastInputValue);
            }
        }

        private void HandleCancel(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.action == _action)
            {
                _lastInputValue = default;
                _isActive = false;
                OnChange?.Invoke(_lastInputValue);
                OnEnd?.Invoke(_lastInputValue);
            }
        }

        public void Dispose()
        {
            _action?.Dispose();
        }
    }
    
    public class StickInputHandler : AnalogueInputHandler<Vector2>
    {
        public StickInputHandler(InputAction inputAction) : base(inputAction)
        {
        }
    }

    public void Start()
    {
        _spawnPosition = gameObject.transform.position;
    }

    public void OnEnable()
    {
        _movementInputHandler = new StickInputHandler(_moveInputAction);
        _movementInputHandler.OnChange += HandleMovementChanged;
        _resetButton.action.performed += HandleResetPerformed;
        _resetButton.action.Enable();
    }

    private void HandleResetPerformed(InputAction.CallbackContext obj)
    {
        gameObject.transform.position = _spawnPosition;
        _velocity = Vector3.zero;
    }

    private void HandleMovementChanged(Vector2 vector)
    {
        Vector3 inputVelocity = _movementInputHandler.LastValue;
        _velocity = inputVelocity;
    }

    private void Update()
    {
        if (!_movementInputHandler.IsActive)
        {
            _velocity = Vector3.Lerp(_velocity, Vector3.zero, 0.1f);
        }
        
        if (_velocity == Vector3.zero)
        {
            return;
        }
        if (_velocity.magnitude < 0.0001f)
        {
            _velocity = Vector3.zero;
        }
        
        var o = gameObject;
        var position = o.transform.position;
        position += _velocity * Time.deltaTime * _speed;
        o.transform.position = position;
    }
}
