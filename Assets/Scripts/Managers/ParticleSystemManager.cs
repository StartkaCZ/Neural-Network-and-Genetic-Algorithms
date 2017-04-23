using UnityEngine;
using System.Collections.Generic;

public class ParticleSystemFactory : MonoBehaviour
{
    public enum ParticleEffect
    {
        Hit,
        Death,
        
        Count
    }

    //prefab holder.
    static Dictionary<ParticleEffect, GameObject>  _particlePrefabs;


    /// <summary>
    /// Initializes the class.
    /// </summary>
    static public void Initialize()
    {
        _particlePrefabs = new Dictionary<ParticleEffect, GameObject>((int)ParticleEffect.Count);

        LoadContent();
    }

    /// <summary>
    /// Loads all of the contect from resource folder.
    /// </summary>
    static void LoadContent()
    {
        _particlePrefabs.Add(ParticleEffect.Hit, Resources.Load<GameObject>("Prefabs/HitParticleSystem"));
        _particlePrefabs.Add(ParticleEffect.Death, Resources.Load<GameObject>("Prefabs/DeathParticleSystem"));
    }


    /// <summary>
    /// Creates a particle system clone of a prefab using provided colour.
    /// </summary>
    /// <param name="particleEffect"></param>
    /// <param name="colour"></param>
    /// <returns></returns>
    static public ParticleSystem CreateParticleEffectSystem(ParticleEffect particleEffect, Color colour)
    {
        ParticleSystem particleSystem = null;

        switch (particleEffect)
        {
            case ParticleEffect.Hit:
                particleSystem = CreateHitParticleSystem(colour);
                break;

            case ParticleEffect.Death:
                particleSystem = CreateDeathParticleSystem(colour);
                break;

            default:
                break;
        }

        return particleSystem;
    }

    #region ParticleSystem Creation

    static ParticleSystem CreateHitParticleSystem(Color colour)
    {
        GameObject clone = Instantiate(_particlePrefabs[ParticleEffect.Hit],
                                       _particlePrefabs[ParticleEffect.Hit].transform.position,
                                       _particlePrefabs[ParticleEffect.Hit].transform.rotation) as GameObject;

        ParticleSystem particleSystem = clone.GetComponent<ParticleSystem>();
        particleSystem.startColor = colour;

        return particleSystem;
    }

    static ParticleSystem CreateDeathParticleSystem(Color colour)
    {
        GameObject clone = Instantiate(_particlePrefabs[ParticleEffect.Death],
                                       _particlePrefabs[ParticleEffect.Death].transform.position,
                                       _particlePrefabs[ParticleEffect.Death].transform.rotation) as GameObject;

        ParticleSystem particleSystem = clone.GetComponent<ParticleSystem>();
        particleSystem.startColor = colour;

        return particleSystem;
    }

    #endregion
}
