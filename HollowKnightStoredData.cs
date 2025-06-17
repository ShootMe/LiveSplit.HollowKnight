using System.Collections.Concurrent;

namespace LiveSplit.HollowKnight
{
    public class HollowKnightStoredData {

        /// <summary>
        /// Tracks a value of type T so that each time it updates, the previous value is stored
        /// </summary>
        private class Tracked<T> {
            public T current;
            public T previous;
            public Tracked(T initialValue) {
                previous = current = initialValue;
            }
            public void Update(T val) {
                previous = current;
                current = val;
            }
        }

        private ConcurrentDictionary<Offset, Tracked<int>> pdInts = new ConcurrentDictionary<Offset, Tracked<int>>();
        private ConcurrentDictionary<Offset, Tracked<bool>> pdBools = new ConcurrentDictionary<Offset, Tracked<bool>>();
        private ConcurrentDictionary<Offset, int> pdEntryInts = new ConcurrentDictionary<Offset, int>();
        private ConcurrentDictionary<Offset, bool> pdEntryBools = new ConcurrentDictionary<Offset, bool>();
        private Tracked<bool> hazardDeath = new Tracked<bool>(false);
        private Tracked<bool> recoilFrozen = new Tracked<bool>(false);
        public bool TraitorLordDeadOnEntry { get; private set; } = false;
        public bool DungDefenderAwakeConvoOnEntry { get; private set; } = false;
        public int HealthBeforeFocus { get; private set; } = 0;
        public int MPChargeBeforeFocus { get; private set; } = 0;
        /// <summary>
        /// Returns true if the knight is currently in a transition and has already split there
        /// </summary>
        public bool SplitThisTransition { get; set; } = false;
        public int GladeEssence { get; set; } = 0;
        /// <summary>
        /// Stores the value sceneName had before the latest transition
        /// During a transition, this can be the same as sceneName if nextScene is different
        /// </summary>
        public string PrevScene { get; private set; } = "";

        // Colosseum enemies
        public int killsColShieldStart { get; set; }
        public int killsColRollerStart { get; set; }
        public int killsColMinerStart { get; set; }
        public int killsSpitterStart { get; set; }
        public int killsSuperSpitterStart { get; set; }
        public int killsBuzzerStart { get; set; }
        public int killsBigBuzzerStart { get; set; }
        public int killsBurstingBouncerStart { get; set; }
        public int killsBigFlyStart { get; set; }
        public int killsColWormStart { get; set; }
        public int killsColFlyingSentryStart { get; set; }
        public int killsColMosquitoStart { get; set; }
        public int killsCeilingDropperStart { get; set; }
        public int killsColHopperStart { get; set; }
        public int killsGiantHopperStart { get; set; }
        public int killsGrubMimicStart { get; set; }
        public int killsBlobbleStart { get; set; }
        public int killsOblobbleStart { get; set; }
        public int killsAngryBuzzerStart { get; set; }
        public int killsHeavyMantisStart { get; set; }
        public int killsHeavyMantisFlyerStart { get; set; }
        public int killsMageKnightStart { get; set; }
        public int killsMageStart { get; set; }
        public int killsElectricMageStart { get; set; }
        public int killsLesserMawlekStart { get; set; }
        public int killsMawlekStart { get; set; }
        public int killsLobsterLancerStart { get; set; }

        private HollowKnightMemory mem;

        /// <summary>
        /// Reset the stored data's memory
        /// </summary>
        public void Reset() {
            pdInts.Clear();
            pdBools.Clear();
            TraitorLordDeadOnEntry = false;
            DungDefenderAwakeConvoOnEntry = false;
            HealthBeforeFocus = 0;
            MPChargeBeforeFocus = 0;
            SplitThisTransition = false;
            GladeEssence = 0;
            ResetKills();
        }

