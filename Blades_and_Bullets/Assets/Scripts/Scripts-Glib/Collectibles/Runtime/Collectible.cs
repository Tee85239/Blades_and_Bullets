using Game.Collectibles.Data;
using Game.Collectibles.Player;
using UnityEngine;

namespace Game.Collectibles.Runtime
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class Collectible : MonoBehaviour
    {
        [Header("cached components")]
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Collider2D triggerCollider; // trigger used for player pickup detection

        [Header("movement")]
        [SerializeField] private float collectDistance = 0.15f;

        private CollectiblePool pool;
        private CollectibleTypeSO collectibleType;
        private PlayerResourceInventory targetInventory;
        private Transform targetTransform; // player transform, used for magnet and auto-collect movement

        private Vector2 driftVelocity; // initial movement right after spawn before the pickup begins homing
        private float age; // tracks alive time for despawning
        private bool isInitialized; // prevents update logic from running on a pooled object before we init it
        private bool isMagnetized; // true when the pickup should move toward the target using normal magnet speed
        private bool isAutoCollecting; // true when the pickup should move toward the target using stronger auto-collect speed
        private bool isCollectedOrDespawning; // prevents duplicate on the same frame

        public CollectibleTypeSO CollectibleType => collectibleType; // exposed for debugging/validation

        public void SetPool(CollectiblePool collectiblePool)
        {
            pool = collectiblePool;
        }

        public void Init(CollectibleTypeSO type, PlayerResourceInventory inventoryTarget, Transform movementTarget, Vector2 initialDriftDirection)
        {
            collectibleType = type;
            targetInventory = inventoryTarget;
            targetTransform = movementTarget;
            age = 0f;
            isMagnetized = false;
            isAutoCollecting = false;
            isCollectedOrDespawning = false;
            isInitialized = collectibleType != null;

            if (!isInitialized)
            {
                Debug.LogWarning($"{nameof(Collectible)} was initialized with a null collectible type.", this);
                gameObject.SetActive(false);
                return;
            }

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = collectibleType.Sprite; // assign the sprite defined by the specific collectible type so one prefab can represent multiple pickup variants
            }

            if (triggerCollider == null)
            {
                triggerCollider = GetComponent<Collider2D>();
            }

            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true; // pickups should use trigger collection, not physical collision response
            }

            Vector2 normalizedDriftDirection = initialDriftDirection.sqrMagnitude > 0f ? initialDriftDirection.normalized : Vector2.down;
            driftVelocity = normalizedDriftDirection * collectibleType.InitialDriftSpeed; // gives the pickup a gentle initial motion after spawning
        }

        public void StartMagnet()
        {
            if (!isInitialized || isCollectedOrDespawning)
            {
                return;
            }

            isMagnetized = true; // normal magnet state,  update logic will move this collectible toward the target
        }

        public void StartAutoCollect()
        {
            if (!isInitialized || isCollectedOrDespawning || collectibleType == null)
            {
                return;
            }

            if (!collectibleType.AllowAutoCollect)
            {
                return;
            }

            isAutoCollecting = true; // stronger than normal magnet mode and usually triggered by the top-of-screen collection rule
            isMagnetized = true; // auto-collect should also count as a homing state in general movement logic
        }

        public void Collect()
        {
            if (!isInitialized || isCollectedOrDespawning)
            {
                return;
            }

            isCollectedOrDespawning = true;

            if (collectibleType != null && targetInventory != null)
            {
                targetInventory.ApplyCollectible(collectibleType);
            }
            else
            {
                Debug.LogWarning($"{nameof(Collectible)} could not apply reward because type or target inventory was missing.", this);
            }

            Despawn();
        }

        public void Despawn()
        {
            if (pool == null)
            {
                Debug.LogWarning($"{nameof(Collectible)} cannot return to pool because no pool reference was assigned.", this);
                gameObject.SetActive(false);
                return;
            }

            ResetRuntimeState(); // important for pooled objects so old state does not leak into the next reuse
            pool.ReturnCollectibleToPool(this);
        }

        private void Update()
        {
            if (!isInitialized || isCollectedOrDespawning || collectibleType == null)
            {
                return;
            }

            age += Time.deltaTime;

            if (collectibleType.Lifetime > 0f && age >= collectibleType.Lifetime)
            {
                Despawn();
                return;
            }

            if (targetTransform == null)
            {
                MoveWithDriftOnly();
                return;
            }

            if (isAutoCollecting)
            {
                MoveTowardTarget(collectibleType.AutoCollectMoveSpeed);
                return;
            }

            if (isMagnetized)
            {
                MoveTowardTarget(collectibleType.MagnetMoveSpeed);
                return;
            }

            MoveWithDriftOnly();
        }

        private void MoveWithDriftOnly()
        {
            transform.position += (Vector3)(driftVelocity * Time.deltaTime); // spawn drift before the player is close enough to magnetize the pickup
        }

        private void MoveTowardTarget(float moveSpeed)
        {
            Vector2 currentPosition = transform.position;
            Vector2 targetPosition = targetTransform.position;
            Vector2 toTarget = targetPosition - currentPosition;
            float distanceToTarget = toTarget.magnitude;

            if (distanceToTarget <= collectDistance)
            {
                Collect(); // avoids tiny jitter near the player by snapping collection once close enough
                return;
            }

            if (distanceToTarget <= Mathf.Epsilon)
            {
                return;
            }

            Vector2 direction = toTarget / distanceToTarget;
            transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isInitialized || isCollectedOrDespawning)
            {
                return;
            }

            if (targetTransform == null)
            {
                return;
            }

            if (other.transform == targetTransform)
            {
                Collect(); // direct collision with the player means immediate pickup, even if magnet mode was never activated
            }
        }

        private void OnDisable()
        {
            ResetRuntimeState(); // pooled objects may be deactivated externally, so cleaning them here too
        }

        private void ResetRuntimeState()
        {
            collectibleType = null;
            targetInventory = null;
            targetTransform = null;
            driftVelocity = Vector2.zero;
            age = 0f;
            isInitialized = false;
            isMagnetized = false;
            isAutoCollecting = false;
            isCollectedOrDespawning = false;

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = null; // clears the previous sprite so pooled objects do not briefly show stale visuals before re-init. NOT CLEARING WHILE TESTING
                spriteRenderer.color = Color.white; // resets pooled renderer tint so previous pickup colors do not leak into the next reuse
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }

            if (triggerCollider == null)
            {
                triggerCollider = GetComponent<Collider2D>();
            }

            if (collectDistance < 0f)
            {
                collectDistance = 0f;
            }
        }
#endif
    }
}