using UnityEngine;
using System.Collections;

//ROLE: implement by anything that should o something when a hitbox hits a collider (attacks, etc)

public interface HitboxResponder
{
  void collisionWith(Collider2D collider);
}
