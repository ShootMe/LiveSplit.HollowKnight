using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.HollowKnight {
	public class HollowKnightComponent : IComponent {
		public string ComponentName { get { return "Hollow Knight Autosplitter"; } }
		public TimerModel Model { get; set; }
		public IDictionary<string, Action> ContextMenuControls { get { return null; } }
		internal static string[] keys = { "CurrentSplit", "State", "GameState", "SceneName", "Charms", "CameraMode", "MenuState", "UIState", "AcceptingInput", "MapZone", "NextSceneName" };
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

			if (Model != null) {
				HandleSplits();
			}

			LogValues();
		}
		private void HandleSplits() {
			bool shouldSplit = false;

			if (currentSplit == -1) {
				shouldSplit = mem.MenuState() == MainMenuState.PLAY_MODE_MENU && mem.GameState() == GameState.MAIN_MENU && !mem.AcceptingInput();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				string nextScene = mem.NextSceneName();
				string sceneName = mem.SceneName();

				if (currentSplit + 1 < Model.CurrentState.Run.Count) {
					foreach (SplitName split in settings.Splits) {
						if (splitsDone.Contains(SplitName.InfectedCrossroads)) { continue; }

						switch (split) {
							case SplitName.AspidHunter: shouldSplit = mem.KilledAspidHunter(); break;
							case SplitName.BlackKnight: shouldSplit = mem.KilledBlackKnight(); break;
							case SplitName.CityOfTears: shouldSplit = mem.VisitedCityOfTears(); break;
							case SplitName.Collector: shouldSplit = mem.KilledCollector(); break;
							case SplitName.CrossroadsStation: shouldSplit = mem.CrossroadsStationOpened(); break;
							case SplitName.CrystalHeart: shouldSplit = mem.HasCrystalHeart(); break;
							case SplitName.CrystalPeak: shouldSplit = mem.VisitedCrystalPeak(); break;
							case SplitName.CycloneSlash: shouldSplit = mem.HasCycloneSlash(); break;
							case SplitName.DashSlash: shouldSplit = mem.HasDashSlash(); break;
							case SplitName.Deepnest: shouldSplit = mem.VisitedDeepnest(); break;
							case SplitName.DeepnestSpa: shouldSplit = mem.VisitedDeepnestSpa(); break;
							case SplitName.DeepnestStation: shouldSplit = mem.DeepnestStationOpened(); break;
							case SplitName.DefendersCrest: shouldSplit = mem.HasDefendersCrest(); break;
							case SplitName.DescendingDark: shouldSplit = mem.DesolateDiveLevel() == 2; break;
							case SplitName.DesolateDive: shouldSplit = mem.DesolateDiveLevel() == 1; break;
							case SplitName.DreamNail: shouldSplit = mem.HasDreamNail(); break;
							case SplitName.DreamNail2: shouldSplit = mem.HasDreamNail2(); break;
							case SplitName.DungDefender: shouldSplit = mem.KilledDungDefender(); break;
							case SplitName.ElderHu: shouldSplit = mem.KilledElderHu(); break;
							case SplitName.FailedKnight: shouldSplit = mem.KilledFailedKnight(); break;
							case SplitName.FalseKnight: shouldSplit = mem.KilledFalseKnight(); break;
							case SplitName.FogCanyon: shouldSplit = mem.VisitedFogCanyon(); break;
							case SplitName.ForgottenCrossroads: shouldSplit = mem.VisitedCrossroads(); break;
							case SplitName.FragileStrength: shouldSplit = mem.HasFragileStrength(); break;
							case SplitName.FungalWastes: shouldSplit = mem.VisitedFungalWastes(); break;
							case SplitName.Galien: shouldSplit = mem.KilledGalien(); break;
							case SplitName.Gorb: shouldSplit = mem.KilledGorb(); break;
							case SplitName.GreatSlash: shouldSplit = mem.HasGreatSlash(); break;
							case SplitName.Greenpath: shouldSplit = mem.VisitedGreenpath(); break;
							case SplitName.GruzMother: shouldSplit = mem.KilledGruzMother(); break;
							case SplitName.Hegemol: shouldSplit = mem.Hegemol(); break;
							case SplitName.Hive: shouldSplit = mem.VisitedHive(); break;
							case SplitName.HollowKnight: shouldSplit = mem.KilledHollowKnight(); break;
							case SplitName.Hornet1: shouldSplit = mem.KilledHornet(); break;
							case SplitName.Hornet2: shouldSplit = mem.KilledHornet2(); break;
							case SplitName.InfectedCrossroads: shouldSplit = mem.CrossroadsInfected() && mem.VisitedCrossroads(); break;
							case SplitName.IsmasTear: shouldSplit = mem.HasIsmasTear(); break;
							case SplitName.KingsBrand: shouldSplit = mem.HasKingsBrand(); break;
							case SplitName.KingsStationStation: shouldSplit = mem.KingsStationOpened(); break;
							case SplitName.LumaflyLantern: shouldSplit = mem.HasLumaflyLantern(); break;
							case SplitName.Lurien: shouldSplit = mem.Lurien(); break;
							case SplitName.MantisClaw: shouldSplit = mem.HasMantisClaw(); break;
							case SplitName.Markoth: shouldSplit = mem.KilledMarkoth(); break;
							case SplitName.Marmu: shouldSplit = mem.KilledMarmu(); break;
							case SplitName.MonarchWings: shouldSplit = mem.HasMonarchWings(); break;
							case SplitName.Monomon: shouldSplit = mem.Monomon(); break;
							case SplitName.MossKnight: shouldSplit = mem.KilledMossKnight(); break;
							case SplitName.MothwingCloak: shouldSplit = mem.HasMothwingCloak(); break;
							case SplitName.MushroomBrawler: shouldSplit = mem.KilledMushroomBrawler(); break;
							case SplitName.NailUpgrade1: shouldSplit = mem.NailUpgrades() == 1; break;
							case SplitName.NailUpgrade2: shouldSplit = mem.NailUpgrades() == 2; break;
							case SplitName.NailUpgrade3: shouldSplit = mem.NailUpgrades() == 3; break;
							case SplitName.NailUpgrade4: shouldSplit = mem.NailUpgrades() == 4; break;
							case SplitName.NoEyes: shouldSplit = mem.KilledNoEyes(); break;
							case SplitName.QueensGardens: shouldSplit = mem.VisitedQueensGardens(); break;
							case SplitName.QueensStationStation: shouldSplit = mem.QueensStationOpened(); break;
							case SplitName.Radiance: shouldSplit = mem.KilledRadiance(); break;
							case SplitName.RestingGrounds: shouldSplit = mem.VisitedRestingGrounds(); break;
							case SplitName.RoyalWaterways: shouldSplit = mem.VisitedRoyalWaterways(); break;
							case SplitName.ShadeCloak: shouldSplit = mem.HasShadeCloak(); break;
							case SplitName.ShadeSoul: shouldSplit = mem.HasShadeSoul(); break;
							case SplitName.SoulCatcher: shouldSplit = mem.HasSoulCatcher(); break;
							case SplitName.SoulMaster: shouldSplit = mem.KilledSoulMaster(); break;
							case SplitName.SoulTyrant: shouldSplit = mem.KilledSoulTyrant(); break;
							case SplitName.TeachersArchive: shouldSplit = sceneName.Equals("Fungus3_archive", StringComparison.OrdinalIgnoreCase); break;
							case SplitName.ThornsOfAgony: shouldSplit = mem.HasThornsOfAgony(); break;
							case SplitName.TramPass: shouldSplit = mem.HasTramPass(); break;
							case SplitName.Uumuu: shouldSplit = mem.KilledUumuu(); break;
							case SplitName.VengefulSpirit: shouldSplit = mem.HasVengefulSpirit(); break;
							case SplitName.WhitePalace: shouldSplit = mem.VisitedWhitePalace(); break;
							case SplitName.Xero: shouldSplit = mem.KilledXero(); break;
						}

						if (shouldSplit) {
							splitsDone.Add(split);
							break;
						}
					}
				} else {
					shouldSplit = nextScene.Equals("Cinematic_Ending_A", StringComparison.OrdinalIgnoreCase);
				}

				GameState gameState = mem.GameState();
				Model.CurrentState.IsGameTimePaused = gameState == GameState.LOADING || (!string.IsNullOrEmpty(nextScene) && nextScene != sceneName);
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
		private void LogValues() {
			if (lastLogCheck == 0) {
				hasLog = File.Exists(LOGFILE);
				lastLogCheck = 300;
			}
			lastLogCheck--;

			if (hasLog || !Console.IsOutputRedirected) {
				if (mem.UIState() == UIState.PLAYING) {
					byte[] playerData = mem.GetPlayerData();
					pdata.UpdateData(mem.Program, playerData, WriteLogWithTime);
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
						case "MapZone": curr = mem.MapZone().ToString(); break;
						case "CameraMode": curr = mem.CameraMode().ToString(); break;
						case "MenuState": curr = mem.MenuState().ToString(); break;
						case "UIState": curr = mem.UIState().ToString(); break;
						case "AcceptingInput": curr = mem.AcceptingInput().ToString(); break;
						default: curr = ""; break;
					}

					if (!prev.Equals(curr)) {
						WriteLogWithTime(key + ": ".PadRight(16 - key.Length, ' ') + prev.PadLeft(25, ' ') + " -> " + curr);

						currentValues[key] = curr;
					}
				}
			}
		}

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
			WriteLog(DateTime.Now.ToString(@"HH\:mm\:ss.fff") + (Model != null && Model.CurrentState.CurrentTime.RealTime.HasValue ? " | " + Model.CurrentState.CurrentTime.RealTime.Value.ToString("G").Substring(3, 11) : "") + ": " + data);
		}

		public Control GetSettingsControl(LayoutMode mode) { return settings; }
		public void SetSettings(XmlNode document) { settings.SetSettings(document); }
		public XmlNode GetSettings(XmlDocument document) { return settings.UpdateSettings(document); }
		public void DrawHorizontal(Graphics g, LiveSplitState state, float height, Region clipRegion) { }
		public void DrawVertical(Graphics g, LiveSplitState state, float width, Region clipRegion) { }
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