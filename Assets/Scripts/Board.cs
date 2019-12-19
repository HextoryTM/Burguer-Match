using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { wait, move}

public class Board : MonoBehaviour
{
    [Header("Board Variables")]
    public GameState currentState = GameState.move;
    public int largura;
    public int altura;
    public int offSet;
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

    // Inicializa o background e as pecas do tabuleiro.
    private void SetUp()
    {
        for (int i = 0; i < largura; i++)
        {
            for(int j = 0; j < altura; j++)
            {
                Vector2 tempPos = new Vector2(i, j + offSet);
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
                dot.GetComponent<Dot>().linha = j;
                dot.GetComponent<Dot>().coluna = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";

                allDots[i, j] = dot;
            }
        }
    }

    // Verifica "Matches" em certas posicoes (coluna, linha) e compara as tags com o objeto passado (dot).
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

    // Destroi objetos que estiverem "matched" em uma posicao especifica (coluna, linha).
    private void DestroyMatchesAt(int coluna, int linha)
    {
        if (allDots[coluna, linha].GetComponent<Dot>().isMatched)
        {
            Destroy(allDots[coluna, linha]);
            allDots[coluna, linha] = null;
        }
    }

    // Destroi todos os objetos que estiverem "matched". Em seguida inicia uma Coroutine que desce as linhas dos objetos afetados.
    public void DestroyMatches()
    {
        for (int i = 0; i < largura; i++)
        {
            for (int j = 0; j < altura; j++)
            {
                if (allDots[i, j] != null)
                    DestroyMatchesAt(i, j);
            }
        }
        StartCoroutine(DecreaseRowCo());
    }

    // Coroutine que desce as linhas das colunas afetadas por objedos destruidos apos o "Match". No final, inicia uma nova Coroutine que instancia novos dots nas colunas e linhas afetadas.
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;

        for(int i = 0; i < largura; i++)
        {
            for (int j = 0; j < altura; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().linha -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }

        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    // Metodo usado para Instanciar novas pecas no tabuleiro, nas linhas e colunas afetadas por cada "Match".
    private void RefillBoard()
    {
        for (int i = 0; i < largura; i++)
        {
            for (int j = 0; j < altura; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offSet);
                    int dotToUse = Random.Range(0, dots.Length);

                    GameObject dot = Instantiate(dots[dotToUse], tempPos, Quaternion.identity) as GameObject;
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                    dot.GetComponent<Dot>().linha = j;
                    dot.GetComponent<Dot>().coluna = i;
                }
            }
        }
    }

    // Metodo de retorno booleano (true/false) usado para verificar se possui "matches" atualmente no tabuleiro.
    private bool MatchesOnBoard()
    {
        for (int i = 0; i < largura; i++)
        {
            for (int j = 0; j < altura; j++)
            {
                if (allDots[i, j] != null)
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
            }
        }
        return false;
    }

    // Coroutine usada para Instanciar novas pecas no tabuleiro, verificar novos "matches" apos a instanciacao, destruir os novos matches;
    //
    // obs: Fica no loop ate que nao tenha mais "matches" apos as instanciacoes.
    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }

}
