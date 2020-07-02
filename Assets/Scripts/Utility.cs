using System;
using UnityEngine;

namespace Game
{
    public enum Act {
        SHOVE,
        ATTACK, 
        TRAVEL, 
        INTERACT, 
        ALARM, 
        SHAPESHIFT}; //interact could hide in bushes, talk to npcs, search containers
    public enum BuildingType {BARRACKS, HOME};
    public enum InteriorSize {SMALL, MEDIUM, LARGE};
    public enum ColliderState {CLOSED, OPEN, COLLIDING};
}