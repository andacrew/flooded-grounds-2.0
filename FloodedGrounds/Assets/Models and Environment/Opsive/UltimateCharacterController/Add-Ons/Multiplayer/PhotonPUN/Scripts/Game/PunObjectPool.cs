/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

using UnityEngine;
using Opsive.UltimateCharacterController.Game;
using Opsive.UltimateCharacterController.Events;
using Opsive.UltimateCharacterController.Networking.Game;
using Opsive.UltimateCharacterController.Utility;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
using System.Collections.Generic;

namespace Opsive.UltimateCharacterController.AddOns.Multiplayer.PhotonPun.Game
{
    /// <summary>
    /// Provides a way to synchronize pooled objects over the network.
    /// </summary>
    public class PunObjectPool : NetworkObjectPool, IPunPrefabPool, IOnEventCallback
    {
        [Tooltip("An array of objects that can be spawned over the network. Any object that can be spawned on the network must be within this list.")]
        [SerializeField] protected GameObject[] m_NetworkSpawnableObjects;

        private Dictionary<string, GameObject> m_ResourceCache = new Dictionary<string, GameObject>();
        private Dictionary<GameObject, int> m_SpawnableGameObjectIDMap = new Dictionary<GameObject, int>();
        private Dictionary<GameObject, ISpawnDataObject> m_ActiveGameObjects = new Dictionary<GameObject, ISpawnDataObject>();
        private HashSet<GameObject> m_SpawnedGameObjects = new HashSet<GameObject>();

        private RaiseEventOptions m_RaisedEventOptions;
        private SendOptions m_ReliableSendOptions;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        private void Awake()
        {
            m_RaisedEventOptions = new RaiseEventOptions {
                Receivers = ReceiverGroup.Others,
                CachingOption = EventCaching.DoNotCache
            };

            m_ReliableSendOptions = new SendOptions {
                Reliability = true
            };

            for (int i = 0; i < m_NetworkSpawnableObjects.Length; ++i) {
                if (m_NetworkSpawnableObjects[i] == null) {
                    continue;
                }
                if (m_NetworkSpawnableObjects[i].GetCachedComponent<PhotonView>() == null) {
                    Debug.LogError("Error: The object " + m_NetworkSpawnableObjects[i].name + " must have a PhotonView attached.");
                    continue;
                }
                m_SpawnableGameObjectIDMap.Add(m_NetworkSpawnableObjects[i], i);
            }

            EventHandler.RegisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
        }

        /// <summary>
        /// The object has been enabled.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            PhotonNetwork.PrefabPool = this;
            PhotonNetwork.AddCallbackTarget(this);
        }

        /// <summary>
        /// A player has entered the room. Ensure the joining player is in sync with the current game state.
        /// </summary>
        /// <param name="player">The Photon Player that entered the room.</param>
        /// <param name="character">The character that the player controls.</param>
        private void OnPlayerEnteredRoom(Player player, GameObject character)
        {
            if (!PhotonNetwork.IsMasterClient) {
                return;
            }

            // The player should receive all spawned objects.
            foreach (var obj in m_ActiveGameObjects) {
                var spawnedObject = obj.Key;
                var index = m_SpawnableGameObjectIDMap[ObjectPool.OriginalObject(spawnedObject)];

                // The pooled object can optionally provide initialization spawn data.
                var spawnDataObject = obj.Value;
                object[] spawnData = null;
                if (spawnDataObject != null) {
                    spawnData = spawnDataObject.SpawnData();
                }
                var data = new object[]
                {
                    index, spawnedObject.transform.position, spawnedObject.transform.rotation, spawnedObject.GetCachedComponent<PhotonView>().ViewID, spawnData
                };

                PhotonNetwork.RaiseEvent(PhotonEventIDs.ObjectInstantiation, data, m_RaisedEventOptions, m_ReliableSendOptions);
            }
        }

        /// <summary>
        /// Internal method which spawns the object on the network.
        /// </summary>
        /// <param name="original">The prefab of the object to spawn.</param>
        /// <param name="position">The position of the spawned object.</param>
        /// <param name="rotation">The rotation of the spawned object.</param>
        /// <returns>The instantiated object.</returns>
        protected override GameObject SpawnInternal(GameObject original, Vector3 position, Quaternion rotation)
        {
            var obj = ObjectPool.Instantiate(original, position, rotation);
            NetworkSpawn(original, obj);
            return obj;
        }

