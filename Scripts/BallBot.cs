using UnityEngine;

public class BallBot : MonoBehaviour
{
    public bool returnBall = false;
    public int ballPower = 2;
    private float speed = 4f;
    public GameObject FirstBallSprite;

    private void FixedUpdate()
    {
        if (returnBall)
        {
            transform.position = Vector2.LerpUnclamped(transform.position, FirstBallSprite.transform.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, FirstBallSprite.transform.position) < 0.05f)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            if (Sound.SoundEnabled)
            {
                Sound.BotShoot.PlayOneShot(Sound.BotShoot.clip);
            }

            Destroy(gameObject);
        }
    }
}
