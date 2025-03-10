using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ProceduralGenerationAlgorithms
{

    public static HashSet<Vector2Int> SimpleRandomWalk(Vector2Int startPosition, int walkLength)
    {
        HashSet<Vector2Int> path = new HashSet<Vector2Int>();

        path.Add (startPosition);
        var previousposition = startPosition;

        for (int i = 0; i < walkLength; i++)
        {
            var newPosition = previousposition + Direction2D.GetRandomCardinalDirection();
            path.Add(newPosition);
            previousposition = newPosition;
        }
        return path;
    }

    public static HashSet<Vector2Int> RandomWalkCorridor(Vector2Int startPosition, int corridorLength)
    {
        List<Vector2Int> corridor = new List<Vector2Int>();
        var direction = Direction2D.GetRandomCardinalDirection();
        var currentPosition = startPosition;
        corridor.Add(currentPosition);

        for (int i = 0;i < corridorLength;i++) 
        {
            currentPosition += direction;
            corridor.Add(currentPosition);
        }
        return new HashSet<Vector2Int>(corridor);
    }
}


public static class Direction2D
{
    public static List<Vector2Int> cardinalDirectionList = new List<Vector2Int>
    {
        new Vector2Int (0, 1), //up
        new Vector2Int (1, 0), //right
        new Vector2Int (0, -1), //down
        new Vector2Int (-1, 0) //left
    };

    public static List<Vector2Int> diagonalDirectionList = new List<Vector2Int>
    {
        new Vector2Int (1, 1), //up-right
        new Vector2Int (1, -1), //right-down
        new Vector2Int (-1, -1), //down-left
        new Vector2Int (-1, 1) //left-up
    };

    public static List<Vector2Int> eightDirectionList = new List<Vector2Int>()
    {
        new Vector2Int (0, 1), //up
        new Vector2Int (1, 1), //up-right
        new Vector2Int (1, 0), //right
        new Vector2Int (1, -1), //right-down
        new Vector2Int (0, -1), //down
        new Vector2Int (-1, -1), //down-left
        new Vector2Int (-1, 0), //left
        new Vector2Int (-1, 1) //left-up
    };

    public static Vector2Int GetRandomCardinalDirection()
    {
        return cardinalDirectionList[Random.Range(0, cardinalDirectionList.Count)];
    }
}