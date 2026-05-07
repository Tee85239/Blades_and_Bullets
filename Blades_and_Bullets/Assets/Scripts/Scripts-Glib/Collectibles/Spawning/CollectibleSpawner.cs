using Game.Collectibles.Data;
using Game.Collectibles.Player;
using Game.Collectibles.Runtime;
using UnityEngine;

namespace Game.Collectibles.Spawning
{
    public sealed class CollectibleSpawner : MonoBehaviour
    {
        [Header("references")]
        [SerializeField] private CollectiblePool pool;
        [SerializeField] private PlayerResourceInventory playerInventory;
        [SerializeField] private Transform playerTransform; // target transform used for magnet movement

        [Header("spawn behavior")]
        [SerializeField] private float defaultScatterAngle = 25f; // spreads drops so they don’t stack perfectly

        public Collectible Spawn(CollectibleTypeSO type, Vector2 position)
        {
            Vector2 randomDirection = GetRandomScatterDirection(defaultScatterAngle); // generates a random direction for initial drift
            return Spawn(type, position, randomDirection);
        }

        public Collectible Spawn(CollectibleTypeSO type, Vector2 position, Vector2 driftDirection)
        {
            if (pool == null)
            {
                Debug.LogError($"{nameof(CollectibleSpawner)} has no pool assigned.", this);
                return null;
            }

            if (type == null)
            {
                Debug.LogWarning($"{nameof(CollectibleSpawner)} tried to spawn a null collectible type.", this);
                return null;
            }

            Collectible collectible = pool.GetCollectible(); // pulls an inactive object from the pool and activate it

            if (collectible == null)
            {
                return null;
            }

            collectible.transform.position = position; // places the collectible at the spawn location
            collectible.transform.rotation = Quaternion.identity; // resets rotation so pooled objects don’t carry old rotation

            collectible.Init(
                type,
                playerInventory,
                playerTransform,
                driftDirection
            ); // initializes collectible with data + movement context

            return collectible;
        }

        private Vector2 GetRandomScatterDirection(float maxAngle)
        {
            float angle = Random.Range(-maxAngle, maxAngle);
            Quaternion rotation = Quaternion.Euler(0f, 0f, angle); // converts angle into a rotation around the Z axis 2D
            Vector2 direction = rotation * Vector2.down; // rotates downward vector to create a spread cone effect

            return direction.normalized; // ensures direction has magnitude 1 so speed is controlled only by drift speed
        }
    }
}