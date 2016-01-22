using UnityEngine;
using System.Collections;
using System;

[Serializable]
public struct Point {
    public Point(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;

    public override string ToString() {
        return String.Format("Point({0},{1})", x, y);
    }
}
