using UnityEngine;
using System.Collections;

//ROLE: implement by anything that should o something when a hitbox hits a collider (attacks, etc)

public interface IHitboxResponder
{
  void collisionWith(Collider2D other, Action action);
}
