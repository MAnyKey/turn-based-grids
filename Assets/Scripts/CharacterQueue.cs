using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CharacterQueue : MonoBehaviour {

    // TODO: move outside of this class
    public List<Point> startingPoints;

    //    public List<Character> Characters { get; set; }
    public void PlaceCharacters(List<Character> characters) {
        allCharacters_ = new HashSet<Character>(characters);
        queue_ = new TurnQueue(characters);
    }

    public Character GetCurrentCharacter() {
        return queue_.Character;
    }

    public void CharacterDidAction() {
        queue_.DoAction();
    }

    public void CharacterIsWaiting() {
        throw new NotImplementedException();
    }

    // Character can move only on next turn
    public void AddCharacter(Character character) {
        throw new NotImplementedException();
    }

    // Character dies
    public void RemoveCharacter(Character character) {
        throw new NotImplementedException();
    }


    private class TurnQueue {
        public TurnQueue(List<Character> characters) {
            // FIXME: changing initiative of character doesn't change it's order in the queue
            // TODO: form queue
        }

        public Character Character { 
            get { 
                if (normalTurn_.Count != 0) {
                    return normalTurn_.Peek();
                }
                return waiting_[waiting_.Count - 1];
            }
        }


        public void Wait() {
            Debug.Assert(!IsEmpty());
            Debug.Assert(!IsWaitStage, "Cannot wait in wait stage");

            using (var notifier = new TurnsHandle(this)) {
                var character = normalTurn_.Dequeue();
                waiting_.Add(character);    
            }
        }

        public void DoAction() {
            Debug.Assert(!IsEmpty());

            using (var notifier = new TurnsHandle(this)) {
                if (IsWaitStage) {
                    waiting_.RemoveAt(waiting_.Count - 1);    
                    return; 
                } 
                normalTurn_.Dequeue();    
            }
        }

        public void RemoveCharacter(Character character) {
            using (var notifier = new TurnsHandle(this)) {
                // TODO: remove from normalTurn_
                waiting_.RemoveAll(character);
            }
        }

        private class TurnsHandle : IDisposable {
            public TurnsHandle(TurnQueue queue) {
                queue_ = queue;
                wasWaitStage_ = queue.IsWaitStage;
            }

            public void Dispose() {
                if (queue_.IsEndOfTheTurn) {
                    queue_.NotifyEndOfTheTurn();
                }                
                if (!wasWaitStage_ && queue_.IsWaitStage) {
                    queue_.NotifyWaitStage();
                }
            }

            private readonly TurnQueue queue_;
            private readonly bool wasWaitStage_;
        }

        private bool IsWaitStage { get { return normalTurn_.Count != 0; } }

        private bool IsEndOfTheTurn { get { return IsEmpty; } }

        private bool IsEmpty() {
            return normalTurn_.Count != 0 || waiting_.Count != 0;
        }


        private void NotifyEndOfTheTurn() {
            throw new NotImplementedException();
        }

        private void NotifyWaitStage() {
            throw new NotImplementedException();
        }

        private Queue<Character> normalTurn_;
        private List<Character> waiting_;

    
    }


    private HashSet<Character> allCharacters_;
    private TurnQueue queue_;
    private int turn_;
}
