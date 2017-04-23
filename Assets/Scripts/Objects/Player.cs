using UnityEngine;

public class Player : Unit
{
    float           _angle;


    /// <summary>
    /// Initializes the class with a type.
    /// </summary>
    public override void Initialize(UnitType type)
    {
        base.Initialize(type);

        _angle = _transform.eulerAngles.y;
    }


    /// <summary>
    /// Updates the class
    /// </summary>
    public override void ManualUpdate()
    {
        if (_alive)
        {
            InputCheck();

            UpdateTransform();

            _points = _yellowBallsCollected + (int)position.z;
        }
    }

    /// <summary>
    /// Updates the velocity and rotation of the object
    /// </summary>
    void UpdateTransform()
    {
        _transform.eulerAngles = new Vector3(0, _angle, 0);

        float radian = -_angle * Mathf.PI / 180.0f;
        _rigidBody.velocity = new Vector3(Mathf.Cos(radian) * DataManager.currentSpeed,
                                           0,
                                           Mathf.Sin(radian) * DataManager.currentSpeed);
    }

    /// <summary>
    /// Checks for player inputs
    /// </summary>
    void InputCheck()
    {
        if (Input.GetKey(KeyCode.A))
        {
            if (_angle > 180)
            {//capped to stop it from inversing.
                _angle -= ConstHolder.UNIT_MAX_ROTATION * Time.deltaTime;

                if (_angle < 180)
                {
                    _angle = 180;
                }
            }
        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (_angle < 360)
            {//capped to stop it from inversing.
                _angle += ConstHolder.UNIT_MAX_ROTATION * Time.deltaTime;

                if (_angle > 360)
                {
                    _angle = 360;
                }
            }
        }
    }
}
