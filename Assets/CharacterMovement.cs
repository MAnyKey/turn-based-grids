using UnityEngine;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Animation))]
//[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour {

    public float speed = 2.5f;
    public float rotationSpeed = 0.04f;

    public float minNextTileDest = 0.07f;

    public Func<Tile, Vector3> TilePosFunc { get; set; }

    private Animation animation_;
    //private CharacterController controller_;
    private bool isMoving_;
    private Transform transform_;

    public bool IsMoving {
        get {
            return isMoving_;
        }
    }

    public Tile tile;
    
    private struct Moving {
        public int curTileIdx;
        public Vector3 curTilePos;
        public List<Tile> path;
        public Action<CharacterMovement> endMove;

        public Moving(int curTileIdx, Vector3 curTilePos, List<Tile> path, Action<CharacterMovement> endMove) {
            this.curTilePos = curTilePos;
            this.curTileIdx = curTileIdx;
            this.path = path;
            this.endMove = endMove;
        }
    } 

    private Moving move_;

    // we assume that our current position is omitted from the path
    public void MoveTo(List<Tile> path, Action<CharacterMovement> endMove) {
        if (path.Count == 0) {
            return;
        }
        var curTileIdx = 0;
        var curTilePos = CalcTilePos(path[curTileIdx]);
        Debug.Log("CurTilePos: " + curTilePos);
        move_ = new Moving(curTileIdx, curTilePos, path, endMove);
        isMoving_ = true;
    }

    public void TeleportTo(Tile tile) {
        if (isMoving_) {
            return;
        }
        this.tile = tile;
        transform_.position = CalcTilePos(tile);
    }

    private Vector3 CalcTilePos(Tile tile) {
        var tilePos = TilePosFunc(tile);
        tilePos.y = transform.position.y;
        return tilePos;
    }

    void Awake() {
        isMoving_ = false;
        //controller_ = GetComponent<CharacterController>();
        animation_ = GetComponent<Animation>();
        animation_.wrapMode = WrapMode.Loop;
        transform_ = transform;
    }
	
	// Update is called once per frame
	void Update () {
	    if (!isMoving_) {
            return;
        }
        if (CloseEnoughToCurrentTile()) {
            Debug.Log("CloseEnoughToCurrentTile");
            bool finish = SwitchToNextTile();
            if (finish) {
                Debug.Log("Finish");
                var endMove = move_.endMove;
                tile = move_.path[move_.path.Count - 1];
                move_ = new Moving();
                isMoving_ = false;

                //animation_.CrossFade("idle");
                endMove(this);
                return;
            }
        }
        Debug.Log("Moving, pos: " + transform.position);
        MoveTowards(move_.curTilePos);
	}

    private void MoveTowards(Vector3 curTilePos) {
        var direction = move_.curTilePos - transform.position;
        //transform.rotation = Quaternion.Slerp(transform.rotation, 
        //    Quaternion.LookRotation(direction), rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.LookRotation(direction);

        var forward = transform.forward;
        forward *= speed;

        // just cosine of angle between |direction| and current object forward direction
        //float speedModifier = Vector3.Dot(direction.normalized, transform.forward);
        //const float speedModifierThreshold = 0.95f;
        //if (speedModifier > speedModifierThreshold) {
        //    // walk
        //    controller_.SimpleMove(forward * speedModifier);
        //    //if (!animation_["walk"].enabled) {
        //    //    animation_.CrossFade("walk");
        //    //}
        //} else {
        //    // idle, just rotating
        //    //if (!animation_["idle"].enabled) {
        //    //    animation_.CrossFade("idle");
        //    //}
        //}
        //controller_.SimpleMove(forward);
        transform.position += forward;
    }

    // returns true if we reached the last tile of the path
    private bool SwitchToNextTile() {
        move_.curTileIdx++;
        if (move_.curTileIdx == move_.path.Count) {
            return true;
        }
        move_.curTilePos = CalcTilePos(move_.path[move_.curTileIdx]);
        Debug.Log("Moving to " + move_.curTilePos);
        return false;
    }

    private bool CloseEnoughToCurrentTile() {
        var distanceVector = transform_.position - move_.curTilePos;
        return distanceVector.sqrMagnitude < minNextTileDest * minNextTileDest;
    }
}
