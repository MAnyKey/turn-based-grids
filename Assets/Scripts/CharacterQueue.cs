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

    public void CharacterHasTurned() {
        queue_.MoveToNextCharacter();
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

            var character = normalTurn_.Dequeue();
            waiting_.Add(character);

            if (IsWaitStage) {
                NotifyWaitStage();    
            }
        }

        public void DoAction() {
            Debug.Assert(!IsEmpty());

            bool wasWaitStage = IsWaitStage;

            if (!wasWaitStage) {
                normalTurn_.Dequeue();    
            } else {
                waiting_.RemoveAt(waiting_.Count - 1);
            }

            if (IsEndOfTheTurn) {
                NotifyEndOfTheTurn();
            }

            if (!wasWaitStage && IsWaitStage) {
                NotifyWaitStage();
            }
        }

        private bool IsWaitStage { get { return normalTurn_.Count != 0; } }

        private bool IsEndOfTheTurn { get { return IsEmpty; } }

        private bool IsEmpty() {
            return normalTurn_.Count != 0 || waiting_.Count != 0;
        }




        private Queue<Character> normalTurn_;
        private List<Character> waiting_;

    
    }


    private HashSet<Character> allCharacters_;
    private TurnQueue queue_;
    private int turn_;
}
