using LiveSplit.Memory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightMemory {
		public Process Program { get; set; }
		public bool IsHooked { get; set; } = false;
		private DateTime lastHooked;
		private ProgramPointer gameManager, playmakerFSM;
		//private Dictionary<string, int> enemyInfo = new Dictionary<string, int>();
		public HollowKnightMemory() {
			lastHooked = DateTime.MinValue;
			gameManager = new ProgramPointer(this, MemPointer.GameManager) { AutoDeref = false };
			playmakerFSM = new ProgramPointer(this, MemPointer.PlaymakerFSM) { AutoDeref = false };
		}

		public byte[] GetPlayerData() {
			return gameManager.ReadBytes(2702, 0x0, 0x30, 0x0);
		}
		//public void ListVariables() {
		//	Dictionary<int, Dictionary<string, object>> enemies = GetEnemyInfo();

		//	foreach (KeyValuePair<int, Dictionary<string, object>> pair in enemies) {
		//		Dictionary<string, object> info = pair.Value;

		//		object value = null;
		//		string name = null;
		//		if (info.TryGetValue("PD Killed Name.str", out value)) {
		//			name = (string)value;
		//		}
		//		if (string.IsNullOrEmpty(name) && info.TryGetValue("PlayerData Name.str", out value)) {
		//			name = (string)value;
		//		}
		//		if (string.IsNullOrEmpty(name) && info.TryGetValue("Send KILLED to.str", out value)) {
		//			name = (string)value;
		//		}
		//		if (info.ContainsKey("HP.int")) {
		//			name = (name ?? "") + "." + pair.Key.ToString();
		//			int hp = (int)info["HP.int"];
		//			int oldHp = 0;
		//			if (!enemyInfo.TryGetValue(name, out oldHp)) {
		//				enemyInfo.Add(name, hp);
		//				Console.WriteLine("Enemy: " + name + " " + hp);
		//			} else if (hp != oldHp) {
		//				enemyInfo[name] = hp;
		//				Console.WriteLine("Enemy: " + name + " " + oldHp + " -> " + hp);
		//			}
		//		}
		//	}
		//}
		//private Dictionary<int, Dictionary<string, object>> GetEnemyInfo() {
		//	Dictionary<int, Dictionary<string, object>> enemies = new Dictionary<int, Dictionary<string, object>>();
		//	int size = playmakerFSM.Read<int>(0x0, 0xc);

		//	for (int x = 0; x < size; x++) {
		//		string fsm = playmakerFSM.Read(0x0, 0x8, 0x10 + x * 4, 0xc, 0x14);
		//		//if (fsm != "health_manager_enemy") { continue; }

		//		Dictionary<string, object> info = new Dictionary<string, object>();
		//		IntPtr ptr = (IntPtr)playmakerFSM.Read<int>(0x0, 0x8, 0x10 + x * 4, 0xc, 0x28);

		//		for (int j = 0x8; j <= 0x30; j += 4) {
		//			int infoSize = Program.Read<int>(ptr, j, 0xc);
		//			if (infoSize == 0) { continue; }

		//			for (int i = 0; i < infoSize; i++) {
		//				string fsmName = Program.Read((IntPtr)Program.Read<int>(ptr, j, 0x10 + i * 4, 0x8));
		//				if (string.IsNullOrEmpty(fsmName)) { continue; }

		//				object value = null;
		//				switch (j) {
		//					case 0x8:
		//						fsmName += ".float";
		//						value = Program.Read<float>(ptr, j, 0x10 + i * 4, 0x14); break;
		//					case 0xc:
		//						fsmName += ".int";
		//						value = Program.Read<int>(ptr, j, 0x10 + i * 4, 0x14); break;
		//					case 0x10:
		//						fsmName += ".bool";
		//						value = Program.Read<bool>(ptr, j, 0x10 + i * 4, 0x14); break;
		//					case 0x14:
		//						fsmName += ".str";
		//						value = Program.Read((IntPtr)Program.Read<int>(ptr, j, 0x10 + i * 4, 0x14)); break;
		//					default:
		//						fsmName += ".obj";
		//						value = Program.Read<int>(ptr, j, 0x10 + i * 4, 0x14); break;
		//				}

		//				if (!info.ContainsKey(fsmName)) {
		//					info.Add(fsmName, value);
		//				}
		//			}
		//		}

		//		if (info.Count > 50) {
		//			enemies.Add(playmakerFSM.Read<int>(0x0, 0x8, 0x10 + x * 4, 0xc), info);
		//		}
		//	}

		//	return enemies;
		//}
		public T PlayerData<T>(Offset offset) where T : struct {
			return gameManager.Read<T>(0x0, 0x30, (int)offset);
		}
		public GameState GameState() {
			//GameManager._instance.gameState
			return (GameState)gameManager.Read<int>(0x0, 0x98);
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
		public int CharmCount() {
			//GameManager._instance.playerData.charms
			int count = 0;
			for (int i = 0x38a; i <= 0x4a1; i += 7) {
				count += gameManager.Read<bool>(0x0, 0x30, i) ? 1 : 0;
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
		health = 0x68,
		maxHealthBase = 0x70,
		MPReserveMax = 0xac,
		mapZone = 0xcc,
		nailDamage = 0x118,
		fireballLevel = 0x12c,
		quakeLevel = 0x130,
		screamLevel = 0x134,
		hasCyclone = 0x139,
		hasDashSlash = 0x13a,
		hasUpwardSlash = 0x13b,
		hasDreamNail = 0x13d,
		dreamNailUpgraded = 0x13e,
		hasDash = 0x144,
		hasWallJump = 0x145,
		hasSuperDash = 0x146,
		hasShadowDash = 0x147,
		hasAcidArmour = 0x148,
		hasDoubleJump = 0x149,
		hasLantern = 0x14a,
		hasTramPass = 0x14b,
		hasKingsBrand = 0x156,
		ore = 0x15c,
		lurienDefeated = 0x1c0,
		hegemolDefeated = 0x1c1,
		monomonDefeated = 0x1c2,
		visitedDeepnestSpa = 0x200,
		mothDeparted = 0x2a9,
		nailSmithUpgrades = 0x2d4,
		colosseumBronzeCompleted = 0x367,
		colosseumSilverCompleted = 0x369,
		colosseumGoldCompleted = 0x36b,
		openedCrossroads = 0x36e,
		openedRuins2 = 0x371,
		openedFungalWastes = 0x372,
		openedDeepnest = 0x375,
		gotCharm_1 = 0x38a,
		gotCharm_2 = 0x391,
		gotCharm_3 = 0x399,
		gotCharm_4 = 0x3a1,
		gotCharm_5 = 0x3a9,
		gotCharm_6 = 0x3b1,
		gotCharm_7 = 0x3b9,
		gotCharm_8 = 0x3c1,
		gotCharm_9 = 0x3c9,
		gotCharm_10 = 0x3d1,
		gotCharm_11 = 0x3d9,
		gotCharm_12 = 0x3e1,
		gotCharm_13 = 0x3e9,
		gotCharm_14 = 0x3f1,
		gotCharm_15 = 0x3f9,
		gotCharm_16 = 0x401,
		gotCharm_17 = 0x409,
		gotCharm_18 = 0x411,
		gotCharm_19 = 0x419,
		gotCharm_20 = 0x421,
		gotCharm_21 = 0x429,
		gotCharm_22 = 0x431,
		gotCharm_23 = 0x439,
		gotCharm_24 = 0x441,
		gotCharm_25 = 0x449,
		gotCharm_26 = 0x451,
		gotCharm_27 = 0x459,
		gotCharm_28 = 0x461,
		gotCharm_29 = 0x469,
		gotCharm_30 = 0x471,
		gotCharm_31 = 0x479,
		gotCharm_32 = 0x481,
		gotCharm_33 = 0x489,
		gotCharm_34 = 0x491,
		gotCharm_35 = 0x499,
		gotCharm_36 = 0x4a1,
		charmCost_36 = 0x4a4,
		killsSpitter = 0x504,
		killedBigFly = 0x551,
		killedMossKnight = 0x5d1,
		killedInfectedKnight = 0x609,
		killedMegaJellyfish = 0x631,
		killsMushroomBrawler = 0x65c,
		killedBlackKnight = 0x689,
		killedMageLord = 0x701,
		killedDungDefender = 0x739,
		killedMimicSpider = 0x7e1,
		killedHornet = 0x851,
		killedTraitorLord = 0x8a1,
		killedGhostAladar = 0x921,
		killedGhostXero = 0x929,
		killedGhostHu = 0x931,
		killedGhostMarmu = 0x939,
		killedGhostNoEyes = 0x941,
		killedGhostMarkoth = 0x949,
		killedGhostGalien = 0x951,
		killedHollowKnight = 0x959,
		killedFinalBoss = 0x961,
		killedFlaseKnight = 0x97e,
		falseKnightDreamDefeated = 0x97f,
		mawlekDefeated = 0x981,
		collectorDefeated = 0x987,
		hornetOutskirtsDefeated = 0x988,
		mageLordDreamDefeated = 0x989,
		infectedKnightDreamDefeated = 0x98b,
		visitedCrossroads = 0x9d1,
		visitedGreenpath = 0x9d2,
		visitedFungus = 0x9d3,
		visitedHive = 0x9d4,
		visitedRuins = 0x9d6,
		visitedMines = 0x9d7,
		visitedRoyalGardens = 0x9d8,
		visitedFogCanyon = 0x9d9,
		visitedDeepnest = 0x9da,
		visitedRestingGrounds = 0x9db,
		visitedWaterways = 0x9dc,
		visitedWhitePalace = 0x9df,
		crossroadsInfected = 0xa1e,
		defeatedMantisLords = 0xa30,
		defeatedMegaBeamMiner = 0xa56,
		defeatedMegaBeamMiner2 = 0xa57,
		gotShadeCharm = 0xa6a
	}
	public enum MemVersion {
		None,
		V1
	}
	public enum MemPointer {
		GameManager,
		PlaymakerFSM
	}
	public class ProgramPointer {
		private static Dictionary<MemVersion, Dictionary<MemPointer, string>> funcPatterns = new Dictionary<MemVersion, Dictionary<MemPointer, string>>() {
			{MemVersion.V1, new Dictionary<MemPointer, string>() {
				{MemPointer.GameManager, "558BEC5783EC048B7D088B05????????83EC086A0050E8????????83C41085C07421B8????????893883EC0C57E8????????83C41083EC0C57E8????????83C410EB3D8B05" },
				{MemPointer.PlaymakerFSM, "558BEC5783EC048B7D088B05????????83EC0857503900E8????????83C4108B470C85C074238B470C8BC83909|-33" }
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
		public byte[] ReadBytes(int count, params int[] offsets) {
			return Memory.Program.Read(Value, count, offsets);
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
	public class PlayerData {
		private Dictionary<int, PlayerKey> data;

		public PlayerData() {
			data = new Dictionary<int, PlayerKey>() { {8, new PlayerKey(8,"version","System.String")},{84, new PlayerKey(84,"profileID","System.Int32")},{88, new PlayerKey(88,"playTime","System.Single")},{92, new PlayerKey(92,"completionPercent","System.Single")},{96, new PlayerKey(96,"openingCreditsPlayed","System.Boolean")},{100, new PlayerKey(100,"permadeathMode","System.Int32")},{104, new PlayerKey(104,"health","System.Int32")},{108, new PlayerKey(108,"maxHealth","System.Int32")},{112, new PlayerKey(112,"maxHealthBase","System.Int32")},{116, new PlayerKey(116,"healthBlue","System.Int32")},{120, new PlayerKey(120,"joniHealthBlue","System.Int32")},{124, new PlayerKey(124,"damagedBlue","System.Boolean")},{128, new PlayerKey(128,"heartPieces","System.Int32")},{132, new PlayerKey(132,"heartPieceCollected","System.Boolean")},{136, new PlayerKey(136,"maxHealthCap","System.Int32")},{140, new PlayerKey(140,"heartPieceMax","System.Boolean")},{144, new PlayerKey(144,"prevHealth","System.Int32")},{148, new PlayerKey(148,"blockerHits","System.Int32")},{152, new PlayerKey(152,"firstGeo","System.Boolean")},{156, new PlayerKey(156,"geo","System.Int32")},{160, new PlayerKey(160,"maxMP","System.Int32")},{164, new PlayerKey(164,"MPCharge","System.Int32")},{168, new PlayerKey(168,"MPReserve","System.Int32")},{172, new PlayerKey(172,"MPReserveMax","System.Int32")},{176, new PlayerKey(176,"soulLimited","System.Boolean")},
					{180, new PlayerKey(180,"vesselFragments","System.Int32")},{184, new PlayerKey(184,"vesselFragmentCollected","System.Boolean")},{188, new PlayerKey(188,"MPReserveCap","System.Int32")},{192, new PlayerKey(192,"vesselFragmentMax","System.Boolean")},{196, new PlayerKey(196,"focusMP_amount","System.Int32")},{200, new PlayerKey(200,"atBench","System.Boolean")},{12, new PlayerKey(12,"respawnScene","System.String")},{204, new PlayerKey(204,"mapZone","GlobalEnums.MapZone")},{16, new PlayerKey(16,"respawnMarkerName","System.String")},{208, new PlayerKey(208,"respawnType","System.Int32")},{212, new PlayerKey(212,"respawnFacingRight","System.Boolean")},{216, new PlayerKey(216,"hazardRespawnLocation","UnityEngine.Vector3")},{228, new PlayerKey(228,"hazardRespawnFacingRight","System.Boolean")},{20, new PlayerKey(20,"shadeScene","System.String")},{24, new PlayerKey(24,"shadeMapZone","System.String")},{232, new PlayerKey(232,"shadePositionX","System.Single")},{236, new PlayerKey(236,"shadePositionY","System.Single")},{240, new PlayerKey(240,"shadeHealth","System.Int32")},{244, new PlayerKey(244,"shadeMP","System.Int32")},{248, new PlayerKey(248,"shadeFireballLevel","System.Int32")},{252, new PlayerKey(252,"shadeQuakeLevel","System.Int32")},{256, new PlayerKey(256,"shadeScreamLevel","System.Int32")},{260, new PlayerKey(260,"shadeSpecialType","System.Int32")},{264, new PlayerKey(264,"shadeMapPos","UnityEngine.Vector3")},{276, new PlayerKey(276,"geoPool","System.Int32")},
					{280, new PlayerKey(280,"nailDamage","System.Int32")},{284, new PlayerKey(284,"nailRange","System.Int32")},{288, new PlayerKey(288,"beamDamage","System.Int32")},{292, new PlayerKey(292,"canDash","System.Boolean")},{293, new PlayerKey(293,"canBackDash","System.Boolean")},{294, new PlayerKey(294,"canWallJump","System.Boolean")},{295, new PlayerKey(295,"canSuperDash","System.Boolean")},{296, new PlayerKey(296,"canShadowDash","System.Boolean")},{297, new PlayerKey(297,"hasSpell","System.Boolean")},{300, new PlayerKey(300,"fireballLevel","System.Int32")},{304, new PlayerKey(304,"quakeLevel","System.Int32")},{308, new PlayerKey(308,"screamLevel","System.Int32")},{312, new PlayerKey(312,"hasNailArt","System.Boolean")},{313, new PlayerKey(313,"hasCyclone","System.Boolean")},{314, new PlayerKey(314,"hasDashSlash","System.Boolean")},{315, new PlayerKey(315,"hasUpwardSlash","System.Boolean")},{316, new PlayerKey(316,"hasAllNailArts","System.Boolean")},{317, new PlayerKey(317,"hasDreamNail","System.Boolean")},{318, new PlayerKey(318,"dreamNailUpgraded","System.Boolean")},{320, new PlayerKey(320,"dreamOrbs","System.Int32")},{324, new PlayerKey(324,"hasDash","System.Boolean")},{325, new PlayerKey(325,"hasWalljump","System.Boolean")},{326, new PlayerKey(326,"hasSuperDash","System.Boolean")},{327, new PlayerKey(327,"hasShadowDash","System.Boolean")},{328, new PlayerKey(328,"hasAcidArmour","System.Boolean")},
					{329, new PlayerKey(329,"hasDoubleJump","System.Boolean")},{330, new PlayerKey(330,"hasLantern","System.Boolean")},{331, new PlayerKey(331,"hasTramPass","System.Boolean")},{332, new PlayerKey(332,"hasQuill","System.Boolean")},{333, new PlayerKey(333,"hasCityKey","System.Boolean")},{334, new PlayerKey(334,"hasSlykey","System.Boolean")},{335, new PlayerKey(335,"gaveSlykey","System.Boolean")},{336, new PlayerKey(336,"hasWhiteKey","System.Boolean")},{337, new PlayerKey(337,"usedWhiteKey","System.Boolean")},{338, new PlayerKey(338,"hasMenderKey","System.Boolean")},{339, new PlayerKey(339,"hasWaterwaysKey","System.Boolean")},{340, new PlayerKey(340,"hasSpaKey","System.Boolean")},{341, new PlayerKey(341,"hasLoveKey","System.Boolean")},{342, new PlayerKey(342,"hasKingsBrand","System.Boolean")},{343, new PlayerKey(343,"hasXunFlower","System.Boolean")},{344, new PlayerKey(344,"ghostCoins","System.Int32")},{348, new PlayerKey(348,"ore","System.Int32")},{352, new PlayerKey(352,"foundGhostCoin","System.Boolean")},{356, new PlayerKey(356,"trinket1","System.Int32")},{360, new PlayerKey(360,"foundTrinket1","System.Boolean")},{364, new PlayerKey(364,"trinket2","System.Int32")},{368, new PlayerKey(368,"foundTrinket2","System.Boolean")},{372, new PlayerKey(372,"trinket3","System.Int32")},{376, new PlayerKey(376,"foundTrinket3","System.Boolean")},{380, new PlayerKey(380,"trinket4","System.Int32")},
					{384, new PlayerKey(384,"foundTrinket4","System.Boolean")},{385, new PlayerKey(385,"noTrinket1","System.Boolean")},{386, new PlayerKey(386,"noTrinket2","System.Boolean")},{387, new PlayerKey(387,"noTrinket3","System.Boolean")},{388, new PlayerKey(388,"noTrinket4","System.Boolean")},{392, new PlayerKey(392,"soldTrinket1","System.Int32")},{396, new PlayerKey(396,"soldTrinket2","System.Int32")},{400, new PlayerKey(400,"soldTrinket3","System.Int32")},{404, new PlayerKey(404,"soldTrinket4","System.Int32")},{408, new PlayerKey(408,"simpleKeys","System.Int32")},{412, new PlayerKey(412,"rancidEggs","System.Int32")},{416, new PlayerKey(416,"notchShroomOgres","System.Boolean")},{417, new PlayerKey(417,"notchFogCanyon","System.Boolean")},{420, new PlayerKey(420,"gMap_doorX","System.Single")},{424, new PlayerKey(424,"gMap_doorY","System.Single")},{28, new PlayerKey(28,"gMap_doorScene","System.String")},{32, new PlayerKey(32,"gMap_doorMapZone","System.String")},{428, new PlayerKey(428,"gMap_doorOriginOffsetX","System.Single")},{432, new PlayerKey(432,"gMap_doorOriginOffsetY","System.Single")},{436, new PlayerKey(436,"gMap_doorSceneWidth","System.Single")},{440, new PlayerKey(440,"gMap_doorSceneHeight","System.Single")},{444, new PlayerKey(444,"guardiansDefeated","System.Int32")},{448, new PlayerKey(448,"lurienDefeated","System.Boolean")},{449, new PlayerKey(449,"hegemolDefeated","System.Boolean")},{450, new PlayerKey(450,"monomonDefeated","System.Boolean")},
					{451, new PlayerKey(451,"maskBrokenLurien","System.Boolean")},{452, new PlayerKey(452,"maskBrokenHegemol","System.Boolean")},{453, new PlayerKey(453,"maskBrokenMonomon","System.Boolean")},{456, new PlayerKey(456,"maskToBreak","System.Int32")},{460, new PlayerKey(460,"elderbug","System.Int32")},{464, new PlayerKey(464,"metElderbug","System.Boolean")},{465, new PlayerKey(465,"elderbugReintro","System.Boolean")},{468, new PlayerKey(468,"elderbugHistory","System.Int32")},{472, new PlayerKey(472,"elderbugHistory1","System.Boolean")},{473, new PlayerKey(473,"elderbugHistory2","System.Boolean")},{474, new PlayerKey(474,"elderbugHistory3","System.Boolean")},{475, new PlayerKey(475,"elderbugSpeechSly","System.Boolean")},{476, new PlayerKey(476,"elderbugSpeechStation","System.Boolean")},{477, new PlayerKey(477,"elderbugSpeechEggTemple","System.Boolean")},{478, new PlayerKey(478,"elderbugSpeechMapShop","System.Boolean")},{479, new PlayerKey(479,"elderbugSpeechBretta","System.Boolean")},{480, new PlayerKey(480,"elderbugSpeechJiji","System.Boolean")},{481, new PlayerKey(481,"elderbugSpeechMinesLift","System.Boolean")},{482, new PlayerKey(482,"elderbugSpeechKingsPass","System.Boolean")},{483, new PlayerKey(483,"elderbugSpeechInfectedCrossroads","System.Boolean")},{484, new PlayerKey(484,"elderbugSpeechFinalBossDoor","System.Boolean")},{485, new PlayerKey(485,"elderbugRequestedFlower","System.Boolean")},{486, new PlayerKey(486,"elderbugGaveFlower","System.Boolean")},{487, new PlayerKey(487,"metQuirrel","System.Boolean")},{488, new PlayerKey(488,"quirrelEggTemple","System.Int32")},
					{492, new PlayerKey(492,"quirrelSlugShrine","System.Int32")},{496, new PlayerKey(496,"quirrelRuins","System.Int32")},{500, new PlayerKey(500,"quirrelMines","System.Int32")},{504, new PlayerKey(504,"quirrelLeftStation","System.Boolean")},{505, new PlayerKey(505,"quirrelLeftEggTemple","System.Boolean")},{506, new PlayerKey(506,"quirrelCityEncountered","System.Boolean")},{507, new PlayerKey(507,"quirrelCityLeft","System.Boolean")},{508, new PlayerKey(508,"quirrelMinesEncountered","System.Boolean")},{509, new PlayerKey(509,"quirrelMinesLeft","System.Boolean")},{510, new PlayerKey(510,"quirrelMantisEncountered","System.Boolean")},{511, new PlayerKey(511,"enteredMantisLordArea","System.Boolean")},{512, new PlayerKey(512,"visitedDeepnestSpa","System.Boolean")},{513, new PlayerKey(513,"quirrelSpaReady","System.Boolean")},{514, new PlayerKey(514,"quirrelSpaEncountered","System.Boolean")},{515, new PlayerKey(515,"quirrelArchiveEncountered","System.Boolean")},{516, new PlayerKey(516,"quirrelEpilogueCompleted","System.Boolean")},{517, new PlayerKey(517,"metRelicDealer","System.Boolean")},{518, new PlayerKey(518,"metRelicDealerShop","System.Boolean")},{519, new PlayerKey(519,"marmOutside","System.Boolean")},{520, new PlayerKey(520,"marmOutsideConvo","System.Boolean")},{521, new PlayerKey(521,"marmConvo1","System.Boolean")},{522, new PlayerKey(522,"marmConvo2","System.Boolean")},{523, new PlayerKey(523,"marmConvo3","System.Boolean")},{524, new PlayerKey(524,"marmConvoNailsmith","System.Boolean")},{528, new PlayerKey(528,"cornifer","System.Int32")},
					{532, new PlayerKey(532,"metCornifer","System.Boolean")},{533, new PlayerKey(533,"corniferIntroduced","System.Boolean")},{534, new PlayerKey(534,"corniferAtHome","System.Boolean")},{535, new PlayerKey(535,"corn_crossroadsEncountered","System.Boolean")},{536, new PlayerKey(536,"corn_crossroadsLeft","System.Boolean")},{537, new PlayerKey(537,"corn_greenpathEncountered","System.Boolean")},{538, new PlayerKey(538,"corn_greenpathLeft","System.Boolean")},{539, new PlayerKey(539,"corn_fogCanyonEncountered","System.Boolean")},{540, new PlayerKey(540,"corn_fogCanyonLeft","System.Boolean")},{541, new PlayerKey(541,"corn_fungalWastesEncountered","System.Boolean")},{542, new PlayerKey(542,"corn_fungalWastesLeft","System.Boolean")},{543, new PlayerKey(543,"corn_cityEncountered","System.Boolean")},{544, new PlayerKey(544,"corn_cityLeft","System.Boolean")},{545, new PlayerKey(545,"corn_waterwaysEncountered","System.Boolean")},{546, new PlayerKey(546,"corn_waterwaysLeft","System.Boolean")},{547, new PlayerKey(547,"corn_minesEncountered","System.Boolean")},{548, new PlayerKey(548,"corn_minesLeft","System.Boolean")},{549, new PlayerKey(549,"corn_cliffsEncountered","System.Boolean")},{550, new PlayerKey(550,"corn_cliffsLeft","System.Boolean")},{551, new PlayerKey(551,"corn_deepnestEncountered","System.Boolean")},{552, new PlayerKey(552,"corn_deepnestLeft","System.Boolean")},{553, new PlayerKey(553,"corn_deepnestMet1","System.Boolean")},{554, new PlayerKey(554,"corn_deepnestMet2","System.Boolean")},{555, new PlayerKey(555,"corn_outskirtsEncountered","System.Boolean")},{556, new PlayerKey(556,"corn_outskirtsLeft","System.Boolean")},
					{557, new PlayerKey(557,"corn_royalGardensEncountered","System.Boolean")},{558, new PlayerKey(558,"corn_royalGardensLeft","System.Boolean")},{559, new PlayerKey(559,"corn_abyssEncountered","System.Boolean")},{560, new PlayerKey(560,"corn_abyssLeft","System.Boolean")},{561, new PlayerKey(561,"metIselda","System.Boolean")},{562, new PlayerKey(562,"iseldaCorniferHomeConvo","System.Boolean")},{563, new PlayerKey(563,"iseldaConvo1","System.Boolean")},{564, new PlayerKey(564,"brettaRescued","System.Boolean")},{568, new PlayerKey(568,"brettaPosition","System.Int32")},{572, new PlayerKey(572,"brettaState","System.Int32")},{576, new PlayerKey(576,"brettaSeenBench","System.Boolean")},{577, new PlayerKey(577,"brettaSeenBed","System.Boolean")},{578, new PlayerKey(578,"brettaSeenBenchDiary","System.Boolean")},{579, new PlayerKey(579,"brettaSeenBedDiary","System.Boolean")},{580, new PlayerKey(580,"slyRescued","System.Boolean")},{581, new PlayerKey(581,"slyBeta","System.Boolean")},{582, new PlayerKey(582,"metSlyShop","System.Boolean")},{583, new PlayerKey(583,"gotSlyCharm","System.Boolean")},{584, new PlayerKey(584,"slyShellFrag1","System.Boolean")},{585, new PlayerKey(585,"slyShellFrag2","System.Boolean")},{586, new PlayerKey(586,"slyShellFrag3","System.Boolean")},{587, new PlayerKey(587,"slyShellFrag4","System.Boolean")},{588, new PlayerKey(588,"slyVesselFrag1","System.Boolean")},{589, new PlayerKey(589,"slyVesselFrag2","System.Boolean")},{590, new PlayerKey(590,"slyVesselFrag3","System.Boolean")},
					{591, new PlayerKey(591,"slyVesselFrag4","System.Boolean")},{592, new PlayerKey(592,"slyNotch1","System.Boolean")},{593, new PlayerKey(593,"slyNotch2","System.Boolean")},{594, new PlayerKey(594,"slySimpleKey","System.Boolean")},{595, new PlayerKey(595,"slyRancidEgg","System.Boolean")},{596, new PlayerKey(596,"slyConvoNailArt","System.Boolean")},{597, new PlayerKey(597,"slyConvoMapper","System.Boolean")},{598, new PlayerKey(598,"slyConvoNailHoned","System.Boolean")},{599, new PlayerKey(599,"jijiDoorUnlocked","System.Boolean")},{600, new PlayerKey(600,"jijiMet","System.Boolean")},{601, new PlayerKey(601,"jijiShadeOffered","System.Boolean")},{602, new PlayerKey(602,"jijiShadeCharmConvo","System.Boolean")},{603, new PlayerKey(603,"metJinn","System.Boolean")},{604, new PlayerKey(604,"jinnConvo1","System.Boolean")},{605, new PlayerKey(605,"jinnConvo2","System.Boolean")},{606, new PlayerKey(606,"jinnConvo3","System.Boolean")},{607, new PlayerKey(607,"jinnConvoKingBrand","System.Boolean")},{608, new PlayerKey(608,"jinnConvoShadeCharm","System.Boolean")},{612, new PlayerKey(612,"jinnEggsSold","System.Int32")},{616, new PlayerKey(616,"zote","System.Int32")},{620, new PlayerKey(620,"zoteRescuedBuzzer","System.Boolean")},{621, new PlayerKey(621,"zoteDead","System.Boolean")},{624, new PlayerKey(624,"zoteDeathPos","System.Int32")},{628, new PlayerKey(628,"zoteSpokenCity","System.Boolean")},{629, new PlayerKey(629,"zoteLeftCity","System.Boolean")},
					{630, new PlayerKey(630,"zoteTrappedDeepnest","System.Boolean")},{631, new PlayerKey(631,"zoteRescuedDeepnest","System.Boolean")},{632, new PlayerKey(632,"zoteDefeated","System.Boolean")},{633, new PlayerKey(633,"zoteSpokenColosseum","System.Boolean")},{636, new PlayerKey(636,"zotePrecept","System.Int32")},{640, new PlayerKey(640,"zoteTownConvo","System.Int32")},{644, new PlayerKey(644,"shaman","System.Int32")},{648, new PlayerKey(648,"shamanScreamConvo","System.Boolean")},{649, new PlayerKey(649,"shamanQuakeConvo","System.Boolean")},{650, new PlayerKey(650,"shamanFireball2Convo","System.Boolean")},{651, new PlayerKey(651,"shamanScream2Convo","System.Boolean")},{652, new PlayerKey(652,"shamanQuake2Convo","System.Boolean")},{653, new PlayerKey(653,"metMiner","System.Boolean")},{656, new PlayerKey(656,"miner","System.Int32")},{660, new PlayerKey(660,"minerEarly","System.Int32")},{664, new PlayerKey(664,"hornetGreenpath","System.Int32")},{668, new PlayerKey(668,"hornetFung","System.Int32")},{672, new PlayerKey(672,"hornet_f19","System.Boolean")},{673, new PlayerKey(673,"hornetFountainEncounter","System.Boolean")},{674, new PlayerKey(674,"hornetCityBridge_ready","System.Boolean")},{675, new PlayerKey(675,"hornetCityBridge_completed","System.Boolean")},{676, new PlayerKey(676,"hornetAbyssEncounter","System.Boolean")},{677, new PlayerKey(677,"hornetDenEncounter","System.Boolean")},{678, new PlayerKey(678,"metMoth","System.Boolean")},{679, new PlayerKey(679,"ignoredMoth","System.Boolean")},
					{680, new PlayerKey(680,"gladeDoorOpened","System.Boolean")},{681, new PlayerKey(681,"mothDeparted","System.Boolean")},{682, new PlayerKey(682,"completedRGDreamPlant","System.Boolean")},{683, new PlayerKey(683,"dreamReward1","System.Boolean")},{684, new PlayerKey(684,"dreamReward2","System.Boolean")},{685, new PlayerKey(685,"dreamReward3","System.Boolean")},{686, new PlayerKey(686,"dreamReward4","System.Boolean")},{687, new PlayerKey(687,"dreamReward5","System.Boolean")},{688, new PlayerKey(688,"dreamReward6","System.Boolean")},{689, new PlayerKey(689,"dreamReward7","System.Boolean")},{690, new PlayerKey(690,"dreamReward8","System.Boolean")},{691, new PlayerKey(691,"dreamReward9","System.Boolean")},{692, new PlayerKey(692,"dreamMothConvo1","System.Boolean")},{693, new PlayerKey(693,"bankerAccountPurchased","System.Boolean")},{694, new PlayerKey(694,"metBanker","System.Boolean")},{696, new PlayerKey(696,"bankerBalance","System.Int32")},{700, new PlayerKey(700,"bankerDeclined","System.Boolean")},{701, new PlayerKey(701,"bankerTheftCheck","System.Boolean")},{704, new PlayerKey(704,"bankerTheft","System.Int32")},{708, new PlayerKey(708,"bankerSpaMet","System.Boolean")},{709, new PlayerKey(709,"metGiraffe","System.Boolean")},{710, new PlayerKey(710,"metCharmSlug","System.Boolean")},{711, new PlayerKey(711,"salubraNotch1","System.Boolean")},{712, new PlayerKey(712,"salubraNotch2","System.Boolean")},{713, new PlayerKey(713,"salubraNotch3","System.Boolean")},
					{714, new PlayerKey(714,"salubraNotch4","System.Boolean")},{715, new PlayerKey(715,"salubraBlessing","System.Boolean")},{716, new PlayerKey(716,"salubraConvoCombo","System.Boolean")},{717, new PlayerKey(717,"salubraConvoOvercharm","System.Boolean")},{718, new PlayerKey(718,"salubraConvoTruth","System.Boolean")},{719, new PlayerKey(719,"cultistTransformed","System.Boolean")},{720, new PlayerKey(720,"metNailsmith","System.Boolean")},{724, new PlayerKey(724,"nailSmithUpgrades","System.Int32")},{728, new PlayerKey(728,"honedNail","System.Boolean")},{729, new PlayerKey(729,"nailsmithCliff","System.Boolean")},{730, new PlayerKey(730,"nailsmithKilled","System.Boolean")},{731, new PlayerKey(731,"nailsmithSpared","System.Boolean")},{732, new PlayerKey(732,"nailsmithKillSpeech","System.Boolean")},{733, new PlayerKey(733,"nailsmithSheo","System.Boolean")},{734, new PlayerKey(734,"nailsmithConvoArt","System.Boolean")},{735, new PlayerKey(735,"metNailmasterMato","System.Boolean")},{736, new PlayerKey(736,"metNailmasterSheo","System.Boolean")},{737, new PlayerKey(737,"metNailmasterOro","System.Boolean")},{738, new PlayerKey(738,"matoConvoSheo","System.Boolean")},{739, new PlayerKey(739,"matoConvoOro","System.Boolean")},{740, new PlayerKey(740,"matoConvoSly","System.Boolean")},{741, new PlayerKey(741,"sheoConvoMato","System.Boolean")},{742, new PlayerKey(742,"sheoConvoOro","System.Boolean")},{743, new PlayerKey(743,"sheoConvoSly","System.Boolean")},{744, new PlayerKey(744,"sheoConvoNailsmith","System.Boolean")},
					{745, new PlayerKey(745,"oroConvoSheo","System.Boolean")},{746, new PlayerKey(746,"oroConvoMato","System.Boolean")},{747, new PlayerKey(747,"oroConvoSly","System.Boolean")},{748, new PlayerKey(748,"hunterRoared","System.Boolean")},{749, new PlayerKey(749,"metHunter","System.Boolean")},{750, new PlayerKey(750,"hunterRewardOffered","System.Boolean")},{751, new PlayerKey(751,"huntersMarkOffered","System.Boolean")},{752, new PlayerKey(752,"hasHuntersMark","System.Boolean")},{753, new PlayerKey(753,"metLegEater","System.Boolean")},{754, new PlayerKey(754,"paidLegEater","System.Boolean")},{755, new PlayerKey(755,"refusedLegEater","System.Boolean")},{756, new PlayerKey(756,"legEaterConvo1","System.Boolean")},{757, new PlayerKey(757,"legEaterConvo2","System.Boolean")},{758, new PlayerKey(758,"legEaterConvo3","System.Boolean")},{759, new PlayerKey(759,"legEaterBrokenConvo","System.Boolean")},{760, new PlayerKey(760,"legEaterDungConvo","System.Boolean")},{761, new PlayerKey(761,"legEaterInfectedCrossroadConvo","System.Boolean")},{762, new PlayerKey(762,"legEaterBoughtConvo","System.Boolean")},{763, new PlayerKey(763,"tukMet","System.Boolean")},{764, new PlayerKey(764,"tukEggPrice","System.Int32")},{768, new PlayerKey(768,"tukDungEgg","System.Boolean")},{769, new PlayerKey(769,"metEmilitia","System.Boolean")},{770, new PlayerKey(770,"emilitiaKingsBrandConvo","System.Boolean")},{771, new PlayerKey(771,"metCloth","System.Boolean")},{772, new PlayerKey(772,"clothEnteredTramRoom","System.Boolean")},
					{773, new PlayerKey(773,"savedCloth","System.Boolean")},{774, new PlayerKey(774,"clothEncounteredQueensGarden","System.Boolean")},{775, new PlayerKey(775,"clothKilled","System.Boolean")},{776, new PlayerKey(776,"clothInTown","System.Boolean")},{777, new PlayerKey(777,"clothLeftTown","System.Boolean")},{778, new PlayerKey(778,"clothGhostSpoken","System.Boolean")},{779, new PlayerKey(779,"bigCatHitTail","System.Boolean")},{780, new PlayerKey(780,"bigCatHitTailConvo","System.Boolean")},{781, new PlayerKey(781,"bigCatMeet","System.Boolean")},{782, new PlayerKey(782,"bigCatTalk1","System.Boolean")},{783, new PlayerKey(783,"bigCatTalk2","System.Boolean")},{784, new PlayerKey(784,"bigCatTalk3","System.Boolean")},{785, new PlayerKey(785,"bigCatKingsBrandConvo","System.Boolean")},{786, new PlayerKey(786,"bigCatShadeConvo","System.Boolean")},{787, new PlayerKey(787,"tisoEncounteredTown","System.Boolean")},{788, new PlayerKey(788,"tisoEncounteredBench","System.Boolean")},{789, new PlayerKey(789,"tisoEncounteredLake","System.Boolean")},{790, new PlayerKey(790,"tisoEncounteredColosseum","System.Boolean")},{791, new PlayerKey(791,"tisoDead","System.Boolean")},{792, new PlayerKey(792,"tisoShieldConvo","System.Boolean")},{796, new PlayerKey(796,"mossCultist","System.Int32")},{800, new PlayerKey(800,"maskmakerMet","System.Boolean")},{801, new PlayerKey(801,"maskmakerConvo1","System.Boolean")},{802, new PlayerKey(802,"maskmakerConvo2","System.Boolean")},{803, new PlayerKey(803,"maskmakerUnmasked1","System.Boolean")},
					{804, new PlayerKey(804,"maskmakerUnmasked2","System.Boolean")},{805, new PlayerKey(805,"maskmakerShadowDash","System.Boolean")},{806, new PlayerKey(806,"maskmakerKingsBrand","System.Boolean")},{807, new PlayerKey(807,"dungDefenderConvo1","System.Boolean")},{808, new PlayerKey(808,"dungDefenderConvo2","System.Boolean")},{809, new PlayerKey(809,"dungDefenderConvo3","System.Boolean")},{810, new PlayerKey(810,"dungDefenderCharmConvo","System.Boolean")},{811, new PlayerKey(811,"dungDefenderIsmaConvo","System.Boolean")},{812, new PlayerKey(812,"midwifeMet","System.Boolean")},{813, new PlayerKey(813,"midwifeConvo1","System.Boolean")},{814, new PlayerKey(814,"midwifeConvo2","System.Boolean")},{815, new PlayerKey(815,"metQueen","System.Boolean")},{816, new PlayerKey(816,"queenTalk1","System.Boolean")},{817, new PlayerKey(817,"queenTalk2","System.Boolean")},{818, new PlayerKey(818,"queenDung1","System.Boolean")},{819, new PlayerKey(819,"queenDung2","System.Boolean")},{820, new PlayerKey(820,"queenHornet","System.Boolean")},{821, new PlayerKey(821,"queenTalkExtra","System.Boolean")},{822, new PlayerKey(822,"gotQueenFragment","System.Boolean")},{823, new PlayerKey(823,"gotKingFragment","System.Boolean")},{824, new PlayerKey(824,"metXun","System.Boolean")},{825, new PlayerKey(825,"xunFailedConvo1","System.Boolean")},{826, new PlayerKey(826,"xunFailedConvo2","System.Boolean")},{827, new PlayerKey(827,"xunFlowerBroken","System.Boolean")},{828, new PlayerKey(828,"xunFlowerBrokeTimes","System.Int32")},
					{832, new PlayerKey(832,"xunFlowerGiven","System.Boolean")},{833, new PlayerKey(833,"xunRewardGiven","System.Boolean")},{836, new PlayerKey(836,"menderState","System.Int32")},{840, new PlayerKey(840,"menderSignBroken","System.Boolean")},{841, new PlayerKey(841,"allBelieverTabletsDestroyed","System.Boolean")},{844, new PlayerKey(844,"mrMushroomState","System.Int32")},{848, new PlayerKey(848,"openedMapperShop","System.Boolean")},{849, new PlayerKey(849,"openedSlyShop","System.Boolean")},{850, new PlayerKey(850,"metStag","System.Boolean")},{851, new PlayerKey(851,"travelling","System.Boolean")},{852, new PlayerKey(852,"stagPosition","System.Int32")},{856, new PlayerKey(856,"stationsOpened","System.Int32")},{860, new PlayerKey(860,"stagConvoTram","System.Boolean")},{861, new PlayerKey(861,"stagConvoTiso","System.Boolean")},{862, new PlayerKey(862,"stagRemember1","System.Boolean")},{863, new PlayerKey(863,"stagRemember2","System.Boolean")},{864, new PlayerKey(864,"stagRemember3","System.Boolean")},{865, new PlayerKey(865,"stagEggInspected","System.Boolean")},{866, new PlayerKey(866,"stagHopeConvo","System.Boolean")},{36, new PlayerKey(36,"nextScene","System.String")},{867, new PlayerKey(867,"littleFoolMet","System.Boolean")},{868, new PlayerKey(868,"ranAway","System.Boolean")},{869, new PlayerKey(869,"seenColosseumTitle","System.Boolean")},{870, new PlayerKey(870,"colosseumBronzeOpened","System.Boolean")},{871, new PlayerKey(871,"colosseumBronzeCompleted","System.Boolean")},
					{872, new PlayerKey(872,"colosseumSilverOpened","System.Boolean")},{873, new PlayerKey(873,"colosseumSilverCompleted","System.Boolean")},{874, new PlayerKey(874,"colosseumGoldOpened","System.Boolean")},{875, new PlayerKey(875,"colosseumGoldCompleted","System.Boolean")},{876, new PlayerKey(876,"openedTown","System.Boolean")},{877, new PlayerKey(877,"openedTownBuilding","System.Boolean")},{878, new PlayerKey(878,"openedCrossroads","System.Boolean")},{879, new PlayerKey(879,"openedGreenpath","System.Boolean")},{880, new PlayerKey(880,"openedRuins1","System.Boolean")},{881, new PlayerKey(881,"openedRuins2","System.Boolean")},{882, new PlayerKey(882,"openedFungalWastes","System.Boolean")},{883, new PlayerKey(883,"openedRoyalGardens","System.Boolean")},{884, new PlayerKey(884,"openedRestingGrounds","System.Boolean")},{885, new PlayerKey(885,"openedDeepnest","System.Boolean")},{886, new PlayerKey(886,"openedStagNest","System.Boolean")},{40, new PlayerKey(40,"dreamReturnScene","System.String")},{888, new PlayerKey(888,"charmSlots","System.Int32")},{892, new PlayerKey(892,"charmSlotsFilled","System.Int32")},{896, new PlayerKey(896,"hasCharm","System.Boolean")},{44, new PlayerKey(44,"equippedCharms","System.Collections.Generic.List<System.Int32>")},{897, new PlayerKey(897,"charmBenchMsg","System.Boolean")},{900, new PlayerKey(900,"charmsOwned","System.Int32")},{904, new PlayerKey(904,"canOvercharm","System.Boolean")},{905, new PlayerKey(905,"overcharmed","System.Boolean")},{906, new PlayerKey(906,"gotCharm_1","System.Boolean")},
					{907, new PlayerKey(907,"equippedCharm_1","System.Boolean")},{908, new PlayerKey(908,"charmCost_1","System.Int32")},{912, new PlayerKey(912,"newCharm_1","System.Boolean")},{913, new PlayerKey(913,"gotCharm_2","System.Boolean")},{914, new PlayerKey(914,"equippedCharm_2","System.Boolean")},{916, new PlayerKey(916,"charmCost_2","System.Int32")},{920, new PlayerKey(920,"newCharm_2","System.Boolean")},{921, new PlayerKey(921,"gotCharm_3","System.Boolean")},{922, new PlayerKey(922,"equippedCharm_3","System.Boolean")},{924, new PlayerKey(924,"charmCost_3","System.Int32")},{928, new PlayerKey(928,"newCharm_3","System.Boolean")},{929, new PlayerKey(929,"gotCharm_4","System.Boolean")},{930, new PlayerKey(930,"equippedCharm_4","System.Boolean")},{932, new PlayerKey(932,"charmCost_4","System.Int32")},{936, new PlayerKey(936,"newCharm_4","System.Boolean")},{937, new PlayerKey(937,"gotCharm_5","System.Boolean")},{938, new PlayerKey(938,"equippedCharm_5","System.Boolean")},{940, new PlayerKey(940,"charmCost_5","System.Int32")},{944, new PlayerKey(944,"newCharm_5","System.Boolean")},{945, new PlayerKey(945,"gotCharm_6","System.Boolean")},{946, new PlayerKey(946,"equippedCharm_6","System.Boolean")},{948, new PlayerKey(948,"charmCost_6","System.Int32")},{952, new PlayerKey(952,"newCharm_6","System.Boolean")},{953, new PlayerKey(953,"gotCharm_7","System.Boolean")},{954, new PlayerKey(954,"equippedCharm_7","System.Boolean")},
					{956, new PlayerKey(956,"charmCost_7","System.Int32")},{960, new PlayerKey(960,"newCharm_7","System.Boolean")},{961, new PlayerKey(961,"gotCharm_8","System.Boolean")},{962, new PlayerKey(962,"equippedCharm_8","System.Boolean")},{964, new PlayerKey(964,"charmCost_8","System.Int32")},{968, new PlayerKey(968,"newCharm_8","System.Boolean")},{969, new PlayerKey(969,"gotCharm_9","System.Boolean")},{970, new PlayerKey(970,"equippedCharm_9","System.Boolean")},{972, new PlayerKey(972,"charmCost_9","System.Int32")},{976, new PlayerKey(976,"newCharm_9","System.Boolean")},{977, new PlayerKey(977,"gotCharm_10","System.Boolean")},{978, new PlayerKey(978,"equippedCharm_10","System.Boolean")},{980, new PlayerKey(980,"charmCost_10","System.Int32")},{984, new PlayerKey(984,"newCharm_10","System.Boolean")},{985, new PlayerKey(985,"gotCharm_11","System.Boolean")},{986, new PlayerKey(986,"equippedCharm_11","System.Boolean")},{988, new PlayerKey(988,"charmCost_11","System.Int32")},{992, new PlayerKey(992,"newCharm_11","System.Boolean")},{993, new PlayerKey(993,"gotCharm_12","System.Boolean")},{994, new PlayerKey(994,"equippedCharm_12","System.Boolean")},{996, new PlayerKey(996,"charmCost_12","System.Int32")},{1000, new PlayerKey(1000,"newCharm_12","System.Boolean")},{1001, new PlayerKey(1001,"gotCharm_13","System.Boolean")},{1002, new PlayerKey(1002,"equippedCharm_13","System.Boolean")},{1004, new PlayerKey(1004,"charmCost_13","System.Int32")},
					{1008, new PlayerKey(1008,"newCharm_13","System.Boolean")},{1009, new PlayerKey(1009,"gotCharm_14","System.Boolean")},{1010, new PlayerKey(1010,"equippedCharm_14","System.Boolean")},{1012, new PlayerKey(1012,"charmCost_14","System.Int32")},{1016, new PlayerKey(1016,"newCharm_14","System.Boolean")},{1017, new PlayerKey(1017,"gotCharm_15","System.Boolean")},{1018, new PlayerKey(1018,"equippedCharm_15","System.Boolean")},{1020, new PlayerKey(1020,"charmCost_15","System.Int32")},{1024, new PlayerKey(1024,"newCharm_15","System.Boolean")},{1025, new PlayerKey(1025,"gotCharm_16","System.Boolean")},{1026, new PlayerKey(1026,"equippedCharm_16","System.Boolean")},{1028, new PlayerKey(1028,"charmCost_16","System.Int32")},{1032, new PlayerKey(1032,"newCharm_16","System.Boolean")},{1033, new PlayerKey(1033,"gotCharm_17","System.Boolean")},{1034, new PlayerKey(1034,"equippedCharm_17","System.Boolean")},{1036, new PlayerKey(1036,"charmCost_17","System.Int32")},{1040, new PlayerKey(1040,"newCharm_17","System.Boolean")},{1041, new PlayerKey(1041,"gotCharm_18","System.Boolean")},{1042, new PlayerKey(1042,"equippedCharm_18","System.Boolean")},{1044, new PlayerKey(1044,"charmCost_18","System.Int32")},{1048, new PlayerKey(1048,"newCharm_18","System.Boolean")},{1049, new PlayerKey(1049,"gotCharm_19","System.Boolean")},{1050, new PlayerKey(1050,"equippedCharm_19","System.Boolean")},{1052, new PlayerKey(1052,"charmCost_19","System.Int32")},{1056, new PlayerKey(1056,"newCharm_19","System.Boolean")},
					{1057, new PlayerKey(1057,"gotCharm_20","System.Boolean")},{1058, new PlayerKey(1058,"equippedCharm_20","System.Boolean")},{1060, new PlayerKey(1060,"charmCost_20","System.Int32")},{1064, new PlayerKey(1064,"newCharm_20","System.Boolean")},{1065, new PlayerKey(1065,"gotCharm_21","System.Boolean")},{1066, new PlayerKey(1066,"equippedCharm_21","System.Boolean")},{1068, new PlayerKey(1068,"charmCost_21","System.Int32")},{1072, new PlayerKey(1072,"newCharm_21","System.Boolean")},{1073, new PlayerKey(1073,"gotCharm_22","System.Boolean")},{1074, new PlayerKey(1074,"equippedCharm_22","System.Boolean")},{1076, new PlayerKey(1076,"charmCost_22","System.Int32")},{1080, new PlayerKey(1080,"newCharm_22","System.Boolean")},{1081, new PlayerKey(1081,"gotCharm_23","System.Boolean")},{1082, new PlayerKey(1082,"equippedCharm_23","System.Boolean")},{1083, new PlayerKey(1083,"brokenCharm_23","System.Boolean")},{1084, new PlayerKey(1084,"charmCost_23","System.Int32")},{1088, new PlayerKey(1088,"newCharm_23","System.Boolean")},{1089, new PlayerKey(1089,"gotCharm_24","System.Boolean")},{1090, new PlayerKey(1090,"equippedCharm_24","System.Boolean")},{1091, new PlayerKey(1091,"brokenCharm_24","System.Boolean")},{1092, new PlayerKey(1092,"charmCost_24","System.Int32")},{1096, new PlayerKey(1096,"newCharm_24","System.Boolean")},{1097, new PlayerKey(1097,"gotCharm_25","System.Boolean")},{1098, new PlayerKey(1098,"equippedCharm_25","System.Boolean")},{1099, new PlayerKey(1099,"brokenCharm_25","System.Boolean")},
					{1100, new PlayerKey(1100,"charmCost_25","System.Int32")},{1104, new PlayerKey(1104,"newCharm_25","System.Boolean")},{1105, new PlayerKey(1105,"gotCharm_26","System.Boolean")},{1106, new PlayerKey(1106,"equippedCharm_26","System.Boolean")},{1108, new PlayerKey(1108,"charmCost_26","System.Int32")},{1112, new PlayerKey(1112,"newCharm_26","System.Boolean")},{1113, new PlayerKey(1113,"gotCharm_27","System.Boolean")},{1114, new PlayerKey(1114,"equippedCharm_27","System.Boolean")},{1116, new PlayerKey(1116,"charmCost_27","System.Int32")},{1120, new PlayerKey(1120,"newCharm_27","System.Boolean")},{1121, new PlayerKey(1121,"gotCharm_28","System.Boolean")},{1122, new PlayerKey(1122,"equippedCharm_28","System.Boolean")},{1124, new PlayerKey(1124,"charmCost_28","System.Int32")},{1128, new PlayerKey(1128,"newCharm_28","System.Boolean")},{1129, new PlayerKey(1129,"gotCharm_29","System.Boolean")},{1130, new PlayerKey(1130,"equippedCharm_29","System.Boolean")},{1132, new PlayerKey(1132,"charmCost_29","System.Int32")},{1136, new PlayerKey(1136,"newCharm_29","System.Boolean")},{1137, new PlayerKey(1137,"gotCharm_30","System.Boolean")},{1138, new PlayerKey(1138,"equippedCharm_30","System.Boolean")},{1140, new PlayerKey(1140,"charmCost_30","System.Int32")},{1144, new PlayerKey(1144,"newCharm_30","System.Boolean")},{1145, new PlayerKey(1145,"gotCharm_31","System.Boolean")},{1146, new PlayerKey(1146,"equippedCharm_31","System.Boolean")},{1148, new PlayerKey(1148,"charmCost_31","System.Int32")},
					{1152, new PlayerKey(1152,"newCharm_31","System.Boolean")},{1153, new PlayerKey(1153,"gotCharm_32","System.Boolean")},{1154, new PlayerKey(1154,"equippedCharm_32","System.Boolean")},{1156, new PlayerKey(1156,"charmCost_32","System.Int32")},{1160, new PlayerKey(1160,"newCharm_32","System.Boolean")},{1161, new PlayerKey(1161,"gotCharm_33","System.Boolean")},{1162, new PlayerKey(1162,"equippedCharm_33","System.Boolean")},{1164, new PlayerKey(1164,"charmCost_33","System.Int32")},{1168, new PlayerKey(1168,"newCharm_33","System.Boolean")},{1169, new PlayerKey(1169,"gotCharm_34","System.Boolean")},{1170, new PlayerKey(1170,"equippedCharm_34","System.Boolean")},{1172, new PlayerKey(1172,"charmCost_34","System.Int32")},{1176, new PlayerKey(1176,"newCharm_34","System.Boolean")},{1177, new PlayerKey(1177,"gotCharm_35","System.Boolean")},{1178, new PlayerKey(1178,"equippedCharm_35","System.Boolean")},{1180, new PlayerKey(1180,"charmCost_35","System.Int32")},{1184, new PlayerKey(1184,"newCharm_35","System.Boolean")},{1185, new PlayerKey(1185,"gotCharm_36","System.Boolean")},{1186, new PlayerKey(1186,"equippedCharm_36","System.Boolean")},{1188, new PlayerKey(1188,"charmCost_36","System.Int32")},{1192, new PlayerKey(1192,"newCharm_36","System.Boolean")},{1196, new PlayerKey(1196,"royalCharmState","System.Int32")},{1200, new PlayerKey(1200,"hasJournal","System.Boolean")},{1204, new PlayerKey(1204,"lastJournalItem","System.Int32")},{1208, new PlayerKey(1208,"killedDummy","System.Boolean")},
					{1212, new PlayerKey(1212,"killsDummy","System.Int32")},{1216, new PlayerKey(1216,"newDataDummy","System.Boolean")},{1217, new PlayerKey(1217,"seenJournalMsg","System.Boolean")},{1218, new PlayerKey(1218,"seenHunterMsg","System.Boolean")},{1219, new PlayerKey(1219,"fillJournal","System.Boolean")},{1220, new PlayerKey(1220,"journalEntriesCompleted","System.Int32")},{1224, new PlayerKey(1224,"journalNotesCompleted","System.Int32")},{1228, new PlayerKey(1228,"journalEntriesTotal","System.Int32")},{1232, new PlayerKey(1232,"killedCrawler","System.Boolean")},{1236, new PlayerKey(1236,"killsCrawler","System.Int32")},{1240, new PlayerKey(1240,"newDataCrawler","System.Boolean")},{1241, new PlayerKey(1241,"killedBuzzer","System.Boolean")},{1244, new PlayerKey(1244,"killsBuzzer","System.Int32")},{1248, new PlayerKey(1248,"newDataBuzzer","System.Boolean")},{1249, new PlayerKey(1249,"killedBouncer","System.Boolean")},{1252, new PlayerKey(1252,"killsBouncer","System.Int32")},{1256, new PlayerKey(1256,"newDataBouncer","System.Boolean")},{1257, new PlayerKey(1257,"killedClimber","System.Boolean")},{1260, new PlayerKey(1260,"killsClimber","System.Int32")},{1264, new PlayerKey(1264,"newDataClimber","System.Boolean")},{1265, new PlayerKey(1265,"killedHopper","System.Boolean")},{1268, new PlayerKey(1268,"killsHopper","System.Int32")},{1272, new PlayerKey(1272,"newDataHopper","System.Boolean")},{1273, new PlayerKey(1273,"killedWorm","System.Boolean")},{1276, new PlayerKey(1276,"killsWorm","System.Int32")},
					{1280, new PlayerKey(1280,"newDataWorm","System.Boolean")},{1281, new PlayerKey(1281,"killedSpitter","System.Boolean")},{1284, new PlayerKey(1284,"killsSpitter","System.Int32")},{1288, new PlayerKey(1288,"newDataSpitter","System.Boolean")},{1289, new PlayerKey(1289,"killedHatcher","System.Boolean")},{1292, new PlayerKey(1292,"killsHatcher","System.Int32")},{1296, new PlayerKey(1296,"newDataHatcher","System.Boolean")},{1297, new PlayerKey(1297,"killedHatchling","System.Boolean")},{1300, new PlayerKey(1300,"killsHatchling","System.Int32")},{1304, new PlayerKey(1304,"newDataHatchling","System.Boolean")},{1305, new PlayerKey(1305,"killedZombieRunner","System.Boolean")},{1308, new PlayerKey(1308,"killsZombieRunner","System.Int32")},{1312, new PlayerKey(1312,"newDataZombieRunner","System.Boolean")},{1313, new PlayerKey(1313,"killedZombieHornhead","System.Boolean")},{1316, new PlayerKey(1316,"killsZombieHornhead","System.Int32")},{1320, new PlayerKey(1320,"newDataZombieHornhead","System.Boolean")},{1321, new PlayerKey(1321,"killedZombieLeaper","System.Boolean")},{1324, new PlayerKey(1324,"killsZombieLeaper","System.Int32")},{1328, new PlayerKey(1328,"newDataZombieLeaper","System.Boolean")},{1329, new PlayerKey(1329,"killedZombieBarger","System.Boolean")},{1332, new PlayerKey(1332,"killsZombieBarger","System.Int32")},{1336, new PlayerKey(1336,"newDataZombieBarger","System.Boolean")},{1337, new PlayerKey(1337,"killedZombieShield","System.Boolean")},{1340, new PlayerKey(1340,"killsZombieShield","System.Int32")},{1344, new PlayerKey(1344,"newDataZombieShield","System.Boolean")},
					{1345, new PlayerKey(1345,"killedZombieGuard","System.Boolean")},{1348, new PlayerKey(1348,"killsZombieGuard","System.Int32")},{1352, new PlayerKey(1352,"newDataZombieGuard","System.Boolean")},{1353, new PlayerKey(1353,"killedBigBuzzer","System.Boolean")},{1356, new PlayerKey(1356,"killsBigBuzzer","System.Int32")},{1360, new PlayerKey(1360,"newDataBigBuzzer","System.Boolean")},{1361, new PlayerKey(1361,"killedBigFly","System.Boolean")},{1364, new PlayerKey(1364,"killsBigFly","System.Int32")},{1368, new PlayerKey(1368,"newDataBigFly","System.Boolean")},{1369, new PlayerKey(1369,"killedMawlek","System.Boolean")},{1372, new PlayerKey(1372,"killsMawlek","System.Int32")},{1376, new PlayerKey(1376,"newDataMawlek","System.Boolean")},{1377, new PlayerKey(1377,"killedFalseKnight","System.Boolean")},{1380, new PlayerKey(1380,"killsFalseKnight","System.Int32")},{1384, new PlayerKey(1384,"newDataFalseKnight","System.Boolean")},{1385, new PlayerKey(1385,"killedRoller","System.Boolean")},{1388, new PlayerKey(1388,"killsRoller","System.Int32")},{1392, new PlayerKey(1392,"newDataRoller","System.Boolean")},{1393, new PlayerKey(1393,"killedBlocker","System.Boolean")},{1396, new PlayerKey(1396,"killsBlocker","System.Int32")},{1400, new PlayerKey(1400,"newDataBlocker","System.Boolean")},{1401, new PlayerKey(1401,"killedPrayerSlug","System.Boolean")},{1404, new PlayerKey(1404,"killsPrayerSlug","System.Int32")},{1408, new PlayerKey(1408,"newDataPrayerSlug","System.Boolean")},{1409, new PlayerKey(1409,"killedMenderBug","System.Boolean")},
					{1412, new PlayerKey(1412,"killsMenderBug","System.Int32")},{1416, new PlayerKey(1416,"newDataMenderBug","System.Boolean")},{1417, new PlayerKey(1417,"killedMossmanRunner","System.Boolean")},{1420, new PlayerKey(1420,"killsMossmanRunner","System.Int32")},{1424, new PlayerKey(1424,"newDataMossmanRunner","System.Boolean")},{1425, new PlayerKey(1425,"killedMossmanShaker","System.Boolean")},{1428, new PlayerKey(1428,"killsMossmanShaker","System.Int32")},{1432, new PlayerKey(1432,"newDataMossmanShaker","System.Boolean")},{1433, new PlayerKey(1433,"killedMosquito","System.Boolean")},{1436, new PlayerKey(1436,"killsMosquito","System.Int32")},{1440, new PlayerKey(1440,"newDataMosquito","System.Boolean")},{1441, new PlayerKey(1441,"killedBlobFlyer","System.Boolean")},{1444, new PlayerKey(1444,"killsBlobFlyer","System.Int32")},{1448, new PlayerKey(1448,"newDataBlobFlyer","System.Boolean")},{1449, new PlayerKey(1449,"killedFungifiedZombie","System.Boolean")},{1452, new PlayerKey(1452,"killsFungifiedZombie","System.Int32")},{1456, new PlayerKey(1456,"newDataFungifiedZombie","System.Boolean")},{1457, new PlayerKey(1457,"killedPlantShooter","System.Boolean")},{1460, new PlayerKey(1460,"killsPlantShooter","System.Int32")},{1464, new PlayerKey(1464,"newDataPlantShooter","System.Boolean")},{1465, new PlayerKey(1465,"killedMossCharger","System.Boolean")},{1468, new PlayerKey(1468,"killsMossCharger","System.Int32")},{1472, new PlayerKey(1472,"newDataMossCharger","System.Boolean")},{1473, new PlayerKey(1473,"killedMegaMossCharger","System.Boolean")},{1476, new PlayerKey(1476,"killsMegaMossCharger","System.Int32")},
					{1480, new PlayerKey(1480,"newDataMegaMossCharger","System.Boolean")},{1481, new PlayerKey(1481,"killedSnapperTrap","System.Boolean")},{1484, new PlayerKey(1484,"killsSnapperTrap","System.Int32")},{1488, new PlayerKey(1488,"newDataSnapperTrap","System.Boolean")},{1489, new PlayerKey(1489,"killedMossKnight","System.Boolean")},{1492, new PlayerKey(1492,"killsMossKnight","System.Int32")},{1496, new PlayerKey(1496,"newDataMossKnight","System.Boolean")},{1497, new PlayerKey(1497,"killedGrassHopper","System.Boolean")},{1500, new PlayerKey(1500,"killsGrassHopper","System.Int32")},{1504, new PlayerKey(1504,"newDataGrassHopper","System.Boolean")},{1505, new PlayerKey(1505,"killedAcidFlyer","System.Boolean")},{1508, new PlayerKey(1508,"killsAcidFlyer","System.Int32")},{1512, new PlayerKey(1512,"newDataAcidFlyer","System.Boolean")},{1513, new PlayerKey(1513,"killedAcidWalker","System.Boolean")},{1516, new PlayerKey(1516,"killsAcidWalker","System.Int32")},{1520, new PlayerKey(1520,"newDataAcidWalker","System.Boolean")},{1521, new PlayerKey(1521,"killedMossFlyer","System.Boolean")},{1524, new PlayerKey(1524,"killsMossFlyer","System.Int32")},{1528, new PlayerKey(1528,"newDataMossFlyer","System.Boolean")},{1529, new PlayerKey(1529,"killedMossKnightFat","System.Boolean")},{1532, new PlayerKey(1532,"killsMossKnightFat","System.Int32")},{1536, new PlayerKey(1536,"newDataMossKnightFat","System.Boolean")},{1537, new PlayerKey(1537,"killedMossWalker","System.Boolean")},{1540, new PlayerKey(1540,"killsMossWalker","System.Int32")},{1544, new PlayerKey(1544,"newDataMossWalker","System.Boolean")},
					{1545, new PlayerKey(1545,"killedInfectedKnight","System.Boolean")},{1548, new PlayerKey(1548,"killsInfectedKnight","System.Int32")},{1552, new PlayerKey(1552,"newDataInfectedKnight","System.Boolean")},{1553, new PlayerKey(1553,"killedLazyFlyer","System.Boolean")},{1556, new PlayerKey(1556,"killsLazyFlyer","System.Int32")},{1560, new PlayerKey(1560,"newDataLazyFlyer","System.Boolean")},{1561, new PlayerKey(1561,"killedZapBug","System.Boolean")},{1564, new PlayerKey(1564,"killsZapBug","System.Int32")},{1568, new PlayerKey(1568,"newDataZapBug","System.Boolean")},{1569, new PlayerKey(1569,"killedJellyfish","System.Boolean")},{1572, new PlayerKey(1572,"killsJellyfish","System.Int32")},{1576, new PlayerKey(1576,"newDataJellyfish","System.Boolean")},{1577, new PlayerKey(1577,"killedJellyCrawler","System.Boolean")},{1580, new PlayerKey(1580,"killsJellyCrawler","System.Int32")},{1584, new PlayerKey(1584,"newDataJellyCrawler","System.Boolean")},{1585, new PlayerKey(1585,"killedMegaJellyfish","System.Boolean")},{1588, new PlayerKey(1588,"killsMegaJellyfish","System.Int32")},{1592, new PlayerKey(1592,"newDataMegaJellyfish","System.Boolean")},{1593, new PlayerKey(1593,"killedFungoonBaby","System.Boolean")},{1596, new PlayerKey(1596,"killsFungoonBaby","System.Int32")},{1600, new PlayerKey(1600,"newDataFungoonBaby","System.Boolean")},{1601, new PlayerKey(1601,"killedMushroomTurret","System.Boolean")},{1604, new PlayerKey(1604,"killsMushroomTurret","System.Int32")},{1608, new PlayerKey(1608,"newDataMushroomTurret","System.Boolean")},{1609, new PlayerKey(1609,"killedMantis","System.Boolean")},
					{1612, new PlayerKey(1612,"killsMantis","System.Int32")},{1616, new PlayerKey(1616,"newDataMantis","System.Boolean")},{1617, new PlayerKey(1617,"killedMushroomRoller","System.Boolean")},{1620, new PlayerKey(1620,"killsMushroomRoller","System.Int32")},{1624, new PlayerKey(1624,"newDataMushroomRoller","System.Boolean")},{1625, new PlayerKey(1625,"killedMushroomBrawler","System.Boolean")},{1628, new PlayerKey(1628,"killsMushroomBrawler","System.Int32")},{1632, new PlayerKey(1632,"newDataMushroomBrawler","System.Boolean")},{1633, new PlayerKey(1633,"killedMushroomBaby","System.Boolean")},{1636, new PlayerKey(1636,"killsMushroomBaby","System.Int32")},{1640, new PlayerKey(1640,"newDataMushroomBaby","System.Boolean")},{1641, new PlayerKey(1641,"killedMantisFlyerChild","System.Boolean")},{1644, new PlayerKey(1644,"killsMantisFlyerChild","System.Int32")},{1648, new PlayerKey(1648,"newDataMantisFlyerChild","System.Boolean")},{1649, new PlayerKey(1649,"killedFungusFlyer","System.Boolean")},{1652, new PlayerKey(1652,"killsFungusFlyer","System.Int32")},{1656, new PlayerKey(1656,"newDataFungusFlyer","System.Boolean")},{1657, new PlayerKey(1657,"killedFungCrawler","System.Boolean")},{1660, new PlayerKey(1660,"killsFungCrawler","System.Int32")},{1664, new PlayerKey(1664,"newDataFungCrawler","System.Boolean")},{1665, new PlayerKey(1665,"killedMantisLord","System.Boolean")},{1668, new PlayerKey(1668,"killsMantisLord","System.Int32")},{1672, new PlayerKey(1672,"newDataMantisLord","System.Boolean")},{1673, new PlayerKey(1673,"killedBlackKnight","System.Boolean")},{1676, new PlayerKey(1676,"killsBlackKnight","System.Int32")},
					{1680, new PlayerKey(1680,"newDataBlackKnight","System.Boolean")},{1681, new PlayerKey(1681,"killedElectricMage","System.Boolean")},{1684, new PlayerKey(1684,"killsElectricMage","System.Int32")},{1688, new PlayerKey(1688,"newDataElectricMage","System.Boolean")},{1689, new PlayerKey(1689,"killedMage","System.Boolean")},{1692, new PlayerKey(1692,"killsMage","System.Int32")},{1696, new PlayerKey(1696,"newDataMage","System.Boolean")},{1697, new PlayerKey(1697,"killedMageKnight","System.Boolean")},{1700, new PlayerKey(1700,"killsMageKnight","System.Int32")},{1704, new PlayerKey(1704,"newDataMageKnight","System.Boolean")},{1705, new PlayerKey(1705,"killedRoyalDandy","System.Boolean")},{1708, new PlayerKey(1708,"killsRoyalDandy","System.Int32")},{1712, new PlayerKey(1712,"newDataRoyalDandy","System.Boolean")},{1713, new PlayerKey(1713,"killedRoyalCoward","System.Boolean")},{1716, new PlayerKey(1716,"killsRoyalCoward","System.Int32")},{1720, new PlayerKey(1720,"newDataRoyalCoward","System.Boolean")},{1721, new PlayerKey(1721,"killedRoyalPlumper","System.Boolean")},{1724, new PlayerKey(1724,"killsRoyalPlumper","System.Int32")},{1728, new PlayerKey(1728,"newDataRoyalPlumper","System.Boolean")},{1729, new PlayerKey(1729,"killedFlyingSentrySword","System.Boolean")},{1732, new PlayerKey(1732,"killsFlyingSentrySword","System.Int32")},{1736, new PlayerKey(1736,"newDataFlyingSentrySword","System.Boolean")},{1737, new PlayerKey(1737,"killedFlyingSentryJavelin","System.Boolean")},{1740, new PlayerKey(1740,"killsFlyingSentryJavelin","System.Int32")},{1744, new PlayerKey(1744,"newDataFlyingSentryJavelin","System.Boolean")},
					{1745, new PlayerKey(1745,"killedSentry","System.Boolean")},{1748, new PlayerKey(1748,"killsSentry","System.Int32")},{1752, new PlayerKey(1752,"newDataSentry","System.Boolean")},{1753, new PlayerKey(1753,"killedSentryFat","System.Boolean")},{1756, new PlayerKey(1756,"killsSentryFat","System.Int32")},{1760, new PlayerKey(1760,"newDataSentryFat","System.Boolean")},{1761, new PlayerKey(1761,"killedMageBlob","System.Boolean")},{1764, new PlayerKey(1764,"killsMageBlob","System.Int32")},{1768, new PlayerKey(1768,"newDataMageBlob","System.Boolean")},{1769, new PlayerKey(1769,"killedGreatShieldZombie","System.Boolean")},{1772, new PlayerKey(1772,"killsGreatShieldZombie","System.Int32")},{1776, new PlayerKey(1776,"newDataGreatShieldZombie","System.Boolean")},{1777, new PlayerKey(1777,"killedJarCollector","System.Boolean")},{1780, new PlayerKey(1780,"killsJarCollector","System.Int32")},{1784, new PlayerKey(1784,"newDataJarCollector","System.Boolean")},{1785, new PlayerKey(1785,"killedMageBalloon","System.Boolean")},{1788, new PlayerKey(1788,"killsMageBalloon","System.Int32")},{1792, new PlayerKey(1792,"newDataMageBalloon","System.Boolean")},{1793, new PlayerKey(1793,"killedMageLord","System.Boolean")},{1796, new PlayerKey(1796,"killsMageLord","System.Int32")},{1800, new PlayerKey(1800,"newDataMageLord","System.Boolean")},{1801, new PlayerKey(1801,"killedGorgeousHusk","System.Boolean")},{1804, new PlayerKey(1804,"killsGorgeousHusk","System.Int32")},{1808, new PlayerKey(1808,"newDataGorgeousHusk","System.Boolean")},{1809, new PlayerKey(1809,"killedFlipHopper","System.Boolean")},
					{1812, new PlayerKey(1812,"killsFlipHopper","System.Int32")},{1816, new PlayerKey(1816,"newDataFlipHopper","System.Boolean")},{1817, new PlayerKey(1817,"killedFlukeman","System.Boolean")},{1820, new PlayerKey(1820,"killsFlukeman","System.Int32")},{1824, new PlayerKey(1824,"newDataFlukeman","System.Boolean")},{1825, new PlayerKey(1825,"killedInflater","System.Boolean")},{1828, new PlayerKey(1828,"killsInflater","System.Int32")},{1832, new PlayerKey(1832,"newDataInflater","System.Boolean")},{1833, new PlayerKey(1833,"killedFlukefly","System.Boolean")},{1836, new PlayerKey(1836,"killsFlukefly","System.Int32")},{1840, new PlayerKey(1840,"newDataFlukefly","System.Boolean")},{1841, new PlayerKey(1841,"killedFlukeMother","System.Boolean")},{1844, new PlayerKey(1844,"killsFlukeMother","System.Int32")},{1848, new PlayerKey(1848,"newDataFlukeMother","System.Boolean")},{1849, new PlayerKey(1849,"killedDungDefender","System.Boolean")},{1852, new PlayerKey(1852,"killsDungDefender","System.Int32")},{1856, new PlayerKey(1856,"newDataDungDefender","System.Boolean")},{1857, new PlayerKey(1857,"killedCrystalCrawler","System.Boolean")},{1860, new PlayerKey(1860,"killsCrystalCrawler","System.Int32")},{1864, new PlayerKey(1864,"newDataCrystalCrawler","System.Boolean")},{1865, new PlayerKey(1865,"killedCrystalFlyer","System.Boolean")},{1868, new PlayerKey(1868,"killsCrystalFlyer","System.Int32")},{1872, new PlayerKey(1872,"newDataCrystalFlyer","System.Boolean")},{1873, new PlayerKey(1873,"killedLaserBug","System.Boolean")},{1876, new PlayerKey(1876,"killsLaserBug","System.Int32")},
					{1880, new PlayerKey(1880,"newDataLaserBug","System.Boolean")},{1881, new PlayerKey(1881,"killedBeamMiner","System.Boolean")},{1884, new PlayerKey(1884,"killsBeamMiner","System.Int32")},{1888, new PlayerKey(1888,"newDataBeamMiner","System.Boolean")},{1889, new PlayerKey(1889,"killedZombieMiner","System.Boolean")},{1892, new PlayerKey(1892,"killsZombieMiner","System.Int32")},{1896, new PlayerKey(1896,"newDataZombieMiner","System.Boolean")},{1897, new PlayerKey(1897,"killedMegaBeamMiner","System.Boolean")},{1900, new PlayerKey(1900,"killsMegaBeamMiner","System.Int32")},{1904, new PlayerKey(1904,"newDataMegaBeamMiner","System.Boolean")},{1905, new PlayerKey(1905,"killedMinesCrawler","System.Boolean")},{1908, new PlayerKey(1908,"killsMinesCrawler","System.Int32")},{1912, new PlayerKey(1912,"newDataMinesCrawler","System.Boolean")},{1913, new PlayerKey(1913,"killedAngryBuzzer","System.Boolean")},{1916, new PlayerKey(1916,"killsAngryBuzzer","System.Int32")},{1920, new PlayerKey(1920,"newDataAngryBuzzer","System.Boolean")},{1921, new PlayerKey(1921,"killedBurstingBouncer","System.Boolean")},{1924, new PlayerKey(1924,"killsBurstingBouncer","System.Int32")},{1928, new PlayerKey(1928,"newDataBurstingBouncer","System.Boolean")},{1929, new PlayerKey(1929,"killedBurstingZombie","System.Boolean")},{1932, new PlayerKey(1932,"killsBurstingZombie","System.Int32")},{1936, new PlayerKey(1936,"newDataBurstingZombie","System.Boolean")},{1937, new PlayerKey(1937,"killedSpittingZombie","System.Boolean")},{1940, new PlayerKey(1940,"killsSpittingZombie","System.Int32")},{1944, new PlayerKey(1944,"newDataSpittingZombie","System.Boolean")},
					{1945, new PlayerKey(1945,"killedBabyCentipede","System.Boolean")},{1948, new PlayerKey(1948,"killsBabyCentipede","System.Int32")},{1952, new PlayerKey(1952,"newDataBabyCentipede","System.Boolean")},{1953, new PlayerKey(1953,"killedBigCentipede","System.Boolean")},{1956, new PlayerKey(1956,"killsBigCentipede","System.Int32")},{1960, new PlayerKey(1960,"newDataBigCentipede","System.Boolean")},{1961, new PlayerKey(1961,"killedCentipedeHatcher","System.Boolean")},{1964, new PlayerKey(1964,"killsCentipedeHatcher","System.Int32")},{1968, new PlayerKey(1968,"newDataCentipedeHatcher","System.Boolean")},{1969, new PlayerKey(1969,"killedLesserMawlek","System.Boolean")},{1972, new PlayerKey(1972,"killsLesserMawlek","System.Int32")},{1976, new PlayerKey(1976,"newDataLesserMawlek","System.Boolean")},{1977, new PlayerKey(1977,"killedSlashSpider","System.Boolean")},{1980, new PlayerKey(1980,"killsSlashSpider","System.Int32")},{1984, new PlayerKey(1984,"newDataSlashSpider","System.Boolean")},{1985, new PlayerKey(1985,"killedSpiderCorpse","System.Boolean")},{1988, new PlayerKey(1988,"killsSpiderCorpse","System.Int32")},{1992, new PlayerKey(1992,"newDataSpiderCorpse","System.Boolean")},{1993, new PlayerKey(1993,"killedShootSpider","System.Boolean")},{1996, new PlayerKey(1996,"killsShootSpider","System.Int32")},{2000, new PlayerKey(2000,"newDataShootSpider","System.Boolean")},{2001, new PlayerKey(2001,"killedMiniSpider","System.Boolean")},{2004, new PlayerKey(2004,"killsMiniSpider","System.Int32")},{2008, new PlayerKey(2008,"newDataMiniSpider","System.Boolean")},{2009, new PlayerKey(2009,"killedSpiderFlyer","System.Boolean")},
					{2012, new PlayerKey(2012,"killsSpiderFlyer","System.Int32")},{2016, new PlayerKey(2016,"newDataSpiderFlyer","System.Boolean")},{2017, new PlayerKey(2017,"killedMimicSpider","System.Boolean")},{2020, new PlayerKey(2020,"killsMimicSpider","System.Int32")},{2024, new PlayerKey(2024,"newDataMimicSpider","System.Boolean")},{2025, new PlayerKey(2025,"killedBeeHatchling","System.Boolean")},{2028, new PlayerKey(2028,"killsBeeHatchling","System.Int32")},{2032, new PlayerKey(2032,"newDataBeeHatchling","System.Boolean")},{2033, new PlayerKey(2033,"killedBeeStinger","System.Boolean")},{2036, new PlayerKey(2036,"killsBeeStinger","System.Int32")},{2040, new PlayerKey(2040,"newDataBeeStinger","System.Boolean")},{2041, new PlayerKey(2041,"killedBigBee","System.Boolean")},{2044, new PlayerKey(2044,"killsBigBee","System.Int32")},{2048, new PlayerKey(2048,"newDataBigBee","System.Boolean")},{2049, new PlayerKey(2049,"killedBlowFly","System.Boolean")},{2052, new PlayerKey(2052,"killsBlowFly","System.Int32")},{2056, new PlayerKey(2056,"newDataBlowFly","System.Boolean")},{2057, new PlayerKey(2057,"killedCeilingDropper","System.Boolean")},{2060, new PlayerKey(2060,"killsCeilingDropper","System.Int32")},{2064, new PlayerKey(2064,"newDataCeilingDropper","System.Boolean")},{2065, new PlayerKey(2065,"killedGiantHopper","System.Boolean")},{2068, new PlayerKey(2068,"killsGiantHopper","System.Int32")},{2072, new PlayerKey(2072,"newDataGiantHopper","System.Boolean")},{2073, new PlayerKey(2073,"killedGrubMimic","System.Boolean")},{2076, new PlayerKey(2076,"killsGrubMimic","System.Int32")},
					{2080, new PlayerKey(2080,"newDataGrubMimic","System.Boolean")},{2081, new PlayerKey(2081,"killedMawlekTurret","System.Boolean")},{2084, new PlayerKey(2084,"killsMawlekTurret","System.Int32")},{2088, new PlayerKey(2088,"newDataMawlekTurret","System.Boolean")},{2089, new PlayerKey(2089,"killedOrangeScuttler","System.Boolean")},{2092, new PlayerKey(2092,"killsOrangeScuttler","System.Int32")},{2096, new PlayerKey(2096,"newDataOrangeScuttler","System.Boolean")},{2097, new PlayerKey(2097,"killedHealthScuttler","System.Boolean")},{2100, new PlayerKey(2100,"killsHealthScuttler","System.Int32")},{2104, new PlayerKey(2104,"newDataHealthScuttler","System.Boolean")},{2105, new PlayerKey(2105,"killedPigeon","System.Boolean")},{2108, new PlayerKey(2108,"killsPigeon","System.Int32")},{2112, new PlayerKey(2112,"newDataPigeon","System.Boolean")},{2113, new PlayerKey(2113,"killedZombieHive","System.Boolean")},{2116, new PlayerKey(2116,"killsZombieHive","System.Int32")},{2120, new PlayerKey(2120,"newDataZombieHive","System.Boolean")},{2121, new PlayerKey(2121,"killedDreamGuard","System.Boolean")},{2124, new PlayerKey(2124,"killsDreamGuard","System.Int32")},{2128, new PlayerKey(2128,"newDataDreamGuard","System.Boolean")},{2129, new PlayerKey(2129,"killedHornet","System.Boolean")},{2132, new PlayerKey(2132,"killsHornet","System.Int32")},{2136, new PlayerKey(2136,"newDataHornet","System.Boolean")},{2137, new PlayerKey(2137,"killedAbyssCrawler","System.Boolean")},{2140, new PlayerKey(2140,"killsAbyssCrawler","System.Int32")},{2144, new PlayerKey(2144,"newDataAbyssCrawler","System.Boolean")},
					{2145, new PlayerKey(2145,"killedSuperSpitter","System.Boolean")},{2148, new PlayerKey(2148,"killsSuperSpitter","System.Int32")},{2152, new PlayerKey(2152,"newDataSuperSpitter","System.Boolean")},{2153, new PlayerKey(2153,"killedSibling","System.Boolean")},{2156, new PlayerKey(2156,"killsSibling","System.Int32")},{2160, new PlayerKey(2160,"newDataSibling","System.Boolean")},{2161, new PlayerKey(2161,"killedPalaceFly","System.Boolean")},{2164, new PlayerKey(2164,"killsPalaceFly","System.Int32")},{2168, new PlayerKey(2168,"newDataPalaceFly","System.Boolean")},{2169, new PlayerKey(2169,"killedEggSac","System.Boolean")},{2172, new PlayerKey(2172,"killsEggSac","System.Int32")},{2176, new PlayerKey(2176,"newDataEggSac","System.Boolean")},{2177, new PlayerKey(2177,"killedMummy","System.Boolean")},{2180, new PlayerKey(2180,"killsMummy","System.Int32")},{2184, new PlayerKey(2184,"newDataMummy","System.Boolean")},{2185, new PlayerKey(2185,"killedOrangeBalloon","System.Boolean")},{2188, new PlayerKey(2188,"killsOrangeBalloon","System.Int32")},{2192, new PlayerKey(2192,"newDataOrangeBalloon","System.Boolean")},{2193, new PlayerKey(2193,"killedAbyssTendril","System.Boolean")},{2196, new PlayerKey(2196,"killsAbyssTendril","System.Int32")},{2200, new PlayerKey(2200,"newDataAbyssTendril","System.Boolean")},{2201, new PlayerKey(2201,"killedHeavyMantis","System.Boolean")},{2204, new PlayerKey(2204,"killsHeavyMantis","System.Int32")},{2208, new PlayerKey(2208,"newDataHeavyMantis","System.Boolean")},{2209, new PlayerKey(2209,"killedTraitorLord","System.Boolean")},
					{2212, new PlayerKey(2212,"killsTraitorLord","System.Int32")},{2216, new PlayerKey(2216,"newDataTraitorLord","System.Boolean")},{2217, new PlayerKey(2217,"killedMantisHeavyFlyer","System.Boolean")},{2220, new PlayerKey(2220,"killsMantisHeavyFlyer","System.Int32")},{2224, new PlayerKey(2224,"newDataMantisHeavyFlyer","System.Boolean")},{2225, new PlayerKey(2225,"killedGardenZombie","System.Boolean")},{2228, new PlayerKey(2228,"killsGardenZombie","System.Int32")},{2232, new PlayerKey(2232,"newDataGardenZombie","System.Boolean")},{2233, new PlayerKey(2233,"killedRoyalGuard","System.Boolean")},{2236, new PlayerKey(2236,"killsRoyalGuard","System.Int32")},{2240, new PlayerKey(2240,"newDataRoyalGuard","System.Boolean")},{2241, new PlayerKey(2241,"killedWhiteRoyal","System.Boolean")},{2244, new PlayerKey(2244,"killsWhiteRoyal","System.Int32")},{2248, new PlayerKey(2248,"newDataWhiteRoyal","System.Boolean")},{2249, new PlayerKey(2249,"openedPalaceGrounds","System.Boolean")},{2250, new PlayerKey(2250,"killedOblobble","System.Boolean")},{2252, new PlayerKey(2252,"killsOblobble","System.Int32")},{2256, new PlayerKey(2256,"newDataOblobble","System.Boolean")},{2257, new PlayerKey(2257,"killedZote","System.Boolean")},{2260, new PlayerKey(2260,"killsZote","System.Int32")},{2264, new PlayerKey(2264,"newDataZote","System.Boolean")},{2265, new PlayerKey(2265,"killedBlobble","System.Boolean")},{2268, new PlayerKey(2268,"killsBlobble","System.Int32")},{2272, new PlayerKey(2272,"newDataBlobble","System.Boolean")},{2273, new PlayerKey(2273,"killedColMosquito","System.Boolean")},
					{2276, new PlayerKey(2276,"killsColMosquito","System.Int32")},{2280, new PlayerKey(2280,"newDataColMosquito","System.Boolean")},{2281, new PlayerKey(2281,"killedColRoller","System.Boolean")},{2284, new PlayerKey(2284,"killsColRoller","System.Int32")},{2288, new PlayerKey(2288,"newDataColRoller","System.Boolean")},{2289, new PlayerKey(2289,"killedColFlyingSentry","System.Boolean")},{2292, new PlayerKey(2292,"killsColFlyingSentry","System.Int32")},{2296, new PlayerKey(2296,"newDataColFlyingSentry","System.Boolean")},{2297, new PlayerKey(2297,"killedColMiner","System.Boolean")},{2300, new PlayerKey(2300,"killsColMiner","System.Int32")},{2304, new PlayerKey(2304,"newDataColMiner","System.Boolean")},{2305, new PlayerKey(2305,"killedColShield","System.Boolean")},{2308, new PlayerKey(2308,"killsColShield","System.Int32")},{2312, new PlayerKey(2312,"newDataColShield","System.Boolean")},{2313, new PlayerKey(2313,"killedColWorm","System.Boolean")},{2316, new PlayerKey(2316,"killsColWorm","System.Int32")},{2320, new PlayerKey(2320,"newDataColWorm","System.Boolean")},{2321, new PlayerKey(2321,"killedColHopper","System.Boolean")},{2324, new PlayerKey(2324,"killsColHopper","System.Int32")},{2328, new PlayerKey(2328,"newDataColHopper","System.Boolean")},{2329, new PlayerKey(2329,"killedLobsterLancer","System.Boolean")},{2332, new PlayerKey(2332,"killsLobsterLancer","System.Int32")},{2336, new PlayerKey(2336,"newDataLobsterLancer","System.Boolean")},{2337, new PlayerKey(2337,"killedGhostAladar","System.Boolean")},{2340, new PlayerKey(2340,"killsGhostAladar","System.Int32")},
					{2344, new PlayerKey(2344,"newDataGhostAladar","System.Boolean")},{2345, new PlayerKey(2345,"killedGhostXero","System.Boolean")},{2348, new PlayerKey(2348,"killsGhostXero","System.Int32")},{2352, new PlayerKey(2352,"newDataGhostXero","System.Boolean")},{2353, new PlayerKey(2353,"killedGhostHu","System.Boolean")},{2356, new PlayerKey(2356,"killsGhostHu","System.Int32")},{2360, new PlayerKey(2360,"newDataGhostHu","System.Boolean")},{2361, new PlayerKey(2361,"killedGhostMarmu","System.Boolean")},{2364, new PlayerKey(2364,"killsGhostMarmu","System.Int32")},{2368, new PlayerKey(2368,"newDataGhostMarmu","System.Boolean")},{2369, new PlayerKey(2369,"killedGhostNoEyes","System.Boolean")},{2372, new PlayerKey(2372,"killsGhostNoEyes","System.Int32")},{2376, new PlayerKey(2376,"newDataGhostNoEyes","System.Boolean")},{2377, new PlayerKey(2377,"killedGhostMarkoth","System.Boolean")},{2380, new PlayerKey(2380,"killsGhostMarkoth","System.Int32")},{2384, new PlayerKey(2384,"newDataGhostMarkoth","System.Boolean")},{2385, new PlayerKey(2385,"killedGhostGalien","System.Boolean")},{2388, new PlayerKey(2388,"killsGhostGalien","System.Int32")},{2392, new PlayerKey(2392,"newDataGhostGalien","System.Boolean")},{2393, new PlayerKey(2393,"killedHollowKnight","System.Boolean")},{2396, new PlayerKey(2396,"killsHollowKnight","System.Int32")},{2400, new PlayerKey(2400,"newDataHollowKnight","System.Boolean")},{2401, new PlayerKey(2401,"killedFinalBoss","System.Boolean")},{2404, new PlayerKey(2404,"killsFinalBoss","System.Int32")},{2408, new PlayerKey(2408,"newDataFinalBoss","System.Boolean")},
					{2409, new PlayerKey(2409,"killedHunterMark","System.Boolean")},{2412, new PlayerKey(2412,"killsHunterMark","System.Int32")},{2416, new PlayerKey(2416,"newDataHunterMark","System.Boolean")},{2420, new PlayerKey(2420,"grubsCollected","System.Int32")},{2424, new PlayerKey(2424,"grubRewards","System.Int32")},{2428, new PlayerKey(2428,"finalGrubRewardCollected","System.Boolean")},{2429, new PlayerKey(2429,"fatGrubKing","System.Boolean")},{2430, new PlayerKey(2430,"falseKnightDefeated","System.Boolean")},{2431, new PlayerKey(2431,"falseKnightDreamDefeated","System.Boolean")},{2432, new PlayerKey(2432,"falseKnightOrbsCollected","System.Boolean")},{2433, new PlayerKey(2433,"mawlekDefeated","System.Boolean")},{2434, new PlayerKey(2434,"giantBuzzerDefeated","System.Boolean")},{2435, new PlayerKey(2435,"giantFlyDefeated","System.Boolean")},{2436, new PlayerKey(2436,"blocker1Defeated","System.Boolean")},{2437, new PlayerKey(2437,"blocker2Defeated","System.Boolean")},{2438, new PlayerKey(2438,"hornet1Defeated","System.Boolean")},{2439, new PlayerKey(2439,"collectorDefeated","System.Boolean")},{2440, new PlayerKey(2440,"hornetOutskirtsDefeated","System.Boolean")},{2441, new PlayerKey(2441,"mageLordDreamDefeated","System.Boolean")},{2442, new PlayerKey(2442,"mageLordOrbsCollected","System.Boolean")},{2443, new PlayerKey(2443,"infectedKnightDreamDefeated","System.Boolean")},{2444, new PlayerKey(2444,"infectedKnightOrbsCollected","System.Boolean")},{2448, new PlayerKey(2448,"aladarSlugDefeated","System.Int32")},{2452, new PlayerKey(2452,"xeroDefeated","System.Int32")},{2456, new PlayerKey(2456,"elderHuDefeated","System.Int32")},
					{2460, new PlayerKey(2460,"mumCaterpillarDefeated","System.Int32")},{2464, new PlayerKey(2464,"noEyesDefeated","System.Int32")},{2468, new PlayerKey(2468,"markothDefeated","System.Int32")},{2472, new PlayerKey(2472,"galienDefeated","System.Int32")},{2476, new PlayerKey(2476,"XERO_encountered","System.Boolean")},{2477, new PlayerKey(2477,"ALADAR_encountered","System.Boolean")},{2478, new PlayerKey(2478,"HU_encountered","System.Boolean")},{2479, new PlayerKey(2479,"MUMCAT_encountered","System.Boolean")},{2480, new PlayerKey(2480,"NOEYES_encountered","System.Boolean")},{2481, new PlayerKey(2481,"MARKOTH_encountered","System.Boolean")},{2482, new PlayerKey(2482,"GALIEN_encountered","System.Boolean")},{2483, new PlayerKey(2483,"xeroPinned","System.Boolean")},{2484, new PlayerKey(2484,"aladarPinned","System.Boolean")},{2485, new PlayerKey(2485,"huPinned","System.Boolean")},{2486, new PlayerKey(2486,"mumCaterpillarPinned","System.Boolean")},{2487, new PlayerKey(2487,"noEyesPinned","System.Boolean")},{2488, new PlayerKey(2488,"markothPinned","System.Boolean")},{2489, new PlayerKey(2489,"galienPinned","System.Boolean")},{2492, new PlayerKey(2492,"currentInvPane","System.Int32")},{2496, new PlayerKey(2496,"showGeoUI","System.Boolean")},{2497, new PlayerKey(2497,"showHealthUI","System.Boolean")},{2498, new PlayerKey(2498,"promptFocus","System.Boolean")},{2499, new PlayerKey(2499,"seenFocusTablet","System.Boolean")},{2500, new PlayerKey(2500,"seenDreamNailPrompt","System.Boolean")},{2501, new PlayerKey(2501,"isFirstGame","System.Boolean")},
					{2502, new PlayerKey(2502,"enteredTutorialFirstTime","System.Boolean")},{2503, new PlayerKey(2503,"isInvincible","System.Boolean")},{2504, new PlayerKey(2504,"infiniteAirJump","System.Boolean")},{2505, new PlayerKey(2505,"invinciTest","System.Boolean")},{2508, new PlayerKey(2508,"currentArea","System.Int32")},{2512, new PlayerKey(2512,"visitedDirtmouth","System.Boolean")},{2513, new PlayerKey(2513,"visitedCrossroads","System.Boolean")},{2514, new PlayerKey(2514,"visitedGreenpath","System.Boolean")},{2515, new PlayerKey(2515,"visitedFungus","System.Boolean")},{2516, new PlayerKey(2516,"visitedHive","System.Boolean")},{2517, new PlayerKey(2517,"visitedCrossroadsInfected","System.Boolean")},{2518, new PlayerKey(2518,"visitedRuins","System.Boolean")},{2519, new PlayerKey(2519,"visitedMines","System.Boolean")},{2520, new PlayerKey(2520,"visitedRoyalGardens","System.Boolean")},{2521, new PlayerKey(2521,"visitedFogCanyon","System.Boolean")},{2522, new PlayerKey(2522,"visitedDeepnest","System.Boolean")},{2523, new PlayerKey(2523,"visitedRestingGrounds","System.Boolean")},{2524, new PlayerKey(2524,"visitedWaterways","System.Boolean")},{2525, new PlayerKey(2525,"visitedAbyss","System.Boolean")},{2526, new PlayerKey(2526,"visitedOutskirts","System.Boolean")},{2527, new PlayerKey(2527,"visitedWhitePalace","System.Boolean")},{2528, new PlayerKey(2528,"visitedCliffs","System.Boolean")},{2529, new PlayerKey(2529,"visitedAbyssLower","System.Boolean")},{2530, new PlayerKey(2530,"visitedMines10","System.Boolean")},{48, new PlayerKey(48,"scenesVisited","System.Collections.Generic.List<System.String>")},
					{52, new PlayerKey(52,"scenesMapped","System.Collections.Generic.List<System.String>")},{56, new PlayerKey(56,"scenesEncounteredBench","System.Collections.Generic.List<System.String>")},{60, new PlayerKey(60,"scenesGrubRescued","System.Collections.Generic.List<System.String>")},{64, new PlayerKey(64,"scenesEncounteredCocoon","System.Collections.Generic.List<System.String>")},{68, new PlayerKey(68,"scenesEncounteredDreamPlant","System.Collections.Generic.List<System.String>")},{72, new PlayerKey(72,"scenesEncounteredDreamPlantC","System.Collections.Generic.List<System.String>")},{2531, new PlayerKey(2531,"hasMap","System.Boolean")},{2532, new PlayerKey(2532,"mapAllRooms","System.Boolean")},{2533, new PlayerKey(2533,"atMapPrompt","System.Boolean")},{2534, new PlayerKey(2534,"mapDirtmouth","System.Boolean")},{2535, new PlayerKey(2535,"mapCrossroads","System.Boolean")},{2536, new PlayerKey(2536,"mapGreenpath","System.Boolean")},{2537, new PlayerKey(2537,"mapFogCanyon","System.Boolean")},{2538, new PlayerKey(2538,"mapRoyalGardens","System.Boolean")},{2539, new PlayerKey(2539,"mapFungalWastes","System.Boolean")},{2540, new PlayerKey(2540,"mapCity","System.Boolean")},{2541, new PlayerKey(2541,"mapWaterways","System.Boolean")},{2542, new PlayerKey(2542,"mapMines","System.Boolean")},{2543, new PlayerKey(2543,"mapDeepnest","System.Boolean")},{2544, new PlayerKey(2544,"mapCliffs","System.Boolean")},{2545, new PlayerKey(2545,"mapOutskirts","System.Boolean")},{2546, new PlayerKey(2546,"mapRestingGrounds","System.Boolean")},{2547, new PlayerKey(2547,"mapAbyss","System.Boolean")},{2548, new PlayerKey(2548,"hasPin","System.Boolean")},{2549, new PlayerKey(2549,"hasPinBench","System.Boolean")},
					{2550, new PlayerKey(2550,"hasPinCocoon","System.Boolean")},{2551, new PlayerKey(2551,"hasPinDreamPlant","System.Boolean")},{2552, new PlayerKey(2552,"hasPinGuardian","System.Boolean")},{2553, new PlayerKey(2553,"hasPinBlackEgg","System.Boolean")},{2554, new PlayerKey(2554,"hasPinShop","System.Boolean")},{2555, new PlayerKey(2555,"hasPinSpa","System.Boolean")},{2556, new PlayerKey(2556,"hasPinStag","System.Boolean")},{2557, new PlayerKey(2557,"hasPinTram","System.Boolean")},{2558, new PlayerKey(2558,"hasPinGhost","System.Boolean")},{2559, new PlayerKey(2559,"hasPinGrub","System.Boolean")},{2560, new PlayerKey(2560,"environmentType","System.Int32")},{2564, new PlayerKey(2564,"environmentTypeDefault","System.Int32")},{2568, new PlayerKey(2568,"previousDarkness","System.Int32")},{2572, new PlayerKey(2572,"openedTramLower","System.Boolean")},{2573, new PlayerKey(2573,"openedTramRestingGrounds","System.Boolean")},{2576, new PlayerKey(2576,"tramLowerPosition","System.Int32")},{2580, new PlayerKey(2580,"tramRestingGroundsPosition","System.Int32")},{2584, new PlayerKey(2584,"mineLiftOpened","System.Boolean")},{2585, new PlayerKey(2585,"menderDoorOpened","System.Boolean")},{2586, new PlayerKey(2586,"vesselFragStagNest","System.Boolean")},{2587, new PlayerKey(2587,"shamanPillar","System.Boolean")},{2588, new PlayerKey(2588,"crossroadsMawlekWall","System.Boolean")},{2589, new PlayerKey(2589,"eggTempleVisited","System.Boolean")},{2590, new PlayerKey(2590,"crossroadsInfected","System.Boolean")},{2591, new PlayerKey(2591,"falseKnightFirstPlop","System.Boolean")},
					{2592, new PlayerKey(2592,"falseKnightWallRepaired","System.Boolean")},{2593, new PlayerKey(2593,"falseKnightWallBroken","System.Boolean")},{2594, new PlayerKey(2594,"falseKnightGhostDeparted","System.Boolean")},{2595, new PlayerKey(2595,"spaBugsEncountered","System.Boolean")},{2596, new PlayerKey(2596,"hornheadVinePlat","System.Boolean")},{2597, new PlayerKey(2597,"infectedKnightEncountered","System.Boolean")},{2598, new PlayerKey(2598,"megaMossChargerEncountered","System.Boolean")},{2599, new PlayerKey(2599,"megaMossChargerDefeated","System.Boolean")},{2600, new PlayerKey(2600,"dreamerScene1","System.Boolean")},{2601, new PlayerKey(2601,"slugEncounterComplete","System.Boolean")},{2602, new PlayerKey(2602,"defeatedDoubleBlockers","System.Boolean")},{2603, new PlayerKey(2603,"oneWayArchive","System.Boolean")},{2604, new PlayerKey(2604,"defeatedMegaJelly","System.Boolean")},{2605, new PlayerKey(2605,"summonedMonomon","System.Boolean")},{2606, new PlayerKey(2606,"sawWoundedQuirrel","System.Boolean")},{2607, new PlayerKey(2607,"encounteredMegaJelly","System.Boolean")},{2608, new PlayerKey(2608,"defeatedMantisLords","System.Boolean")},{2609, new PlayerKey(2609,"encounteredGatekeeper","System.Boolean")},{2610, new PlayerKey(2610,"deepnestWall","System.Boolean")},{2611, new PlayerKey(2611,"queensStationNonDisplay","System.Boolean")},{2612, new PlayerKey(2612,"cityBridge1","System.Boolean")},{2613, new PlayerKey(2613,"cityBridge2","System.Boolean")},{2614, new PlayerKey(2614,"cityLift1","System.Boolean")},{2615, new PlayerKey(2615,"cityLift1_isUp","System.Boolean")},{2616, new PlayerKey(2616,"liftArrival","System.Boolean")},
					{2617, new PlayerKey(2617,"openedMageDoor","System.Boolean")},{2618, new PlayerKey(2618,"openedMageDoor_v2","System.Boolean")},{2619, new PlayerKey(2619,"brokenMageWindow","System.Boolean")},{2620, new PlayerKey(2620,"brokenMageWindowGlass","System.Boolean")},{2621, new PlayerKey(2621,"mageLordEncountered","System.Boolean")},{2622, new PlayerKey(2622,"mageLordEncountered_2","System.Boolean")},{2623, new PlayerKey(2623,"mageLordDefeated","System.Boolean")},{2624, new PlayerKey(2624,"ruins1_5_tripleDoor","System.Boolean")},{2625, new PlayerKey(2625,"openedCityGate","System.Boolean")},{2626, new PlayerKey(2626,"cityGateClosed","System.Boolean")},{2627, new PlayerKey(2627,"bathHouseOpened","System.Boolean")},{2628, new PlayerKey(2628,"bathHouseWall","System.Boolean")},{2629, new PlayerKey(2629,"cityLift2","System.Boolean")},{2630, new PlayerKey(2630,"cityLift2_isUp","System.Boolean")},{2631, new PlayerKey(2631,"city2_sewerDoor","System.Boolean")},{2632, new PlayerKey(2632,"openedLoveDoor","System.Boolean")},{2633, new PlayerKey(2633,"watcherChandelier","System.Boolean")},{2634, new PlayerKey(2634,"completedQuakeArea","System.Boolean")},{2635, new PlayerKey(2635,"kingsStationNonDisplay","System.Boolean")},{2636, new PlayerKey(2636,"tollBenchCity","System.Boolean")},{2637, new PlayerKey(2637,"waterwaysGate","System.Boolean")},{2638, new PlayerKey(2638,"defeatedDungDefender","System.Boolean")},{2639, new PlayerKey(2639,"dungDefenderEncounterReady","System.Boolean")},{2640, new PlayerKey(2640,"flukeMotherEncountered","System.Boolean")},{2641, new PlayerKey(2641,"flukeMotherDefeated","System.Boolean")},
					{2642, new PlayerKey(2642,"openedWaterwaysManhole","System.Boolean")},{2643, new PlayerKey(2643,"waterwaysAcidDrained","System.Boolean")},{2644, new PlayerKey(2644,"dungDefenderWallBroken","System.Boolean")},{2645, new PlayerKey(2645,"dungDefenderSleeping","System.Boolean")},{2646, new PlayerKey(2646,"defeatedMegaBeamMiner","System.Boolean")},{2647, new PlayerKey(2647,"defeatedMegaBeamMiner2","System.Boolean")},{2648, new PlayerKey(2648,"brokeMinersWall","System.Boolean")},{2649, new PlayerKey(2649,"encounteredMimicSpider","System.Boolean")},{2650, new PlayerKey(2650,"steppedBeyondBridge","System.Boolean")},{2651, new PlayerKey(2651,"deepnestBridgeCollapsed","System.Boolean")},{2652, new PlayerKey(2652,"spiderCapture","System.Boolean")},{2653, new PlayerKey(2653,"openedRestingGrounds02","System.Boolean")},{2654, new PlayerKey(2654,"restingGroundsCryptWall","System.Boolean")},{2655, new PlayerKey(2655,"dreamNailConvo","System.Boolean")},{2656, new PlayerKey(2656,"openedGardensStagStation","System.Boolean")},{2657, new PlayerKey(2657,"extendedGramophone","System.Boolean")},{2658, new PlayerKey(2658,"tollBenchQueensGardens","System.Boolean")},{2659, new PlayerKey(2659,"blizzardEnded","System.Boolean")},{2660, new PlayerKey(2660,"encounteredHornet","System.Boolean")},{2661, new PlayerKey(2661,"savedByHornet","System.Boolean")},{2662, new PlayerKey(2662,"outskirtsWall","System.Boolean")},{2663, new PlayerKey(2663,"abyssGateOpened","System.Boolean")},{2664, new PlayerKey(2664,"abyssLighthouse","System.Boolean")},{2665, new PlayerKey(2665,"blueVineDoor","System.Boolean")},{2666, new PlayerKey(2666,"gotShadeCharm","System.Boolean")},
					{2667, new PlayerKey(2667,"tollBenchAbyss","System.Boolean")},{2668, new PlayerKey(2668,"fountainGeo","System.Int32")},{2672, new PlayerKey(2672,"fountainVesselSummoned","System.Boolean")},{2673, new PlayerKey(2673,"openedBlackEggPath","System.Boolean")},{2674, new PlayerKey(2674,"enteredDreamWorld","System.Boolean")},{2675, new PlayerKey(2675,"duskKnightDefeated","System.Boolean")},{2676, new PlayerKey(2676,"whitePalaceOrb_1","System.Boolean")},{2677, new PlayerKey(2677,"whitePalaceOrb_2","System.Boolean")},{2678, new PlayerKey(2678,"whitePalaceOrb_3","System.Boolean")},{2679, new PlayerKey(2679,"whitePalace05_lever","System.Boolean")},{2680, new PlayerKey(2680,"whitePalaceMidWarp","System.Boolean")},{2681, new PlayerKey(2681,"whitePalaceSecretRoomVisited","System.Boolean")},{2682, new PlayerKey(2682,"tramOpenedDeepnest","System.Boolean")},{2683, new PlayerKey(2683,"tramOpenedCrossroads","System.Boolean")},{2684, new PlayerKey(2684,"openedBlackEggDoor","System.Boolean")},{2685, new PlayerKey(2685,"unchainedHollowKnight","System.Boolean")},{2688, new PlayerKey(2688,"completionPercentage","System.Single")},{2692, new PlayerKey(2692,"disablePause","System.Boolean")},{2693, new PlayerKey(2693,"backerCredits","System.Boolean")},{2694, new PlayerKey(2694,"unlockedCompletionRate","System.Boolean")},{2696, new PlayerKey(2696,"mapKeyPref","System.Int32")},{76, new PlayerKey(76,"playerStory","System.Collections.Generic.List<System.String>")},{80, new PlayerKey(80,"playerStoryOutput","System.String")},{2700, new PlayerKey(2700,"betaEnd","System.Boolean")},{2701, new PlayerKey(2701,"newDatTraitorLord","System.Boolean")}};
		}

		public object GetValue(int offset) {
			PlayerKey key = null;
			if (data.TryGetValue(offset, out key)) {
				return key.Value;
			}
			return null;
		}
		public void UpdateData(Process program, byte[] newData, Action<string> logWriter) {
			foreach (KeyValuePair<int, PlayerKey> pair in data) {
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
					case "UnityEngine.Vector3": key.Value = new PointF(BitConverter.ToSingle(newData, key.Index), BitConverter.ToSingle(newData, key.Index + 4)); break;
					case "System.Single": key.Value = BitConverter.ToSingle(newData, key.Index); break;
					case "System.Int16": key.Value = BitConverter.ToInt16(newData, key.Index); break;
					case "System.Int64": key.Value = BitConverter.ToInt64(newData, key.Index); break;
					case "System.String": key.Value = program.Read((IntPtr)BitConverter.ToInt32(newData, key.Index)); break;
					case "System.Byte": key.Value = newData[key.Index]; break;
					case "System.Boolean": key.Value = newData[key.Index] == 1; break;
					default: key.Value = BitConverter.ToInt32(newData, key.Index); break;
				}

				bool changed = oldValue == null;
				if (!changed) {
					switch (key.Type) {
						case "UnityEngine.Vector3": changed = (PointF)oldValue != (PointF)key.Value; break;
						case "System.Single": changed = (float)oldValue != (float)key.Value; break;
						case "System.Int16": changed = (short)oldValue != (short)key.Value; break;
						case "System.Int64": changed = (long)oldValue != (long)key.Value; break;
						case "System.String": changed = (string)oldValue != (string)key.Value; break;
						case "System.Byte": changed = (byte)oldValue != (byte)key.Value; break;
						case "System.Boolean": changed = (bool)oldValue != (bool)key.Value; break;
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