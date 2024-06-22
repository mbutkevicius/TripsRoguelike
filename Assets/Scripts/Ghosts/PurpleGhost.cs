using UnityEngine;
using UnityEngine.UIElements;

public class PurpleGhost : MonoBehaviour
{
    public GameDataManager gameDataManager;

    public GameOverScript gameOverScript;

    public DelayedStartScript countdownTimer;

    [Header("Movement")]
    [SerializeField] private float amplitude = 10f; // Amplitude of the sine wave
    [SerializeField] private float frequency = 0.25f; // Frequency of the sine wave
    [SerializeField] private float moveSpeed = 3.5f; // Movement speed

    private Vector2 initialPosition;

    private bool inverted = false;
    private bool facingRight = true;

    private bool canMove;

    void Start()
    {
        // Find the game data manager
        gameDataManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameDataManager>();
        // Find the game over script
        gameOverScript = GameObject.FindGameObjectWithTag("Logic").GetComponent<GameOverScript>();
        // Find the game over script
        countdownTimer = GameObject.FindGameObjectWithTag("Logic").GetComponent<DelayedStartScript>();

        initialPosition = transform.position;
    }

    public void FixedUpdate(){

        Vector2 pos = transform.position;

        float sin = Mathf.Sin(pos.x * frequency) * amplitude;

        if (canMove)
        {
            // wall bounds (shouldn't be hard coded. Probably should use a raycast to check but for now this works)
            if (pos.x < -18 || pos.x > 18){
                inverted = !inverted;
                //pos.y = (initialPosition.y + Mathf.Sign(pos.x) * 1) + sin;
                Flip();
            }

            // movement for going left
            if (inverted){
                pos.x -= moveSpeed * Time.deltaTime * gameDataManager.ghostTimeFraction;
                pos.y = initialPosition.y + sin;
            }
            // movement for going right
            else{
                pos.x += moveSpeed * Time.deltaTime * gameDataManager.ghostTimeFraction;
                pos.y = initialPosition.y - sin;
                }

            //pos.x -= moveSpeed * Time.deltaTime;
        }

        pos.y = initialPosition.y + sin;

        transform.position = pos;

    }

        // Method to flip the player sprite
    public void Flip(){
        facingRight = !facingRight;
        // flip sprite along the y axis
        transform.rotation = Quaternion.Euler(0, facingRight ? 0 : 180, 0);
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void DisableMovement()
    {
        canMove = false;
    }
}
