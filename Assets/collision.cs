using UnityEngine;

public class DogCollisions : MonoBehaviour
{
    public Game game; 

    void Awake() { if (!game) game = FindObjectOfType<Game>(); }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            game.GameOver();
        }
        else if (other.CompareTag("Goal"))
        {
            game.LevelComplete(); 

        }
    }
}
