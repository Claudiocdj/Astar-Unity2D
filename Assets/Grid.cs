using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    public Vector2 gridWorldSize;       //tamanho global do grid

    public float nodeRadius;            //raio de cada no dentro do grid

    public LayerMask wallMask;          //objetos nesta layer serao considerados como obstaculos

    public List<Node> finalPath;        //caminho entre o noh de inicio e o de fim

    public Node[,] nodeArray;           //grid 2d formado por nos

    private float nodeDiameter;         //diametro de cada noh dentro do grid

    private int gridSizeX, gridSizeY;   //tamanho de cada elemento do grid

    private void Start() {
        //calcula o diametro que cada noh tera
        nodeDiameter = nodeRadius * 2;

        //o tamanho X e Y do grid eh definido pela quantidade de nos que cabem dentro do tamanho global do grid
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);

        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        CreateGrid();
    }
    
    //Cria o grid de nos
    private void CreateGrid() {
        nodeArray = new Node[gridSizeX, gridSizeY];

        //vamos usar o ponto inferior mais a esquerda para determinar a posicao global de cada no
        Vector2 bottomLeft;

        bottomLeft.x = transform.position.x - gridWorldSize.x / 2;

        bottomLeft.y = transform.position.y - gridWorldSize.y / 2;

        //loop passando por todos os campos da nodeArray e gerando os nos
        for(int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {

                //calcula a posicao global de cada noh considerando sua posicao e tamanho
                Vector2 worldPoint;

                worldPoint.x = bottomLeft.x + x * nodeDiameter + nodeRadius;

                worldPoint.y = bottomLeft.y + y * nodeDiameter + nodeRadius;

                //determina se aquele grid esta sobre algum obstaculo ou nao
                bool wall = false;

                if (Physics2D.OverlapCircle(worldPoint, nodeRadius, wallMask) != null)
                    wall = true;

                //cria o noh
                nodeArray[x, y] = new Node(wall, worldPoint, x, y);
            }
        }
    }

    //Funcao que recebe um noh do grid e retorna os seus vizinhos
    public List<Node> GetNeighboringNodes(Node node) {
        List<Node> neighborList = new List<Node>();

        int x, y;

        //checa o vizinho da direita
        x = node.gridX + 1;

        y = node.gridY;

        if (CheckGridBorders(x,y))
            neighborList.Add(nodeArray[x, y]);

        //checa o vizinho da esquerda
        x = node.gridX - 1;

        y = node.gridY;

        if (CheckGridBorders(x, y))
            neighborList.Add(nodeArray[x, y]);

        //checa o vizinho de cima
        x = node.gridX;

        y = node.gridY + 1;

        if (CheckGridBorders(x, y))
            neighborList.Add(nodeArray[x, y]);

        //checa o vizinho de baixo
        x = node.gridX;

        y = node.gridY - 1;

        if (CheckGridBorders(x, y))
            neighborList.Add(nodeArray[x, y]);

        return neighborList;
    }

    //Funcao que recebe uma posicao x,y e verifica se ela estah contida no grid
    private bool CheckGridBorders(int x, int y) {
        if (x < 0 || y < 0 || x >= gridSizeX || y >= gridSizeY)
            return false;

        return true;
    }

    //Funcao que recebe uma posicao global e retorna o noh mais proximo dele dentro do grid
    public Node NodeFromWorldPoint(Vector2 worldPos) {
        float xPos = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;

        float yPos = (worldPos.y + gridWorldSize.y / 2) / gridWorldSize.y;

        int x = Mathf.RoundToInt((gridSizeX - 1) * Mathf.Clamp01(xPos));

        int y = Mathf.RoundToInt((gridSizeY - 1) * Mathf.Clamp01(yPos));

        return nodeArray[x, y];
    }

    //Funcao para desenhar o grid, trajetoria A* e obstaculos
    private void OnDrawGizmos() {
        Gizmos.color = new Color(0, 0, 1, 1);

        //desenha o tamanho global do grid
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y,0f));

        //faz um loop por todos os elementos do grid
        if(nodeArray != null) {
            foreach(Node n in nodeArray) {
                //determina a cor do noh e depois desenha ele
                if (n.isWall)
                    Gizmos.color = new Color(1, 0, 0, .5f);
                
                else
                    Gizmos.color = new Color(1, 1, 1, .5f);

                if(finalPath != null && finalPath.Contains(n))
                    Gizmos.color = new Color(0, 1, 0, .5f);
                
                Gizmos.DrawCube(n.position, Vector3.one * nodeDiameter);
            }
        }
    }
}
