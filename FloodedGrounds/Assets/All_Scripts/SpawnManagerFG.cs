
using UnityEngine;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game
{
[System.Serializable]

  
 public class SpawnManagerFG : MonoBehaviourPunCallbacks, IOnEventCallback
    {
        // Specifies the location that the character should spawn.
        
        public GameObject Human { get { return m_Human; } set { m_Human = value; } }
        public GameObject Monster { get { return m_Monster; } set { m_Monster = value; } }  
        public enum SpawnMode
        {
            SpawnPoint      // Uses the Spawn Point system.
        }

        [SerializeField] public GameObject m_Human;
        [SerializeField] public GameObject m_Monster;
        
        [Tooltip("The offset to apply to the spawn position multiplied by the number of characters within the room.")]
        [SerializeField] protected Vector3 m_SpawnLocationOffset = new Vector3(2, 0, 0);
        [Tooltip("The grouping index to use when spawning to a spawn point. A value of -1 will ignore the grouping value.")]
        protected int m_SpawnPointGrouping = 0;

        public Vector3 SpawnLocationOffset { get { return m_SpawnLocationOffset; } set { m_SpawnLocationOffset = value; } }
        public int SpawnPointGrouping { get { return m_SpawnPointGrouping; } set { m_SpawnPointGrouping = value; } }

        private PhotonView[] m_Players;
        private int m_PlayerCount;

        private SendOptions m_ReliableSendOption;
        private RaiseEventOptions m_RaiseEventOptions;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Start()
        {
            var kinematicObjectManager = GameObject.FindObjectOfType<KinematicObjectManager>();
            m_Players = new PhotonView[kinematicObjectManager.StartCharacterCount];

            // Cache the raise event options.
            m_ReliableSendOption = new SendOptions { Reliability = true };
            m_RaiseEventOptions = new RaiseEventOptions();
            m_RaiseEventOptions.CachingOption = EventCaching.DoNotCache;
            m_RaiseEventOptions.Receivers = ReceiverGroup.Others;

            SpawnPlayer(PhotonNetwork.LocalPlayer);
        }
    protected GameObject GetCharacterPrefab(Player newPlayer)
    {
        if(PhotonNetwork.PlayerList.Length == 1){
            return m_Monster;
        }
        return m_Human;
            
    }
     protected bool isMonster(Player newPlayer){
         if(newPlayer.CustomProperties == null) return false;  
         var role = newPlayer.CustomProperties["Role"];
         Debug.LogFormat("This player is a {0}", role);
         return role.Equals("Monster"); 
     }
    
        /// <summary>
        /// Spawns the character within the room. A manual spawn method is used to have complete control over the spawn location.
        /// </summary>
        /// <param name="newPlayer">The player that entered the room.</param>
        public void SpawnPlayer(Player newPlayer)
        {
            // Only the master client can spawn new players.
            if (!PhotonNetwork.IsMasterClient) {
                return;
            }

            // Spawn the new player based on the spawn mode.
            var spawnPosition = Vector3.zero;
            var spawnRotation = Quaternion.identity;
            //determines spawn location based on team
            m_SpawnPointGrouping = isMonster(newPlayer) ? 1 : 0;
                if (!SpawnPointManager.GetPlacement(null, m_SpawnPointGrouping, ref spawnPosition, ref spawnRotation)) {
                    Debug.LogWarning("Warning: The Spawn Point Manager is unable to determine a spawn location for grouping " + m_SpawnPointGrouping + ". " +
                                     "Consider adding more spawn points.");
                }
                spawnPosition += m_PlayerCount * m_SpawnLocationOffset;
            

            // Instantiate the player and let the PhotonNetwork know of the new character.
            var player = GameObject.Instantiate(GetCharacterPrefab(newPlayer), spawnPosition, spawnRotation);
            var photonView = player.GetComponent<PhotonView>();
            photonView.ViewID = PhotonNetwork.AllocateViewID(newPlayer.ActorNumber);
            if (photonView.ViewID > 0) {
                // The character has been created. All other clients need to instantiate the character as well.
                var data = new object[]
                {
                    player.transform.position, player.transform.rotation, photonView.ViewID
                };
                m_RaiseEventOptions.TargetActors = null;
                PhotonNetwork.RaiseEvent(PhotonEventIDs.PlayerInstantiation, data, m_RaiseEventOptions, m_ReliableSendOption);

                // The new player should instantiate all existing characters in addition to their character.
                if (newPlayer != PhotonNetwork.LocalPlayer) {
                    // Deactivate the character until the remote machine has the chance to create it. This will prevent the character from
                    // being active on the Master Client without being able to be controlled.
                    player.SetActive(false);

                    data = new object[m_PlayerCount * 3];
                    for (int i = 0; i < m_PlayerCount; ++i) {
                        data[i * 3] = m_Players[i].transform.position;
                        data[i * 3 + 1] = m_Players[i].transform.rotation;
                        data[i * 3 + 2] = m_Players[i].ViewID;
                    }
                    m_RaiseEventOptions.TargetActors = new int[] { newPlayer.ActorNumber };
                    PhotonNetwork.RaiseEvent(PhotonEventIDs.PlayerInstantiation, data, m_RaiseEventOptions, m_ReliableSendOption);
                }

                AddPhotonView(photonView);
                EventHandler.ExecuteEvent("OnPlayerEnteredRoom", photonView.Owner, photonView.gameObject);
            } else {
                Debug.LogError("Failed to allocate a ViewId.");
                Destroy(player);
            }
        }


        /// <summary>
        /// Adds the PhotonView to the player list.
        /// </summary>
        /// <param name="photonView">The PhotonView that should be added.</param>
        private void AddPhotonView(PhotonView photonView)
        {
            if (m_PlayerCount == m_Players.Length) {
                System.Array.Resize(ref m_Players, m_PlayerCount + 1);
            }
            m_Players[m_PlayerCount] = photonView;
            m_PlayerCount++;
        }

        /// <summary>
        /// Called when a remote player entered the room. This Player is already added to the playerlist.
        /// </summary>
        /// <param name="newPlayer">The player that entered the room.</param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            
            if(PhotonNetwork.PlayerList.Length == 2)
            {
                Hashtable hash = new Hashtable();
                hash.Add("Role", "Monster");
                newPlayer.SetCustomProperties(hash);
            }
                
        
            SpawnPlayer(newPlayer);
        }

        /// <summary>
        /// Called when a remote player left the room or became inactive. Check otherPlayer.IsInactive.
        /// </summary>
        /// <param name="otherPlayer">The player that left the room.</param>
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            // Notify others that the player has left the room.
            for (int i = 0; i < m_PlayerCount; ++i) {
                if (m_Players[i].OwnerActorNr == otherPlayer.ActorNumber) {
                    EventHandler.ExecuteEvent("OnPlayerLeftRoom", otherPlayer, m_Players[i].gameObject);
                    GameObject.Destroy(m_Players[i].gameObject);
                    for (int j = i; j < m_PlayerCount - 1; ++j) {
                        m_Players[j] = m_Players[j + 1];
                    }
                    m_PlayerCount--;
                    break;
                }
            }
        }

        /// <summary>
        /// A event from Photon has been sent.
        /// </summary>
        /// <param name="photonEvent">The Photon event.</param>
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == PhotonEventIDs.PlayerInstantiation) {
                // The Master Client has instantiated a character. Create that character on the local client as well.
                var data = (object[])photonEvent.CustomData;
                for (int i = 0; i < data.Length / 3; ++i) {
                    var viewID = (int)data[i * 3 + 2];
                    if (PhotonNetwork.GetPhotonView(viewID) != null) {
                        continue;
                    }
                    var player = PhotonNetwork.CurrentRoom.GetPlayer(photonEvent.Sender);
                    var character = Instantiate(GetCharacterPrefab(player), (Vector3)data[i * 3], (Quaternion)data[i * 3 + 1]);
                    var photonView = character.GetCachedComponent<PhotonView>();
                    photonView.ViewID = viewID;
                    AddPhotonView(photonView);

                    // If the instantiated character is a local player then the Master Client is waiting for it to be created on the client. Notify the Master Client
                    // that the character has been created so it can be activated.
                    if (photonView.IsMine) {
                        m_RaiseEventOptions.TargetActors = new int[] { PhotonNetwork.MasterClient.ActorNumber };
                        PhotonNetwork.RaiseEvent(PhotonEventIDs.RemotePlayerInstantiationComplete, photonView.Owner.ActorNumber, m_RaiseEventOptions, m_ReliableSendOption);
                    } else {
                        // Call start manually before any events are received. This ensures the remote character has been initialized.
                        var characterLocomotion = character.GetCachedComponent<UltimateCharacterController.Character.UltimateCharacterLocomotion>();
                        characterLocomotion.Start();
                    }
                    EventHandler.ExecuteEvent("OnPlayerEnteredRoom", photonView.Owner, photonView.gameObject);
                }
            } else if (photonEvent.Code == PhotonEventIDs.RemotePlayerInstantiationComplete) {
                // The remote player has instantiated the character. It can now be enabled (on the Master Client).
                var ownerActor = (int)photonEvent.CustomData;
                for (int i = 0; i < m_PlayerCount; ++i) {
                    if (m_Players[i].Owner.ActorNumber == ownerActor) {
                        m_Players[i].gameObject.SetActive(true);
                        break;
                    }
                }
            }
        }
    }
}


     


