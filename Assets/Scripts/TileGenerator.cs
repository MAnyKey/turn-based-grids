using UnityEngine;
using System.Collections;
using System;

public class TileGenerator : MonoBehaviour {

    public GameObject hexPrefab;
    public Transform grid;

    public int gridWidth;
    public int gridHeight;

    public class ConfigurationException : Exception {
        public ConfigurationException(string message) : base(message) {
        }
    }

	void Start () {
        if (gridWidth <= 0 || gridHeight <= 0) {
            throw new ConfigurationException("Grid sizes should be positive");
        }
        GenerateMap(GetHexSize());
	}
	
    private Vector2 GetHexSize() {
        var hexRenderer = hexPrefab.GetComponent<Renderer>();
        var rendererSize = hexRenderer.bounds.size;

        var hexWidth = rendererSize.x;
        var hexHeight = rendererSize.z;
        return new Vector2(hexWidth, hexHeight);
    }
    
    private void GenerateMap(Vector2 hexSize) {
        var initPos = CalculateInitPosition(hexSize);

        Action<float, float> createHex = (float x, float y) => {
            var pos = CalculateNeededPosition(initPos, new Vector2(x, y), hexSize);
            CreateHex(pos);
        };

        for (int y = 0; y < gridHeight; ++y) {
            for (int x = 0; x < gridWidth; ++x) {
                createHex(x, y);
            }
        }
    }

    private Vector3 CalculateInitPosition(Vector2 hexSize) {
        var width = hexSize.x * gridWidth;
        if (gridHeight > 1) {
            width += hexSize.x / 2;
        }
        var height = hexSize.y;
        if (gridHeight > 1) {
            height += hexSize.y * (gridHeight - 1) * 0.75f;
        }
        var leftUpCorner = new Vector2(-width / 2, height / 2);
        var init2D = new Vector2(leftUpCorner.x + hexSize.x / 2, leftUpCorner.y - hexSize.y / 2);
        return new Vector3(init2D.x, 0, init2D.y);
    }

    private Vector3 CalculateNeededPosition(Vector3 initPos, Vector2 gridPos, Vector2 hexSize) {
        float xOffset = 0;
        if (gridPos.y % 2 != 0) {
            xOffset = hexSize.x / 2f;
        }
        var offsetFromInit = new Vector3(xOffset + gridPos.x * hexSize.x, 0, -gridPos.y * hexSize.y * 0.75f);
        return initPos + offsetFromInit;
    }

    private void CreateHex(Vector3 position) {
        var hex = Instantiate(hexPrefab);
        hex.transform.position = position;
        hex.transform.parent = grid;
    }
}
