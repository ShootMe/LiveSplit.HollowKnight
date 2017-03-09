using System;
using System.Collections.Generic;
using System.Drawing;
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
		private TargetMode lastTargetMode = TargetMode.FOLLOW_HERO;
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
				lblUIState.Text = "UI State: " + Memory.UIState().ToString();

				string scene = Memory.SceneName();
				if (lastScene != scene) {
					enemyInfo.Clear();
					currentEnemy.Pointer = 0;
					currentEnemy.HP = 0;
					Memory.UpdateGeoCounter(false, 0);
					lastScene = scene;
				}
				lblSceneName.Text = "Scene: " + scene;

				TargetMode target = Memory.GetCameraTargetMode();
				if (chkCameraTarget.Checked && target != TargetMode.FOLLOW_HERO) {
					lastTargetMode = target;
					target = TargetMode.FOLLOW_HERO;
					Memory.SetCameraTargetMode(target);
				}
				lblTargetMode.Text = "Target Mode: " + target.ToString();

				PointF position = Memory.GetCameraTarget();
				lblPosition.Text = "Position: " + position.X.ToString("0.00") + ", " + position.Y.ToString("0.00");

				bool disablePause = Memory.PlayerData<bool>(Offset.disablePause);
				btnEnablePause.Enabled = disablePause;
				if (chkShowEnemyHP.Checked) {
					DisplayEnemyHP();
				}
				if (chkInfiniteHP.Checked) {
					Memory.SetPlayerData(Offset.health, Memory.PlayerData<int>(Offset.maxHealthBase));
				}
				if (chkInfiniteSoul.Checked) {
					Memory.SetPlayerData(Offset.MPCharge, 99);
				}
				if (chkInvincible.Checked) {
					Memory.SetPlayerData(Offset.isInvincible, true);
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
				if (oldHP != info.HP && info.HP < 5000 && info.HP > 0) {
					currentEnemy = info;
					changed = true;
				}
			}

			if (maxHP > 0 && maxHP < 5000) {
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
		private void btnEnablePause_Click(object sender, EventArgs e) {
			Memory.SetPlayerData(Offset.disablePause, false);
		}
		private void chkInvincible_CheckedChanged(object sender, EventArgs e) {
			if (!chkInvincible.Checked) {
				Memory.SetPlayerData(Offset.isInvincible, false);
			}
		}
		private void chkCameraTarget_CheckedChanged(object sender, EventArgs e) {
			if (!chkCameraTarget.Checked) {
				Memory.SetCameraTargetMode(lastTargetMode);
			}
		}
	}
}