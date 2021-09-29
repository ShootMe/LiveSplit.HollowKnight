using System.Collections.Concurrent;

namespace LiveSplit.HollowKnight
{
    public class HollowKnightStoredData {

        private ConcurrentDictionary<Offset, int> pdInts = new ConcurrentDictionary<Offset, int>();
        public bool TraitorLordDeadOnEntry { get; private set; } = false;

        private HollowKnightMemory mem;

        private int GetValue(Offset offset) {
            if (!pdInts.ContainsKey(offset)) {
                pdInts[offset] = mem.PlayerData<int>(offset);
            }
            return pdInts[offset];
        }

        public bool CheckIncreased(Offset offset, int value = 1) {
            bool ans = mem.PlayerData<int>(offset) == GetValue(offset) + value;
            return ans;
        }
        public bool CheckChanged(Offset offset) {
            bool ans = mem.PlayerData<int>(offset) != GetValue(offset);
            return ans;
        }

        public HollowKnightStoredData(HollowKnightMemory mem) {
            this.mem = mem;
        }

        public void Update() {
            foreach (Offset offset in pdInts.Keys) {
                pdInts[offset] = mem.PlayerData<int>(offset);
            }
            if (mem.HeroTransitionState() != HeroTransitionState.WAITING_TO_TRANSITION) TraitorLordDeadOnEntry = mem.PlayerData<bool>(Offset.killedTraitorLord);
        }
    }
}
