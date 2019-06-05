using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar : MonoBehaviour{

    public Transform startPos, targetPos; //posicao global do ponto inicial e do final

    private Grid grid;                    //referencia ao grid

    private void Awake() {
        grid = GetComponent<Grid>();
    }

    private void Update() {
        FindPath(startPos.position, targetPos.position);
    }

    //funcao que recebe duas posicoes globais e calcula o A* entre eles
    private void FindPath(Vector2 startPos, Vector2 targetPos) {
        Node startNode = grid.NodeFromWorldPoint(startPos);

        Node targetNode = grid.NodeFromWorldPoint(targetPos);
        
        //lista dos nos que ja foram checados mas ainda nao se sabe se o caminho ate eles eh o menor mesmo
        List<Node> openList = new List<Node>();
        
        //lista dos nos que ja possuem o menor caminho ateh o noh inicial
        HashSet<Node> closedList = new HashSet<Node>();

        //adiciona o noh inicial na openList
        openList.Add(startNode);

        //loop principal, roda enquano houver nos na openlist
        while(openList.Count > 0) {
            //seleciona o noh atual como o noh da openlist com o menor custo
            Node currentNode = openList[0];
            
            for (int i = 1; i < openList.Count; i++)
                if (openList[i].FCost < currentNode.FCost)
                    currentNode = openList[i];

            //remove ele da openList e coloca na closedList
            openList.Remove(currentNode);

            closedList.Add(currentNode);

            //verifica se o noh atual eh o noh final
            if (currentNode == targetNode)
                GetFinalPath(startNode, targetNode);
             
            //loop checando os nos vizinhos do noh atual
            foreach(Node neighborNode in grid.GetNeighboringNodes(currentNode)) {
                //se o noh vizinho for um obstaculo ou ja esta na closedList entao nao faz nada
                if (neighborNode.isWall || closedList.Contains(neighborNode))
                    continue;

                //calcula o custo para se mover do noh atual para o vizinho
                int moveCost = currentNode.gCost + GetManhattenDistance(currentNode, neighborNode);

                //se o custo for menor que o custo atual do vizinho ou se o vizinho ainda nao possui um custo
                if(moveCost < neighborNode.gCost || !openList.Contains(neighborNode)) {
                    //refaz os custos do noh vizinho e adicina o noh atual como pai do noh vizinho
                    neighborNode.gCost = moveCost;

                    neighborNode.hCost = GetManhattenDistance(neighborNode, targetNode);

                    neighborNode.parent = currentNode;

                    //se o noh vizinho nao estiver na openList, adiciona ele
                    if (!openList.Contains(neighborNode))
                        openList.Add(neighborNode);
                }
            }
        }
    }

    //gera o caminho entre um noh inicial e um noh final
    private void GetFinalPath(Node startNode, Node endNode) {
        List<Node> finalPath = new List<Node>();

        Node currentNode = endNode;

        //faz o caminho do noh final ateh o noh inicial utilizando o pai de cada noh
        while(currentNode != startNode) {
            finalPath.Add(currentNode);

            currentNode = currentNode.parent;
        }
        
        finalPath.Reverse();

        grid.finalPath = finalPath;
    }

    //funcao que calcula a distancia de Manhatten
    private int GetManhattenDistance(Node a, Node b) {
        int x = Mathf.Abs(a.gridX - b.gridX);

        int y = Mathf.Abs(a.gridY - b.gridY);

        return x + y;
    }
}