        public void ResetKills()
        {
            killsColShieldStart = mem.PlayerData<int>(Offset.killsColShield);
            killsColRollerStart = mem.PlayerData<int>(Offset.killsColRoller);
            killsColMinerStart = mem.PlayerData<int>(Offset.killsColMiner);
            killsSpitterStart = mem.PlayerData<int>(Offset.killsSpitter);
            killsSuperSpitterStart = mem.PlayerData<int>(Offset.killsSuperSpitter);
            killsBuzzerStart = mem.PlayerData<int>(Offset.killsBuzzer);
            killsBigBuzzerStart = mem.PlayerData<int>(Offset.killsBigBuzzer);
            killsBurstingBouncerStart = mem.PlayerData<int>(Offset.killsBurstingBouncer);
            killsBigFlyStart = mem.PlayerData<int>(Offset.killsBigFly);
            killsColWormStart = mem.PlayerData<int>(Offset.killsColWorm);
            killsColFlyingSentryStart = mem.PlayerData<int>(Offset.killsColFlyingSentry);
            killsColMosquitoStart = mem.PlayerData<int>(Offset.killsColMosquito);
            killsCeilingDropperStart = mem.PlayerData<int>(Offset.killsCeilingDropper);
            killsColHopperStart = mem.PlayerData<int>(Offset.killsColHopper);
            killsGiantHopperStart = mem.PlayerData<int>(Offset.killsGiantHopper);
            killsGrubMimicStart = mem.PlayerData<int>(Offset.killsGrubMimic);
            killsBlobbleStart = mem.PlayerData<int>(Offset.killsBlobble);
            killsOblobbleStart = mem.PlayerData<int>(Offset.killsOblobble);
            killsAngryBuzzerStart = mem.PlayerData<int>(Offset.killsAngryBuzzer);
            killsHeavyMantisStart = mem.PlayerData<int>(Offset.killsHeavyMantis);
            killsHeavyMantisFlyerStart = mem.PlayerData<int>(Offset.killsMantisHeavyFlyer);
            killsMageKnightStart = mem.PlayerData<int>(Offset.killsMageKnight);
            killsMageStart = mem.PlayerData<int>(Offset.killsMage);
            killsElectricMageStart = mem.PlayerData<int>(Offset.killsElectricMage);
            killsLesserMawlekStart = mem.PlayerData<int>(Offset.killsLesserMawlek);
            killsMawlekStart = mem.PlayerData<int>(Offset.killsMawlek);
            killsLobsterLancerStart = mem.PlayerData<int>(Offset.killsLobsterLancer);
        }
        private Tracked<int> GetValue(Offset offset) {
            if (!pdInts.ContainsKey(offset)) {
                pdInts[offset] = new Tracked<int>(mem.PlayerData<int>(offset));
            }
            return pdInts[offset];
        }

        private Tracked<bool> GetBoolValue(Offset offset) {
            if (!pdBools.ContainsKey(offset)) {
                pdBools[offset] = new Tracked<bool>(mem.PlayerData<bool>(offset));
            }
            return pdBools[offset];
        }

        /// <summary>
        /// Gets the value the PD int given by offset had, on entry after the last transition
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public int GetValueOnEntry(Offset offset) {
            if (!pdEntryInts.ContainsKey(offset)) {
                pdEntryInts[offset] = mem.PlayerData<int>(offset);
            }
            return pdEntryInts[offset];
        }

        /// <summary>
        /// Gets the value the PD bool given by offset had, on entry after the last transition
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool GetBoolValueOnEntry(Offset offset) {
            if (!pdEntryBools.ContainsKey(offset)) {
                pdEntryBools[offset] = mem.PlayerData<bool>(offset);
            }
            return pdEntryBools[offset];
        }

