using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightInfo : Form {
		public HollowKnightMemory Memory { get; set; }
		private HashSet<EnemyInfo> enemyInfo = new HashSet<EnemyInfo>();
		private EnemyInfo currentEnemy = new EnemyInfo();
		private bool changed = false;
		private string lastScene = null;
		private DateTime lastCheck = DateTime.MinValue;
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
					currentEnemy.HP = 0;
					Memory.UpdateGeoCounter(false, 0);
					lastScene = scene;
				}
				lblSceneName.Text = "Scene: " + scene;
				lblUIState.Text = "UI State: " + Memory.UIState().ToString();
				if (chkShowEnemyHP.Checked) {
					DisplayEnemyHP();
				}
			}
		}
		public void DisplayEnemyHP() {
			int maxHP = 0;
			EnemyInfo maxPointer = null;

			if (lastCheck.AddSeconds(1) < DateTime.Now) {
				lastCheck = DateTime.Now;
				List<EnemyInfo> enemies = Memory.GetEnemyInfo();
				foreach (EnemyInfo info in enemies) {
					if (enemyInfo.Add(info)) {
						if (info.HP > maxHP && info.HP > 100 && info.HP != 999) {
							maxHP = info.HP;
							maxPointer = info;
						}
					}
				}
			}

			foreach (EnemyInfo info in enemyInfo) {
				int oldHP = info.UpdateHP(Memory);
				if (oldHP != info.HP) {
					currentEnemy = info;
					changed = true;
				}
			}

			if (maxHP > 0) {
				currentEnemy = maxPointer;
				changed = true;
			}

			if (changed) {
				Memory.UpdateGeoCounter(true, currentEnemy.HP < 0 ? 0 : currentEnemy.HP);
			}
		}
		private void chkShowEnemyHP_CheckedChanged(object sender, EventArgs e) {
			if (!chkShowEnemyHP.Checked) {
				Memory.UpdateGeoCounter(false, 0);
			}
		}
	}
}