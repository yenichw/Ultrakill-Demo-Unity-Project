public static class GetMissionName
{
	public static string GetMission(int missionNum)
	{
		switch (missionNum)
		{
		default:
			return "???";
		case 1:
			return "0-1: INTO THE FIRE";
		case 2:
			return "0-2: THE MEATGRINDER";
		case 3:
			return "0-3: DOUBLE DOWN";
		case 4:
			return "0-4: A ONE-MACHINE ARMY";
		case 5:
			return "0-5: CERBERUS";
		case 6:
			return "1-1: HEART OF THE SUNRISE";
		case 7:
			return "1-2: THE BURNING WORLD";
		case 8:
			return "1-3: HALLS OF SACRED REMAINS";
		case 9:
			return "1-4: CLAIR DE LUNE";
		case 10:
			return "2-1: BRIDGEBURNER";
		case 11:
			return "2-2: DEATH AT 20,000 VOLTS";
		}
	}

	public static string GetSceneName(int missionNum)
	{
		switch (missionNum)
		{
		default:
			return "???";
		case 1:
			return "Level 0-1";
		case 2:
			return "Level 0-2";
		case 3:
			return "Level 0-3";
		case 4:
			return "Level 0-4";
		case 5:
			return "Level 0-5";
		case 6:
			return "Level 1-1";
		case 7:
			return "Level 1-2";
		case 8:
			return "Level 1-3";
		case 9:
			return "Level 1-4";
		case 10:
			return "Level 2-1";
		case 11:
			return "Level 2-2";
		}
	}
}
