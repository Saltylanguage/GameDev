using UnityEngine;

namespace SaltyGame
{
    public sealed class GameRuntime : MonoBehaviour
    {
        public GameState State { get; private set; } = GameState.Booting;
        public GameClock Clock { get; private set; }
        public InventoryState Inventory { get; private set; }
        public PlayerInputAdapter Input { get; private set; }
        public WorldRuntime World { get; private set; }
        public PlayerController Player { get; private set; }
        public InteractionController Interaction { get; private set; }
        public ActivityController Activities { get; private set; }
        public string TimeMessage { get; private set; }
        public bool HasTimeMessage => timeMessageRemaining > 0f;
        bool initialized;
        float timeMessageRemaining;

        void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            if (initialized)
                return;

            initialized = true;
            Clock = new GameClock();
            Inventory = new InventoryState();
            Input = gameObject.AddComponent<PlayerInputAdapter>();

            World = new GameObject("WorldRuntime").AddComponent<WorldRuntime>();
            World.Build();

            Activities = new ActivityController(Inventory);
            Player = new PlayerController(World.PlayerTransform, Input, Activities);
            Interaction = new InteractionController(World.PlayerTransform, Input, Activities, World.Targets);

            var hud = gameObject.AddComponent<GameHud>();
            hud.Initialize(this);
            var debug = gameObject.AddComponent<RuntimeDebugPanel>();
            debug.Initialize(this);

            State = GameState.Playing;
        }

        void Update()
        {
            if (Input.DebugPressed)
                RuntimeDebugPanel.Toggle();

            if (State == GameState.Paused)
                return;

            if (timeMessageRemaining > 0f)
                timeMessageRemaining -= Time.deltaTime;

            if (Activities.Tick(Time.deltaTime))
            {
                var newDay = Clock.AdvanceActivity();
                if (newDay)
                    World.ResetTargetsForNewDay();

                TimeMessage = newDay ? $"NEW DAY\nDay {Clock.Day} - Morning" : Clock.TimeOfDay.ToString().ToUpperInvariant();
                timeMessageRemaining = 3f;
            }
            Player.Tick(Time.deltaTime);
            Interaction.Tick();
        }
    }
}
