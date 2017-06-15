#if !Info
using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
#endif
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.HollowKnight {
#if !Info
	public class HollowKnightComponent : IComponent {
		public TimerModel Model { get; set; }
#else
	public class HollowKnightComponent {
#endif
		public string ComponentName { get { return "Hollow Knight Autosplitter"; } }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		internal static string[] keys = { "CurrentSplit", "State", "GameState", "SceneName", "Charms", "CameraMode", "MenuState", "UIState", "AcceptingInput", "MapZone", "NextSceneName", "ActorState" };
		private HollowKnightMemory mem;
		private int currentSplit = -1, state = 0, lastLogCheck = 0;
		private bool hasLog = false;
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		private HollowKnightSettings settings;
		private HashSet<SplitName> splitsDone = new HashSet<SplitName>();
		private static string LOGFILE = "_HollowKnight.log";
		private PlayerData pdata = new PlayerData();
		public HollowKnightComponent() {
			mem = new HollowKnightMemory();
			settings = new HollowKnightSettings();
			foreach (string key in keys) {
				currentValues[key] = "";
			}
		}

		public void GetValues() {
			if (!mem.HookProcess()) { return; }

#if !Info
			if (Model != null) {
				HandleSplits();
			}
#endif

			LogValues();
		}
#if !Info
		private void HandleSplits() {
			bool shouldSplit = false;

			if (currentSplit == -1) {
				shouldSplit = mem.MenuState() == MainMenuState.PLAY_MODE_MENU && mem.GameState() == GameState.MAIN_MENU && !mem.AcceptingInput();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				string nextScene = mem.NextSceneName();
				string sceneName = mem.SceneName();

				if (currentSplit + 1 < Model.CurrentState.Run.Count) {
					foreach (SplitName split in settings.Splits) {
						if (splitsDone.Contains(split)) { continue; }

						switch (split) {
							case SplitName.AbyssShriek: shouldSplit = mem.PlayerData<int>(Offset.screamLevel) == 2; break;
							case SplitName.AspidHunter: shouldSplit = mem.PlayerData<int>(Offset.killsSpitter) == 17; break;
							case SplitName.BaldurShell: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_5); break;
							case SplitName.BlackKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedBlackKnight); break;
							case SplitName.BrokenVessel: shouldSplit = mem.PlayerData<bool>(Offset.killedInfectedKnight); break;
							case SplitName.BroodingMawlek: shouldSplit = mem.PlayerData<bool>(Offset.killedMawlek); break;
							case SplitName.CityOfTears: shouldSplit = mem.PlayerData<bool>(Offset.visitedRuins); break;
							case SplitName.Collector: shouldSplit = mem.PlayerData<bool>(Offset.collectorDefeated); break;
							case SplitName.ColosseumBronze: shouldSplit = mem.PlayerData<bool>(Offset.colosseumBronzeCompleted); break;
							case SplitName.ColosseumGold: shouldSplit = mem.PlayerData<bool>(Offset.colosseumGoldCompleted); break;
							case SplitName.ColosseumSilver: shouldSplit = mem.PlayerData<bool>(Offset.colosseumSilverCompleted); break;
							case SplitName.CrossroadsStation: shouldSplit = mem.PlayerData<bool>(Offset.openedCrossroads); break;
							case SplitName.CrystalGuardian1: shouldSplit = mem.PlayerData<bool>(Offset.defeatedMegaBeamMiner); break;
							case SplitName.CrystalGuardian2: shouldSplit = mem.PlayerData<int>(Offset.killsMegaBeamMiner) == 0; break;
							case SplitName.CrystalHeart: shouldSplit = mem.PlayerData<bool>(Offset.hasSuperDash); break;
							case SplitName.CrystalPeak: shouldSplit = mem.PlayerData<bool>(Offset.visitedMines); break;
							case SplitName.CycloneSlash: shouldSplit = mem.PlayerData<bool>(Offset.hasCyclone); break;
							case SplitName.Dashmaster: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_31); break;
							case SplitName.DashSlash: shouldSplit = mem.PlayerData<bool>(Offset.hasDashSlash); break;
							case SplitName.DeepFocus: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_34); break;
							case SplitName.Deepnest: shouldSplit = mem.PlayerData<bool>(Offset.visitedDeepnest); break;
							case SplitName.DeepnestSpa: shouldSplit = mem.PlayerData<bool>(Offset.visitedDeepnestSpa); break;
							case SplitName.DeepnestStation: shouldSplit = mem.PlayerData<bool>(Offset.openedDeepnest); break;
							case SplitName.DefendersCrest: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_10); break;
							case SplitName.DescendingDark: shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 2; break;
							case SplitName.DesolateDive: shouldSplit = mem.PlayerData<int>(Offset.quakeLevel) == 1; break;
							case SplitName.DreamNail: shouldSplit = mem.PlayerData<bool>(Offset.hasDreamNail); break;
							case SplitName.DreamNail2: shouldSplit = mem.PlayerData<bool>(Offset.dreamNailUpgraded); break;
							case SplitName.DreamWielder: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_30); break;
							case SplitName.DungDefender: shouldSplit = mem.PlayerData<bool>(Offset.killedDungDefender); break;
							case SplitName.ElderHu: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostHu); break;
							case SplitName.FailedKnight: shouldSplit = mem.PlayerData<bool>(Offset.falseKnightDreamDefeated); break;
							case SplitName.FalseKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedFalseKnight); break;
							case SplitName.Flukemarm: shouldSplit = mem.PlayerData<bool>(Offset.killedFlukeMother); break;
							case SplitName.Flukenest: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_11); break;
							case SplitName.FogCanyon: shouldSplit = mem.PlayerData<bool>(Offset.visitedFogCanyon); break;
							case SplitName.ForgottenCrossroads: shouldSplit = mem.PlayerData<bool>(Offset.visitedCrossroads); break;
							case SplitName.FragileGreed: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_24); break;
							case SplitName.FragileHeart: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_23); break;
							case SplitName.FragileStrength: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_25); break;
							case SplitName.FungalWastes: shouldSplit = mem.PlayerData<bool>(Offset.visitedFungus); break;
							case SplitName.FuryOfTheFallen: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_6); break;
							case SplitName.Galien: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostGalien); break;
							case SplitName.GatheringSwarm: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_1); break;
							case SplitName.GlowingWomb: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_22); break;
							case SplitName.Gorb: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostAladar); break;
							case SplitName.GreatSlash: shouldSplit = mem.PlayerData<bool>(Offset.hasUpwardSlash); break;
							case SplitName.Greenpath: shouldSplit = mem.PlayerData<bool>(Offset.visitedGreenpath); break;
							case SplitName.GrubberflysElegy: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_35); break;
							case SplitName.Grubsong: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_3); break;
							case SplitName.GruzMother: shouldSplit = mem.PlayerData<bool>(Offset.killedBigFly); break;
							case SplitName.HeavyBlow: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_15); break;
							case SplitName.Hegemol: shouldSplit = mem.PlayerData<bool>(Offset.hegemolDefeated); break;
							case SplitName.Hive: shouldSplit = mem.PlayerData<bool>(Offset.visitedHive); break;
							case SplitName.Hiveblood: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_29); break;
							case SplitName.HollowKnight: shouldSplit = nextScene.Equals("Dream_Final_Boss", StringComparison.OrdinalIgnoreCase); break;
							case SplitName.Hornet1: shouldSplit = mem.PlayerData<bool>(Offset.killedHornet); break;
							case SplitName.Hornet2: shouldSplit = mem.PlayerData<bool>(Offset.hornetOutskirtsDefeated); break;
							case SplitName.HowlingWraiths: shouldSplit = mem.PlayerData<int>(Offset.screamLevel) == 1; break;
							case SplitName.InfectedCrossroads: shouldSplit = mem.PlayerData<bool>(Offset.crossroadsInfected) && mem.PlayerData<bool>(Offset.visitedCrossroads); break;
							case SplitName.IsmasTear: shouldSplit = mem.PlayerData<bool>(Offset.hasAcidArmour); break;
							case SplitName.JonisBlessing: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_27); break;
							case SplitName.KingsBrand: shouldSplit = mem.PlayerData<bool>(Offset.hasKingsBrand); break;
							case SplitName.Kingsoul: shouldSplit = mem.PlayerData<int>(Offset.charmCost_36) == 5; break;
							case SplitName.KingsStationStation: shouldSplit = mem.PlayerData<bool>(Offset.openedRuins2); break;
							case SplitName.LifebloodCore: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_9); break;
							case SplitName.LifebloodHeart: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_8); break;
							case SplitName.Longnail: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_18); break;
							case SplitName.LostKin: shouldSplit = mem.PlayerData<bool>(Offset.infectedKnightDreamDefeated); break;
							case SplitName.LoveKey: shouldSplit = mem.PlayerData<bool>(Offset.hasLoveKey); break;
							case SplitName.LumaflyLantern: shouldSplit = mem.PlayerData<bool>(Offset.hasLantern); break;
							case SplitName.Lurien: shouldSplit = mem.PlayerData<bool>(Offset.lurienDefeated); break;
							case SplitName.MantisClaw: shouldSplit = mem.PlayerData<bool>(Offset.hasWallJump); break;
							case SplitName.MantisLords: shouldSplit = mem.PlayerData<bool>(Offset.defeatedMantisLords); break;
							case SplitName.MarkOfPride: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_13); break;
							case SplitName.Markoth: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostMarkoth); break;
							case SplitName.Marmu: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostMarmu); break;
							case SplitName.Mask1: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 6; break;
							case SplitName.Mask2: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 7; break;
							case SplitName.Mask3: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 8; break;
							case SplitName.Mask4: shouldSplit = mem.PlayerData<int>(Offset.maxHealthBase) == 9; break;
							case SplitName.MegaMossCharger: shouldSplit = mem.PlayerData<bool>(Offset.megaMossChargerDefeated); break;
							case SplitName.MonarchWings: shouldSplit = mem.PlayerData<bool>(Offset.hasDoubleJump); break;
							case SplitName.Monomon: shouldSplit = mem.PlayerData<bool>(Offset.monomonDefeated); break;
							case SplitName.MossKnight: shouldSplit = mem.PlayerData<bool>(Offset.killedMossKnight); break;
							case SplitName.MothwingCloak: shouldSplit = mem.PlayerData<bool>(Offset.hasDash); break;
							case SplitName.MushroomBrawler: shouldSplit = mem.PlayerData<int>(Offset.killsMushroomBrawler) == 6; break;
							case SplitName.NailmastersGlory: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_26); break;
							case SplitName.NailUpgrade1: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 1; break;
							case SplitName.NailUpgrade2: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 2; break;
							case SplitName.NailUpgrade3: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 3; break;
							case SplitName.NailUpgrade4: shouldSplit = mem.PlayerData<int>(Offset.nailSmithUpgrades) == 4; break;
							case SplitName.NoEyes: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostNoEyes); break;
							case SplitName.Nosk: shouldSplit = mem.PlayerData<bool>(Offset.killedMimicSpider); break;
							case SplitName.NotchFogCanyon: shouldSplit = mem.PlayerData<bool>(Offset.notchFogCanyon); break;
							case SplitName.NotchSalubra1: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch1); break;
							case SplitName.NotchSalubra2: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch2); break;
							case SplitName.NotchSalubra3: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch3); break;
							case SplitName.NotchSalubra4: shouldSplit = mem.PlayerData<bool>(Offset.salubraNotch4); break;
							case SplitName.NotchShrumalOgres: shouldSplit = mem.PlayerData<bool>(Offset.notchShroomOgres); break;
							case SplitName.PaleOre: shouldSplit = mem.PlayerData<int>(Offset.ore) > 0; break;
							case SplitName.QueensGardens: shouldSplit = mem.PlayerData<bool>(Offset.visitedRoyalGardens); break;
							case SplitName.QueensStationStation: shouldSplit = mem.PlayerData<bool>(Offset.openedFungalWastes); break;
							case SplitName.QuickSlash: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_32); break;
							case SplitName.QuickFocus: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_7); break;
							case SplitName.RestingGrounds: shouldSplit = mem.PlayerData<bool>(Offset.visitedRestingGrounds); break;
							case SplitName.RoyalWaterways: shouldSplit = mem.PlayerData<bool>(Offset.visitedWaterways); break;
							case SplitName.SeerDeparts: shouldSplit = mem.PlayerData<bool>(Offset.mothDeparted); break;
							case SplitName.ShadeCloak: shouldSplit = mem.PlayerData<bool>(Offset.hasShadowDash); break;
							case SplitName.ShadeSoul: shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 2; break;
							case SplitName.ShamanStone: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_19); break;
							case SplitName.ShapeOfUnn: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_28); break;
							case SplitName.SharpShadow: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_16); break;
							case SplitName.SimpleKey: shouldSplit = mem.PlayerData<int>(Offset.simpleKeys) > 0; break;
							case SplitName.SoulCatcher: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_20); break;
							case SplitName.SoulEater: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_21); break;
							case SplitName.SoulMaster: shouldSplit = mem.PlayerData<bool>(Offset.killedMageLord); break;
							case SplitName.SoulTyrant: shouldSplit = mem.PlayerData<bool>(Offset.mageLordDreamDefeated); break;
							case SplitName.SpellTwister: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_33); break;
							case SplitName.SporeShroom: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_17); break;
							case SplitName.StalwartShell: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_4); break;
							case SplitName.SteadyBody: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_14); break;
							case SplitName.TeachersArchive: shouldSplit = sceneName.Equals("Fungus3_archive", StringComparison.OrdinalIgnoreCase); break;
							case SplitName.ThornsOfAgony: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_12); break;
							case SplitName.TraitorLord: shouldSplit = mem.PlayerData<bool>(Offset.killedTraitorLord); break;
							case SplitName.TramPass: shouldSplit = mem.PlayerData<bool>(Offset.hasTramPass); break;
							case SplitName.Uumuu: shouldSplit = mem.PlayerData<bool>(Offset.killedMegaJellyfish); break;
							case SplitName.VengefulSpirit: shouldSplit = mem.PlayerData<int>(Offset.fireballLevel) == 1; break;
							case SplitName.Vessel1: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 33; break;
							case SplitName.Vessel2: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 66; break;
							case SplitName.Vessel3: shouldSplit = mem.PlayerData<int>(Offset.MPReserveMax) == 99; break;
							case SplitName.VoidHeart: shouldSplit = mem.PlayerData<bool>(Offset.gotShadeCharm); break;
							case SplitName.WaywardCompass: shouldSplit = mem.PlayerData<bool>(Offset.gotCharm_2); break;
							case SplitName.WhitePalace: shouldSplit = mem.PlayerData<bool>(Offset.visitedWhitePalace); break;
							case SplitName.Xero: shouldSplit = mem.PlayerData<bool>(Offset.killedGhostXero); break;
							case SplitName.Zote1: shouldSplit = mem.PlayerData<bool>(Offset.zoteRescuedBuzzer); break;
							case SplitName.Zote2: shouldSplit = mem.PlayerData<bool>(Offset.zoteRescuedDeepnest); break;
						}

						if (shouldSplit) {
							splitsDone.Add(split);
							break;
						}
					}
				} else {
					shouldSplit = nextScene.StartsWith("Cinematic_Ending", StringComparison.OrdinalIgnoreCase);
				}

				GameState gameState = mem.GameState();
				Model.CurrentState.IsGameTimePaused = ((gameState == GameState.PLAYING || gameState == GameState.ENTERING_LEVEL) && mem.UIState() != UIState.PLAYING) || (gameState != GameState.PLAYING && !mem.AcceptingInput()) || gameState == GameState.EXITING_LEVEL || gameState == GameState.LOADING || mem.HeroTransitionState() == HeroTransitionState.WAITING_TO_ENTER_LEVEL || ((!string.IsNullOrEmpty(nextScene) || sceneName == "_test_charms") && nextScene != sceneName);
			}

			HandleSplit(shouldSplit);
		}
		private void HandleSplit(bool shouldSplit, bool shouldReset = false) {
			if (shouldReset) {
				if (currentSplit >= 0) {
					Model.Reset();
				}
			} else if (shouldSplit) {
				if (currentSplit < 0) {
					Model.Start();
				} else {
					Model.Split();
				}
			}
		}
