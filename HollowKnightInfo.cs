using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
namespace LiveSplit.HollowKnight {
	public partial class HollowKnightInfo : Form {
		public HollowKnightMemory Memory { get; set; }
		private HashSet<EnemyInfo> enemyInfo = new HashSet<EnemyInfo>();
		private EnemyInfo currentEnemy = new EnemyInfo();
		private bool changed = false, showDebug = false;
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
			Text = "Hollow Knight Info " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
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
				string sceneName = Memory.SceneName();
				string nextScene = Memory.NextSceneName();
				GameState gameState = Memory.GameState();
				UIState uiState = Memory.UIState();
				bool acceptingInput = Memory.AcceptingInput();

				lblCameraMode.Text = "Camera Mode: " + Memory.CameraMode().ToString();
				lblGameState.Text = "Game State: " + gameState.ToString().PadRight(15, ' ') + "Accepting: " + acceptingInput.ToString();
				lblMenuState.Text = "Hero State: " + Memory.HeroTransitionState().ToString();
				lblUIState.Text = "UI State: " + uiState.ToString().PadRight(15, ' ') + "Menu State: " + Memory.MenuState().ToString();

				if (lastScene != sceneName) {
					enemyInfo.Clear();
					currentEnemy.Pointer = 0;
					currentEnemy.HP = 0;
					Memory.UpdateGeoCounter(false, 0);
					lastScene = sceneName;
				}
				lblSceneName.Text = "Scene: " + sceneName + " Next: " + nextScene;

				TargetMode target = Memory.GetCameraTargetMode();
				if (chkCameraTarget.Checked && target != TargetMode.FOLLOW_HERO) {
					lastTargetMode = target;
					target = TargetMode.FOLLOW_HERO;
					Memory.SetCameraTargetMode(target);
				}
				lblTargetMode.Text = "Target Mode: " + target.ToString().PadRight(15, ' ') + "State: " + Memory.HeroActorState().ToString();

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

			int oldHP = 0;
			foreach (EnemyInfo info in enemyInfo) {
				oldHP = info.UpdateHP(Memory);
				if (oldHP != info.HP && info.HP < 5000 && info.HP > 0) {
					currentEnemy = info;
					changed = true;
					break;
				}
			}

			if (maxHP > 0 && maxHP < 5000) {
				currentEnemy = maxPointer;
				changed = true;
			}

			if (changed) {
				if (chkEnemyInvincible.Checked && currentEnemy.HP > 0) {
					currentEnemy.UpdateHP(Memory, oldHP);
				}
				Memory.UpdateGeoCounter(true, currentEnemy.HP < 0 ? 0 : currentEnemy.HP);
				changed = false;
			}
		}
		private void chkShowEnemyHP_CheckedChanged(object sender, EventArgs e) {
			if (!chkShowEnemyHP.Checked) {
				Memory.UpdateGeoCounter(false, 0);
			}
			chkEnemyInvincible.Enabled = chkShowEnemyHP.Checked;
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
		private void chkLockZoom_CheckedChanged(object sender, EventArgs e) {
			if (!chkLockZoom.Checked) {
				Memory.SetCameraZoom(1f);
				zoomValue.Value = 200;
				chkLockZoom.Text = "Zoom (1.00)";
			}
			zoomValue.Enabled = chkLockZoom.Checked;
		}
		private void zoomValue_Scroll(object sender, EventArgs e) {
			if (chkLockZoom.Checked) {
				chkLockZoom.Text = "Zoom (" + (zoomValue.Value / 200f).ToString("0.00") + ")";
				Memory.SetCameraZoom(zoomValue.Value / 200f);
			}
		}
		private void btnDebugInfo_Click(object sender, EventArgs e) {
			showDebug = !showDebug;
			Memory.EnableDebug(showDebug);
		}
	}
}