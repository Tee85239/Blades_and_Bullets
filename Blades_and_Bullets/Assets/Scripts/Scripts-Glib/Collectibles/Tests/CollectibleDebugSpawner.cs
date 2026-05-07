using Game.Collectibles.Data;
using Game.Collectibles.Spawning;
using UnityEngine;

namespace Game.Collectibles.Tests
{
    public sealed class CollectibleDebugSpawner : MonoBehaviour
    {
        [Header("references")]
        [SerializeField] private CollectibleSpawner collectibleSpawner; // runtime spawner

        [Header("collectible types")]
        [SerializeField] private CollectibleTypeSO pointsCollectible; // 1
        [SerializeField] private CollectibleTypeSO powerCollectible; // 2
        [SerializeField] private CollectibleTypeSO bombCollectible; // 3
        [SerializeField] private CollectibleTypeSO lifeCollectible; // 4

        [Header("spawn settings")]
        [SerializeField] private Vector2 spawnOffset = new Vector2(0f, 4f); // prevents spawning on top of the player
        [Min(0f)][SerializeField] private float randomHorizontalOffset = 4f; // spreads debug spawns left/right
        [SerializeField] private bool logSpawns = true; //debugging

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Spawn(pointsCollectible, "Points");
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Spawn(powerCollectible, "Power");
            }

            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Spawn(bombCollectible, "Bomb");
            }

            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                Spawn(lifeCollectible, "Life");
            }
        }

        private void Spawn(CollectibleTypeSO collectibleType, string label)
        {
            if (collectibleSpawner == null)
            {
                Debug.LogError($"{nameof(CollectibleDebugSpawner)} has no {nameof(CollectibleSpawner)} assigned.", this);
                return;
            }

            if (collectibleType == null)
            {
                Debug.LogWarning($"{nameof(CollectibleDebugSpawner)} cannot spawn {label} because no collectible type asset is assigned.", this);
                return;
            }

            float horizontalOffset = Random.Range(-randomHorizontalOffset, randomHorizontalOffset); // so repeated test spawnes have different locatiobns
            Vector2 spawnPosition = (Vector2)transform.position + spawnOffset + new Vector2(horizontalOffset, 0f); // spawning close for visual ease

            collectibleSpawner.Spawn(collectibleType, spawnPosition); // routes all spawning through the real spawner

            if (logSpawns)
            {
                Debug.Log($"Spawned debug collectible: {label} at {spawnPosition}", this);
            }
        }
    }
}