using UnityEngine;

public abstract class Item : MonoBehaviour
{
  public ItemInfo iteminfo;
  public GameObject itemGameObject;

  public abstract void Use();
  
}