        /// <summary>
        /// Checks if the PD int given by offset has increased by value (i.e. new - old = value) since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckIncreasedBy(Offset offset, int value) {
            Tracked<int> tracked = GetValue(offset);
            return tracked.current - tracked.previous == value;
        }
        /// <summary>
        /// Checks if the PD int given by offset has increased by 1 since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool CheckIncremented(Offset offset) {
            return CheckIncreasedBy(offset, 1);
        }
        /// <summary>
        /// Checks if the PD int given by offset has changed since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool CheckChanged(Offset offset) {
            Tracked<int> tracked = GetValue(offset);
            return tracked.current != tracked.previous;
        }
        /// <summary>
        /// Checks if the PD int given by offset has increased since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool CheckIncreased(Offset offset) {
            Tracked<int> tracked = GetValue(offset);
            return tracked.current > tracked.previous;
        }
        /// <summary>
        /// Checks if the PD int given by offset has decreased to value since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckDecreasedTo(Offset offset, int value) {
            Tracked<int> tracked = GetValue(offset);
            return tracked.current == value && tracked.previous > value;
        }


        /// <summary>
        /// Checks if the PD bool given by offset has toggled since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool CheckToggled(Offset offset) {
            Tracked<bool> tracked = GetBoolValue(offset);
            return tracked.current != tracked.previous;
        }

        /// <summary>
        /// Checks if the PD bool given by offset has toggled from False to True since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool CheckToggledTrue(Offset offset) {
            Tracked<bool> tracked = GetBoolValue(offset);
            return tracked.current && !tracked.previous;
        }

        /// <summary>
        /// Checks if the PD bool given by offset has toggled from True to False since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool CheckToggledFalse(Offset offset) {
            Tracked<bool> tracked = GetBoolValue(offset);
            return tracked.previous && !tracked.current;
        }

        /// <summary>
        /// Checks if the PD bool given by offset has been True since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool CheckBeenTrue(Offset offset) {
            Tracked<bool> tracked = GetBoolValue(offset);
            return tracked.previous && tracked.current;
        }

        /// <summary>
        /// Checks if the PD int given by offset has increased by value (i.e. new - old = value) since the last transition
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckIncreasedSinceEntryBy(Offset offset, int value) {
            int previous = GetValueOnEntry(offset);
            return mem.PlayerData<int>(offset) - previous == value;
        }

        /// <summary>
        /// Checks if hazardDeath has toggled from False to True since the last update
        /// </summary>
        /// <returns></returns>
        public bool HazardDeathToggledTrue() {
            return hazardDeath.current && !hazardDeath.previous;
        }
        /// <summary>
        /// Checks if recoilFrazen has toggled from False to True since the last update
        /// </summary>
        /// <returns></returns>
        public bool RecoilFrozenToggledTrue() {
            return recoilFrozen.current && !recoilFrozen.previous;
        }

        public HollowKnightStoredData(HollowKnightMemory mem) {
            this.mem = mem;
        }

        public void Update() {
            if (mem.SceneName() == "RestingGrounds_08" && CheckIncremented(Offset.dreamOrbs)) {
                GladeEssence++;
            }
            if (!mem.Focusing()) {
                HealthBeforeFocus = mem.PlayerData<int>(Offset.health);
                MPChargeBeforeFocus = mem.PlayerData<int>(Offset.MPCharge);
            }
            foreach (Offset offset in pdInts.Keys) {
                pdInts[offset].Update(mem.PlayerData<int>(offset));
            }
            foreach (Offset offset in pdBools.Keys) {
                pdBools[offset].Update(mem.PlayerData<bool>(offset));
            }
            hazardDeath.Update(mem.HazardDeath());
            recoilFrozen.Update(mem.RecoilFrozen());
            if (mem.HeroTransitionState() != HeroTransitionState.WAITING_TO_TRANSITION 
                || mem.GameState() is GameState.EXITING_LEVEL or GameState.LOADING
                || mem.SceneName() != mem.NextSceneName()) {
                // In transition
                PrevScene = mem.SceneName();
                TraitorLordDeadOnEntry = mem.PlayerData<bool>(Offset.killedTraitorLord);
                DungDefenderAwakeConvoOnEntry = mem.PlayerData<bool>(Offset.dungDefenderAwakeConvo);
                pdEntryInts.Clear();
                pdEntryBools.Clear();
            } else {
                // Not in transition
                SplitThisTransition = false;
            }
        }
    }
}
