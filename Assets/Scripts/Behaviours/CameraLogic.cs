using UnityEngine;

public class CameraLogic : MonoBehaviour
{
    Transform       _transform;
    Transform       _target;

    Camera          _camera;

    Vector2         _viewportSize;

    float           _heightOffset;
    float           _offsetFromTarget;
    float           _targetTimer;
    float           _changeTargetTimer;


    /// <summary>
    /// Initializes the class
    /// </summary>
    /// <param name="targetTransform"></param>
    public void Initialize(Transform targetTransform)
    {
        _camera = GetComponent<Camera>();
        _transform = transform;

        _heightOffset = _transform.position.y;
        _offsetFromTarget = _transform.position.z - targetTransform.position.z;

        _viewportSize.y =  2 * _camera.orthographicSize;
        _viewportSize.x = _viewportSize.y * _camera.aspect;
        _viewportSize *= 0.5f;

        _targetTimer = ConstHolder.TRANSITION_TIMER;
        _changeTargetTimer = 0;

        ResetToTarget(targetTransform);
    }
	

    /// <summary>
    /// Updates the camera manually.
    /// Camera tracks its target along the Z axis.
    /// </summary>
	public void ManualUpdate ()
    {
        Vector3 targetPosition = new Vector3(_transform.position.x, _heightOffset, _target.position.z + _offsetFromTarget);

        if (_targetTimer < ConstHolder.TRANSITION_TIMER)
        {//lerpt to target position over time
            _targetTimer += Time.deltaTime;

            if (_targetTimer > ConstHolder.TRANSITION_TIMER)
            {
                _targetTimer = ConstHolder.TRANSITION_TIMER;
            }

            Vector3 direction = targetPosition - _transform.position;
            _transform.position += direction * (_targetTimer / ConstHolder.TRANSITION_TIMER);
        }
        else
        {//otherwise locked on the target
            _transform.position = targetPosition;
        }       
    }


    /// <summary>
    /// zooms in or out from its target.
    /// </summary>
    public void Zoom(bool zoomIn)
    {
        if (zoomIn)
        {//zoom in
            if (_camera.orthographicSize > ConstHolder.CAMERA_MIN_ORTHOGRAPHIC_SIZE)
            {//check if we haven't reached the minimum cap
                _camera.orthographicSize -= ConstHolder.CAMERA_SCROLL_SPEED * Time.deltaTime;

                if (_camera.orthographicSize < ConstHolder.CAMERA_MIN_ORTHOGRAPHIC_SIZE)
                {
                    _camera.orthographicSize = ConstHolder.CAMERA_MIN_ORTHOGRAPHIC_SIZE;
                }
            }
        }
        else
        {//othewise zoom out
            if (_camera.orthographicSize < ConstHolder.CAMERA_MAX_ORTHOGRAPHIC_SIZE)
            {// check if we haven't reached the maximum cap
                _camera.orthographicSize += ConstHolder.CAMERA_SCROLL_SPEED * Time.deltaTime;

                if (_camera.orthographicSize > ConstHolder.CAMERA_MAX_ORTHOGRAPHIC_SIZE)
                {
                    _camera.orthographicSize = ConstHolder.CAMERA_MAX_ORTHOGRAPHIC_SIZE;
                }
            }
        }

        SetupCamera();
    }

    /// <summary>
    /// sets up the viewport size.
    /// </summary>
    void SetupCamera()
    {
        _viewportSize.y = 2 * _camera.orthographicSize;
        _viewportSize.x = _viewportSize.y * _camera.aspect;
        _viewportSize *= 0.5f;
    }


    /// <summary>
    /// Sets the target to follow if it has filled the change target timer.
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform targetTransform)
    {
        if (targetTransform != _target)
        {//checks whether or not its already aiming at this target
            if (_changeTargetTimer > ConstHolder.SWITCH_TARGET_TIMER)
            {//checks if it has bypassed the change target timer
                targetSetup(targetTransform);
            }
            else
            {//if not, increase the timer
                _changeTargetTimer += Time.deltaTime;
            }
        }
        else
        {//if not then reset the timer.
            _changeTargetTimer = 0;
        }
    }

    /// <summary>
    /// Resets the camera to be locked to its target.
    /// </summary>
    /// <param name="targetTransform"></param>
    public void ResetToTarget(Transform targetTransform)
    {
        targetSetup(targetTransform);
        _transform.position = new Vector3(_transform.position.x, _heightOffset, _target.position.z + _offsetFromTarget);
        _targetTimer = ConstHolder.TRANSITION_TIMER;
    }

    /// <summary>
    /// Sets up the camera to track the new target,
    /// </summary>
    /// <param name="targetTransform"></param>
    void targetSetup(Transform targetTransform)
    {
        _target = targetTransform;
        _targetTimer = 0;
        _changeTargetTimer = 0;
    }

    public Vector3 position
    {
        get { return _transform.position; }
    }
}
