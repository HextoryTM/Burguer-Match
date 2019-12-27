using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    // Variaveis do Tabuleiro
    [Header("Board Variables")]
    public int coluna;
    public int linha;
    public int colunaAnt;
    public int linhaAnt;
    public int targetX;
    public int targetY;
    public bool isMatched = false;
    public GameObject otherDot;

    private FindMatches findMatches;
    private Board board;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;

    [Header("Swipe Stuff")]
    public float swipeAngle = 0f;
    public float swipeResist = 1f;

    [Header("Powerup Stuff")]
    public bool isColunaBomb;
    public bool isLinhaBomb;
    public GameObject linhaArrow;
    public GameObject colunaArrow;

    // Start is called before the first frame update
    void Start()
    {
        isColunaBomb = false;
        isLinhaBomb = false;

        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }

    //Apenas para teste e Debug;
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isColunaBomb = true;
            GameObject arrow = Instantiate(colunaArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            isLinhaBomb = true;
            GameObject arrow = Instantiate(linhaArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (isLinhaBomb || isColunaBomb)
        //    gameObject.tag = "Untagged";

        /*
        if (isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(1f, 1f, 1f, .2f);
        }
        */

        targetX = coluna;
        targetY = linha;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            // Move o target pra direcao escolhida
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);
            if (board.allDots[coluna, linha] != this.gameObject)
            {
                board.allDots[coluna, linha] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            // Define a posicao diretamente
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            // Move o target pra direcao escolhida
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);
            if (board.allDots[coluna, linha] != this.gameObject)
            {
                board.allDots[coluna, linha] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            // Define a posicao diretamente
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }

    // Coroutine que checa o movimento entre 2 pecas diferentes, retornando a posicao inicial caso nao tenha "matches" depois do movimento;
    //Caso tenha "matches" chama um metodo para destruir os novos matches.
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.5f);
        if(otherDot != null)
        {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().linha = linha;
                otherDot.GetComponent<Dot>().coluna = coluna;
                linha = linhaAnt;
                coluna = colunaAnt;

                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
            //otherDot = null;
        }
    }

    // Metodo que detecta quando o botao mouse1 foi pressionado.
    private void OnMouseDown()
    {
        if (board.currentState == GameState.move)
        {
            firstTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    // Metodo que detecta quando o botao mouse1 foi solto.
    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalTouchPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }
    }

    // Metodo que calcula o angulo para mover a peca na direcao certa conforme o mouse do jogador.
    void CalculateAngle()
    {
        if (Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist || Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            MoveDots();
            board.currentState = GameState.wait;
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    // Metodo que move as pecas conforme o angulo calculado. Assim defininco o lado que ira mover;
    // Depois inicia uma Coroutine para verificar o novo movimento e retornar a posicao caso nao tenha "matches".
    void MoveDots()
    {
        if(swipeAngle > -45 && swipeAngle <= 45 && coluna < board.largura - 1)
        {
            //Right Swipe
            otherDot = board.allDots[coluna + 1, linha];
            linhaAnt = linha;
            colunaAnt = coluna;
            otherDot.GetComponent<Dot>().coluna -= 1;
            coluna += 1;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && linha < board.altura - 1)
        {
            //Up Swipe
            otherDot = board.allDots[coluna, linha + 1];
            linhaAnt = linha;
            colunaAnt = coluna;
            otherDot.GetComponent<Dot>().linha -= 1;
            linha += 1;
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && coluna > 0)
        {
            //Left Swipe
            otherDot = board.allDots[coluna - 1, linha];
            linhaAnt = linha;
            colunaAnt = coluna;
            otherDot.GetComponent<Dot>().coluna += 1;
            coluna -= 1;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && linha > 0)
        {
            //Down Swipe
            otherDot = board.allDots[coluna, linha - 1];
            linhaAnt = linha;
            colunaAnt = coluna;
            otherDot.GetComponent<Dot>().linha += 1;
            linha -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    // Metodo usado para verificar se ha matches envolvendo essa peca e as adjacentes.
    void FindMatches()
    {
        if (coluna > 0 && coluna < board.largura - 1)
        {
            GameObject leftDot1 = board.allDots[coluna - 1, linha];
            GameObject rightDot1 = board.allDots[coluna + 1, linha];
            if (leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    this.isMatched = true;
                }
            }
        }

        if (linha > 0 && linha < board.altura - 1)
        {
            GameObject upDot1 = board.allDots[coluna, linha + 1];
            GameObject downDot1 = board.allDots[coluna, linha - 1];
            if (upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    this.isMatched = true;
                }
            }
        }
    }

    public void MakeColunaBomb()
    {
        isColunaBomb = true;
        GameObject arrow = Instantiate(colunaArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeLinhaBomb()
    {
        isLinhaBomb = true;
        GameObject arrow = Instantiate(linhaArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
}
