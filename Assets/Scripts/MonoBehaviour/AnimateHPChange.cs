using UnityEngine;

public class AnimateHPChange : MonoBehaviour
{
    //Serialize variables
    [SerializeField] float seconds;
    [SerializeField] float heightOffset;
    [SerializeField] float depthOffset;

    //Derived variables
    float velocity;
    float decay;

    private void Start()
    {
        //The formula appears to be (v = Mathf.Sqrt(2 * height * Mathf.Abs(gravity))
        //Decay is meant to equal (g = (2 * height) / timeToPeak^2)

        float timeToPeak = heightOffset / ((2 * heightOffset) - depthOffset) * seconds;
        decay = (2 * heightOffset) / Mathf.Pow(timeToPeak, 2);
        velocity = Mathf.Sqrt(2 * heightOffset * decay);
    }

    // Update is called once per frame
    void Update()
    {
        //*
        if (seconds > 0)
        {
            //Convert to ViewportPoint and go up by velocity
            Vector2 normalPosition = Camera.main.ScreenToViewportPoint(GetComponent<RectTransform>().position) + Vector3.up * velocity * Time.deltaTime;

            //Convert back to screen coordinates
            GetComponent<RectTransform>().position = Camera.main.ViewportToScreenPoint(normalPosition);

            //Decay velocity and time
            velocity -= decay * Time.deltaTime;
            seconds -= Time.deltaTime;
        }
        else { Destroy(gameObject); }
        //*/
    }
}
