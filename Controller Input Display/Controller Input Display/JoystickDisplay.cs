using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
using System.Threading;

using SharpDX;
using SharpDX.XInput;
using UserSettings = Controller_Input_Display.Properties.Settings;
using System.Threading.Tasks;

namespace JoystickDisplay
{
	public partial class Display : Form
	{
		public static MenuItem startupUpdatesCheck;

		public static Timer refreshTimer = new Timer { Interval = 14 };

		public static bool isSettingsOpened = false;
		public static Controller controller;
		public static int LRButtons;
		public static int RStickBehave;
		public static int DeadzoneL;
		public static int DeadzoneR;
		public static int DPadBehave;
		public static int OpacityT;

		public static int previousLRButtons;
		public static int previousRStickBehave;
		public static int previousDeadzoneL;
		public static int previousDeadzoneR;
		public static int previousDPadBehave;
		public static int previousOpacityT;

		private static int tryCount = 5;
		ColorMatrix colormatrixL = new ColorMatrix();
		ColorMatrix colormatrixR = new ColorMatrix();
		ColorMatrix colormatrixLT = new ColorMatrix();
		ColorMatrix colormatrixRT = new ColorMatrix();

		private static int x;
		private static int y;
		private static int Rx;
		private static int Ry;
		private static int joyX;
		private static int joyY;
		private static int joyRX;
		private static int joyRY;
		private static int A;
		private static int B;
		private static int X;
		private static int Y;
		private static int L;
		private static int R;
		private static int LT;
		private static int LB;
		private static int RT;
		private static int RB;
		private static int S;
		private static int S2;
		private static int DUp;
		private static int DDown;
		private static int DRight;
		private static int DLeft;

		private static int prevA;
		private static int prevB;
		private static int prevX;
		private static int prevY;
		private static int prevL;
		private static int prevR;
		private static int prevLT;
		private static int prevLB;
		private static int prevRT;
		private static int prevRB;
		private static int prevS;
		private static int prevS2;

		private static int countA;
		private static int countB;
		private static int countX;
		private static int countY;
		private static int countL;
		private static int countR;
		private static int countLT;
		private static int countLB;
		private static int countRT;
		private static int countRB;
		private static int countS;
		private static int countS2;

		private static Rectangle recA;
		private static Rectangle recB;
		private static Rectangle recX;
		private static Rectangle recY;
		private static Rectangle recL;
		private static Rectangle recR;
		private static Rectangle recLT;
		private static Rectangle recLB;
		private static Rectangle recRT;
		private static Rectangle recRB;
		private static Rectangle recS;
		private static Rectangle recS2;

		private static Bitmap imgBase;
		private static Bitmap imgStick;
		private static Bitmap imgRStick;
		private static Bitmap imgStickSmall;
		private static Bitmap imgRStickSmall;
		private static Bitmap imgA;
		private static Bitmap imgB;
		private static Bitmap imgX;
		private static Bitmap imgY;
		private static Bitmap imgL;
		private static Bitmap imgR;
		private static Bitmap imgLT;
		private static Bitmap imgLB;
		private static Bitmap imgRT;
		private static Bitmap imgRB;
		private static Bitmap imgS;
		private static Bitmap imgS2;
		private static Bitmap imgDUp;
		private static Bitmap imgDDown;
		private static Bitmap imgDRight;
		private static Bitmap imgDLeft;

		public static Color ColorLStick;
		public static Color ColorRStick;
		public static float LStickWidth;
		public static float RStickWidth;
		public static Pen penLStick;
		public static Pen penRStick;

		public static Color previousColorLStick;
		public static Color previousColorRStick;
		public static float previousLStickWidth;
		public static float previousRStickWidth;
		public static Pen previousPenLStick;
		public static Pen previousPenRStick;

		public static Color previousFormBackColor;

		private static Font fontArial;

		private static StringFormat formatLeft;
		private static StringFormat formatCenter;
		private static StringFormat formatRight;

		public static int folderIndex;

		private static bool drawButtonCount = false;

