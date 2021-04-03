using System;
using System.Windows.Forms;
using System.Drawing;
using UserSettings = Controller_Input_Display.Properties.Settings;

namespace JoystickDisplay
{
	public partial class Settings : Form
	{
		private static ComboBox TSbuttons;
		private static ComboBox TAlpha;
		private static ComboBox RSbehave;
		private static ComboBox DPbehave;
		private static NumericUpDown LUpDown;
		private static NumericUpDown RUpDown;
		private static Button LColor2;
		private static Button RColor2;
		private static NumericUpDown LSWidthUpDown;
		private static NumericUpDown RSWidthUpDown;
		private static Button FormBackColor2;
		private static EventHandler LeftColor = new EventHandler(SelectLColor);
		private static EventHandler RightColor = new EventHandler(SelectRColor);
		private static EventHandler BackColorE = new EventHandler(SelectBackColor);
		private static EventHandler OKE = new EventHandler(OkF);
		private static EventHandler CancelE = new EventHandler(CancelF);

		public Settings()
		{
			MaximizeBox = false;
			BackColor = Color.FromArgb(240, 240, 240);
			ClientSize = new Size(401, 420);
			FormBorderStyle = FormBorderStyle.FixedSingle;
			Icon = new Icon("icon.ico");
			Text = "Settings";
			FormClosing += CancelF;

			Label TSButt = AddConstantLabel("Trigger and Shoulder buttons =", new Point(20, 22));
			Controls.Add(TSButt);

			TSbuttons = new ComboBox();
			TSbuttons.Items.Add("L & R");
			TSbuttons.Items.Add("Only Trigger = L & R");
			TSbuttons.Items.Add("Only Shoulder = L & R");
			TSbuttons.Items.Add("LT/LB & RT/RB");
			TSbuttons.Location = new Point(230, 20);
			TSbuttons.Size = new Size(150, 1);
			TSbuttons.DropDownStyle = ComboBoxStyle.DropDownList;
			TSbuttons.SelectedIndex = Display.LRButtons - 1;
			TSbuttons.SelectedIndexChanged += UpdateSettings;
			Controls.Add(TSbuttons);

			Label TriggAlpha = AddConstantLabel("Trigger buttons opacity :", new Point(20, 52));
			Controls.Add(TriggAlpha);

			TAlpha = new ComboBox();
			TAlpha.Items.Add("Varies with the pression");
			TAlpha.Items.Add("Full or nothing");
			TAlpha.Location = new Point(230, 50);
			TAlpha.Size = new Size(150, 1);
			TAlpha.DropDownStyle = ComboBoxStyle.DropDownList;
			TAlpha.SelectedIndex = Display.OpacityT - 1;
			TAlpha.SelectedIndexChanged += UpdateSettings;
			Controls.Add(TAlpha);

			Label RSbeha = AddConstantLabel("Right Stick =", new Point(20, 82));
			Controls.Add(RSbeha);

			RSbehave = new ComboBox();
			RSbehave.Items.Add("An other stick");
			RSbehave.Items.Add("L & R");
			RSbehave.Items.Add("LT & RT");
			RSbehave.Items.Add("LB & RB");
			RSbehave.Location = new Point(230, 80);
			RSbehave.Size = new Size(150, 1);
			RSbehave.DropDownStyle = ComboBoxStyle.DropDownList;
			RSbehave.SelectedIndex = Display.RStickBehave - 1;
			RSbehave.SelectedIndexChanged += UpdateSettings;
			Controls.Add(RSbehave);

			Label DPad = AddConstantLabel("DPad behaviour :", new Point(20, 112));
			Controls.Add(DPad);

			DPbehave = new ComboBox
			{
				Location = new Point(230, 110),
				Size = new Size(150, 1),
				DropDownStyle = ComboBoxStyle.DropDownList
			};
			DPbehave.Items.Add("Uses the images");
			DPbehave.Items.Add("Uses the stick line");
			DPbehave.SelectedIndex = Display.DPadBehave - 1;
			DPbehave.SelectedIndexChanged += UpdateSettings;
			Controls.Add(DPbehave);

			Label Dead = AddConstantLabel("Deadzones :", new Point(20, 152));
			Controls.Add(Dead);

			Label LDead = AddConstantLabel("Left Stick :", new Point(20, 179));
			Controls.Add(LDead);

			LUpDown = new NumericUpDown
			{
				Maximum = 130,
				Minimum = 1,
				Location = new Point(100, 178),
				Size = new Size(70, 23),
				Value = Display.DeadzoneL
			};
			LUpDown.ValueChanged += UpdateSettings;
			Controls.Add(LUpDown);

			Label RDead = AddConstantLabel("Right Stick :", new Point(220, 179));
			Controls.Add(RDead);

			RUpDown = new NumericUpDown
			{
				Maximum = 130,
				Minimum = 1,
				Location = new Point(310, 178),
				Size = new Size(70, 23),
				Value = Display.DeadzoneR
			};
			RUpDown.ValueChanged += UpdateSettings;
			Controls.Add(RUpDown);

			Label lineSettings = AddConstantLabel("Line stick settings :", new Point(20, 229));
			Controls.Add(lineSettings);

			Label lineColors = AddConstantLabel("Color :", new Point(99, 257));
			Controls.Add(lineColors);

			Label lineThickness = AddConstantLabel("Thickness :", new Point(220, 257));
			Controls.Add(lineThickness);

			Label LCol = AddConstantLabel("L Stick :", new Point(20, 286));
			Controls.Add(LCol);

			Button LColor = new Button
			{
				Text = "...",
				Location = new Point(131, 282),
				Size = new Size(40, 23)
			};
			LColor.Click += LeftColor;
			Controls.Add(LColor);

			LColor2 = new Button
			{
				Text = "",
				Location = new Point(99, 282),
				Size = new Size(23, 23),
				Enabled = false,
				BackColor = Display.ColorLStick
			};
			Controls.Add(LColor2);

			Label RCol = AddConstantLabel("R Stick :", new Point(20, 315));
			Controls.Add(RCol);

			Button RColor = new Button
			{
				Text = "...",
				Location = new Point(131, 311),
				Size = new Size(40, 23)
			};
			RColor.Click += RightColor;
			Controls.Add(RColor);

			RColor2 = new Button
			{
				Text = "",
				Location = new Point(99, 311),
				Size = new Size(23, 23),
				Enabled = false,
				BackColor = Display.ColorRStick
			};
			Controls.Add(RColor2);

			Label separator = new Label()
			{
				Location = new Point(195, 250),
				Width = 2,
				Size = new Size(1, 90),
				BorderStyle = BorderStyle.Fixed3D,
				Enabled = false
			};
			Controls.Add(separator);

			LSWidthUpDown = new NumericUpDown
			{
				Maximum = 50,
				Minimum = 1,
				Location = new Point(220, 283),
				Size = new Size(70, 23),
				Value = (decimal)Display.LStickWidth,
				DecimalPlaces = 2,
			};
			LSWidthUpDown.ValueChanged += UpdateThicknessSettings;
			Controls.Add(LSWidthUpDown);

			RSWidthUpDown = new NumericUpDown
			{
				Maximum = 50,
				Minimum = 1,
				Location = new Point(220, 312),
				Size = new Size(70, 23),
				Value = (decimal)Display.RStickWidth,
				DecimalPlaces = 2,
			};
			RSWidthUpDown.ValueChanged += UpdateThicknessSettings;
			Controls.Add(RSWidthUpDown);

			Label FormBackColorLabel = AddConstantLabel("Backcolor :", new Point(20, ClientSize.Height - 36));
			Controls.Add(FormBackColorLabel);

			Button FormBackColor = new Button
			{
				Text = "...",
				Location = new Point(131, ClientSize.Height - 40),
				Size = new Size(40, 23)
			};
			FormBackColor.Click += BackColorE;
			Controls.Add(FormBackColor);

			FormBackColor2 = new Button
			{
				Text = "",
				Location = new Point(99, ClientSize.Height - 40),
				Size = new Size(23, 23),
				Enabled = false,
				BackColor = SonicInputDisplay.theDisplay.BackColor
			};
			Controls.Add(FormBackColor2);

			Button OK = new Button
			{
				Text = "OK",
				Location = new Point(220, ClientSize.Height - 40)
			};
			OK.Click += OKE;
			Controls.Add(OK);
			AcceptButton = OK;

			Button Cancel = new Button
			{
				Text = "Cancel",
				Location = new Point(306, ClientSize.Height - 40)
			};
			Cancel.Click += CancelE;
			Controls.Add(Cancel);
			CancelButton = Cancel;
		}

