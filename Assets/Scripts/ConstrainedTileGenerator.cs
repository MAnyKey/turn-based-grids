using UnityEngine;
using System;
using System.Collections.Generic;

public class ConstrainedTileGenerator : MonoBehaviour {

    public GameObject hexSample;
    public Transform hexGrid;
    public GameObject constraint;
    public GameObject character;

    private Vector2 hexSize;
    private Vector2 constraintSize;

    private int gridWidth;
    private int gridHeight;


	void Start () {
        hexSize = GetPlaneSize(hexSample);
        constraintSize = GetPlaneSize(constraint);

        ComputeGridSize();
        GenerateGrid();
	}

    private void GenerateGrid() {
        GridManager gridManager = hexGrid.gameObject.AddComponent<GridManager>();

        bool canPlaceFullRows = ((gridWidth + 0.5) * hexSize.x) <= constraintSize.x;
        var initPos = CalculateInitialPosition(canPlaceFullRows);
        Func<int, int, TileBehavior> createHex = (x, y) => {
            var actualPoint = CalculateNeededPosition(initPos, new Vector2(x, y));
            return CreateHex(actualPoint, new Point(x - (y / 2), y));
        };
        var board = new Dictionary<Point, TileBehavior>();
        for (int y = 0; y < gridHeight; ++y) {
            var currentRowWidth = gridWidth;
            if (y % 2 != 0 && !canPlaceFullRows) {
                currentRowWidth--;
            }
            for (int x = 0; x < currentRowWidth; ++x) { 
                var tb = createHex(x, y);
                board.Add(tb.Tile.Location, tb);
            }
        }
        gridManager.Board = board;
        gridManager.CharacterSample = character;
    }

    private TileBehavior CreateHex(Vector3 actualPoint, Point location) {
        var newHex = Instantiate(hexSample);
        newHex.transform.position = actualPoint;
        newHex.transform.parent = hexGrid;

        var tb = newHex.GetComponent<TileBehavior>();
        if (tb == null) {
            tb = newHex.AddComponent<TileBehavior>();
        }
        var tile = new Tile(location);
        tb.Tile = tile;
        return tb;
    }

    private Vector3 CalculateNeededPosition(Vector3 initPos, Vector2 gridPos) {
        float xOffset = 0;
        if (gridPos.y % 2 != 0) {
            xOffset = hexSize.x / 2f;
        }
        var offsetFromInit = new Vector3(xOffset + gridPos.x * hexSize.x, 0, -gridPos.y * hexSize.y * 0.75f);
        return initPos + offsetFromInit;
    }

    private Vector3 CalculateInitialPosition(bool canPlaceFullOddRow) {
        var constraintBounds = constraint.GetComponent<Renderer>().bounds;
        var center = constraintBounds.center;

        var width = hexSize.x * gridWidth;
        if (gridHeight > 1 && canPlaceFullOddRow) {
            width += hexSize.x / 2;
        }
        var height = hexSize.y;
        if (gridHeight > 1) {
            height += hexSize.y * (gridHeight - 1) * 0.75f;
        }
        var leftUpCorner = new Vector2(-width / 2, height / 2) + RemoveYCoord(center);
        var init2D = new Vector2(leftUpCorner.x + hexSize.x / 2, leftUpCorner.y - hexSize.y / 2);

        return AddZeroYCoord(init2D);
    }

    private void ComputeGridSize() {
        gridWidth = (int)(constraintSize.x / hexSize.x);


        if (constraintSize.y < hexSize.y) {
            gridHeight = 0;
            return;
        }
        // we need to place one full-height row and then only rows by 3/4 of the hex's height
        gridHeight = 1 + (int)((constraintSize.y - hexSize.y) / (hexSize.y * 0.75f));
    }

    private Vector2 GetPlaneSize(GameObject obj) {
        var renderer = obj.GetComponent<Renderer>();
        var size = renderer.bounds.size;
        return RemoveYCoord(size);
    }

    private static Vector2 RemoveYCoord(Vector3 v) {
        return new Vector2(v.x, v.z);
    }

    private static Vector3 AddZeroYCoord(Vector2 v) {
        return new Vector3(v.x, 0, v.y);
    }
    
}
