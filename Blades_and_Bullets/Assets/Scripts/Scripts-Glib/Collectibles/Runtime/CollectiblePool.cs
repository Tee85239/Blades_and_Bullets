using System.Collections.Generic;
using UnityEngine;

namespace Game.Collectibles.Runtime
{
    public sealed class CollectiblePool : MonoBehaviour
    {
        [Header("pool setup")]
        [SerializeField] private Collectible collectiblePrefab; // the prefab that will be instantiated whenever the pool needs to create a new collectible object
        [Min(1)][SerializeField] private int startingPoolSize = 16; // how many collectible instances should be pre-created during Awake so the first spawns are allocation-free
        [SerializeField] private Transform pooledObjectsParent; // optional parent transform used to keep spawned pooled objects organized in the hierarchy

        private readonly Queue<Collectible> availableCollectibles = new Queue<Collectible>(); // stores currently inactive collectibles that are ready to be reused

        public int AvailableCount => availableCollectibles.Count; // exposes the current number of inactive pooled collectibles for debugging and tests

        private void Awake()
        {
            if (collectiblePrefab == null)
            {
                Debug.LogError($"{nameof(CollectiblePool)} on '{name}' is missing a collectible prefab reference.", this);
                return;
            }

            Transform parentToUse = pooledObjectsParent != null ? pooledObjectsParent : transform; // chooses the configured parent if one exists, otherwise falls back to the pool object's own transform

            for (int i = 0; i < startingPoolSize; i++) // creates the configured number of pooled collectibles up front so the first gameplay spawns do not instantiate at runtime
            {
                CreateAndStoreNewCollectible(parentToUse); // creates one new collectible instance, initializes it for pooling, and enqueues it as available
            }
        }

        public Collectible GetCollectible()
        {
            if (collectiblePrefab == null)
            {
                Debug.LogError($"{nameof(CollectiblePool)} cannot provide a collectible because no prefab is assigned.", this);
                return null;
            }

            if (availableCollectibles.Count == 0)
            {
                Transform parentToUse = pooledObjectsParent != null ? pooledObjectsParent : transform; // reuses the same parent selection logic when expanding the pool at runtime
                CreateAndStoreNewCollectible(parentToUse); // creates one extra collectible on demand so the request can still be satisfied
            }

            Collectible collectible = availableCollectibles.Dequeue(); // removes the next available collectible from the queue so it can be used by gameplay code
            collectible.gameObject.SetActive(true);
            return collectible;
        }

        public void ReturnCollectibleToPool(Collectible collectible)
        {
            if (collectible == null)
            {
                Debug.LogWarning($"{nameof(CollectiblePool)} received a null collectible to return.", this);
                return;
            }

            collectible.transform.SetParent(pooledObjectsParent != null ? pooledObjectsParent : transform); // reparents the object under the pool container so the hierarchy stays organized after gameplay moves it around
            collectible.gameObject.SetActive(false);
            availableCollectibles.Enqueue(collectible); // pushes the returned collectible back into the queue so it can be reused later
        }

        private void CreateAndStoreNewCollectible(Transform parentToUse)
        {
            Collectible newCollectible = Instantiate(collectiblePrefab, parentToUse); // clones the configured collectible prefab under the chosen pool parent
            newCollectible.name = $"{collectiblePrefab.name}_Pooled_{availableCollectibles.Count}"; // assigns a readable runtime name to make hierarchy debugging easier
            newCollectible.SetPool(this); // injects this pool reference into the collectible so the collectible knows where to return itself later
            newCollectible.gameObject.SetActive(false);
            availableCollectibles.Enqueue(newCollectible); // stores the new inactive instance in the queue so it is available for reuse
        }
    }
}