		static void OkF(object sender, EventArgs e)
		{
			SaveSettings();
			Display.isSettingsOpened = false;
			Application.ExitThread();
		}

		public static void SaveSettings()
		{
			UserSettings.Default.LRButtons = Display.LRButtons;
			UserSettings.Default.RStickBehave = Display.RStickBehave;
			UserSettings.Default.DeadzoneL = Display.DeadzoneL;
			UserSettings.Default.DeadzoneR = Display.DeadzoneR;
			UserSettings.Default.DPadBehave = Display.DPadBehave;
			UserSettings.Default.OpacityT = Display.OpacityT;
			UserSettings.Default.ColorLStick = Display.ColorLStick;
			UserSettings.Default.ColorRStick = Display.ColorRStick;
			UserSettings.Default.Index = Display.folderIndex;
			UserSettings.Default.BackgroundColor = SonicInputDisplay.theDisplay.BackColor;
			UserSettings.Default.LStickWidth = Display.LStickWidth;
			UserSettings.Default.RStickWidth = Display.RStickWidth;
			UserSettings.Default.Save();
		}

		static void CancelF(object sender, EventArgs e)
		{
			Display.LRButtons = Display.previousLRButtons;
			Display.RStickBehave = Display.previousRStickBehave;
			Display.DeadzoneL = Display.previousDeadzoneL;
			Display.DeadzoneR = Display.previousDeadzoneR;
			Display.DPadBehave = Display.previousDPadBehave;
			Display.OpacityT = Display.previousOpacityT;

			Display.ColorLStick = Display.previousColorLStick;
			Display.ColorRStick = Display.previousColorRStick;

			Display.LStickWidth = Display.previousLStickWidth;
			Display.RStickWidth = Display.previousRStickWidth;

			Display.penLStick = Display.previousPenLStick;
			Display.penRStick = Display.previousPenRStick;

			SonicInputDisplay.theDisplay.BackColor = Display.previousFormBackColor;

			Display.isSettingsOpened = false;
			Application.ExitThread();
		}

