using UnityEngine;

public class Warning : MonoBehaviour
{
    [SerializeField] private GameObject WarningAnimation;

    public int collisions = 0;


    public void Start()
    {
        gameObject.GetComponent<BoxCollider2D>().size = gameObject.GetComponent<RectTransform>().rect.size;
        gameObject.GetComponent<BoxCollider2D>().offset = new Vector2(0, gameObject.GetComponent<RectTransform>().rect.size.y / 2);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            collisions++;

            collision.transform.parent.GetComponent<Creature>().tooClose = true;

            if (WarningAnimation != null)
            {
                if (!WarningAnimation.activeSelf)
                {
                    WarningAnimation.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            collisions--;

            if (WarningAnimation != null)
            {
                if (WarningAnimation.activeSelf && collisions <= 0)
                {
                    WarningAnimation.SetActive(false);
                }
            }
        }
    }
}