		public Display()
		{
			joyX = 0;
			joyY = 0;
			joyRX = 0;
			joyRY = 0;
			A = 1;
			B = 1;
			X = 1;
			Y = 1;
			L = 255;
			R = 255;
			LT = 255;
			LB = 255;
			RT = 255;
			RB = 255;
			S = 1;
			S2 = 1;
			DUp = 1;
			DDown = 1;
			DRight = 1;
			DLeft = 1;

			prevA = 1;
			prevB = 1;
			prevX = 1;
			prevY = 1;
			prevL = 1;
			prevR = 1;
			prevLT = 1;
			prevLB = 1;
			prevRT = 1;
			prevRB = 1;
			prevS = 1;
			prevS2 = 1;

			countA = 0;
			countB = 0;
			countX = 0;
			countY = 0;
			countL = 0;
			countR = 0;
			countLT = 0;
			countLB = 0;
			countRT = 0;
			countRB = 0;
			countS = 0;
			countS2 = 0;

			folderIndex = UserSettings.Default.Index;

			LRButtons = UserSettings.Default.LRButtons;
			RStickBehave = UserSettings.Default.RStickBehave;
			DPadBehave = UserSettings.Default.DPadBehave;
			OpacityT = UserSettings.Default.OpacityT;
			DeadzoneL = UserSettings.Default.DeadzoneL;
			DeadzoneR = UserSettings.Default.DeadzoneR;
			ColorLStick = UserSettings.Default.ColorLStick;
			ColorRStick = UserSettings.Default.ColorRStick;
			LStickWidth = UserSettings.Default.LStickWidth;
			RStickWidth = UserSettings.Default.RStickWidth;

			reloadImages();

			penLStick = new Pen(ColorLStick, LStickWidth);
			penRStick = new Pen(ColorRStick, RStickWidth);
			fontArial = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);

			recL = new Rectangle(40, 4, 96, 36);
			recLT = new Rectangle(40, 1, 96, 36);
			recLB = new Rectangle(40, 19, 96, 36);
			recY = new Rectangle(40, 52, 96, 36);
			recA = new Rectangle(40, 100, 96, 36);
			recR = new Rectangle(80, 4, 96, 36);
			recRT = new Rectangle(80, 1, 96, 36);
			recRB = new Rectangle(80, 19, 96, 36);
			recX = new Rectangle(80, 52, 96, 36);
			recB = new Rectangle(80, 100, 96, 36);
			recS = new Rectangle(120, 30, 96, 36);
			recS2 = new Rectangle(2, 30, 96, 36);

			formatLeft = new StringFormat();
			formatCenter = new StringFormat();
			formatRight = new StringFormat();

			formatLeft.Alignment = StringAlignment.Near;
			formatLeft.LineAlignment = StringAlignment.Center;

			formatCenter.Alignment = StringAlignment.Center;
			formatCenter.LineAlignment = StringAlignment.Center;

			formatRight.Alignment = StringAlignment.Far;
			formatRight.LineAlignment = StringAlignment.Center;

			this.DoubleBuffered = true;
			this.Icon = new Icon("icon.ico");

			this.KeyPreview = true;
			this.PreviewKeyDown += new PreviewKeyDownEventHandler(keyPress);

			EventHandler settings = new EventHandler(Msettings);

			EventHandler checkForUpdates = new EventHandler((o, e) =>
			{
				Task.Run(() => SonicInputDisplay.StartUpdateChecking());
			});

			EventHandler exit = new EventHandler(Mexit);

			startupUpdatesCheck = new MenuItem("Check for updates at program startup", new EventHandler(MstartupUpdatesCheck));
			startupUpdatesCheck.Checked = UserSettings.Default.UpdateCheckAtStartup;

			ContextMenu context = new ContextMenu();
			context.MenuItems.Add("Settings", settings);
			context.MenuItems.Add("-");
			context.MenuItems.Add(startupUpdatesCheck);
			context.MenuItems.Add("Check for updates now", checkForUpdates);
			context.MenuItems.Add("-");
			context.MenuItems.Add("Exit", exit);
			ContextMenu = context;

