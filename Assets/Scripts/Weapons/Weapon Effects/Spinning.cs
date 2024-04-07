using UnityEngine;

public class SpinAroundPlayer : MonoBehaviour
{
    // Reference to the player for rotation around them
    public Transform player;

    // Rotation speed
    public float rotationSpeed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is not set for SpinAroundPlayer script on " + gameObject.name);
            return;
        }

        // Set the initial rotation towards the player
        Vector2 direction = player.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the object around the player
        transform.RotateAround(player.position, Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
