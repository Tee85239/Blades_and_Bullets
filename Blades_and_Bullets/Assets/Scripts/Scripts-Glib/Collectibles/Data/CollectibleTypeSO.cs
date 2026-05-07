using UnityEngine;

namespace Game.Collectibles.Data
{
    [CreateAssetMenu(fileName = "CollectibleType", menuName = "Game/Collectibles/Collectible Type")]
    public sealed class CollectibleTypeSO : ScriptableObject
    {
        [Header("identity")]
        [SerializeField] private CollectibleKind kind = CollectibleKind.Points;
        [SerializeField] private string displayName = "Points";
        [SerializeField] private Sprite sprite;
        [SerializeField] private Color debugColor = Color.white; // temporary tint used while final sprites arent ready yet

        [Header("reward")]
        [Min(1f)][SerializeField] private int value = 1; // amount awarded to the player when collected such as score, power, bombs, or lives

        [Header("lifetime")]
        [Min(0f)][SerializeField] private float lifetime = 12f;

        [Header("magnet")]
        [Min(0f)][SerializeField] private float magnetRange = 1f;
        [Min(0f)][SerializeField] private float magnetMoveSpeed = 8f;
        [Min(0f)][SerializeField] private float autoCollectMoveSpeed = 12f; // faster movement speed used during top-of-screen auto-collect
        [SerializeField] private bool allowAutoCollect = true; // determines whether this collectible can be pulled in by the global auto-collect rule

        [Header("spawn drift")]
        [Min(0f)][SerializeField] private float initialDriftSpeed = 1.25f; // starting drift speed applied on spawn before magnet logic takes over
        [Range(0f, 180f)][SerializeField] private float randomScatterAngle = 25f; // maximum random angle offset used to vary spawned motion so drops do not overlap perfectly

        public CollectibleKind Kind => kind; // collectible category read-only property to other systems
        public string DisplayName => displayName; // exposes the debug-friendly display name without allowing runtime mutation
        public Sprite Sprite => sprite; // exposes the configured sprite to the runtime collectible renderer
        public int Value => value; // exposes how much resource this collectible gives on pickup
        public float Lifetime => lifetime; // exposes how long the collectible should remain active in the scene
        public float MagnetRange => magnetRange; // exposes the attraction distance used by the magnet system
        public float MagnetMoveSpeed => magnetMoveSpeed; // exposes the movement speed used while the item is magnetized
        public float AutoCollectMoveSpeed => autoCollectMoveSpeed; // exposes the movement speed used during forced auto-collect
        public bool AllowAutoCollect => allowAutoCollect; // exposes whether the item can participate in top-of-screen auto-collect
        public float InitialDriftSpeed => initialDriftSpeed; // exposes the base drift speed that gets assigned when the item spawns
        public float RandomScatterAngle => randomScatterAngle; // exposes the spawn scatter angle used to spread out dropped items
        public Color DebugColor => debugColor; // exposes the debug tint color so the runtime collectible can apply it to its sprite renderer

#if UNITY_EDITOR // compiles this validation method only inside the unity editor where inspector editing happens
        private void OnValidate() // unity calls this when values change in the inspector so we can keep the asset data sane
        {
            if (string.IsNullOrWhiteSpace(displayName)) displayName = kind.ToString(); // ensures the asset always has a readable display name even if the field is cleared
            if (value < 1) value = 1; // prevents invalid zero or negative reward amounts
            if (lifetime < 0f) lifetime = 0f; // clamps lifetime so it cannot become negative
            if (magnetRange < 0f) magnetRange = 0f; // clamps magnet range so distance logic stays valid
            if (magnetMoveSpeed < 0f) magnetMoveSpeed = 0f; // clamps homing speed so movement math never receives a negative speed by mistake
            if (autoCollectMoveSpeed < 0f) autoCollectMoveSpeed = 0f; // clamps auto-collect speed for the same reason
            if (initialDriftSpeed < 0f) initialDriftSpeed = 0f; // clamps drift speed so spawn movement remains valid
            if (randomScatterAngle < 0f) randomScatterAngle = 0f; // clamps the lower bound of the scatter angle in case serialized data is malformed
            if (randomScatterAngle > 180f) randomScatterAngle = 180f; // clamps the upper bound so spawn spread calculations stay predictable
        }
#endif 
    }
}