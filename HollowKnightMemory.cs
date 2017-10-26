using LiveSplit.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightMemory {
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;
		private ProgramPointer gameManager, playmakerFSM;
		private int uiManager, inputHandler, cameraCtrl, gameState, heroController, camTarget, camMode, menuState, uiState;
		private int geoCounter, heroAccepting, actorState, transistionState;

		public HollowKnightMemory() {
			lastHooked = DateTime.MinValue;
			gameManager = new ProgramPointer(this, MemPointer.GameManager) { AutoDeref = true, UpdatedPointer = UpdatedPointer };
			playmakerFSM = new ProgramPointer(this, MemPointer.PlaymakerFSM) { AutoDeref = true, UpdatedPointer = UpdatedPointer };
		}

		private void UpdatedPointer(ProgramPointer pointer) {
			if (pointer == gameManager) {
				int len = gameManager.Read<int>(0x68, 0x2c, 0x1c, 0x8);

				Version version = null;

				//GameManager
				uiManager = 0x84;
				inputHandler = 0x68;
				cameraCtrl = 0x74;
				gameState = 0x98;
				heroController = 0x78;

				//CameraController
				camTarget = 0x28;
				camMode = 0x40;

				//HeroController
				heroAccepting = 0x457;
				actorState = 0x374;
				transistionState = 0x37c;
				geoCounter = 0x1dc;

				if (len != 7) {
					string ver = gameManager.Read(0x6c, 0x2c, 0x1c);
					version = new Version(ver);

					uiManager = 0x88;
					inputHandler = 0x6c;
					cameraCtrl = 0x78;
					gameState = 0x9c;
					heroController = 0x7c;
					camTarget = 0x24;
					camMode = 0x3c;
					geoCounter = version.Build > 0 ? 0x1dc : 0x1d4;

					if (version.Minor == 0 && (version.Build < 3 || version.Revision < 4)) {
						uiState = 0x128;
						menuState = 0x12c;
					} else if (version.Minor == 0) {
						uiState = 0x12c;
						menuState = 0x130;
					} else if (version.Minor == 1) {
						uiState = 0x130;
						menuState = 0x134;
						heroAccepting = 0x45b;
						actorState = 0x378;
						transistionState = 0x380;
					} else {
						uiState = 0x130;
						menuState = 0x134;
						uiManager = 0x8c;
						cameraCtrl = 0x7c;
						gameState = 0xa0;
						heroController = 0x80;
						heroAccepting = 0x46b;
						actorState = 0x388;
						transistionState = 0x390;
						geoCounter = 0x1e4;
					}
				} else {
					string ver = gameManager.Read(0x68, 0x2c, 0x1c);
					version = new Version(ver);

					geoCounter = version.Build > 0 ? 0x1dc : 0x1d4;
					menuState = 0x128;
					uiState = 0x124;
				}

				HollowKnight.PlayerData.InitializeData(version);
			}
		}
		public byte[] GetPlayerData(int length) {
			//GameManger._instance.playerData
			return gameManager.ReadBytes(length, 0x30, 0x0);
		}
		public void SetCameraZoom(float zoom) {
			//GameManger._instance.gameCams.tk2dCam.zoomFactor
			gameManager.Write<float>(zoom, 0x20, 0x40, 0x48);
		}
		public PointF GetCameraTarget() {
			//GameManger._instance.cameraCtrl.camTarget.destination
			float x = gameManager.Read<float>(cameraCtrl, camTarget, 0x24);
			float y = gameManager.Read<float>(cameraCtrl, camTarget, 0x28);
			return new PointF(x, y);
		}
		public TargetMode GetCameraTargetMode() {
			//GameManger._instance.cameraCtrl.camTarget.mode
			return (TargetMode)gameManager.Read<int>(cameraCtrl, camTarget, 0x20);
		}
		public void SetCameraTargetMode(TargetMode mode) {
			//GameManger._instance.cameraCtrl.camTarget.mode
			gameManager.Write((int)mode, cameraCtrl, camTarget, 0x20);
		}
		public CameraMode CameraMode() {
			//GameManager._instance.cameraCtrl.mode
			return (CameraMode)gameManager.Read<int>(cameraCtrl, camMode);
		}
		public void UpdateGeoCounter(bool enable, int geo) {
			//GameManger._instance.heroCtrl.geoCounter.digitChangeTimer
			gameManager.Write(-0.02f, heroController, geoCounter, 0x50);
			//GameManger._instance.heroCtrl.geoCounter.changePerTick
			gameManager.Write(enable ? 0 : 1, heroController, geoCounter, 0x44);
			//GameManger._instance.heroCtrl.geoCounter.addCounter
			gameManager.Write(1, heroController, geoCounter, 0x34);
			//GameManger._instance.heroCtrl.geoCounter.counterCurrent
			gameManager.Write(geo, heroController, geoCounter, 0x2c);
			//GameManger._instance.heroCtrl.geoCounter.addRollerState
			gameManager.Write(2, heroController, geoCounter, 0x3c);
		}
		public void EnableDebug(bool enable) {
			//inputHandler.onScreenDebugInfo.showFPS
			gameManager.Write(enable, inputHandler, 0x2c, 0x7c);
			//inputHandler.onScreenDebugInfo.showInfo
			gameManager.Write(enable, inputHandler, 0x2c, 0x7d);
			//inputHandler.onScreenDebugInfo.showInput
			gameManager.Write(enable, inputHandler, 0x2c, 0x7e);
			//inputHandler.onScreenDebugInfo.showLoadingTime
			gameManager.Write(enable, inputHandler, 0x2c, 0x7f);
			//inputHandler.onScreenDebugInfo.showTFR
			gameManager.Write(enable, inputHandler, 0x2c, 0x80);
		}
		public void SetPlayerData(Offset offset, int value) {
			//GameManger._instance.playerData.(offset)
			gameManager.Write(value, 0x30, HollowKnight.PlayerData.GetOffset(offset));
		}
		public void SetPlayerData(Offset offset, bool value) {
			//GameManger._instance.playerData.(offset)
			gameManager.Write(value, 0x30, HollowKnight.PlayerData.GetOffset(offset));
		}
		public List<EnemyInfo> GetEnemyInfo() {
			List<EnemyInfo> enemies = new List<EnemyInfo>();
			int size = playmakerFSM.Read<int>(0xc);
			IntPtr basePointer = (IntPtr)playmakerFSM.Read<uint>(0x8);
			for (int x = 0; x < size; x++) {
				IntPtr fsmPtr = (IntPtr)Program.Read<uint>(basePointer, 0x10 + x * 4, 0xc);
				if (fsmPtr == IntPtr.Zero) { continue; }
				int fsmLength = Program.Read<int>(fsmPtr, 0x14, 0x8);
				byte fsmChar = Program.Read<byte>(fsmPtr, 0x14, 0xc);
				if (fsmLength != 20 || fsmChar != (byte)'h') { continue; }

				EnemyInfo info = new EnemyInfo();
				info.Pointer = Program.Read<uint>(fsmPtr, 0x28, 0xc);

				int infoSize = Program.Read<int>((IntPtr)info.Pointer, 0xc);
				if (infoSize == 0) { continue; }

				for (int i = 0; i < infoSize; i++) {
					fsmLength = Program.Read<int>((IntPtr)info.Pointer, 0x10 + i * 4, 0x8, 0x8);
					fsmChar = Program.Read<byte>((IntPtr)info.Pointer, 0x10 + i * 4, 0x8, 0xc);
					if (fsmLength != 2 || fsmChar != (byte)'H') { continue; }

					info.HPIndex = i;
					info.HP = Program.Read<int>((IntPtr)info.Pointer, 0x10 + i * 4, 0x14);
				}

				enemies.Add(info);
			}

			return enemies;
		}
		public List<EntityInfo> GetEntityInfo() {
			List<EntityInfo> entities = new List<EntityInfo>();
			int size = playmakerFSM.Read<int>(0xc);
			for (int x = 0; x < size; x++) {
				IntPtr fsmPtr = (IntPtr)playmakerFSM.Read<uint>(0x8, 0x10 + x * 4, 0xc);
				if (fsmPtr == IntPtr.Zero) { continue; }
				string fsm = Program.Read((IntPtr)Program.Read<uint>(fsmPtr, 0x14));

				EntityInfo info = new EntityInfo();
				info.Name = fsm;
				info.Pointer = Program.Read<uint>(fsmPtr, 0x28);

				for (int j = 0x8; j <= 0x30; j += 4) {
					int infoSize = Program.Read<int>((IntPtr)info.Pointer, j, 0xc);
					if (infoSize == 0) { continue; }

					for (int i = 0; i < infoSize; i++) {
						string fsmName = Program.Read((IntPtr)Program.Read<uint>((IntPtr)info.Pointer, j, 0x10 + i * 4, 0x8));
						if (string.IsNullOrEmpty(fsmName)) { continue; }

						switch (j) {
							case 0x8: info.FloatVars.Add(new KeyValuePair<string, float>(fsmName, Program.Read<float>((IntPtr)info.Pointer, j, 0x10 + i * 4, 0x14))); break;
							case 0xc: info.IntVars.Add(new KeyValuePair<string, int>(fsmName, Program.Read<int>((IntPtr)info.Pointer, j, 0x10 + i * 4, 0x14))); break;
							case 0x10: info.BoolVars.Add(new KeyValuePair<string, bool>(fsmName, Program.Read<bool>((IntPtr)info.Pointer, j, 0x10 + i * 4, 0x14))); break;
							case 0x14: info.StringVars.Add(new KeyValuePair<string, string>(fsmName, Program.Read((IntPtr)Program.Read<int>((IntPtr)info.Pointer, j, 0x10 + i * 4, 0x14)))); break;
							case 0x18:
							case 0x1c: info.VectorVars.Add(new KeyValuePair<string, PointF>(fsmName, new PointF(Program.Read<float>((IntPtr)info.Pointer, j, 0x10 + i * 4, 0x14), Program.Read<float>((IntPtr)info.Pointer, j, 0x10 + i * 4, 0x18)))); break;
							default: info.ObjVars.Add(new KeyValuePair<string, int>(fsmName, Program.Read<int>((IntPtr)info.Pointer, j, 0x10 + i * 4, 0x14))); break;
						}
					}
				}

				if (info.Count > 0) {
					entities.Add(info);
				}
			}

			return entities;
		}
		public T PlayerData<T>(Offset offset) where T : struct {
			//GameManger._instance.playerData.(offset)
			return gameManager.Read<T>(0x30, HollowKnight.PlayerData.GetOffset(offset));
		}
		public GameState GameState() {
			//GameManager._instance.gameState
			return (GameState)gameManager.Read<int>(gameState);
		}
		public MainMenuState MenuState() {
			//GameManager._instance.uiManager.menuState
			return (MainMenuState)gameManager.Read<int>(uiManager, menuState);
		}
		public UIState UIState() {
			//GameManager._instance.uiManager.uiState
			int ui = gameManager.Read<int>(uiManager, uiState);
			if (uiState != 0x124 && ui >= 2) {
				ui += 2;
			}
			return (UIState)ui;
		}
		public bool AcceptingInput() {
			//GameManager._instance.InputHandler.acceptingInput
			return gameManager.Read<bool>(inputHandler, 0x58);
		}
		public bool AcceptingInputHero() {
			//GameManager._instance.heroCtrl.acceptingInput
			return gameManager.Read<bool>(heroController, heroAccepting);
		}
		public ActorStates HeroActorState() {
			//GameManager._instance.heroCtrl.actor_state
			return (ActorStates)gameManager.Read<int>(heroController, actorState);
		}
		public HeroTransitionState HeroTransitionState() {
			//GameManager._instance.heroCtrl.transitionState
			return (HeroTransitionState)gameManager.Read<int>(heroController, transistionState);
		}
		public string SceneName() {
			//GameManager._instance.sceneName
			return gameManager.Read(0xc);
		}
		public string NextSceneName() {
			//GameManager._instance.nextSceneName
			return gameManager.Read(0x10);
		}
		public int CharmCount() {
			//GameManager._instance.playerData.charms
			int count = 0;
			for (int i = 0x38a; i <= 0x4a1; i += 7) {
				count += gameManager.Read<bool>(0x30, i) ? 1 : 0;
				if ((i & 1) != 0) {
					i++;
				}
			}
			return count;
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
	public enum Offset : int {
		health,
		maxHealthBase,
		MPCharge,
		MPReserveMax,
		mapZone,
		nailDamage,
		fireballLevel,
		quakeLevel,
		screamLevel,
		hasCyclone,
		hasDashSlash,
		hasUpwardSlash,
		hasDreamNail,
		dreamNailUpgraded,
		hasDash,
		hasWallJump,
		hasSuperDash,
		hasShadowDash,
		hasAcidArmour,
		hasDoubleJump,
		hasLantern,
		hasTramPass,
		hasLoveKey,
		hasKingsBrand,
		ore,
		simpleKeys,
		notchShroomOgres,
		notchFogCanyon,
		lurienDefeated,
		hegemolDefeated,
		monomonDefeated,
		visitedDeepnestSpa,
		zoteRescuedBuzzer,
		zoteRescuedDeepnest,
		mothDeparted,
		salubraNotch1,
		salubraNotch2,
		salubraNotch3,
		salubraNotch4,
		nailSmithUpgrades,
		colosseumBronzeCompleted,
		colosseumSilverCompleted,
		colosseumGoldCompleted,
		openedCrossroads,
		openedRuins2,
		openedFungalWastes,
		openedDeepnest,
		gotCharm_1,
		gotCharm_2,
		gotCharm_3,
		gotCharm_4,
		gotCharm_5,
		gotCharm_6,
		gotCharm_7,
		gotCharm_8,
		gotCharm_9,
		gotCharm_10,
		gotCharm_11,
		gotCharm_12,
		gotCharm_13,
		gotCharm_14,
		gotCharm_15,
		gotCharm_16,
		gotCharm_17,
		gotCharm_18,
		gotCharm_19,
		gotCharm_20,
		gotCharm_21,
		gotCharm_22,
		gotCharm_23,
		gotCharm_24,
		gotCharm_25,
		gotCharm_26,
		gotCharm_27,
		gotCharm_28,
		gotCharm_29,
		gotCharm_30,
		gotCharm_31,
		gotCharm_32,
		gotCharm_33,
		gotCharm_34,
		gotCharm_35,
		gotCharm_36,
		gotCharm_37,
		gotCharm_38,
		gotCharm_39,
		gotCharm_40,
		charmCost_36,
		killsSpitter,
		killedBigFly,
		killedMawlek,
		killedMossKnight,
		killedInfectedKnight,
		killedMegaJellyfish,
		killsMushroomBrawler,
		killedBlackKnight,
		killedMageLord,
		killedFlukeMother,
		killedDungDefender,
		killsMegaBeamMiner,
		killedMimicSpider,
		killedHornet,
		killedTraitorLord,
		killedGhostAladar,
		killedGhostXero,
		killedGhostHu,
		killedGhostMarmu,
		killedGhostNoEyes,
		killedGhostMarkoth,
		killedGhostGalien,
		killedHollowKnight,
		killedFinalBoss,
		killedFalseKnight,
		falseKnightDreamDefeated,
		killedGrimm,
		killedNightmareGrimm,
		killedBindingSeal,
		destroyedNightmareLantern,
		mawlekDefeated,
		collectorDefeated,
		hornetOutskirtsDefeated,
		mageLordDreamDefeated,
		infectedKnightDreamDefeated,
		isInvincible,
		visitedDirtmouth,
		visitedCrossroads,
		visitedGreenpath,
		visitedFungus,
		visitedHive,
		visitedRuins,
		visitedMines,
		visitedRoyalGardens,
		visitedFogCanyon,
		visitedDeepnest,
		visitedRestingGrounds,
		visitedWaterways,
		visitedWhitePalace,
		crossroadsInfected,
		megaMossChargerDefeated,
		defeatedMantisLords,
		defeatedMegaBeamMiner,
		defeatedMegaBeamMiner2,
		gotShadeCharm,
		disablePause,
		mrMushroomState,
		killedWhiteDefender,
		killedGreyPrince,
		hasDreamGate,
		metRelicDealer,
		metRelicDealerShop
	}
	public enum MemVersion {
		None,
		V1006 = 2552320,
		V1026 = 2563072,
		V1031 = 2567680,
		V1032 = 2568704
	}
	public enum MemPointer {
		GameManager,
		PlaymakerFSM
	}
	public class ProgramPointer {
		private static Dictionary<MemVersion, Dictionary<MemPointer, string>> funcPatterns = new Dictionary<MemVersion, Dictionary<MemPointer, string>>() {
			{MemVersion.None, new Dictionary<MemPointer, string>() {
				{MemPointer.GameManager, "558BEC5783EC048B7D088B05????????83EC086A0050E8????????83C41085C07421B8????????893883EC0C57E8????????83C41083EC0C57E8????????83C410EB3D8B05" },
				{MemPointer.PlaymakerFSM, "558BEC5783EC048B7D088B05????????83EC0857503900E8????????83C4108B470C85C074238B470C8BC83909|-33" }
			}},
		};
		private IntPtr pointer;
		public HollowKnightMemory Memory { get; set; }
		public MemPointer Name { get; set; }
		public static MemVersion Version { get; set; }
		public bool AutoDeref { get; set; }
		public Action<ProgramPointer> UpdatedPointer { get; set; }
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
		public byte[] ReadBytes(int count, params int[] offsets) {
			return Memory.Program.Read(Value, count, offsets);
		}
		public string Read(params int[] offsets) {
			if (!Memory.IsHooked) { return string.Empty; }

			bool is64bit = Memory.Program.Is64Bit();
			IntPtr p = IntPtr.Zero;
			if (is64bit) {
				p = (IntPtr)Memory.Program.Read<ulong>(Value, offsets);
			} else {
				p = (IntPtr)Memory.Program.Read<uint>(Value, offsets);
			}
			return Memory.Program.Read(p, is64bit);
		}
		public void Write<T>(T value, params int[] offsets) where T : struct {
			Memory.Program.Write<T>(Value, value, offsets);
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
				if (Version == MemVersion.None) {
					FileInfo info = new FileInfo(Path.Combine(Path.GetDirectoryName(Memory.Program.MainModule.FileName), @"hollow_knight_Data\Managed\Assembly-CSharp.dll"));
					Version = (MemVersion)info.Length;
				}
				pointer = GetVersionedFunctionPointer();
				if (pointer != IntPtr.Zero) {
					bool is64bit = Memory.Program.Is64Bit();
					pointer = (IntPtr)Memory.Program.Read<uint>(pointer);
					if (AutoDeref) {
						if (is64bit) {
							pointer = (IntPtr)Memory.Program.Read<ulong>(pointer);
						} else {
							pointer = (IntPtr)Memory.Program.Read<uint>(pointer);
						}
					}
					UpdatedPointer?.Invoke(this);
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
	public enum ActorStates {
		GROUNDED,
		IDLE,
		RUNNING,
		AIRBORNE,
		WALL_SLIDING,
		HARD_LANDING,
		DASH_LANDING,
		NO_INPUT,
		PREVIOUS
	}
	public enum HeroTransitionState {
		WAITING_TO_TRANSITION,
		EXITING_SCENE,
		WAITING_TO_ENTER_LEVEL,
		ENTERING_SCENE,
		DROPPING_DOWN
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
	public enum TargetMode {
		FOLLOW_HERO,
		LOCK_ZONE,
		BOSS,
		FREE
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
		MENU_HOME,
		MENU_OPTIONS,
		MENU_PROFILES,
		LOADING,
		CUTSCENE,
		PLAYING,
		PAUSED,
		OPTIONS
	}
	public class EnemyInfo {
		public uint Pointer { get; set; }
		public int HP { get; set; }
		public int HPIndex { get; set; }

		public int UpdateHP(HollowKnightMemory mem, int newHP = -1) {
			int hp = HP;
			if (Pointer != 0) {
				if (newHP > 0) {
					HP = newHP;
					mem.Program.Write<int>((IntPtr)Pointer, newHP, 0x10 + HPIndex * 4, 0x14);
				} else {
					HP = mem.Program.Read<int>((IntPtr)Pointer, 0x10 + HPIndex * 4, 0x14);
				}
			}
			return hp;
		}
		public override int GetHashCode() {
			return (int)Pointer;
		}
		public override bool Equals(object obj) {
			return obj != null && (obj is EnemyInfo) && ((EnemyInfo)obj).Pointer == this.Pointer;
		}
	}
	public class EntityInfo {
		public string Name { get; set; }
		public uint Pointer { get; set; }
		public List<KeyValuePair<string, float>> FloatVars { get; set; } = new List<KeyValuePair<string, float>>();
		public List<KeyValuePair<string, int>> IntVars { get; set; } = new List<KeyValuePair<string, int>>();
		public List<KeyValuePair<string, bool>> BoolVars { get; set; } = new List<KeyValuePair<string, bool>>();
		public List<KeyValuePair<string, string>> StringVars { get; set; } = new List<KeyValuePair<string, string>>();
		public List<KeyValuePair<string, PointF>> VectorVars { get; set; } = new List<KeyValuePair<string, PointF>>();
		public List<KeyValuePair<string, int>> ObjVars { get; set; } = new List<KeyValuePair<string, int>>();

		public int Count { get { return FloatVars.Count + IntVars.Count + BoolVars.Count + StringVars.Count + VectorVars.Count + ObjVars.Count; } }
		public override int GetHashCode() {
			return (int)Pointer;
		}
		public bool Same(EntityInfo info) {
			return info.Pointer == this.Pointer;
		}
		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(Name);
			for (int i = 0; i < FloatVars.Count; i++) {
				sb.Append("    ").Append(FloatVars[i].Key).Append(" = ").AppendLine(FloatVars[i].Value.ToString());
			}
			for (int i = 0; i < VectorVars.Count; i++) {
				sb.Append("    ").Append(VectorVars[i].Key).Append(" = ").AppendLine(VectorVars[i].Value.ToString());
			}
			for (int i = 0; i < IntVars.Count; i++) {
				sb.Append("    ").Append(IntVars[i].Key).Append(" = ").AppendLine(IntVars[i].Value.ToString());
			}
			for (int i = 0; i < BoolVars.Count; i++) {
				sb.Append("    ").Append(BoolVars[i].Key).Append(" = ").AppendLine(BoolVars[i].Value.ToString());
			}
			for (int i = 0; i < StringVars.Count; i++) {
				sb.Append("    ").Append(StringVars[i].Key).Append(" = ").AppendLine(StringVars[i].Value.ToString());
			}
			for (int i = 0; i < ObjVars.Count; i++) {
				sb.Append("    ").Append(ObjVars[i].Key).Append(" = [").Append(ObjVars[i].Value.ToString()).AppendLine("]");
			}
			return sb.ToString();
		}
		public override bool Equals(object obj) {
			EntityInfo info = obj as EntityInfo;
			if (info == null || info.Count != this.Count) { return false; }

			if (this.FloatVars.Count != info.FloatVars.Count) { return false; }
			for (int i = 0; i < FloatVars.Count; i++) {
				if (FloatVars[i].Value != info.FloatVars[i].Value) { return false; }
			}

			if (this.VectorVars.Count != info.VectorVars.Count) { return false; }
			for (int i = 0; i < VectorVars.Count; i++) {
				if (VectorVars[i].Value != info.VectorVars[i].Value) { return false; }
			}

			if (this.IntVars.Count != info.IntVars.Count) { return false; }
			for (int i = 0; i < IntVars.Count; i++) {
				if (IntVars[i].Value != info.IntVars[i].Value) { return false; }
			}

			if (this.BoolVars.Count != info.BoolVars.Count) { return false; }
			for (int i = 0; i < BoolVars.Count; i++) {
				if (BoolVars[i].Value != info.BoolVars[i].Value) { return false; }
			}

			if (this.StringVars.Count != info.StringVars.Count) { return false; }
			for (int i = 0; i < StringVars.Count; i++) {
				if (StringVars[i].Value != info.StringVars[i].Value) { return false; }
			}

			if (this.ObjVars.Count != info.ObjVars.Count) { return false; }
			for (int i = 0; i < ObjVars.Count; i++) {
				if (ObjVars[i].Value != info.ObjVars[i].Value) { return false; }
			}
			return true;
		}
	}
	public class PlayerData {
		public static Dictionary<string, PlayerKey> Data = new Dictionary<string, PlayerKey>(StringComparer.OrdinalIgnoreCase);
		public static int DataLength;

		public PlayerData() { }

		public static void InitializeData(Version ver) {
			Assembly asm = Assembly.GetExecutingAssembly();

			Stream file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1211.txt");
			if (ver.Minor == 0 && (ver.Build < 3 || ver.Revision < 2)) {
				file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.Original.txt");
			} else if (ver.Minor == 0) {
				file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1032.txt");
			} else if (ver.Minor == 1) {
				file = asm.GetManifestResourceStream("LiveSplit.HollowKnight.PlayerData.V1114.txt");
			}

			if (file != null) {
				Data.Clear();

				string line = null;
				DataLength = 0;
				using (StreamReader sr = new StreamReader(file)) {
					while ((line = sr.ReadLine()) != null) {
						string[] record = line.Split(':');
						int offset = int.Parse(record[0], NumberStyles.HexNumber);
						Data.Add(record[1], new PlayerKey(offset, record[1], record[2]));
						if (offset > DataLength) { DataLength = offset; }
					}
				}
				DataLength++;
			}
		}

		public static int GetOffset(Offset offset) {
			PlayerKey key = null;
			if (Data.TryGetValue(offset.ToString(), out key)) {
				return key.Index;
			}
			return 0;
		}
		public void UpdateData(HollowKnightMemory mem, Action<string> logWriter) {
			if (ProgramPointer.Version == MemVersion.None) { return; }

			Process program = mem.Program;
			byte[] playerData = mem.GetPlayerData(DataLength);
			foreach (KeyValuePair<string, PlayerKey> pair in Data) {
				PlayerKey key = pair.Value;

				switch (key.Name) {
					case "scenesVisited":
					case "scenesMapped":
					case "scenesEncounteredBench":
					case "scenesGrubRescued":
					case "scenesEncounteredCocoon":
					case "scenesEncounteredDreamPlant":
					case "scenesEncounteredDreamPlantC":
					case "playerStory":
					case "MPCharge":
					case "MPReserve":
					case "equippedCharms": continue;
				}

				object oldValue = key.Value;

				switch (key.Type) {
					case "Vector3": key.Value = new PointF(BitConverter.ToSingle(playerData, key.Index), BitConverter.ToSingle(playerData, key.Index + 4)); break;
					case "Single": key.Value = BitConverter.ToSingle(playerData, key.Index); break;
					case "Int16": key.Value = BitConverter.ToInt16(playerData, key.Index); break;
					case "Int64": key.Value = BitConverter.ToInt64(playerData, key.Index); break;
					case "String": key.Value = program.Read((IntPtr)BitConverter.ToUInt32(playerData, key.Index)); break;
					case "Byte": key.Value = playerData[key.Index]; break;
					case "Boolean": key.Value = playerData[key.Index] == 1; break;
					default: key.Value = BitConverter.ToInt32(playerData, key.Index); break;
				}

				bool changed = oldValue == null;
				if (!changed) {
					switch (key.Type) {
						case "Vector3": changed = (PointF)oldValue != (PointF)key.Value; break;
						case "Single": changed = (float)oldValue != (float)key.Value; break;
						case "Int16": changed = (short)oldValue != (short)key.Value; break;
						case "Int64": changed = (long)oldValue != (long)key.Value; break;
						case "String": changed = (string)oldValue != (string)key.Value; break;
						case "Byte": changed = (byte)oldValue != (byte)key.Value; break;
						case "Boolean": changed = (bool)oldValue != (bool)key.Value; break;
						default: changed = (int)oldValue != (int)key.Value; break;
					}
				}
				if (changed && oldValue != null && logWriter != null) {
					logWriter(key.ToString(oldValue));
				}
			}
		}
	}
	public class PlayerKey {
		public int Index { get; set; }
		public string Name { get; set; }
		public string Type { get; set; }
		public object Value { get; set; }

		public PlayerKey(int index, string name, string type) {
			Index = index;
			Name = name;
			Type = type;
		}

		public override string ToString() {
			return Index.ToString("X") + " " + Name + " " + Type + " = " + (Value ?? "").ToString();
		}
		public string ToString(object oldValue) {
			return Name + ": " + (oldValue ?? "").ToString() + " -> " + (Value ?? "").ToString();
		}
	}
}