using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {
    public int gridX, gridY;    //posicao do noh no grid

    public Vector2 position;    //posicao global do noh

    public bool isWall;         //se o noh eh percorrivel ou nao
    
    public Node parent;         //noh pai

    public int gCost, hCost;    //custo G e H

    public int FCost { get { return gCost + hCost; } } //custo F

    //Construtor
    public Node(bool isWall, Vector2 position, int gridX, int gridY) {
        this.isWall = isWall;

        this.position = position;

        this.gridX = gridX;

        this.gridY = gridY;
    }
}