#endif
		private void LogValues() {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				if (mem.UIState() == UIState.PLAYING) {
					pdata.UpdateData(mem, WriteLogWithTime);
				}

				string prev = "", curr = "";
				foreach (string key in keys) {
					prev = currentValues[key];

					switch (key) {
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "State": curr = state.ToString(); break;
						case "GameState": curr = mem.GameState().ToString(); break;
						case "SceneName": curr = mem.SceneName(); break;
						case "NextSceneName": curr = mem.NextSceneName(); break;
						case "Charms": curr = mem.CharmCount().ToString(); break;
						case "MapZone": curr = ((MapZone)mem.PlayerData<int>(Offset.mapZone)).ToString(); break;
						case "CameraMode": curr = mem.CameraMode().ToString(); break;
						case "MenuState": curr = mem.MenuState().ToString(); break;
						case "UIState": curr = mem.UIState().ToString(); break;
						case "AcceptingInput": curr = mem.AcceptingInput().ToString(); break;
						case "ActorState": curr = mem.HeroActorState().ToString(); break;
						default: curr = ""; break;
					}

					if (!prev.Equals(curr)) {
						WriteLogWithTime(key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}
		private void WriteLog(string data) {
			if (hasLog || !Console.IsOutputRedirected) {
				if (!Console.IsOutputRedirected) {
					Console.WriteLine(data);
				}
				if (hasLog) {
					using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
						wr.WriteLine(data);
					}
				}
			}
		}
		private void WriteLogWithTime(string data) {
#if !Info
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null && Model.CurrentState.CurrentTime.RealTime.HasValue ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + data);
#else
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + ": " + data);
#endif
		}

