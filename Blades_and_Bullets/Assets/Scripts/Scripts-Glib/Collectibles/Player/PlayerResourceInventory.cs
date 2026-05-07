using System;
using Game.Collectibles.Data;
using UnityEngine;


    public class PlayerResourceInventory : MonoBehaviour
    {
        [Header("starting values")]
        [Min(0)][SerializeField] private int startingScore = 0;
        [Min(0)][SerializeField] private int startingPower = 0;
        [Min(0)][SerializeField] private int startingBombs = 0;
        [Min(0)][SerializeField] private int startingLives = 3;

        [Header("limits")]
        [SerializeField] private int maxBombs = 3;
        [SerializeField] private int maxLives = 6;

        [Header("debug")]
        [SerializeField] private bool logChanges = true; // controls whether resource changes are printed to the console for testing

        private int score;
        private int power;
        private int bombs;
        private int lives;

        public int Score => score;
        public int Power => power;
        public int Bombs => bombs;
        public int Lives => lives;
        
    public static EventHandler<OnSendPlayerDataArgs> OnSendPlayerData;
    public class OnSendPlayerDataArgs : EventArgs
    {
        public int BombsRemaining;
        public int LivesRemaining;

    }

        private void Awake()
        {
            ResetToStartingValues();
        }

        void Start()
        {
            WaveTimeline.onWaveRepeat += OnWaveRepeat;
            OnSendPlayerData?.Invoke(this, new  OnSendPlayerDataArgs{BombsRemaining = bombs, LivesRemaining = lives});
        }

        private void OnWaveRepeat(object sender, EventArgs e)
        {
            bombs++;
            lives++;
            LogState("wave reached");
        }

        private void OnDestroy()
        {
            
        }

        public void ResetToStartingValues() // resets all runtime values back to the configured inspector defaults
        {
            // never negative
            score = Mathf.Max(0, startingScore);
            power = Mathf.Max(0, startingPower);
            bombs = Mathf.Max(0, startingBombs);
            lives = Mathf.Max(0, startingLives);

            LogState("inventory reset to starting values");
        }

        public void ApplyCollectible(CollectibleTypeSO collectibleType) // applies the reward from a collected ScriptableObject-defined pickup
        {
            if (collectibleType == null) // guards against null references so the game does not throw errors if a bad collectible is passed in
            {
                Debug.LogWarning($"{nameof(PlayerResourceInventory)} received a null collectible type.", this);
                return;
            }

            switch (collectibleType.Kind) // branches behavior based on the collectible category defined in the ScriptableObject
            {
                case CollectibleKind.Points:
                    AddPoints(collectibleType.Value);
                    break;

                case CollectibleKind.Power:
                    AddPower(collectibleType.Value);
                    break;

                case CollectibleKind.Bomb:
                    AddBombs(collectibleType.Value);
                    break;

                case CollectibleKind.Life:
                    AddLives(collectibleType.Value);
                    break;

                default: // catches any unsupported or future enum values that are not handled above
                    Debug.LogWarning($"Unsupported collectible kind: {collectibleType.Kind}", this);
                    break;
            }
        }

        public void AddPoints(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            score += amount;
            LogState($"added {amount} points");
        }

        public void AddPower(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            power += amount;
            LogState($"added {amount} power");
        }

        public void AddBombs(int amount)
        {
            if (amount <= 0) return;

            bombs = Mathf.Min(bombs + amount, maxBombs);
            LogState($"added {amount} bomb(s)");
        }

        public void AddLives(int amount)
        {
            if (amount <= 0) return;

            lives = Mathf.Min(lives + amount, maxLives);
            LogState($"added {amount} life/lives");
        }
        public void SubtractBomb()
        {
            if (bombs <= 0)
            {
                return;
            }
            bombs--;
           
            LogState("subtracted 1 bomb");
        }

        public void SubtractLife()
        {
            if (lives <= 0)
            {
                
                return;
            }
            lives--;
            LogState("subtracted 1 life");
        }

        private void LogState(string reason)
        {
            if (!logChanges) // respects the inspector toggle so logging can be disabled without changing code
            {
                return;
            }
            
            OnSendPlayerData?.Invoke(this, new  OnSendPlayerDataArgs{BombsRemaining = bombs, LivesRemaining = lives});
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (startingScore < 0) startingScore = 0;
            if (startingPower < 0) startingPower = 0;
            if (startingBombs < 0) startingBombs = 0;
            if (startingLives < 0) startingLives = 0;
        }
#endif
    }

