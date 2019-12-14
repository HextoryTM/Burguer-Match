using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    public int coluna;
    public int linha;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private Board board;
    private GameObject otherDot;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;
    public float swipeAngle = 0f;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;
        targetY = (int)transform.position.y;
        linha = targetY;
        coluna = targetX;
    }

    // Update is called once per frame
    void Update()
    {
        FindMatches();

        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }

        targetX = coluna;
        targetY = linha;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //Move Towards the Target
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .4f);
        }
        else
        {
            //Directly Set the Pos
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.allDots[coluna, linha] = this.gameObject;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            //Move Towards the Target
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .4f);
        }
        else
        {
            //Directly Set the Pos
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
            board.allDots[coluna, linha] = this.gameObject;
        }
    }

    private void OnMouseDown()
    {
        firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void OnMouseUp()
    {
        finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
        MoveDots();
    }

    void MoveDots()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && coluna < board.largura)
        {
            //Right Swipe
            otherDot = board.allDots[coluna + 1, linha];
            otherDot.GetComponent<Dot>().coluna -= 1;
            coluna += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && linha < board.altura)
        {
            //Up Swipe
            otherDot = board.allDots[coluna, linha + 1];
            otherDot.GetComponent<Dot>().linha -= 1;
            linha += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && coluna > 0)
        {
            //Left Swipe
            otherDot = board.allDots[coluna - 1, linha];
            otherDot.GetComponent<Dot>().coluna += 1;
            coluna -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && linha > 0)
        {
            //Down Swipe
            otherDot = board.allDots[coluna, linha - 1];
            otherDot.GetComponent<Dot>().linha += 1;
            linha -= 1;
        }
    }

    void FindMatches()
    {
        if (coluna > 0 && coluna < board.largura - 1)
        {
            GameObject leftDot1 = board.allDots[coluna - 1, linha];
            GameObject rightDot1 = board.allDots[coluna + 1, linha];
            if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
            {
                leftDot1.GetComponent<Dot>().isMatched = true;
                rightDot1.GetComponent<Dot>().isMatched = true;
                this.isMatched = true;
            }
        }

        if (linha > 0 && linha < board.altura - 1)
        {
            GameObject upDot1 = board.allDots[coluna, linha + 1];
            GameObject downDot1 = board.allDots[coluna, linha - 1];
            if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
            {
                upDot1.GetComponent<Dot>().isMatched = true;
                downDot1.GetComponent<Dot>().isMatched = true;
                this.isMatched = true;
            }
        }
    }
}
