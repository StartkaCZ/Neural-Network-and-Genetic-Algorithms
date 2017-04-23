using UnityEngine;
using System.Collections.Generic;

public abstract class Unit : MonoBehaviour
{
    protected Transform             _transform;
    protected Rigidbody             _rigidBody;
    protected Collider              _collider;
    protected MeshRenderer          _meshRenderer;
    protected ParticleSystem        _particleSystem;

    protected Vector3               _initialPosition;

    protected int                   _points;
    protected int                   _yellowBallsCollected;

    protected bool                  _alive;
    protected bool                  _hit;

    protected UnitType              _type;
    

    /// <summary>
    /// Initialized the class with a type.
    /// Gets all of the components used.
    /// </summary>
    /// <param name="type"></param>
    public virtual void Initialize(UnitType type)
    {
        _transform = transform;
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _meshRenderer = GetComponent<MeshRenderer>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();

        _particleSystem.startColor = _meshRenderer.sharedMaterial.color;

        _initialPosition = _transform.position;

        _type = type;

        _hit = false;
        _alive = true;
    }

    public abstract void ManualUpdate();


    void OnCollisionEnter(Collision colInfo)
    {
        if (colInfo.collider.tag == "Obstacle")
        {//on collision with an obstacle
            Killed();
        }
    }

    /// <summary>
    /// Unity trigger collision event.
    /// </summary>
    /// <param name="colInfo"></param>
    void OnTriggerEnter(Collider colInfo)
    {
        if (colInfo.gameObject.tag == "YellowBall")
        {//on a collision with food
            YellowBallCollisionResponse(colInfo);
        }
    }

    /// <summary>
    /// Collision response with the YellowBall.
    /// </summary>
    protected void YellowBallCollisionResponse(Collider colInfo)
    {
        AudioManager.instance.PlaySoundEffect(AudioManager.SoundEffect.YellowBallCollected);

        _yellowBallsCollected++;

        colInfo.GetComponent<YellowBall>().Consume();
    }

    /// <summary>
    /// It has been killed.
    /// Turns off particles, disabled a collider.
    /// </summary>
    protected void Killed()
    {
        _rigidBody.velocity = Vector3.zero;

        _hit = true;
        _alive = false;

        _collider.enabled = false;
        _particleSystem.enableEmission = false;

        AudioManager.instance.PlaySoundEffect(AudioManager.SoundEffect.Death);
    }

    /// <summary>
    /// resets the unit back to the initialization state.
    /// So it can be ran on a next test.
    /// </summary>
    public virtual void Reset()
    {
        _transform.position = _initialPosition;

        _transform.eulerAngles = new Vector3(ConstHolder.INITIAL_ANGLE_X, ConstHolder.INITIAL_ANGLE_Y, ConstHolder.INITIAL_ANGLE_Z);

        _alive = true;

        _yellowBallsCollected = 0;

        _collider.enabled = true;
        _particleSystem.enableEmission = true;
    }

    public bool hit
    {
        get { return _hit; }
        set { _hit = value; }
    }

    public Collider collider
    {
        get { return _collider; }
    }

    public Vector3 position
    {
        get { return _transform.position; }
    }

    public Vector3 velocity
    {
        get { return _rigidBody.velocity; }
    }

    public Color colour
    {
        get { return _meshRenderer.sharedMaterial.color; }
    }

    public Quaternion rotation
    {
        get { return _transform.rotation; }
    }

    public bool alive
    {
        get { return _alive; }
    }

    public UnitType type
    {
        get { return _type; }
    }

    public int points
    {
        get { return _points; }
    }
}
