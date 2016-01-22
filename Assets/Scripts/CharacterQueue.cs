using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CharacterQueue : MonoBehaviour {

    // TODO: move outside of this class
    public List<Point> startingPoints;

    //    public List<Character> Characters { get; set; }
    public void PlaceCharacters(List<Character> characters) {
        characters_ = new CharQueue(characters);
    }

    public void NextCharacter() {
        characters_.MoveToNextCharacter();
    }

    public Character CurrentCharacter() {
        return characters_.Character;
    }


    private class CharQueue {
        public CharQueue(List<Character> characters) {
            characters_ = characters;
            currentCharacter_ = 0;
        }

        public Character Character { 
            get {
                return characters_[currentCharacter_];
            }
        }

        public void MoveToNextCharacter() {
            currentCharacter_ = (currentCharacter_ + 1) % characters_.Count;
        }

        private List<Character> characters_;
        private int currentCharacter_;
    }

    private CharQueue characters_;
}
