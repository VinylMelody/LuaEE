/*
 * Melody's Player Helper .cs
 * (aka lazy way to do EE things)
*/
// I'll consider using Proc's FutureProof addon incase I leave EE
using System;
using System.Linq;
using PlayerIOClient;

namespace Melody.EEHelper
{
	public static class ExtendedEEHelp {
		public static string GameVer(this Client c) {
			return c.BigDB.Load("config","config")["version"].ToString();
		}
		public static string RoomType(this string w) {
			if(w.StartsWith("PW"))
				return "Everybodyedits";
			if(w.StartsWith("BW"))
				return "Beta";
			return null;
		}
	}
	public class EEHelp
	{
		/// <summary>
		/// Gets the player who sent the message's id
		/// </summary>
		/// <returns>An integer to be used on your Player list</returns>
		public static int getPlayerUsingMType(Message m) {
			switch(m.Type) {
				case "add":
				case "aura":
				case "autotext":
				case "admin":
				case "badgeChange":
				case "c":
				case "editRights":
				case "effect":
				case "face":
				case "god":
				case "k":
				case "kill":
				case "ks":
				case "left":
				case "m":
				case "mod":
				case "ps":
				case "psi":
				case "say":
				case "smileyGoldBorder":
				case "team":
				case "teleport":
				case "toggleGod":
					return m.GetInt(0);
				/*case "hide":
					return m.GetInt(1);*/ // Commented because error :/
				case "b":
				case "bc":
				case "bs":
				case "wp":
					return m.GetInt(4U);
				case "br":
				case "lb":
				case "ts":
					return m.GetInt(5U);
				case "pt":
					return m.GetInt(6);
				default:
					return -1;
			}
		}
		public static string GameID = "everybody-edits-su9rn58o40itdbnw69plyw";

	}
}
