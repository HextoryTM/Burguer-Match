using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int largura;
    public int altura;
    public GameObject tilePrefab;
    public GameObject[] dots;
    public GameObject[,] allDots;

    private BackgroundTile[,] allTiles;
    

    // Start is called before the first frame update
    void Start()
    {
        allTiles = new BackgroundTile[largura, altura];
        allDots = new GameObject[largura, altura];
        SetUp();
    }

    private void SetUp()
    {
        for (int i = 0; i < largura; i++)
        {
            for(int j = 0; j < altura; j++)
            {
                Vector3 tempPos = new Vector3(i, j, 0f);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPos, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";

                int dotToUse = Random.Range(0, dots.Length);

                int maxInterations = 0;
                while (MatchesAt(i, j, dots[dotToUse]) && maxInterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxInterations++;
                    Debug.Log(maxInterations);
                }
                maxInterations = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity);
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";

                allDots[i, j] = dot;
            }
        }
    }

    private bool MatchesAt(int coluna, int linha, GameObject dot)
    {
        if(coluna > 1 && linha > 1)
        {
            if (allDots[coluna - 1, linha].tag == dot.tag && allDots[coluna - 2, linha].tag == dot.tag)
            {
                return true;
            }
            if (allDots[coluna, linha - 1].tag == dot.tag && allDots[coluna, linha - 2].tag == dot.tag)
            {
                return true;
            }
        }
        else if (coluna <= 1 || linha <= 1)
        {
            if(linha > 1)
            {
                if(allDots[coluna, linha - 1].tag == dot.tag && allDots[coluna, linha - 2].tag == dot.tag)
                {
                    return true;
                }
            }
            if (coluna > 1)
            {
                if (allDots[coluna - 1, linha].tag == dot.tag && allDots[coluna - 2, linha].tag == dot.tag)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
