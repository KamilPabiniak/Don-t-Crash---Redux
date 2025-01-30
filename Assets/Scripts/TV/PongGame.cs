using UnityEngine;
using UnityEngine.UI;

public class PongGame : MonoBehaviour
{
    public GameObject ball;
    public GameObject leftPaddle;
    public GameObject rightPaddle;
    public Text leftScoreText;
    public Text rightScoreText;

    private Rigidbody2D ballRb;
    private Rigidbody2D leftPaddleRb;
    private Rigidbody2D rightPaddleRb;

    public float paddleSpeed = 5f;
    public float ballSpeed = 6f;

    private int leftScore = 0;
    private int rightScore = 0;

    void Start()
    {
        ballRb = ball.GetComponent<Rigidbody2D>();
        leftPaddleRb = leftPaddle.GetComponent<Rigidbody2D>();
        rightPaddleRb = rightPaddle.GetComponent<Rigidbody2D>();

        ResetBall();
        UpdateScoreText();
    }

    void Update()
    {
        MoveRightPaddleAI();
        CheckBounds();
    }

    public void MoveLeftPaddleUp()
    {
        leftPaddleRb.linearVelocity = Vector2.up * paddleSpeed;
    }

    public void MoveLeftPaddleDown()
    {
        leftPaddleRb.linearVelocity = Vector2.down * paddleSpeed;
    }

    public void StopLeftPaddle()
    {
        leftPaddleRb.linearVelocity = Vector2.zero;
    }

    void MoveRightPaddleAI()
    {
        float ballY = ball.transform.position.y;
        float paddleY = rightPaddle.transform.position.y;

        if (ballY > paddleY + 0.5f)
        {
            rightPaddleRb.linearVelocity = Vector2.up * paddleSpeed;
        }
        else if (ballY < paddleY - 0.5f)
        {
            rightPaddleRb.linearVelocity = Vector2.down * paddleSpeed;
        }
        else
        {
            rightPaddleRb.linearVelocity = Vector2.zero;
        }
    }

    void ResetBall()
    {
        ball.transform.position = Vector2.zero;
        ballRb.linearVelocity = new Vector2(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(-1f, 1f)).normalized * ballSpeed;
    }

    void CheckBounds()
    {
        if (ball.transform.position.x < -9f)
        {
            rightScore++;
            UpdateScoreText();
            ResetBall();
        }
        else if (ball.transform.position.x > 9f)
        {
            leftScore++;
            UpdateScoreText();
            ResetBall();
        }
    }

    void UpdateScoreText()
    {
        leftScoreText.text = leftScore.ToString();
        rightScoreText.text = rightScore.ToString();
    }

    public void QuitGame()
    {
        // Mo�esz tu doda� Application.Quit();
    }
}
