using System;
using System.Linq;
//
using PlayerIOClient;
//
using Melody.Helper;
using Melody.EEHelper;
using Melody.BlockHandler;
//
using NLua;

namespace LuaEE
{
	class Program
	{
		public static void Main(string[] args)
		{
			"Write the name of the file you want to run (leave blank to use the default file): ".Write();
			string temp = Console.ReadLine().TrimEnd(new [] {'.','l','u','a'});
			file = temp == "" ? "bot" : temp ;
			botlua.LoadCLRPackage();
			botlua["Connect"] = new Connect();
			botlua["Bot"] = new Bot();
			botlua["BlockWorld"] = new BlockWorld();
			botlua.DoFile(@"{0}{1}.lua".MelodyFormat(LuaDir,file));
			botlua.GetFunction("init").Call();
			while(true) {
				string a = Console.ReadLine();
				if(a == "-restart") {
					if(isConnectedToLobby || isConnectedToWorld)
						con.Disconnect();
					con = null;
					client = null;
					isConnectedToWorld = false;
					isConnectedToLobby = false;
					isConnectedToServer = false;
					isGuest = false;
					botlua.DoFile(@"{0}{1}.lua".MelodyFormat(LuaDir,file));
					botlua.GetFunction("init").Call();
				}
				if(a == "-quit" || a == "-exit")
					Environment.Exit(0);
				if(a.StartsWith("-cmd ") && isConnectedToWorld)
					con.Send("say", "/"+a.Remove(0,5));
				if(a.StartsWith("-say ") && isConnectedToWorld)
					con.Send("say", a.Remove(0,5));
			}
		}
		public static string LuaDir = "Lua/";
		public static string file = "bot";
		public static Lua botlua = new Lua();
		public static bool isConnectedToWorld = false;
		public static bool isConnectedToLobby = false;
		public static bool isConnectedToServer = false;
		public static bool isGuest = false;
		public static Connection con;
		public static Client client;
		public class Connect {
			public void ToServer(string email, string password) {
				if(isConnectedToLobby || isConnectedToWorld)
					return;
				if("{0}{1}".MelodyFormat(email.ToLower(),password.ToLower()) == "guestguest")
					isGuest = true;
				client = PlayerIO.QuickConnect.SimpleConnect(EEHelp.GameID, email, password, null);
				isConnectedToServer = true;
			}
			public void ToWorld(string worldID) {
				if((isConnectedToLobby || isConnectedToWorld) && !isConnectedToServer)
					return;
				con = client.Multiplayer.CreateJoinRoom(worldID, "{0}{1}".MelodyFormat(worldID.RoomType(), client.GameVer()), true, null, null);
				con.OnMessage += async (sender, e) => {
					if(botlua["onMsg"] is LuaFunction) { botlua.GetFunction("onMsg").Call(e); }
					if(botlua["on"+e.Type.CapitalFirst()] is LuaFunction) { botlua.GetFunction("on"+e.Type.CapitalFirst()).Call(e); }
					else if(botlua["on"+e.Type] is LuaFunction) { botlua.GetFunction("on"+e.Type).Call(e); }
				};
				con.OnDisconnect += async (sender, message) => {
					if(botlua["onDisconnect"] is LuaFunction) { botlua.GetFunction("onDisconnect").Call(message); }
				};
				isConnectedToWorld = true;
			}
			public void ToLobby() {
				if((isConnectedToLobby || isConnectedToWorld) && !isConnectedToServer)
					return;
				con = client.Multiplayer.CreateJoinRoom(client.ConnectUserId, "{1}Lobby{0}".MelodyFormat(client.GameVer(), isGuest ? "Guest" : ""), true, null, null);
				con.OnMessage += async (sender, e) => {
					if(botlua["on"+e.Type.CapitalFirst()] is LuaFunction) { botlua.GetFunction("on"+e.Type.CapitalFirst()).Call(e); }
					else if(botlua["on"+e.Type] is LuaFunction) { botlua.GetFunction("on"+e.Type).Call(e); }
				};
				con.OnDisconnect += async (sender, message) => {
					if(botlua["onDisconnect"] is LuaFunction) { botlua.GetFunction("onDisconnect").Call(message); }
				};
				isConnectedToLobby = true;
			}
		}
		public class Bot {
			public void Send(string msg) { con.Send(msg); }
			public void Send(string msg, params string[] args) { con.Send(msg, args); }
		}
	}
}