        /// <summary>
        /// Internal method which spawns the object over the network. This does not instantiate a new object on the local client.
        /// </summary>
        /// <param name="original">The object that the object was instantiated from.</param>
        /// <param name="instanceObject">The object that was instantiated from the original object.</param>
        protected override void NetworkSpawnInternal(GameObject original, GameObject instanceObject)
        {
            int index;
            if (!m_SpawnableGameObjectIDMap.TryGetValue(original, out index)) {
                Debug.LogError("Error: Unable to spawn " + original.name + " on the network. Ensure the object has been added to the PunObjectPool.");
                return;
            }

            m_ActiveGameObjects.Add(instanceObject, instanceObject.GetCachedComponent<ISpawnDataObject>());
            if (!m_SpawnedGameObjects.Contains(instanceObject)) {
                m_SpawnedGameObjects.Add(instanceObject);
            }

            var photonView = instanceObject.GetCachedComponent<PhotonView>();
            if (photonView.ViewID != 0 || PhotonNetwork.AllocateSceneViewID(photonView)) {
                // The pooled object can optionally provide initialization spawn data.
                var spawnDataObject = instanceObject.GetCachedComponent<ISpawnDataObject>();
                object[] spawnData = null;
                if (spawnDataObject != null) {
                    spawnData = spawnDataObject.SpawnData();
                }
                var data = new object[]
                {
                    index, instanceObject.transform.position, instanceObject.transform.rotation, photonView.ViewID, spawnData
                };

                PhotonNetwork.RaiseEvent(PhotonEventIDs.ObjectInstantiation, data, m_RaisedEventOptions, m_ReliableSendOptions);
            }
        }

        /// <summary>
        /// Destroys the object.
        /// </summary>
        /// <param name="obj">The object that should be destroyed.</param>
        public new void Destroy(GameObject obj)
        {
            if (ObjectPool.InstantiatedWithPool(obj)) {
                DestroyInternal(obj);
            } else {
                GameObject.Destroy(obj);
            }
        }

        /// <summary>
        /// Internal method which destroys the object instance on the network.
        /// </summary>
        /// <param name="obj">The object to destroy.</param>
        protected override void DestroyInternal(GameObject obj)
        {
            m_ActiveGameObjects.Remove(obj);
            ObjectPool.Destroy(obj);

            if (PhotonNetwork.IsMasterClient) {
                var photonView = obj.GetCachedComponent<PhotonView>();
                PhotonNetwork.RaiseEvent(PhotonEventIDs.ObjectDestruction, photonView.ViewID, m_RaisedEventOptions, m_ReliableSendOptions);
            }
        }

        /// <summary>
        /// Called to get an instance of a prefab. Must return valid, disabled GameObject with PhotonView. Required by IPunPrefabPool.
        /// </summary>
        /// <param name="prefabId">The id of this prefab.</param>
        /// <param name="position">The position for the instance.</param>
        /// <param name="rotation">The rotation for the instance.</param>
        /// <returns>A disabled instance to use by PUN or null if the prefabId is unknown.</returns>
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            GameObject value;
            if (!m_ResourceCache.TryGetValue(prefabId, out value)) {
                value = (GameObject)Resources.Load(prefabId, typeof(GameObject));
                m_ResourceCache.Add(prefabId, value);
            }
            // Pun requires the instantiated object to be deactivated.
            var obj = ObjectPool.Instantiate(value, position, rotation);
            obj.SetActive(false);
            return obj;
        }

        /// <summary>
        /// A event from Photon has been sent.
        /// </summary>
        /// <param name="photonEvent">The Photon event.</param>
        public void OnEvent(EventData photonEvent)
        {
            if (photonEvent.Code == PhotonEventIDs.ObjectInstantiation) {
                // Instantiate the pooled object.
                var data = (object[])photonEvent.CustomData;
                var original = m_NetworkSpawnableObjects[(int)data[0]];
                var obj = ObjectPool.Instantiate(original, (Vector3)data[1], (Quaternion)data[2]);
                var photonView = obj.GetCachedComponent<PhotonView>();
                photonView.ViewID = (int)data[3];
                photonView.InstantiationData = (object[])data[4];
                // Issue a callback after the object has been spawned to allow the object to be initialized.
                var spawnDataObject = obj.GetCachedComponent<ISpawnDataObject>();
                if (spawnDataObject != null) {
                    spawnDataObject.ObjectSpawned();
                }

                if (!m_ActiveGameObjects.ContainsKey(obj)) {
                    m_ActiveGameObjects.Add(obj, spawnDataObject);
                }
            } else if (photonEvent.Code == PhotonEventIDs.ObjectDestruction) {
                var photonView = PhotonNetwork.GetPhotonView((int)photonEvent.CustomData);
                // The object may have already been pooled by the time it was received over the network.
                if (ObjectPool.InstantiatedWithPool(photonView.gameObject)) {
                    DestroyInternal(photonView.gameObject);
                }
            }
        }

        /// <summary>
        /// Internal method which returns if the specified object was spawned with the network object pool.
        /// </summary>
        /// <param name="obj">The object instance to determine if was spawned with the object pool.</param>
        /// <returns>True if the object was spawned with the network object pool.</returns>
        protected override bool SpawnedWithPoolInternal(GameObject obj)
        {
            return m_SpawnedGameObjects.Contains(obj);
        }

        /// <summary>
        /// The object has been disabled.
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();

            PhotonNetwork.RemoveCallbackTarget(this);
        }

        /// <summary>
        /// The object has been destroyed.
        /// </summary>
        private void OnDestroy()
        {
            EventHandler.UnregisterEvent<Player, GameObject>("OnPlayerEnteredRoom", OnPlayerEnteredRoom);
        }
    }
}