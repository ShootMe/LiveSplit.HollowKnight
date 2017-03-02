using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using LiveSplit.Memory;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightMemory {
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;
		private ProgramPointer gameManager;

		public HollowKnightMemory() {
			lastHooked = DateTime.MinValue;
			gameManager = new ProgramPointer(this, MemPointer.GameManager) { AutoDeref = false };
		}

		public GameState GameState() {
			//GameManager._instance.gameState
			return (GameState)gameManager.Read<int>(0x0, 0x98);
		}
		public MapZone MapZone() {
			//GameManager._instance.playerData.mapZone
			return (MapZone)gameManager.Read<int>(0x0, 0x30, 0xcc);
		}
		public MainMenuState MenuState() {
			//GameManager._instance.uiManager.menuState
			return (MainMenuState)gameManager.Read<int>(0x0, 0x84, 0x128);
		}
		public UIState UIState() {
			//GameManager._instance.uiManager.uiState
			return (UIState)gameManager.Read<int>(0x0, 0x84, 0x124);
		}
		public bool AcceptingInput() {
			//GameManager._instance.InputHandler.acceptingInput
			return gameManager.Read<bool>(0x0, 0x68, 0x58);
		}
		public CameraMode CameraMode() {
			//GameManager._instance.cameraController.mode
			return (CameraMode)gameManager.Read<int>(0x0, 0x74, 0x40);
		}
		public string SceneName() {
			//GameManager._instance.sceneName
			return gameManager.Read(0x0, 0xc);
		}
		public string NextSceneName() {
			//GameManager._instance.nextSceneName
			return gameManager.Read(0x0, 0x10);
		}
		public int Health() {
			//GameManager._instance.playerData.health
			return gameManager.Read<int>(0x0, 0x30, 0x68);
		}
		public int MaxHealth() {
			//GameManager._instance.playerData.maxHealth
			return gameManager.Read<int>(0x0, 0x30, 0x6c);
		}
		public int NailDamage() {
			//GameManager._instance.playerData.nailDamage
			return gameManager.Read<int>(0x0, 0x30, 0x118);
		}
		public int Completion() {
			//GameManager._instance.playerData.completionPercentage
			return (int)gameManager.Read<float>(0x0, 0x30, 0xa80);
		}
		public int Geo() {
			//GameManager._instance.playerData.geo
			return gameManager.Read<int>(0x0, 0x30, 0x9c);
		}
		public int CharmCount() {
			//GameManager._instance.playerData.charms
			int count = 0;
			int index = 1;
			for (int i = 0x38a; i <= 0x4a1; i += 7) {
				count += gameManager.Read<bool>(0x0, 0x30, i) ? 1 : 0;
				bool equiped = gameManager.Read<bool>(0x0, 0x30, i+1);
				if(equiped) {
					Console.WriteLine(index);
				}
				if ((i & 1) != 0) {
					i++;
				}
				index++;
			}
			return count;
		}
		public bool KilledFalseKnight() {
			//GameManager._instance.playerData.killedFlaseKnight
			return gameManager.Read<bool>(0x0, 0x30, 0x561);
		}
		public bool MothwingCloak() {
			//GameManager._instance.playerData.hasDash
			return gameManager.Read<bool>(0x0, 0x30, 0x144);
		}
		public bool ThornsOfAgony() {//911
			//GameManager._instance.playerData.gotCharm_10
			return gameManager.Read<bool>(0x0, 0x30, 0x3d1);
		}
		public bool MantisClaw() {
			//GameManager._instance.playerData.hasWallJump
			return gameManager.Read<bool>(0x0, 0x30, 0x145);
		}
		public bool CrystalHeart() {
			//GameManager._instance.playerData.hasSuperDash
			return gameManager.Read<bool>(0x0, 0x30, 0x146);
		}
		public bool GruzMother() {
			//GameManager._instance.playerData.killedFlukeMother
			return gameManager.Read<bool>(0x0, 0x30, 0x731);
		}
		public bool DreamNail() {
			//GameManager._instance.playerData.hasDreamNail
			return gameManager.Read<bool>(0x0, 0x30, 0x13d);
		}
		public bool WatcherKnight() {
			//GameManager._instance.playerData.killedBlackKnight
			return gameManager.Read<bool>(0x0, 0x30, 0x689);
		}
		public bool Lurien() {
			//GameManager._instance.playerData.lurienDefeated
			return gameManager.Read<bool>(0x0, 0x30, 0x1c0);
		}
		public bool Hegemol() {
			//GameManager._instance.playerData.hegemolDefeated
			return gameManager.Read<bool>(0x0, 0x30, 0x1c1);
		}
		public bool Monomon() {
			//GameManager._instance.playerData.monomonDefeated
			return gameManager.Read<bool>(0x0, 0x30, 0x1c2);
		}
		public bool Uumuu() {
			//GameManager._instance.playerData.killedMegaJellyfish
			return gameManager.Read<bool>(0x0, 0x30, 0x631);
		}

		public bool HookProcess() {
			if ((Program == null || Program.HasExited) && DateTime.Now > lastHooked.AddSeconds(1)) {
				lastHooked = DateTime.Now;
				Process[] processes = Process.GetProcessesByName("Hollow_Knight");
				Program = processes.Length == 0 ? null : processes[0];
				IsHooked = true;
			}

			if (Program == null || Program.HasExited) {
				IsHooked = false;
			}

			return IsHooked;
		}
		public void Dispose() {
			if (Program != null) {
				Program.Dispose();
			}
		}
	}
	public enum MemVersion {
		None,
		V1
	}
	public enum MemPointer {
		GameManager
	}
	public class ProgramPointer {
		private static Dictionary<MemVersion, Dictionary<MemPointer, string>> funcPatterns = new Dictionary<MemVersion, Dictionary<MemPointer, string>>() {
			{MemVersion.V1, new Dictionary<MemPointer, string>() {
				{MemPointer.GameManager, "558BEC5783EC048B7D088B05????????83EC086A0050E8????????83C41085C07421B8????????893883EC0C57E8????????83C41083EC0C57E8????????83C410EB3D8B05" }
			}},
		};
		private IntPtr pointer;
		public HollowKnightMemory Memory { get; set; }
		public MemPointer Name { get; set; }
		public MemVersion Version { get; set; }
		public bool AutoDeref { get; set; }
		private int lastID;
		private DateTime lastTry;
		public ProgramPointer(HollowKnightMemory memory, MemPointer pointer) {
			this.Memory = memory;
			this.Name = pointer;
			this.AutoDeref = true;
			lastID = memory.Program == null ? -1 : memory.Program.Id;
			lastTry = DateTime.MinValue;
		}

		public IntPtr Value {
			get {
				GetPointer();
				return pointer;
			}
		}
		public T Read<T>(params int[] offsets) where T : struct {
			return Memory.Program.Read<T>(Value, offsets);
		}
		public string Read(params int[] offsets) {
			if (!Memory.IsHooked) { return string.Empty; }

			bool is64bit = Memory.Program.Is64Bit();
			IntPtr p = IntPtr.Zero;
			if (is64bit) {
				p = (IntPtr)Memory.Program.Read<long>(Value, offsets);
			} else {
				p = (IntPtr)Memory.Program.Read<int>(Value, offsets);
			}
			return Memory.Program.Read(p, is64bit);
		}
		public void Write(int value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(long value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(double value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(float value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(short value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(byte value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		public void Write(bool value, params int[] offsets) {
			Memory.Program.Write(Value, value, offsets);
		}
		private void GetPointer() {
			if (!Memory.IsHooked) {
				pointer = IntPtr.Zero;
				Version = MemVersion.None;
				return;
			}

			if (Memory.Program.Id != lastID) {
				pointer = IntPtr.Zero;
				Version = MemVersion.None;
				lastID = Memory.Program.Id;
			}
			if (pointer == IntPtr.Zero && DateTime.Now > lastTry.AddSeconds(1)) {
				lastTry = DateTime.Now;
				pointer = GetVersionedFunctionPointer();
				if (pointer != IntPtr.Zero) {
					bool is64bit = Memory.Program.Is64Bit();
					pointer = (IntPtr)Memory.Program.Read<int>(pointer);
					if (AutoDeref) {
						if (is64bit) {
							pointer = (IntPtr)Memory.Program.Read<long>(pointer);
						} else {
							pointer = (IntPtr)Memory.Program.Read<int>(pointer);
						}
					}
				}
			}
		}
		private IntPtr GetVersionedFunctionPointer() {
			foreach (MemVersion version in Enum.GetValues(typeof(MemVersion))) {
				Dictionary<MemPointer, string> patterns = null;
				if (!funcPatterns.TryGetValue(version, out patterns)) { continue; }

				string pattern = null;
				if (!patterns.TryGetValue(Name, out pattern)) { continue; }

				IntPtr ptr = Memory.Program.FindSignatures(pattern)[0];
				if (ptr != IntPtr.Zero) {
					Version = version;
					return ptr;
				}
			}
			Version = MemVersion.None;
			return IntPtr.Zero;
		}
	}
	public enum GameState {
		INACTIVE,
		MAIN_MENU,
		LOADING,
		ENTERING_LEVEL,
		PLAYING,
		PAUSED,
		EXITING_LEVEL,
		CUTSCENE,
		PRIMER
	}
	public enum MapZone {
		NONE,
		TEST_AREA,
		KINGS_PASS,
		CLIFFS,
		TOWN,
		CROSSROADS,
		GREEN_PATH,
		ROYAL_GARDENS,
		FOG_CANYON,
		WASTES,
		DEEPNEST,
		HIVE,
		BONE_FOREST,
		PALACE_GROUNDS,
		MINES,
		RESTING_GROUNDS,
		CITY,
		DREAM_WORLD,
		COLOSSEUM,
		ABYSS,
		ROYAL_QUARTER,
		WHITE_PALACE,
		SHAMAN_TEMPLE,
		WATERWAYS,
		QUEENS_STATION,
		OUTSKIRTS,
		KINGS_STATION,
		MAGE_TOWER,
		TRAM_UPPER,
		TRAM_LOWER,
		FINAL_BOSS,
		SOUL_SOCIETY,
		ACID_LAKE,
		NOEYES_TEMPLE,
		MONOMON_ARCHIVE,
		MANTIS_VILLAGE,
		RUINED_TRAMWAY,
		DISTANT_VILLAGE,
		ABYSS_DEEP,
		ISMAS_GROVE,
		WYRMSKIN,
		LURIENS_TOWER,
		LOVE_TOWER,
		GLADE,
		BLUE_LAKE,
		PEAK,
		JONI_GRAVE,
		OVERGROWN_MOUND,
		CRYSTAL_MOUND,
		BEASTS_DEN
	}
	public enum CameraMode {
		FROZEN,
		FOLLOWING,
		LOCKED,
		PANNING,
		FADEOUT,
		FADEIN,
		PREVIOUS
	}
	public enum MainMenuState {
		LOGO,
		MAIN_MENU,
		OPTIONS_MENU,
		GAMEPAD_MENU,
		KEYBOARD_MENU,
		SAVE_PROFILES,
		AUDIO_MENU,
		VIDEO_MENU,
		EXIT_PROMPT,
		OVERSCAN_MENU,
		GAME_OPTIONS_MENU,
		ACHIEVEMENTS_MENU,
		QUIT_GAME_PROMPT,
		RESOLUTION_PROMPT,
		BRIGHTNESS_MENU,
		PAUSE_MENU,
		PLAY_MODE_MENU,
		EXTRAS_MENU,
		REMAP_GAMEPAD_MENU
	}
	public enum UIState {
		INACTIVE,
		MAIN_MENU_HOME,
		MAIN_MENU_OPTIONS,
		MAIN_MENU_PROFILES,
		LOADING,
		CUTSCENE,
		PLAYING,
		PAUSED,
		OPTIONS
	}
}