            refreshTimer.Tick += RefreshTimer_Tick;
			refreshTimer.Start();
		}

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
			setControllerDataSADX();
			Refresh();
		}

        public void Msettings(object sender, EventArgs e)
        {
			if(isSettingsOpened)
            {
				return;
            }
			previousLRButtons = LRButtons;
			previousRStickBehave = RStickBehave;
			previousDeadzoneL = DeadzoneL;
			previousDeadzoneR = DeadzoneR;
			previousDPadBehave = DPadBehave;
			previousOpacityT = OpacityT;

			previousColorLStick = ColorLStick;
			previousColorRStick = ColorRStick;
			previousLStickWidth = LStickWidth;
			previousRStickWidth = RStickWidth;
			previousPenLStick = penLStick;
			previousPenRStick = penRStick;

			previousFormBackColor = SonicInputDisplay.theDisplay.BackColor;

			isSettingsOpened = true;

			new Thread(() =>
			{
				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault(false);
				Application.Run(new Settings());
			}).Start();
		}

		public void MstartupUpdatesCheck(object sender, EventArgs e)
        {
			UserSettings.Default.UpdateCheckAtStartup = !UserSettings.Default.UpdateCheckAtStartup;

			startupUpdatesCheck.Checked = UserSettings.Default.UpdateCheckAtStartup;
		}

		public void Mexit(object sender, EventArgs e)
		{
			if (isSettingsOpened)
			{
				return;
			}
			Settings.SaveSettings();
			Environment.Exit(0);
		}

		public void setControllerDataSADX()
		{
			State state;
			try
			{
				state = controller.GetState();
			}
			catch
			{
				int i;
				for (i = 0; i < 4; i++)
				{
					controller = new Controller((UserIndex)i);
					if (controller.IsConnected)
					{
						break;
					}
				}
				return;
			}

			int gamepadButtons = (int)state.Gamepad.Buttons;

			A = gamepadButtons & 4096;
			B = gamepadButtons & 8192;
			X = gamepadButtons & 16384;
			Y = gamepadButtons & 32768;
			S = gamepadButtons & 16;
			S2 = gamepadButtons & 32;

			DUp = gamepadButtons & 1;
			DDown = gamepadButtons & 2;
			DLeft = gamepadButtons & 4;
			DRight = gamepadButtons & 8;

			//Option using the stick line chosen
			if (DPadBehave == 2)
			{
				joyY = 0;
                joyY += DUp * 128;
				joyY += DDown * (-64);

				joyX = 0;
				joyX += DLeft * (-32);
				joyX += DRight * 16;

				if (DDown + DUp == 0)
				{
					joyY = 0;
				}
				if (DLeft + DRight == 0)
				{
					joyX = 0;
				}

				if (DUp + DDown + DLeft + DRight == 0)
				{
					x = state.Gamepad.LeftThumbX / 257;
					y = state.Gamepad.LeftThumbY / 257;
					//if the stick has passed the deadzone
					if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) > DeadzoneL)
					{
						joyX = x;
						joyY = y;
					}
				}
			}
			//Option uses the images chosen
			else
			{
				x = state.Gamepad.LeftThumbX / 257;
				y = state.Gamepad.LeftThumbY / 257;
				//if the stick has passed the deadzone
				if (Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2)) > DeadzoneL)
				{
					joyX = x;
					joyY = y;
				}
				else
                {
					joyX = 0;
					joyY = 0;
				}
			}

			Rx = state.Gamepad.RightThumbX / 257;
			Ry = state.Gamepad.RightThumbY / 257;
			//if the stick has passed the deadzone
			if (Math.Sqrt(Math.Pow(Rx, 2) + Math.Pow(Ry, 2)) > DeadzoneR)
			{
				joyRX = Rx;
				joyRY = Ry;
			}
			else
			{
				joyRX = 0;
				joyRY = 0;
			}

			//Option L & R		OR		Option Only trigger = L & R
			if (LRButtons == 1 || LRButtons == 2)
			{
				//Option Varies with the pression
				if (OpacityT == 1)
				{
					L = state.Gamepad.LeftTrigger;
					R = state.Gamepad.RightTrigger;
				}
				//option Full or nothing
				if (OpacityT == 2)
				{
					L = state.Gamepad.LeftTrigger * 255;
					R = state.Gamepad.RightTrigger * 255;
				}
			}
			if (LRButtons == 1)
			{
				L += gamepadButtons & 256;
				R += gamepadButtons & 512;
			}

			//Option Only shoulder = L & R
			if (LRButtons == 3)
            {
				L = gamepadButtons & 256;
				R = gamepadButtons & 512;
			}

			//Option LT/LB & RT/RB
			if (LRButtons == 4)
			{
				if (OpacityT == 1)
				{
					LT = state.Gamepad.LeftTrigger;
					RT = state.Gamepad.RightTrigger;
				}
				if (OpacityT == 2)
				{
					LT = state.Gamepad.LeftTrigger * 255;
					RT = state.Gamepad.RightTrigger * 255;
				}

				LB = gamepadButtons & 256;
				RB = gamepadButtons & 512;
			}
			if (RStickBehave == 2)
			{
				if (L == 0 && joyRX < 0)
				{
					L = 255;
				}
				if (R == 0 && joyRX > 0)
				{
					R = 255;
				}
			}
			else if (RStickBehave == 3)
			{
				if (LT == 0 && joyRX < 0)
				{
					LT = 255;
				}
				if (RT == 0 && joyRX > 0)
				{
					RT = 255;
				}
			}
			else if (RStickBehave == 4)
			{
				if (LB == 0 && joyRX < 0)
				{
					LB = 255;
				}
				if (RB == 0 && joyRX > 0)
				{
					RB = 255;
				}
			}
			refreshButtonCounts();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (A != 0)
			{
				e.Graphics.DrawImage(imgA, 4, 100, 32, 32);
			}

			if (B != 0)
			{
				e.Graphics.DrawImage(imgB, 180, 100, 32, 32);
			}

			if (X != 0)
			{
				e.Graphics.DrawImage(imgX, 180, 52, 32, 32);
			}

			if (Y != 0)
			{
				e.Graphics.DrawImage(imgY, 4, 52, 32, 32);
			}

			if (L != 0 && LRButtons != 4)
			{
				colormatrixL.Matrix33 = L / 255f;
				ImageAttributes imgAttributeL = new ImageAttributes();
				imgAttributeL.SetColorMatrix(colormatrixL, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				e.Graphics.DrawImage(imgL, new Rectangle(4, 4, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel, imgAttributeL);
			}

			if (R != 0 && LRButtons != 4)
			{
				colormatrixR.Matrix33 = R / 255f;
				ImageAttributes imgAttributeR = new ImageAttributes();
				imgAttributeR.SetColorMatrix(colormatrixR, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
				e.Graphics.DrawImage(imgR, new Rectangle(180, 4, 32, 32), 0, 0, 32, 32, GraphicsUnit.Pixel, imgAttributeR);
			}

			if (LRButtons == 4)
			{
				if (LT != 0)
				{
					colormatrixLT.Matrix33 = LT / 255f;
					ImageAttributes imgAttributeLT = new ImageAttributes();
					imgAttributeLT.SetColorMatrix(colormatrixLT, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
					e.Graphics.DrawImage(imgLT, new Rectangle(4, 4, 32, 42), 0, 0, 32, 42, GraphicsUnit.Pixel, imgAttributeLT);
				}
				if (RT != 0)
				{
					colormatrixRT.Matrix33 = RT / 255f;
					ImageAttributes imgAttributeRT = new ImageAttributes();
					imgAttributeRT.SetColorMatrix(colormatrixRT, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
					e.Graphics.DrawImage(imgRT, new Rectangle(180, 4, 32, 42), 0, 0, 32, 42, GraphicsUnit.Pixel, imgAttributeRT);
				}
				if (LB != 0)
				{
					e.Graphics.DrawImage(imgLB, 4, 4, 32, 42);
				}
				if (RB != 0)
				{
					e.Graphics.DrawImage(imgRB, 180, 4, 32, 42);
				}
			}



			if (!drawButtonCount)
			{
				if (S != 0)
				{
					e.Graphics.DrawImage(imgS, 156, 4, 16, 16);
				}
				if (S2 != 0)
				{
					e.Graphics.DrawImage(imgS2, 44, 4, 16, 16);
				}

				int drawRX = 108 + ((64 * joyRX) / 128);
				int drawRY = 68 - ((64 * joyRY) / 128);

				e.Graphics.DrawImage(imgBase, 108 - 64, 68 - 64, 128, 128);
				if (DPadBehave == 1)
				{
					if (DUp != 0)
					{
						e.Graphics.DrawImage(imgDUp, 92, 3, 32, 62);
					}
					if (DDown != 0)
					{
						e.Graphics.DrawImage(imgDDown, 92, 71, 32, 62);
					}
					if (DRight != 0)
					{
						e.Graphics.DrawImage(imgDRight, 111, 52, 62, 32);
					}
					if (DLeft != 0)
					{
						e.Graphics.DrawImage(imgDLeft, 43, 52, 62, 32);
					}
				}
				if (RStickBehave == 1 && (joyRX != 0 || joyRY != 0))
				{
					e.Graphics.DrawLine(penRStick, 108, 68, drawRX, drawRY);
					e.Graphics.DrawImage(imgRStickSmall, drawRX - 4, drawRY - 4, 8, 8);

					double radiusR = Math.Min(Math.Sqrt((joyRX * joyRX) + (joyRY * joyRY)), 128.0);
					double angleR = Math.Atan2(joyRY, joyRX);
					int capRX = (int)(radiusR * Math.Cos(angleR));
					int capRY = (int)(radiusR * Math.Sin(angleR));
					int drawCapRX = 108 + ((64 * capRX) / 128);
					int drawCapRY = 68 - ((64 * capRY) / 128);

					e.Graphics.DrawImage(imgRStick, drawCapRX - 4, drawCapRY - 4, 8, 8);
				}

				int drawX = 108 + ((64 * joyX) / 128);
				int drawY = 68 - ((64 * joyY) / 128);

				e.Graphics.DrawLine(penLStick, 108, 68, drawX, drawY);
				e.Graphics.DrawImage(imgStickSmall, drawX - 4, drawY - 4, 8, 8);

				double radius = Math.Min(Math.Sqrt((joyX * joyX) + (joyY * joyY)), 128.0);
				double angle = Math.Atan2(joyY, joyX);
				int capX = (int)(radius * Math.Cos(angle));
				int capY = (int)(radius * Math.Sin(angle));
				int drawCapX = 108 + ((64 * capX) / 128);
				int drawCapY = 68 - ((64 * capY) / 128);

				e.Graphics.DrawImage(imgStick, drawCapX - 4, drawCapY - 4, 8, 8);
			}
			else
			{
				if (S != 0)
				{
					e.Graphics.DrawImage(imgS, 120, 12, 16, 16);
				}
				if (S2 != 0)
				{
					e.Graphics.DrawImage(imgS2, 80, 12, 16, 16);
				}

				if (LRButtons != 4)
				{
					e.Graphics.DrawString("" + countL, fontArial, Brushes.White, recL, formatLeft);
					e.Graphics.DrawString("" + countR, fontArial, Brushes.White, recR, formatRight);
				}
				if (LRButtons == 4)
				{
					e.Graphics.DrawString("" + countRT, fontArial, Brushes.White, recRT, formatRight);
					e.Graphics.DrawString("" + countRB, fontArial, Brushes.White, recRB, formatRight);
					e.Graphics.DrawString("" + countLT, fontArial, Brushes.White, recLT, formatLeft);
					e.Graphics.DrawString("" + countLB, fontArial, Brushes.White, recLB, formatLeft);
				}

				e.Graphics.DrawString("" + countY, fontArial, Brushes.White, recY, formatLeft);
				e.Graphics.DrawString("" + countA, fontArial, Brushes.White, recA, formatLeft);
				e.Graphics.DrawString("" + countX, fontArial, Brushes.White, recX, formatRight);
				e.Graphics.DrawString("" + countB, fontArial, Brushes.White, recB, formatRight);
				e.Graphics.DrawString("" + countS, fontArial, Brushes.White, recS, formatLeft);
				e.Graphics.DrawString("" + countS2, fontArial, Brushes.White, recS2, formatRight);
			}
			base.OnPaint(e);
		}

		public void refreshButtonCounts()
		{
			if (A != 0 && prevA == 0) { countA++; }
			if (B != 0 && prevB == 0) { countB++; }
			if (X != 0 && prevX == 0) { countX++; }
			if (Y != 0 && prevY == 0) { countY++; }
			if (L != 0 && prevL == 0) { countL++; }
			if (R != 0 && prevR == 0) { countR++; }
			if (LT != 0 && prevLT == 0) { countLT++; }
			if (LB != 0 && prevLB == 0) { countLB++; }
			if (RT != 0 && prevRT == 0) { countRT++; }
			if (RB != 0 && prevRB == 0) { countRB++; }
			if (S != 0 && prevS == 0) { countS++; }
			if (S2 != 0 && prevS2 == 0) { countS2++; }

			prevA = A;
			prevB = B;
			prevX = X;
			prevY = Y;
			prevL = L;
			prevR = R;
			prevLT = LT;
			prevLB = LB;
			prevRT = RT;
			prevRB = RB;
			prevS = S;
			prevS2 = S2;
		}

		public void reloadImages()
		{
			string path = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\res";
			string[] folders = System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories);

			if (folderIndex < 0 || folderIndex >= folders.Length)
			{
				folderIndex = 0;
			}

			string folder = folders[folderIndex];

			if (tryCount != 0)
			{
				try
				{
					imgBase = new Bitmap(folder + "/base.png", false);
					imgStick = new Bitmap(folder + "/stick.png", false);
					imgRStick = new Bitmap(folder + "/stickR.png", false);
					imgA = new Bitmap(folder + "/buttA.png", false);
					imgB = new Bitmap(folder + "/buttB.png", false);
					imgX = new Bitmap(folder + "/buttX.png", false);
					imgY = new Bitmap(folder + "/buttY.png", false);
					imgL = new Bitmap(folder + "/buttL.png", false);
					imgR = new Bitmap(folder + "/buttR.png", false);
					imgLT = new Bitmap(folder + "/buttLT.png", false);
					imgLB = new Bitmap(folder + "/buttLB.png", false);
					imgRT = new Bitmap(folder + "/buttRT.png", false);
					imgRB = new Bitmap(folder + "/buttRB.png", false);
					imgS = new Bitmap(folder + "/buttS.png", false);
					imgS2 = new Bitmap(folder + "/buttS2.png", false);
					imgDUp = new Bitmap(folder + "/DUp.png", false);
					imgDDown = new Bitmap(folder + "/DDown.png", false);
					imgDRight = new Bitmap(folder + "/DRight.png", false);
					imgDLeft = new Bitmap(folder + "/DLeft.png", false);
					imgStickSmall = new Bitmap(folder + "/stickSmall.png", false);
					imgRStickSmall = new Bitmap(folder + "/stickSmallR.png", false);
					tryCount = 5;
				}
				catch
				{
					MessageBox.Show("This folder doesn't have all the images required to work.\nThe program will attempt to go to the next folder.\nThe program will attempt " + tryCount + " more times", "Input Display", MessageBoxButtons.OK, MessageBoxIcon.Error);
					folderIndex++;
					tryCount--;
					reloadImages();
				}
			}
			else
			{
				MessageBox.Show("The Program didn't found a folder with all the required images", "Input Display", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void keyPress(object sender, PreviewKeyDownEventArgs e)
		{
			string path = System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName) + "\\res";
			string[] folders = System.IO.Directory.GetDirectories(path, "*", System.IO.SearchOption.AllDirectories);

			int prevFolderIndex = folderIndex;

			folderIndex = Math.Max(0, Math.Min(folders.Length - 1, folderIndex));

			switch (e.KeyCode)
			{
				case Keys.Left:
					{
						folderIndex--;
						if (folderIndex == -1)
						{
							folderIndex = folders.Length - 1;
						}
						break;
					}

				case Keys.Right:
					{
						folderIndex++;
						if (folderIndex == folders.Length)
						{
							folderIndex = 0;
						}
						break;
					}

				case Keys.R:
					{
						countA = 0;
						countB = 0;
						countX = 0;
						countY = 0;
						countL = 0;
						countR = 0;
						countLT = 0;
						countLB = 0;
						countRT = 0;
						countRB = 0;
						countS = 0;
						countS2 = 0;
						break;
					}

				case Keys.B:
					{
						drawButtonCount = !drawButtonCount;
						break;
					}

				default:
					break;
			}

			if (folderIndex != prevFolderIndex)
			{
				reloadImages();
			}
		}
	}
}