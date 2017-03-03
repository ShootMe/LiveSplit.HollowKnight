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
				if (currentSplit + 1 < Model.CurrentState.Run.Count) {
					if (settings.HasSplit(SplitName.ForgottenCrossroads) && !splitsDone.Contains(SplitName.ForgottenCrossroads) && mem.VisitedCrossroads()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.ForgottenCrossroads);
					} else if (settings.HasSplit(SplitName.FlyingSpitter) && !splitsDone.Contains(SplitName.FlyingSpitter) && mem.KilledFlyingSpitter()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.FlyingSpitter);
					} else if (settings.HasSplit(SplitName.CrossroadsStation) && !splitsDone.Contains(SplitName.CrossroadsStation) && mem.CrossroadsStationOpened()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.CrossroadsStation);
					} else if (settings.HasSplit(SplitName.FalseKnight) && !splitsDone.Contains(SplitName.FalseKnight) && mem.KilledFalseKnight()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.FalseKnight);
					} else if (settings.HasSplit(SplitName.SoulBlast) && !splitsDone.Contains(SplitName.SoulBlast) && mem.HasSoulSpell()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.SoulBlast);
					} else if (settings.HasSplit(SplitName.Greenpath) && !splitsDone.Contains(SplitName.Greenpath) && mem.VisitedGreenpath()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.Greenpath);
					} else if (settings.HasSplit(SplitName.MossKnight) && !splitsDone.Contains(SplitName.MossKnight) && mem.KilledMossKnight()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.MossKnight);
					} else if (settings.HasSplit(SplitName.Hornet1) && !splitsDone.Contains(SplitName.Hornet1) && mem.KilledHornet()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.Hornet1);
					} else if (settings.HasSplit(SplitName.MothwingCloak) && !splitsDone.Contains(SplitName.MothwingCloak) && mem.MothwingCloak()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.MothwingCloak);
					} else if (settings.HasSplit(SplitName.ThornsOfAgony) && !splitsDone.Contains(SplitName.ThornsOfAgony) && mem.ThornsOfAgony()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.ThornsOfAgony);
					} else if (settings.HasSplit(SplitName.FogCanyon) && !splitsDone.Contains(SplitName.FogCanyon) && mem.VisitedFogCanyon()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.FogCanyon);
					} else if (settings.HasSplit(SplitName.QueensStationStation) && !splitsDone.Contains(SplitName.QueensStationStation) && mem.QueensStationOpened()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.QueensStationStation);
					} else if (settings.HasSplit(SplitName.FungalWastes) && !splitsDone.Contains(SplitName.FungalWastes) && mem.VisitedFungalWastes()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.FungalWastes);
					} else if (settings.HasSplit(SplitName.MushroomBrawler) && !splitsDone.Contains(SplitName.MushroomBrawler) && mem.KilledMushroomBrawler()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.MushroomBrawler);
					} else if (settings.HasSplit(SplitName.MantisClaw) && !splitsDone.Contains(SplitName.MantisClaw) && mem.MantisClaw()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.MantisClaw);
					} else if (settings.HasSplit(SplitName.Deepnest) && !splitsDone.Contains(SplitName.Deepnest) && mem.VisitedDeepnest()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.Deepnest);
					} else if (settings.HasSplit(SplitName.DeepnestSpa) && !splitsDone.Contains(SplitName.DeepnestSpa) && mem.VisitedDeepnestSpa()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.DeepnestSpa);
					} else if (settings.HasSplit(SplitName.DeepnestStation) && !splitsDone.Contains(SplitName.DeepnestStation) && mem.DeepnestStationOpened()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.DeepnestStation);
					} else if (settings.HasSplit(SplitName.CrystalPeak) && !splitsDone.Contains(SplitName.CrystalPeak) && mem.VisitedCrystalPeak()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.CrystalPeak);
					} else if (settings.HasSplit(SplitName.CrystalHeart) && !splitsDone.Contains(SplitName.CrystalHeart) && mem.CrystalHeart()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.CrystalHeart);
					} else if (settings.HasSplit(SplitName.GruzMother) && !splitsDone.Contains(SplitName.GruzMother) && mem.KilledGruzMother()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.GruzMother);
					} else if (settings.HasSplit(SplitName.DreamNail) && !splitsDone.Contains(SplitName.DreamNail) && mem.HasDreamNail()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.DreamNail);
					} else if (settings.HasSplit(SplitName.RestingGrounds) && !splitsDone.Contains(SplitName.RestingGrounds) && mem.VisitedRestingGrounds()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.RestingGrounds);
					} else if (settings.HasSplit(SplitName.CityOfTears) && !splitsDone.Contains(SplitName.CityOfTears) && mem.VisitedCityOfTears()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.CityOfTears);
					} else if (settings.HasSplit(SplitName.NailUpgrade1) && !splitsDone.Contains(SplitName.NailUpgrade1) && mem.NailDamage() == 9) {
						shouldSplit = true;
						splitsDone.Add(SplitName.NailUpgrade1);
					} else if (settings.HasSplit(SplitName.BlackKnight) && !splitsDone.Contains(SplitName.BlackKnight) && mem.KilledWatcherKnight()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.BlackKnight);
					} else if (settings.HasSplit(SplitName.Lurien) && !splitsDone.Contains(SplitName.Lurien) && mem.Lurien()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.Lurien);
					} else if (settings.HasSplit(SplitName.KingsStation) && !splitsDone.Contains(SplitName.KingsStation) && mem.KingsStationOpened()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.KingsStation);
					} else if (settings.HasSplit(SplitName.Hegemol) && !splitsDone.Contains(SplitName.Hegemol) && mem.Hegemol()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.Hegemol);
					} else if (settings.HasSplit(SplitName.TeachersArchive) && !splitsDone.Contains(SplitName.TeachersArchive) && mem.SceneName().Equals("Fungus3_archive", StringComparison.OrdinalIgnoreCase)) {
						shouldSplit = true;
						splitsDone.Add(SplitName.TeachersArchive);
					} else if (settings.HasSplit(SplitName.Uumuu) && !splitsDone.Contains(SplitName.Uumuu) && mem.KilledUumuu()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.Uumuu);
					} else if (settings.HasSplit(SplitName.Monomon) && !splitsDone.Contains(SplitName.Monomon) && mem.Monomon()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.Monomon);
					} else if (settings.HasSplit(SplitName.InfectedCrossroads) && !splitsDone.Contains(SplitName.InfectedCrossroads) && mem.CrossroadsInfected() && mem.VisitedCrossroads()) {
						shouldSplit = true;
						splitsDone.Add(SplitName.InfectedCrossroads);
					}
				} else {
					string nextScene = mem.NextSceneName();
					shouldSplit = nextScene.Equals("Cinematic_Ending_A", StringComparison.OrdinalIgnoreCase);
				}

				GameState gameState = mem.GameState();
				Model.CurrentState.IsGameTimePaused = gameState != GameState.PLAYING && gameState != GameState.PAUSED;
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