using UnityEngine;

public static class HudMessager
{
	public static void SendHudMessage(string message, string input = "", string message2 = "", int delay = 0, bool silent = false)
	{
		GameObject.FindWithTag("MessageHud").GetComponentInChildren<HudMessageReceiver>().SendHudMessage(message, input, message2, delay, silent);
	}
}
