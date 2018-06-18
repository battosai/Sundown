using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: be a hitbox for attacks, player action key, etc. (no collider needed)

public enum ColliderState {CLOSED, OPEN, COLLIDING};
public class Hitbox : MonoBehaviour
{
  public ContactFilter2D mask;
  public Color closedColor, openColor, collidingColor;

  private int maxTargets = 5;
  private Vector2 size = new Vector2(5, 5); //half dimensions
  private Vector2 offset = new Vector2(0, 0);
  private ColliderState state;
  private Action action;
  private Transform trans;
  private IHitboxResponder responder;

  public void SetResponder(IHitboxResponder responder){this.responder = responder;}
  public void SetSize(Vector2 size){this.size = size;}
  public void SetOffset(Vector2 offset){this.offset = offset;}
  public void SetAction(Action action){this.action = action;}

  void Awake()
  {
    trans = GetComponent<Transform>();
  }
  void Start()
  {
    responder = null;
    state = ColliderState.CLOSED;
  }

  public void StartCheckingCollision()
  {
    state = ColliderState.OPEN;
  }
  public void StopCheckingCollision()
  {
    state = ColliderState.CLOSED;
  }

  public void CheckCollision()
  {
    if(state == ColliderState.CLOSED)
      return;
    Collider2D[] colliders = new Collider2D[maxTargets];
    int collisions = Physics2D.OverlapBox((Vector2)trans.position+offset, size, 0f, mask, colliders);
    for(int i = 0; i < collisions; i++)
    {
      Collider2D coll = colliders[i];
      if(responder != null)
        responder.Hit(coll, action);
    }
    state = collisions > 0 ? ColliderState.COLLIDING : ColliderState.OPEN;
  }

  void OnDrawGizmos()
  {
    colorGizmo();
    Gizmos.matrix = Matrix4x4.TRS((Vector2)transform.position+offset, transform.rotation, transform.localScale);
    Gizmos.DrawCube(Vector3.zero, new Vector3(size.x*2, size.y*2, 0)); // Because size is halfExtents
  }

  private void colorGizmo()
  {
    switch(state)
    {
      case ColliderState.CLOSED:
        Gizmos.color = closedColor;
        break;
      case ColliderState.OPEN:
        Gizmos.color = openColor;
        break;
      case ColliderState.COLLIDING:
        Gizmos.color = collidingColor;
        break;
    }
  }
}
