using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

using JoystickDisplay;
using UserSettings = Controller_Input_Display.Properties.Settings;

public class SonicInputDisplay
{
	const int SW_HIDE = 0;

	[DllImport("kernel32.dll")]
	static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	public static IntPtr processHandle = IntPtr.Zero;
	public static bool loop = true;

	public static Display theDisplay;

	public readonly static string programDir = AppDomain.CurrentDomain.BaseDirectory;

	public readonly static int processID = Process.GetCurrentProcess().Id;

	public const string Version = "3.0.0";

	public static void Main()
	{
		var handle = GetConsoleWindow();
		ShowWindow(handle, SW_HIDE);

		if(UserSettings.Default.UpdateCheckAtStartup)
        {
			Task.Run(() => StartUpdateChecking());
        }

		theDisplay = new Display();
		
		theDisplay.ClientSize = new Size(216, 136);
		theDisplay.MinimizeBox = false;
		theDisplay.MaximizeBox = false;
		theDisplay.StartPosition = FormStartPosition.CenterScreen;
		theDisplay.BackColor = Color.FromArgb(0, 0, 0);
		
		
        theDisplay.FormClosing += TheDisplay_FormClosing;

		theDisplay.BackColor = UserSettings.Default.BackgroundColor;
		

		

#if DEBUG //prevent a stupid error from the try catch (no idea why it still even throws it)
		Display.controller = new SharpDX.XInput.Controller(SharpDX.XInput.UserIndex.Any);
#endif
		
		theDisplay.ShowDialog();
		Settings.SaveSettings();
	}

    private static void TheDisplay_FormClosing(object sender, FormClosingEventArgs e)
    {
		if (Display.isSettingsOpened)
		{
			e.Cancel = true;
		}
	}

	public static void StartUpdateChecking()
    {
		using (Process updaterProcess = new Process())
		{
			updaterProcess.StartInfo = new ProcessStartInfo()
			{
				FileName = programDir + "\\" + "Updater.exe",
				WorkingDirectory = programDir,
				Arguments = "-v:\'" + Version + "\' -pid:" +processID,
				Verb = "runas"
			};
			updaterProcess.Start();
		}
	}
}
