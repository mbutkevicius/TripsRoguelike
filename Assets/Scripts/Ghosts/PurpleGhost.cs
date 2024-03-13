using UnityEngine;
using UnityEngine.UIElements;

public class SineWaveMovement : MonoBehaviour
{
    [SerializeField] private float amplitude = 10f; // Amplitude of the sine wave
    [SerializeField] private float frequency = 0.25f; // Frequency of the sine wave
    [SerializeField] private float moveSpeed = 3.5f; // Movement speed

    private Vector2 initialPosition;

    private bool inverted = false;
    private bool facingRight = true;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {

    }

    private void FixedUpdate(){
        Vector2 pos = transform.position;

        float sin = Mathf.Sin(pos.x * frequency) * amplitude;

        // wall bounds (shouldn't be hard coded. Probably should use a raycast to check but for now this works)
        if (pos.x < -18 || pos.x > 18){
            inverted = !inverted;
            //pos.y = (initialPosition.y + Mathf.Sign(pos.x) * 1) + sin;
            Flip();
        }

        // movement for going left
        if (inverted){
            pos.x -= moveSpeed * Time.deltaTime;
            pos.y = initialPosition.y + sin;
        }
        // movement for going right
        else{
            pos.x += moveSpeed * Time.deltaTime;
            pos.y = initialPosition.y - sin;
        }

        //pos.x -= moveSpeed * Time.deltaTime;
        pos.y = initialPosition.y + sin;

        transform.position = pos;
    }

        // Method to flip the player sprite
    public void Flip(){
        facingRight = !facingRight;
        // flip sprite along the y axis
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }
}
