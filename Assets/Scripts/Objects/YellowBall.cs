using UnityEngine;
using System.Collections;

public class YellowBall : MonoBehaviour
{
    Transform       _transform;
    MeshRenderer    _meshRenderer;
    Collider        _collider;

    ParticleSystem  _particleSystem;


    /// <summary>
    /// Manual initialization for the class
    /// </summary>
    public void Initialize ()
    {
        _transform = transform;
        _meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<Collider>();
        _particleSystem = GetComponentInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Consumes the YellowBal
    /// </summary>
    public void Consume()
    {
        _particleSystem.enableEmission = false;

        _meshRenderer.enabled = false;

        _collider.enabled = false;
    }
}