		static void SelectLColor(object sender, EventArgs e)
		{
			ColorDialog LSColor = new ColorDialog { Color = Display.ColorLStick };
			if (LSColor.ShowDialog() == DialogResult.OK)
			{
				Display.ColorLStick = LSColor.Color;
				Display.penLStick = new Pen(Display.ColorLStick, Display.LStickWidth);
				LColor2.BackColor = Display.ColorLStick;
			}
		}

		static void SelectRColor(object sender, EventArgs e)
		{
			ColorDialog RSColor = new ColorDialog { Color = Display.ColorRStick };
			if (RSColor.ShowDialog() == DialogResult.OK)
			{
				Display.ColorRStick = RSColor.Color;
				Display.penRStick = new Pen(Display.ColorRStick, Display.RStickWidth);
				RColor2.BackColor = Display.ColorRStick;
			}
		}

		static void SelectBackColor(object sender, EventArgs e)
		{
			ColorDialog BackColorD = new ColorDialog { Color = SonicInputDisplay.theDisplay.BackColor };
			if (BackColorD.ShowDialog() == DialogResult.OK)
			{
				SonicInputDisplay.theDisplay.BackColor = BackColorD.Color;
				FormBackColor2.BackColor = BackColorD.Color;
			}
		}

		static void UpdateSettings(object sender, EventArgs e)
		{
			Display.LRButtons = TSbuttons.SelectedIndex + 1;
			Display.OpacityT = TAlpha.SelectedIndex + 1;
			Display.RStickBehave = RSbehave.SelectedIndex + 1;
			Display.DPadBehave = DPbehave.SelectedIndex + 1;
			Display.DeadzoneL = (int)LUpDown.Value;
			Display.DeadzoneR = (int)RUpDown.Value;
		}

		static void UpdateThicknessSettings(object sender, EventArgs e)
        {
			Display.LStickWidth = (float)LSWidthUpDown.Value;
			Display.RStickWidth = (float)RSWidthUpDown.Value;

			Display.penLStick = new Pen(Display.ColorLStick, Display.LStickWidth);
			Display.penRStick = new Pen(Display.ColorRStick, Display.RStickWidth);
		}

		private static Font defaultFont = new Font("Arial", 10);
		static Label AddConstantLabel(string text, Point location)
		{
			Label label = new Label
			{
				Text = text,
				Location = location,
				AutoSize = true,
				Font = defaultFont
			};
			return label;
		}
	}
}
