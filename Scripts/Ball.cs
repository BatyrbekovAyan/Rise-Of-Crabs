using UnityEngine;

public class Ball : MonoBehaviour
{
    public bool returnBall = false;
    public int ballPower = 1;
    private float speed = 4f;

    private void FixedUpdate()
    {
        if (returnBall)
        {
            transform.position = Vector2.LerpUnclamped(transform.position, Shooter.FirstBallSpriteStatic.transform.position, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, Shooter.FirstBallSpriteStatic.transform.position) < 0.05f)
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
                Sound.CreatureHit.PlayOneShot(Sound.CreatureHit.clip);
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        Shooter.FirstBallSpriteStatic.SetActive(true);
        Shooter.totalBallCount++;
        Shooter.CurrentBallCountTextStatic.text = Shooter.totalBallCount.ToString();
        Shooter.BallsSliderStatic.value = Shooter.totalBallCount;
    }
}
