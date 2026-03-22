using UnityEngine;

public class FakeGravityX : MonoBehaviour
{
    public float gravity = 20f;
    public float velocity = 0f;
    public float stopX = 0f;

    void Update()
    {
        velocity += gravity * Time.deltaTime;

        transform.position += Vector3.left * velocity * Time.deltaTime;

        if (transform.position.x <= stopX)
        {
            Vector3 pos = transform.position;
            pos.x = stopX;
            transform.position = pos;

            velocity = 0f;
            enabled = false;
        }
    }
}