#if !Info
		public void Update(IInvalidator invalidator, LiveSplitState lvstate, float width, float height, LayoutMode mode) {
			if (Model == null) {
				Model = new TimerModel() { CurrentState = lvstate };
				Model.InitializeGameTime();
				Model.CurrentState.IsGameTimePaused = true;
				lvstate.OnReset += OnReset;
				lvstate.OnPause += OnPause;
				lvstate.OnResume += OnResume;
				lvstate.OnStart += OnStart;
				lvstate.OnSplit += OnSplit;
				lvstate.OnUndoSplit += OnUndoSplit;
				lvstate.OnSkipSplit += OnSkipSplit;
			}

			GetValues();
		}

		public void OnReset(object sender, TimerPhase e) {
			currentSplit = -1;
			state = 0;
			Model.CurrentState.IsGameTimePaused = true;
			splitsDone.Clear();
			WriteLog("---------Reset----------------------------------");
		}
		public void OnResume(object sender, EventArgs e) {
			WriteLog("---------Resumed--------------------------------");
		}
		public void OnPause(object sender, EventArgs e) {
			WriteLog("---------Paused---------------------------------");
		}
		public void OnStart(object sender, EventArgs e) {
			currentSplit = 0;
			state = 0;
			Model.CurrentState.IsGameTimePaused = false;
			splitsDone.Clear();
			WriteLog("---------New Game-------------------------------");
		}
		public void OnUndoSplit(object sender, EventArgs e) {
			currentSplit--;
			state = 0;
		}
		public void OnSkipSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
		}
		public void OnSplit(object sender, EventArgs e) {
			currentSplit++;
			state = 0;
		}
		public Control GetSettingsControl(LayoutMode mode) { return settings; }
		public void SetSettings(XmlNode document) { settings.SetSettings(document); }
		public XmlNode GetSettings(XmlDocument document) { return settings.UpdateSettings(document); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
#endif
		public float HorizontalWidth { get { return 0; } }
		public float MinimumHeight { get { return 0; } }
		public float MinimumWidth { get { return 0; } }
		public float PaddingBottom { get { return 0; } }
		public float PaddingLeft { get { return 0; } }
		public float PaddingRight { get { return 0; } }
		public float PaddingTop { get { return 0; } }
		public float VerticalHeight { get { return 0; } }
		public void Dispose() { }
	}
}