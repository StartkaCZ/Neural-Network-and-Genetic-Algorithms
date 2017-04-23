using UnityEngine;
using System.Collections;

public class SpotLight : MonoBehaviour
{
    Transform   _transform;
    Transform   _target;
    
    Light       _light;
       
    Vector3     _targetPosition;
    Vector3     _currentPosition;

    Color       _targetColour;
    Color       _currentColour;

    float       _colourTimer;
    float       _targetTimer;
    float       _changeTargetTimer;
    float       _changeTargetColourTimer;


    /// <summary>
    /// Initializes the class
    /// </summary>
    /// <param name="target"></param>
    public void Initialize(Transform target)
    {
        _transform = transform;
        _light = GetComponent<Light>();

        ResetToTarget(target);

        _colourTimer = ConstHolder.TRANSITION_TIMER;
        _targetTimer = ConstHolder.TRANSITION_TIMER;

        _changeTargetColourTimer = 0;
        _changeTargetTimer = 0;
    }


    /// <summary>
    /// Updates the class
    /// </summary>
    public void ManualUpdate()
    {
        LookAtTarget();
        LerpColours();
    }

    /// <summary>
    /// Looks at the target.
    /// Changes rotation to aim at the target's position
    /// </summary>
    void LookAtTarget()
    {
        if (_targetTimer < ConstHolder.TRANSITION_TIMER)
        {//if the lerp timer hasn't passed its cap then lerp to target
            _targetPosition = _target.position;
            _targetTimer += Time.deltaTime;

            if (_targetTimer > ConstHolder.TRANSITION_TIMER)
            {
                _targetTimer = ConstHolder.TRANSITION_TIMER;
            }

            Vector3 direction = (_targetPosition - _currentPosition);
            _currentPosition += direction * (_targetTimer / ConstHolder.TRANSITION_TIMER);
            _transform.LookAt(_currentPosition, Vector3.down);
        }
        else
        {//otherwise stay locked on the target.
            _currentPosition = _target.position;
            _transform.LookAt(_currentPosition, Vector3.down);
        }
    }

    /// <summary>
    /// Lerps to target colour unless it has reached it.
    /// </summary>
    void LerpColours()
    {
        if (_colourTimer < ConstHolder.TRANSITION_TIMER)
        {
            _colourTimer += Time.deltaTime;

            if (_colourTimer > ConstHolder.TRANSITION_TIMER)
            {
                _colourTimer = ConstHolder.TRANSITION_TIMER;
            }

            _light.color = Color.Lerp(_currentColour, _targetColour, _colourTimer / ConstHolder.TRANSITION_TIMER);
        }
    }

    /// <summary>
    /// Sets the target to shine on.
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(Transform targetTransform)
    {
        if (targetTransform != _target)
        {//if the target is different
            if (_changeTargetTimer > ConstHolder.SWITCH_TARGET_TIMER)
            {//and the change timer has passed its cap
                SetupTarget(targetTransform);
            }
            else
            {//update the timer
                _changeTargetTimer += Time.deltaTime;
            }
        }
        else
        {//reset the timer
            _changeTargetTimer = 0;
        }
    }

    /// <summary>
    /// Resets so that it is locked to the target.
    /// </summary>
    /// <param name="targetTransform"></param>
    public void ResetToTarget(Transform targetTransform)
    {
        SetupTarget(targetTransform);

        _currentPosition = _targetPosition;
        _transform.LookAt(_currentPosition, Vector3.down);

        _targetTimer = ConstHolder.TRANSITION_TIMER;
    }

    /// <summary>
    /// sets up the target to point at.
    /// </summary>
    /// <param name="targetTransform"></param>
    void SetupTarget(Transform targetTransform)
    {
        _target = targetTransform;
        _targetPosition = _target.position;
        _targetTimer = 0;
    }

    /// <summary>
    /// Sets the target colour to be of the one provided.
    /// It sets the colour once its timer has bypassed its cap
    /// </summary>
    /// <param name="colour"></param>
    public void SetTargetColour(Color colour)
    {
        if (colour != _targetColour)
        {//if the target colour is different from the colour provided
            if (_changeTargetColourTimer > ConstHolder.SWITCH_TARGET_TIMER)
            {//check if the timer has bypassed the cap and set the new target colour
                _currentColour = _light.color;
                _targetColour = colour;
                _colourTimer = 0;
                _changeTargetColourTimer = 0;
            }
            else
            {//otherwise increment the timer
                _changeTargetColourTimer += Time.deltaTime;
            }
        }
        else
        {//reset the timer
            _changeTargetColourTimer = 0;
        }
    }

    /// <summary>
    /// Sets the current colour to the one provided.
    /// Colour will not lerp
    /// </summary>
    /// <param name="colour"></param>
    public void SetCurrentColour(Color colour)
    {
        _currentColour = colour;
        _colourTimer = ConstHolder.TRANSITION_TIMER;
    }
}
