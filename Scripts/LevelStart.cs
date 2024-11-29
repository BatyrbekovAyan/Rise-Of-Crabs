using UnityEngine;

public class LevelStart : MonoBehaviour
{
    public static bool touched;


    private void Start()
    {
        touched = false;

        StartUI.HideStartUI();
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            touched = true;
            gameObject.SetActive(false);
        }
    }
}
