using System;
using UnityEngine;

namespace Game
{
    public enum Act {ATTACK, TRAVEL, INTERACT, ALARM, SHAPESHIFT}; //interact could hide in bushes, talk to npcs, search containers
	public enum CharacterType {PLAYER, HERO, WILDLIFE, TOWNSPERSON};
    public enum BuildingType {BARRACKS, HOME};
    public enum InteriorSize {SMALL, MEDIUM, LARGE};
    public enum ColliderState {CLOSED, OPEN, COLLIDING};
}