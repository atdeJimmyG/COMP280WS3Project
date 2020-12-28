using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController : MonoBehaviour
{
    public const float POWER_PILL_TIMER = 10;
    public const int PILL_SCORE = 25;
    public const int GHOST_SCORE = 100;
    public const float SPEED_FACTOR = 5.5F;

    private Rigidbody body;
    private GameController game;
    public float invulnTimer;

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        game = GameObject.Find("GameController").GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (invulnTimer > 0)
        {
            invulnTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 inputVector = new Vector3(h, 0, v) *SPEED_FACTOR;
                
        body.AddForce(inputVector);
    }

    bool isPowered()
    {
        return invulnTimer > 0;
    }

    /*void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "ghost") {
            if (isPowered())
            {
                game.score += GHOST_SCORE;
                Destroy(col.gameObject);
            } else {
                Debug.Log("test");
                game.lives--;
                Destroy(gameObject);
            }
        }
    }*/

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "ghost")
        {
            game.score += PILL_SCORE;
            Destroy(col.gameObject);
        }
    }
}
