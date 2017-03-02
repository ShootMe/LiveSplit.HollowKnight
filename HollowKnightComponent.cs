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
		internal static string[] keys = { "CurrentSplit", "State", "GameState", "SceneName", "NailDamage", "Charms", "CameraMode", "MenuState", "UIState", "AcceptingInput", "MapZone", "Completion", "NextSceneName" };
		private HollowKnightMemory mem;
		private int currentSplit = -1, state = 0, lastLogCheck = 0;
		private bool hasLog = false;
		private Dictionary<string, string> currentValues = new Dictionary<string, string>();
		private HollowKnightSettings settings;
		private HashSet<string> splitsDone = new HashSet<string>();
		private static string LOGFILE = "_HollowKnight.log";

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
				shouldSplit = mem.MenuState() == MainMenuState.PLAY_MODE_MENU && !mem.AcceptingInput();
			} else if (Model.CurrentState.CurrentPhase == TimerPhase.Running) {
				if (currentSplit + 1 < Model.CurrentState.Run.Count) {
					if (settings.FalseKnight && !splitsDone.Contains("FalseKnight") && mem.KilledFalseKnight()) {
						shouldSplit = true;
						splitsDone.Add("FalseKnight");
					} else if (settings.MothwingCloak && !splitsDone.Contains("MothwingCloak") && mem.MothwingCloak()) {
						shouldSplit = true;
						splitsDone.Add("MothwingCloak");
					} else if (settings.ThornsOfAgony && !splitsDone.Contains("ThornsOfAgony") && mem.ThornsOfAgony()) {
						shouldSplit = true;
						splitsDone.Add("ThornsOfAgony");
					} else if (settings.MantisClaw && !splitsDone.Contains("MantisClaw") && mem.MantisClaw()) {
						shouldSplit = true;
						splitsDone.Add("MantisClaw");
					} else if (settings.DistantVillageStag && !splitsDone.Contains("DistantVillageStag") && mem.SceneName() == "DistantVillage") {
						shouldSplit = true;
						splitsDone.Add("DistantVillageStag");
					} else if (settings.CrystalHeart && !splitsDone.Contains("CrystalHeart") && mem.CrystalHeart()) {
						shouldSplit = true;
						splitsDone.Add("CrystalHeart");
					} else if (settings.GruzMother && !splitsDone.Contains("GruzMother") && mem.GruzMother()) {
						shouldSplit = true;
						splitsDone.Add("GruzMother");
					} else if (settings.DreamNail && !splitsDone.Contains("DreamNail") && mem.DreamNail()) {
						shouldSplit = true;
						splitsDone.Add("DreamNail");
					} else if (settings.NailUpgrade1 && !splitsDone.Contains("NailUpgrade1") && mem.NailDamage() == 8) {
						shouldSplit = true;
						splitsDone.Add("NailUpgrade1");
					} else if (settings.WatcherKnight && !splitsDone.Contains("WatcherKnight") && mem.WatcherKnight()) {
						shouldSplit = true;
						splitsDone.Add("WatcherKnight");
					} else if (settings.Lurien && !splitsDone.Contains("Lurien") && mem.Lurien()) {
						shouldSplit = true;
						splitsDone.Add("Lurien");
					} else if (settings.Hegemol && !splitsDone.Contains("Hegemol") && mem.Hegemol()) {
						shouldSplit = true;
						splitsDone.Add("Hegemol");
					} else if (settings.Monomon && !splitsDone.Contains("Monomon") && mem.Monomon()) {
						shouldSplit = true;
						splitsDone.Add("Monomon");
					} else if (settings.Uumuu && !splitsDone.Contains("Uumuu") && mem.Uumuu()) {
						shouldSplit = true;
						splitsDone.Add("Uumuu");
					}
				} else {
					string nextScene = mem.NextSceneName();
					shouldSplit = nextScene.Equals("Cinematic_Ending_A", StringComparison.OrdinalIgnoreCase);
				}

				GameState gameState = mem.GameState();
				Model.CurrentState.IsGameTimePaused = gameState != GameState.PLAYING && gameState != GameState.PAUSED;
			}

			HandleSplit(shouldSplit, false);
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
				string prev = "", curr = "";
				foreach (string key in keys) {
					prev = currentValues[key];

					switch (key) {
						case "CurrentSplit": curr = currentSplit.ToString(); break;
						case "State": curr = state.ToString(); break;
						case "GameState": curr = mem.GameState().ToString(); break;
						case "SceneName": curr = mem.SceneName(); break;
						case "NextSceneName": curr = mem.NextSceneName(); break;
						case "NailDamage": curr = mem.NailDamage().ToString(); break;
						case "Charms": curr = mem.CharmCount().ToString(); break;
						case "Completion": curr = mem.Completion().ToString(); break;
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
				if (Console.IsOutputRedirected) {
					using (StreamWriter wr = new StreamWriter(LOGFILE, true)) {
						wr.WriteLine(data);
					}
				} else {
					Console.WriteLine(data);
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