using UnityEngine;
using UnityEngine.UI;

public class TransitionAnimation : MonoBehaviour
{
    Image   _image;

    Color   _colour;

    float   _alpha;
    float   _timer;

    bool    _fadeIn;
    bool    _playAnimation;


    /// <summary>
    /// Initializes the transition to start to fade out.
    /// </summary>
    public void Initialize()
    {
        gameObject.SetActive(true);
        _image = GetComponent<Image>();

        FadeOut();

        _colour = _image.color;
    }

    
    /// <summary>
    /// Updates the transition if its to play animation.
    /// </summary>
    public void ManualUpdate()
    {
        if (_playAnimation)
        {
            if (_fadeIn)
            {
                FadeInAnimation();
            }
            else
            {
                FadeOutAnimation();
            }
        }
    }

    /// <summary>
    /// Fade out animation, reduces alpha based on time.
    /// </summary>
    void FadeInAnimation()
    {
        if (_timer > ConstHolder.TRANSITION_ANIMATION)
        {
            _playAnimation = false;

            _alpha = 255;
            _colour.a = _alpha;
            _image.color = _colour;
        }
        else
        {
            _timer += Time.deltaTime;

            _alpha = _timer / ConstHolder.TRANSITION_ANIMATION;
        }

        _colour.a = _alpha;
        _image.color = _colour;
    }

    /// <summary>
    /// Fade in animation, increases alpha based on time.
    /// Turns the game object off when finished.
    /// </summary>
    void FadeOutAnimation()
    {
        if (_timer < 0)
        {
            _playAnimation = false;

            _alpha = 0;
            _colour.a = _alpha;
            _image.color = _colour;

            gameObject.SetActive(false);
        }
        else
        {
            _timer -= Time.deltaTime;

            _alpha = _timer / ConstHolder.TRANSITION_ANIMATION;
        }

        _colour.a = _alpha;
        _image.color = _colour;
    }

    /// <summary>
    /// Setup for fade in animation.
    /// Activates the object
    /// </summary>
    public void FadeIn()
    {
        gameObject.SetActive(true);

        _fadeIn = true;
        _playAnimation = true;
        _timer = 0;
    }

    /// <summary>
    /// Setup for fade out animation
    /// Activates the object
    /// </summary>
    public void FadeOut()
    {
        gameObject.SetActive(true);

        _fadeIn = false;
        _playAnimation = true;
        _timer = ConstHolder.TRANSITION_ANIMATION;
    }


    public bool playingAnimation
    {
        get { return _playAnimation; }
    }
}
