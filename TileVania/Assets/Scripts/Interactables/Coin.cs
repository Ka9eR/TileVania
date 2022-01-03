using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] int pointsForCoinPickup = 10;

    bool wasCollected = false;

    AudioSource myAudioSource;
    SpriteRenderer mySpriteRenderer;
    CircleCollider2D myCircleCollider;
    private void Start()
    {
        myAudioSource = gameObject.GetComponent<AudioSource>();
        mySpriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        myCircleCollider = gameObject.GetComponent<CircleCollider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<GameSession>().AddToScore(pointsForCoinPickup);
            myAudioSource.Play();
            mySpriteRenderer.enabled = false;
            myCircleCollider.enabled = false;
            Destroy(gameObject, 0.5f);
        }
    }
}
