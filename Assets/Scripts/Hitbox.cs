using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ROLE: be a hitbox for attacks, player action key, etc. (no collider needed)

public enum ColliderState {CLOSED, OPEN, COLLIDING};
public class Hitbox : MonoBehaviour
{
  public LayerMask mask;
  public Color closedColor, openColor, collidingColor;

  private Vector2 size = Vector2.one; //half dimensions
  private ColliderState state;
  private Transform trans;
  private IHitboxResponder responder;

  public void setResponder(IHitboxResponder responder){this.responder = responder;}

  void Awake()
  {
    trans = GetComponent<Transform>();
  }
  void Start()
  {
    responder = null;
  }
  void Update()
  {
  }

  private void startCheckingCollision()
  {
    state = ColliderState.OPEN;
  }
  private void stopCheckingCollision()
  {
    state = ColliderState.CLOSED;
  }

  private void checkCollision()
  {
    if(state == ColliderState.CLOSED)
      return;
    Colliders2D[] colliders = Physics2D.OverlapBox(trans.position, size, tran.rotation, mask);
    for(int i = 0; i < colliders.Length; i++)
    {
      Collider2D coll = colliders[i];
      responder?.collisionWith(coll);
    }
    state = colliders.Length > 0? ColliderState.COLLIDING : ColliderState.OPEN;
  }

  private void OnDrawGizmos()
  {
    colorGizmo();
    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
    Gizmos.DrawCube(Vector3.zero, new Vector3(size.x * 2, size.y * 2, 0)); // Because size is halfExtents
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
