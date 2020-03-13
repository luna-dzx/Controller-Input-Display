using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

using JoystickDisplay;

public class SonicInputDisplay
{
	const int PROCESS_VM_READ = 0x0010;
	const int SW_HIDE = 0;

	[DllImport("kernel32.dll")]
	static extern IntPtr GetConsoleWindow();

	[DllImport("user32.dll")]
	static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

	[DllImport("kernel32.dll")]
	public static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

	[DllImport("kernel32.dll")]
	public static extern bool ReadProcessMemory(int hProcess,
		int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

	public static IntPtr processHandle = IntPtr.Zero;
	public static int gameID = 1;
	public static bool loop = true;

	private static Display theDisplay;

	public static void Main()
	{
		var handle = GetConsoleWindow();
		ShowWindow(handle, SW_HIDE);

		theDisplay = new Display();
		theDisplay.ClientSize = new Size(216, 136);
		theDisplay.MinimizeBox = false;
		theDisplay.MaximizeBox = false;
		theDisplay.FormBorderStyle = FormBorderStyle.FixedSingle;
		theDisplay.StartPosition = FormStartPosition.CenterScreen;
		theDisplay.BackColor = Color.FromArgb(2, 2, 2); //Almost, but not exactly black
		theDisplay.Text = "Input Display";

		try
		{
			string[] lines = System.IO.File.ReadAllLines("BackgroundColor.ini");
			string[] color = lines[0].Split(',');
			int r = Int32.Parse(color[0]);
			int g = Int32.Parse(color[1]);
			int b = Int32.Parse(color[2]);
			theDisplay.BackColor = Color.FromArgb(r, g, b);
		}
		catch { }

		//Thread to handle the window
		new Thread(() =>
		{
			Thread.CurrentThread.IsBackground = true;
			theDisplay.ShowDialog();
			theDisplay.saveIndex();
			loop = false;
		}).Start();

		while (loop)
		{
			switch (gameID)
			{
				case 1: setValuesFromSADX(); break;
			}
			theDisplay.Refresh();
			System.Threading.Thread.Sleep(14);
		}
	}

	private static void setValuesFromSADX()
	{
		theDisplay.setControllerDataSADX();
	}
}