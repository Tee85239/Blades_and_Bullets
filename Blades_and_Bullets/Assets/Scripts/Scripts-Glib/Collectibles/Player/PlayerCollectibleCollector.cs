using Game.Collectibles.Runtime;
using UnityEngine;

namespace Game.Collectibles.Player
{
    public sealed class PlayerCollectibleCollector : MonoBehaviour
    {
        [Header("magnet settings")]
        [Min(0f)][SerializeField] private float magnetRadius = 2.5f;
        [SerializeField] private LayerMask collectibleLayerMask = Physics2D.DefaultRaycastLayers; // lets restict overlap checks to specific layers
        [Min(1)][SerializeField] private int maxDetectedCollectibles = 64; // avoids per-frame array allocations

        [Header("debug")]
        [SerializeField] private bool drawGizmo = true; // draws the magnet radius in the scene view so tuning is easier

        private Collider2D[] overlapResults; // avoids allocating a new array every frame. allocates once in awake and reuses the same array each physics tick

        private void Awake()
        {
            overlapResults = new Collider2D[maxDetectedCollectibles];
        }

        private void FixedUpdate()
        {
            int hitCount = Physics2D.OverlapCircle(
                transform.position,
                magnetRadius,
                new ContactFilter2D
                {
                    useLayerMask = true,
                    layerMask = collectibleLayerMask,
                    useTriggers = true
                },
                overlapResults
            ); // asks the 2D physics system for all trigger colliders within the magnet radius and writes them into the reusable buffer

            for (int i = 0; i < hitCount; i++)
            {
                Collider2D hitCollider = overlapResults[i]; // grabs the current overlap result from the reusable buffer

                if (hitCollider == null)
                {
                    continue;
                }

                Collectible collectible = hitCollider.GetComponent<Collectible>(); // checks whether this overlapped collider belongs to a collectible we can magnetize

                if (collectible == null)
                {
                    continue;
                }

                collectible.StartMagnet(); // collectible starts moving towards player with its logic
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!drawGizmo)
            {
                return;
            }

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, magnetRadius); // for tuning
        }

        private void OnValidate()
        {
            if (magnetRadius < 0f)
            {
                magnetRadius = 0f;
            }

            if (maxDetectedCollectibles < 1)
            {
                maxDetectedCollectibles = 1;
            }
        }
#endif
    }
}