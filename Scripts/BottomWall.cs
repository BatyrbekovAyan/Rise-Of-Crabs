using UnityEngine;

public class BottomWall : MonoBehaviour
{
    [SerializeField] private GameObject FirstBallSprite;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ball"))
        {
            collision.transform.position = new Vector3(collision.transform.position.x, FirstBallSprite.transform.position.y, collision.transform.position.z);

            collision.transform.GetComponent<Rigidbody2D>().bodyType=RigidbodyType2D.Static;
            collision.transform.GetComponent<Ball>().returnBall = true;
        }

        else if (collision.transform.CompareTag("BallBot"))
        {
            collision.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
            Destroy(collision.gameObject);
        }
    }
}