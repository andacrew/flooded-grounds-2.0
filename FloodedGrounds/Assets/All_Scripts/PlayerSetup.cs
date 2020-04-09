
using UnityEngine;
using UnityEngine.Networking;

namespace FloodedGrounds
{
    
public class PlayerSetup : NetworkBehaviour
{
   [SerializeField]
    Behaviour[] componentsToDisable;


    Camera MPsceneCamera;

   void Start()
   {
       if (!isLocalPlayer)
       {
       for(int i=0; i < componentsToDisable.Length; i++){
           componentsToDisable[i].enabled = false;
       }
       } else
       {
           MPsceneCamera =Camera.main;
           if(MPsceneCamera != null)
           {
            MPsceneCamera.gameObject.SetActive(false);
           }
       }
}
}
}