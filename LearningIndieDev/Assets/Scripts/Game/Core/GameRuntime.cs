using UnityEngine;

namespace SaltyGame
{
    public sealed class GameRuntime : MonoBehaviour
    {
        public GameState State { get; private set; } = GameState.Booting;
        public GameClock Clock { get; private set; }
        public DailyObjective Objective { get; private set; }
        public StormScenario Storm { get; private set; }
        public CampState Camp { get; private set; }
        public SurvivalState Survival { get; private set; }
        public InventoryState Inventory { get; private set; }
        public PlayerInputAdapter Input { get; private set; }
        public WorldRuntime World { get; private set; }
        public PlayerController Player { get; private set; }
        public InteractionController Interaction { get; private set; }
        public ActivityController Activities { get; private set; }
        public string ActivityMessage { get; private set; }
        public string DaySummary { get; private set; }
        public float MessageTimer { get; private set; }
        bool initialized;

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
            Objective = new DailyObjective();
            Storm = new StormScenario();
            Inventory = new InventoryState();
            Camp = new CampState();
            Survival = new SurvivalState(new SurvivalTuning());
            Input = gameObject.AddComponent<PlayerInputAdapter>();

            var worldObject = new GameObject("WorldRuntime");
            worldObject.transform.SetParent(transform, false);
            World = worldObject.AddComponent<WorldRuntime>();
            World.Build(Camera.main, Inventory, Camp);
            World.SetTimeOfDay(Clock.TimeOfDay);
            RefreshShelterMarker();

            Activities = new ActivityController(Inventory, Survival);
            Player = new PlayerController(World.PlayerTransform, Input, Activities);
            Interaction = new InteractionController(World.PlayerTransform, Input, Activities, Survival, World.Targets);

            var hud = gameObject.AddComponent<GameHud>();
            hud.Initialize(this);
            var debug = gameObject.AddComponent<RuntimeDebugPanel>();
            debug.Initialize(this);
            State = GameState.Playing;
        }

        void Update()
        {
            if (!initialized || Input == null || Activities == null || Player == null || Interaction == null || World == null)
                return;

            if (Input.DebugPressed)
                RuntimeDebugPanel.Toggle();

            if (State == GameState.Paused)
                return;

            MessageTimer = Mathf.Max(0f, MessageTimer - Time.deltaTime);

            if (Activities.Tick(Time.deltaTime))
            {
                DaySummary = null;
                ActivityMessage = Activities.LastResult.Amount > 0
                    ? $"+{Activities.LastResult.Amount} {Activities.LastResult.ResourceId}"
                    : Activities.LastActivityName + " complete";
                MessageTimer = 2f;
                var reachedNight = Clock.AdvanceActivity();
                World.SetTimeOfDay(Clock.TimeOfDay);
                RefreshShelterMarker();
                if (reachedNight)
                    ShowMessage("Night has fallen. Return to the campfire and press [R] to sleep.", 3f);
            }
            Player.Tick(Time.deltaTime);
            Interaction.Tick();
            World.SetInteractionTarget(Activities.IsActive ? null : Interaction.CurrentTarget);
            var interactionMessage = Interaction.ConsumeMessage();
            if (!string.IsNullOrEmpty(interactionMessage))
                ShowMessage(interactionMessage, 2f);

            if (Interaction.ConsumeSleepRequest())
                TrySleepAtCamp();
        }

        public bool TrySleepAtCamp()
        {
            if (!Camp.CampfireBuilt || Activities.IsActive)
                return false;

            var endingDay = Clock.Day;
            var result = Survival.Sleep();
            Clock.BeginNextDay();
            World.SetTimeOfDay(Clock.TimeOfDay);
            RefreshShelterMarker();
            World.ResetTargetsForNewDay();
            var stormMessage = Storm.Resolve(endingDay, Inventory, Camp, Survival);
            DaySummary = $"Day {endingDay} complete - Energy +{result.EnergyRecovered}. {(result.SleptHungry ? "You slept hungry, so recovery was reduced." : "You were fed and rested well.")} {stormMessage}";
            MessageTimer = 5f;
            return true;
        }

        void ShowMessage(string message, float duration)
        {
            DaySummary = null;
            ActivityMessage = message;
            MessageTimer = duration;
        }

        void RefreshShelterMarker()
        {
            World.SetShelterMarkerVisible(Clock.Day == 2 && Clock.TimeOfDay == TimeOfDay.Night);
        }
    }
}
