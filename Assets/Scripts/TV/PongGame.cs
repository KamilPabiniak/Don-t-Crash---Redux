using TMPro;
using UnityEngine;

public class PongGame : MonoBehaviour
{
    public GameObject ball;
    public GameObject leftPaddle;
    public GameObject rightPaddle;
    public TextMeshProUGUI leftScoreText;
    public TextMeshProUGUI rightScoreText;
    public RectTransform panel;

    private RectTransform ballRect;
    private RectTransform leftPaddleRect;
    private RectTransform rightPaddleRect;

    public float paddleSpeed = 300f; 
    public float ballSpeed = 300f;

    public LeverXR lever;

    private int leftScore = 0;
    private int rightScore = 0;

    private float panelWidth;
    private float panelHeight;
    private Vector2 ballDirection;

    void Start()
    {
        ballRect = ball.GetComponent<RectTransform>();
        leftPaddleRect = leftPaddle.GetComponent<RectTransform>();
        rightPaddleRect = rightPaddle.GetComponent<RectTransform>();

        panelWidth = panel.rect.width / 2;
        panelHeight = panel.rect.height / 2;

        ResetBall();
        UpdateScoreText();
    }

    void Update()
    {
        MoveRightPaddleAI();
        MoveBall();
        CheckBounds();
        
        float angle = lever.hingeJoint.angle;

        if (angle >= lever.GetForwadLimit() / 2)
        {
            MoveLeftPaddleDown();
        }
        else if (angle <= lever.GetBackwardLimit() / 2)
        {
            MoveLeftPaddleUp();
        }
    }

    private void MoveLeftPaddleUp()
    {
        if (leftPaddleRect.anchoredPosition.y < panelHeight - leftPaddleRect.rect.height / 2)
        {
            leftPaddleRect.anchoredPosition += Vector2.up * paddleSpeed * Time.deltaTime;
        }
    }

    private void MoveLeftPaddleDown()
    {
        if (leftPaddleRect.anchoredPosition.y > -panelHeight + leftPaddleRect.rect.height / 2)
        {
            leftPaddleRect.anchoredPosition += Vector2.down * paddleSpeed * Time.deltaTime;
        }
    }

    void MoveRightPaddleAI()
    {
        float ballY = ballRect.anchoredPosition.y;
        float paddleY = rightPaddleRect.anchoredPosition.y;

        if (ballY > paddleY + 10f && paddleY < panelHeight - rightPaddleRect.rect.height / 2)
        {
            rightPaddleRect.anchoredPosition += Vector2.up * paddleSpeed * Time.deltaTime;
        }
        else if (ballY < paddleY - 10f && paddleY > -panelHeight + rightPaddleRect.rect.height / 2)
        {
            rightPaddleRect.anchoredPosition += Vector2.down * paddleSpeed * Time.deltaTime;
        }
    }

    void MoveBall()
    {
        ballRect.anchoredPosition += ballDirection * ballSpeed * Time.deltaTime;
    }

    void ResetBall()
    {
        ballRect.anchoredPosition = Vector2.zero;
        ballDirection = new Vector2(Random.Range(0, 2) == 0 ? 1 : -1, Random.Range(-0.5f, 0.5f)).normalized;
    }

    void CheckBounds()
    {
        if (ballRect.anchoredPosition.x < -panelWidth)
        {
            rightScore++;
            UpdateScoreText();
            ResetBall();
        }
        else if (ballRect.anchoredPosition.x > panelWidth)
        {
            leftScore++;
            UpdateScoreText();
            ResetBall();
        }
        
        if (ballRect.anchoredPosition.y > panelHeight - ballRect.rect.height / 2 || 
            ballRect.anchoredPosition.y < -panelHeight + ballRect.rect.height / 2)
        {
            ballDirection.y = -ballDirection.y;
        }

        CheckPaddleCollision();
    }

    void CheckPaddleCollision()
    {
        // Kolizja z lewą paletką
        if (IsBallTouchingPaddle(leftPaddleRect))
        {
            ballDirection.x = Mathf.Abs(ballDirection.x);
            AdjustBallBounce(leftPaddleRect);
        }
        // Kolizja z prawą paletką
        else if (IsBallTouchingPaddle(rightPaddleRect))
        {
            ballDirection.x = -Mathf.Abs(ballDirection.x);
            AdjustBallBounce(rightPaddleRect);
        }
    }

    bool IsBallTouchingPaddle(RectTransform paddle)
    {
        return ballRect.anchoredPosition.x < paddle.anchoredPosition.x + paddle.rect.width / 3 &&
               ballRect.anchoredPosition.x > paddle.anchoredPosition.x - paddle.rect.width / 3 &&
               ballRect.anchoredPosition.y < paddle.anchoredPosition.y + paddle.rect.height / 1 &&
               ballRect.anchoredPosition.y > paddle.anchoredPosition.y - paddle.rect.height / 1;
    }

    void AdjustBallBounce(RectTransform paddle)
    {
        float hitPosition = (ballRect.anchoredPosition.y - paddle.anchoredPosition.y) / (paddle.rect.height / 2);
        ballDirection = new Vector2(ballDirection.x, hitPosition).normalized;
    }

    void UpdateScoreText()
    {
        leftScoreText.text = leftScore.ToString();
        rightScoreText.text = rightScore.ToString();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
