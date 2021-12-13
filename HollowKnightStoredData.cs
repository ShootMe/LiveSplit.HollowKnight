using System.Collections.Concurrent;

namespace LiveSplit.HollowKnight
{
    public class HollowKnightStoredData {

        private ConcurrentDictionary<Offset, int> pdInts = new ConcurrentDictionary<Offset, int>();
        public bool TraitorLordDeadOnEntry { get; private set; } = false;
        /// <summary>
        /// Returns true if the knight is currently in a transition and has already split there
        /// </summary>
        public bool SplitThisTransition { get; set; } = false;
        public int GladeEssence { get; set; } = 0;

        private HollowKnightMemory mem;

        /// <summary>
        /// Reset the stored data's memory
        /// </summary>
        public void Reset() {
            pdInts.Clear();
            TraitorLordDeadOnEntry = false;
            SplitThisTransition = false;
            GladeEssence = 0;
        }

        private int GetValue(Offset offset) {
            if (!pdInts.ContainsKey(offset)) {
                pdInts[offset] = mem.PlayerData<int>(offset);
            }
            return pdInts[offset];
        }

        /// <summary>
        /// Checks if the PD int given by offset has increased by value (i.e. new - old = value) since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool CheckIncreasedBy(Offset offset, int value) {
            bool ans = mem.PlayerData<int>(offset) == GetValue(offset) + value;
            return ans;
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
            bool ans = mem.PlayerData<int>(offset) != GetValue(offset);
            return ans;
        }
        /// <summary>
        /// Checks if the PD int given by offset has increased since the last update
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        public bool CheckIncreased(Offset offset) {
            bool ans = mem.PlayerData<int>(offset) > GetValue(offset);
            return ans;
        }

        public HollowKnightStoredData(HollowKnightMemory mem) {
            this.mem = mem;
        }

        public void Update() {
            if (CheckIncremented(Offset.dreamOrbs) && mem.SceneName() == "RestingGrounds_08") {
                GladeEssence++;
            }
            foreach (Offset offset in pdInts.Keys) {
                pdInts[offset] = mem.PlayerData<int>(offset);
            }
            
            if (mem.HeroTransitionState() != HeroTransitionState.WAITING_TO_TRANSITION || mem.GameState() is GameState.EXITING_LEVEL or GameState.LOADING) {
                // In transition
                TraitorLordDeadOnEntry = mem.PlayerData<bool>(Offset.killedTraitorLord);
            } else {
                // Not in transition
                SplitThisTransition = false;
            }
        }
    }
}
