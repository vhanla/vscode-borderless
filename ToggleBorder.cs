using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

public static class CBS {
	private const int GWL_STYLE 	= -16;
	private const int WS_CAPTION 	= 0x00C00000;
	private const int WS_THICKFRAME	= 0x00040000;

	private const int SWP_ASYNCWINDOWPOS = 0x4000;
	private const int SwP_DEFERERASE = 0x2000;
	private const int SWP_DRAWFRAME = 0x0020;
	private const int SWP_FRAMECHANGED = 0x0020;
	private const int SWP_HIDEWINDOW = 0x0080;
	private const int SWP_NOACTIVATE = 0x0010;
	private const int SWP_NOCOPYBITS = 0x0100;
	private const int SWP_NOMOVE = 0x0002;
	private const int SWP_NOOWNERZORDER = 0x0200;
	private const int SWP_NOREDRAW = 0x0008;
	private const int SWP_NOREPOSITION = 0x0200;
	private const int SWP_NOSENDCHANGING = 0x0400;
	private const int SWP_NOSIZE = 0x0001;
	private const int SWP_NOZORDER = 0x0004;
	private const int SWP_SHOWWINDOW = 0x004;

	private delegate bool EnumWindowsProc(int hWnd, int lParam);
	[DllImport("user32.dll")]
	private static extern bool EnumThreadWindows(uint dwThreadId, EnumWindowsProc lpEnumFunc, int lParam);
	[DllImport("user32.dll")]
	private static extern uint GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);
	[DllImport("user32.dll")]
	private static extern bool IsWindowVisible(int hWnd);
	[DllImport("user32.dll")]
	private static extern int GetWindowLong(int hWnd, int nIndex);
	[DllImport("user32.dll")]
	private static extern int SetWindowLong(int hWnd, int nIndex, int dwNewLong);
	[DllImport("user32.dll")]
	private static extern bool UpdateWindow(int hWnd);
	[DllImport("user32.dll")]
	private static extern bool SetWindowPos(int hWnd, int hWndInserAfter, int x, int y, int cx, int cy, int uFlags);

	public static bool ToggleBorder(int pid, byte state) {
		Process mainproc = Process.GetProcessById(pid);
		foreach (Process proc in Process.GetProcessesByName(mainproc.ProcessName)) {
			if (proc.StartInfo.FileName == mainproc.StartInfo.FileName) {
				int hMainWnd = proc.MainWindowHandle.ToInt32();
				if (hMainWnd != 0) {
					uint tid = GetWindowThreadProcessId(hMainWnd, out pid);
					bool result = EnumThreadWindows(tid, delegate(int hWnd, int lParam) {
						if (IsWindowVisible(hWnd)) {
							int windowLong = GetWindowLong(hWnd, GWL_STYLE);
							if (state == 0){
								SetWindowLong(hWnd, GWL_STYLE, windowLong | WS_CAPTION | WS_THICKFRAME);
							}else{
								SetWindowLong(hWnd, GWL_STYLE, windowLong & ~WS_CAPTION & ~WS_THICKFRAME);
							}
							UpdateWindow(hWnd);
							int swpFlags = SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOZORDER | SWP_NOSIZE | SWP_NOACTIVATE;
							SetWindowPos(hWnd, 0, 0, 0, 0, 0, swpFlags);
						}
						return true;
					}, 0);
					if (!result) {
						return false;
					}
				}
			}
		}
		return true;
	}
}