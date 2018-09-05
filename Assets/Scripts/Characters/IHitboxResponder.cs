using UnityEngine;
using System.Collections;

//ROLE: implement by anything that should do something when a hitbox hits a collider (attacks, etc)

public interface IHitboxResponder
{
  void Hit(Collider2D other, Act act);
}
