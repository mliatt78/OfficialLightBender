using UnityEngine;

public class QuitGameG : MonoBehaviour
{
   public void QuitGame ()
   {
      Debug.Log("QUIT");
      Application.Quit();
   }
}
