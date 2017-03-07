using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightInfo : Form {
		public HollowKnightMemory Memory { get; set; }
		private Dictionary<int, int> enemyInfo = new Dictionary<int, int>();
		private EntityInfo currentEnemy = new EntityInfo();
		private bool changed = false, lastChanged = false;
		private string lastScene = null;
		public static void Main(string[] args) {
			try {
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new HollowKnightInfo());
			} catch (Exception ex) {
				Console.WriteLine(ex.ToString());
			}
		}
		public HollowKnightInfo() {
			this.DoubleBuffered = true;
			InitializeComponent();
			Memory = new HollowKnightMemory();
			Thread t = new Thread(UpdateLoop);
			t.IsBackground = true;
			t.Start();
		}

		private void UpdateLoop() {
			bool lastHooked = false;
			while (true) {
				try {
					bool hooked = Memory.HookProcess();
					if (hooked) {
						UpdateValues();
					}
					if (lastHooked != hooked) {
						lastHooked = hooked;
						this.Invoke((Action)delegate () { lblNote.Visible = !hooked; });
					}
					Thread.Sleep(12);
				} catch { }
			}
		}
		public void UpdateValues() {
			if (this.InvokeRequired) {
				this.Invoke((Action)UpdateValues);
			} else {
				lblCameraMode.Text = "Camera Mode: " + Memory.CameraMode().ToString();
				lblGameState.Text = "Game State: " + Memory.GameState().ToString();
				lblMenuState.Text = "Menu State: " + Memory.MenuState().ToString();
				string scene = Memory.SceneName();
				if (lastScene != scene) {
					enemyInfo.Clear();
					currentEnemy.Pointer = 0;
				}
				lblSceneName.Text = "Scene: " + scene;
				lblUIState.Text = "UI State: " + Memory.UIState().ToString();
				if (chkShowEnemyHP.Checked) {
					DisplayEnemyHP();
				} else if (lastChanged) {
					Memory.UpdateGeoCounter(false, 0);
					lastChanged = false;
				}
			}
		}
		public void DisplayEnemyHP() {
			List<EntityInfo> enemies = Memory.GetEnemyInfo();

			bool found = false;
			foreach (EntityInfo info in enemies) {
				if (info.Same(currentEnemy)) {
					found = true;
				}

				int maxHP = 0;
				EntityInfo maxPointer = null;
				foreach (KeyValuePair<string, int> pair in info.IntVars) {
					if (pair.Key == "HP") {
						int HP = pair.Value;

						int oldHp = 0;
						if (!enemyInfo.TryGetValue(info.Pointer, out oldHp)) {
							enemyInfo.Add(info.Pointer, HP);
							if (HP > maxHP && HP > 100 && HP != 999) {
								maxHP = HP;
								maxPointer = info;
							}
						} else if (HP != oldHp) {
							enemyInfo[info.Pointer] = HP;
							currentEnemy = info;
							changed = true;
						}

						break;
					}
				}

				if (maxHP > 0) {
					currentEnemy = maxPointer;
					found = true;
					changed = true;
				}
			}

			if (found && changed) {
				int hp = enemyInfo[currentEnemy.Pointer];
				Memory.UpdateGeoCounter(true, hp < 0 ? 0 : hp);
				lastChanged = true;
			} else if (!found && lastChanged) {
				Memory.UpdateGeoCounter(false, 0);
				lastChanged = false;
			}
		}
	}
}