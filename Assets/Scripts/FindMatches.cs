using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FindMatches : MonoBehaviour
{
    private Board board;

    public List<GameObject> currentMatches = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);

        for (int i = 0; i < board.largura; i++)
        {
            for(int j = 0; j < board.altura; j++)
            {
                GameObject currentDot = board.allDots[i, j];
                if(currentDot != null)
                {
                    // Horizontal Match
                    if (i > 0 && i < board.largura - 1)
                    {
                        GameObject leftDot = board.allDots[i - 1, j];
                        GameObject rightDot = board.allDots[i + 1, j];
                        if (leftDot != null & rightDot != null)
                        {
                            if (leftDot.tag == currentDot.tag && rightDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isLinhaBomb || leftDot.GetComponent<Dot>().isLinhaBomb || rightDot.GetComponent<Dot>().isLinhaBomb)
                                {
                                    currentMatches.Union(GetLinhaDots(j));
                                }

                                if (currentDot.GetComponent<Dot>().isColunaBomb)
                                {
                                    currentMatches.Union(GetColunaDots(i));
                                }

                                if (leftDot.GetComponent<Dot>().isColunaBomb)
                                {
                                    currentMatches.Union(GetColunaDots(i - 1));
                                }

                                if (rightDot.GetComponent<Dot>().isColunaBomb)
                                {
                                    currentMatches.Union(GetColunaDots(i + 1));
                                }


                                if (!currentMatches.Contains(leftDot))
                                    currentMatches.Add(leftDot);
                                leftDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(rightDot))
                                    currentMatches.Add(rightDot);
                                rightDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(currentDot))
                                    currentMatches.Add(currentDot);
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }

                    // Vertical Match
                    if (j > 0 && j < board.altura - 1)
                    {
                        GameObject upDot = board.allDots[i, j + 1];
                        GameObject downDot = board.allDots[i, j - 1];
                        if (upDot != null & downDot != null)
                        {
                            if (upDot.tag == currentDot.tag && downDot.tag == currentDot.tag)
                            {
                                if (currentDot.GetComponent<Dot>().isColunaBomb || upDot.GetComponent<Dot>().isColunaBomb || downDot.GetComponent<Dot>().isColunaBomb)
                                {
                                    currentMatches.Union(GetColunaDots(i));
                                }

                                if (currentDot.GetComponent<Dot>().isLinhaBomb)
                                {
                                    currentMatches.Union(GetLinhaDots(j));
                                }

                                if (upDot.GetComponent<Dot>().isLinhaBomb)
                                {
                                    currentMatches.Union(GetLinhaDots(j + 1));
                                }

                                if (downDot.GetComponent<Dot>().isLinhaBomb)
                                {
                                    currentMatches.Union(GetLinhaDots(j - 1));
                                }


                                if (!currentMatches.Contains(upDot))
                                    currentMatches.Add(upDot);
                                upDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(downDot))
                                    currentMatches.Add(downDot);
                                downDot.GetComponent<Dot>().isMatched = true;

                                if (!currentMatches.Contains(currentDot))
                                    currentMatches.Add(currentDot);
                                currentDot.GetComponent<Dot>().isMatched = true;
                            }
                        }
                    }
                }
            }
        }
    }

    List<GameObject> GetColunaDots(int coluna)
    {
        List<GameObject> dots = new List<GameObject>();

        for(int i = 0; i < board.altura; i++)
        {
            if(board.allDots[coluna, i] != null)
            {
                dots.Add(board.allDots[coluna, i]);
                board.allDots[coluna, i].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    List<GameObject> GetLinhaDots(int linha)
    {
        List<GameObject> dots = new List<GameObject>();

        for (int i = 0; i < board.largura; i++)
        {
            if (board.allDots[i, linha] != null)
            {
                dots.Add(board.allDots[i, linha]);
                board.allDots[i, linha].GetComponent<Dot>().isMatched = true;
            }
        }

        return dots;
    }

    public void CheckBombs()
    {
        //Se o jogador mover algo
        if(board.currentDot != null)
        {
            //Se for a peca que foi mexida
            if (board.currentDot.isMatched)
            {
                //Unmatch
                board.currentDot.isMatched = false;

                //Decide qual bomba criar
                /*
                int typeOfBomb = Random.Range(0, 100);
                if (typeOfBomb < 50)
                {
                    //Bomba de linha
                    board.currentDot.MakeLinhaBomb();
                } else if (typeOfBomb >= 50)
                {
                    //Bomba de coluna
                    board.currentDot.MakeColunaBomb();
                }
                */

                if((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                    board.currentDot.MakeLinhaBomb();
                else
                    board.currentDot.MakeColunaBomb();
            }
            //Se a outra peca estiver "Matched"
            else if (board.currentDot.otherDot != null)
            {
                Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
                //Se o outro dot for Matched
                if (otherDot.isMatched)
                {
                    //Unmatch
                    otherDot.isMatched = false;

                    //Decide qual bomba criar
                    /*
                    int typeOfBomb = Random.Range(0, 100);
                    if (typeOfBomb < 50)
                    {
                        //Bomba de linha
                        otherDot.MakeLinhaBomb();
                    }
                    else if (typeOfBomb >= 50)
                    {
                        //Bomba de coluna
                        otherDot.MakeColunaBomb();
                    }
                    */

                    if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
                        otherDot.MakeLinhaBomb();
                    else
                        otherDot.MakeColunaBomb();
                }
            }
        }
    }
}
