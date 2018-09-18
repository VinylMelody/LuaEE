using System;
using System.IO;
using Melody.EEHelper;
using Melody.BlockHandler;
using PlayerIOClient;
using DynamicLua;

namespace LuaEE
{
	class Program
	{
		
		public static void Main(string[] args)
		{
        Restart:
            Console.Clear();
            if (con != null)
                con.Disconnect();
            con = null;
            client = null;
            connectedToLobby = false;
            connectedToServer = false;
            connectedToWorld = false;
            string dir = "";
			if (args.Length == 1 && Directory.Exists(args[0])) {
				dir = args[0];
			}
			Console.Write("Input directory here: ");
			while (true) {
				dir = Console.ReadLine();
				if (Directory.Exists(dir)) {
					if (!File.Exists(dir + "\\main.lua"))
						Console.WriteLine("MAIN.LUA DOESN'T EXIST INSIDE DIRECTORY!");
					else
						break;
				}
				Console.Write("(INVALID DIRECTORY) Input directory here: ");
			}
            Reset:
			lua = new DynamicLua.DynamicLua();
			try
            {
                lua.DoFile(dir + "\\main.lua");
            }
            catch (NLua.Exceptions.LuaScriptException ex)
            {
                Console.WriteLine("(ERROR) > " + ex.Message);
                goto Restart;
            }
			lua.trace = new Action<string>((m) => Console.WriteLine(m)); // Fuck it, why not
            lua.BlockWorld = new BlockWorld();
            //
			if (lua.init.GetType() != typeof(DynamicLuaFunction)) {
				Console.WriteLine("Init function is missing!");
				goto Restart;
			}
			//
			dynamic _conFunc = lua.NewTable("Connect");
			_conFunc.ToServer = new Action<string,string,NLua.LuaFunction>((e, p, f) => {
                if (connectedToServer)
                    return;
				object res=null;
                try
                {
                    client = PlayerIO.QuickConnect.SimpleConnect(EEHelp.GameID, e, p, null);
                    connectedToServer = true;
                    f.Call();
                }
                catch (PlayerIOError error)
                {
                    f.Call(error.ToString());
                }
			});
			_conFunc.ToWorld = new Action<string,NLua.LuaFunction>((w,f) => {
                if (connectedToWorld || !connectedToServer)
                    return;
                try
                {
                    con = client.Multiplayer.CreateJoinRoom(w, w.RoomType() + client.GameVer(), true, null, null);
                    con.OnMessage += con_OnMessage;
                    con.OnDisconnect += con_OnDisconnect;
                    connectedToWorld = true;
                    f.Call();
                }
                catch (PlayerIOError error)
                {
                    f.Call(error.ToString());
                }

			});
			//
			dynamic _botfunc = lua.NewTable("Bot");
            _botfunc.Send = new Action<string>((s)=> { if (connectedToWorld) con.Send(s); });
            _botfunc.SendArgs = new Action<string,NLua.LuaTable>((s,a) => {
                if (connectedToWorld)
                {
                    object[] values = new object[a.Values.Count];
                    a.Values.CopyTo(values, 0);
                    con.Send(s, values);
                }
            });
            _botfunc.Disconnect = new Action(() =>
            {
                con.Disconnect();
            });
            //
            try
            {
                lua.init(dir);
            }
            catch (NLua.Exceptions.LuaScriptException ex)
            {
                Console.WriteLine("(Lua Script Error) > " + ex.ToString());
                Console.WriteLine("Press any key to restart");
                Console.ReadKey();
                goto Restart;
            } catch (Exception ex)
            {
                Console.WriteLine("(Error) > " + ex.ToString());
                Console.WriteLine("Press any key to restart");
                Console.ReadKey();
                goto Restart;
            }
			//
			Console.WriteLine("Finished!");
            while(true)
            {
                string cmd = Console.ReadLine();
                if(cmd == "-restart")
                {
                    goto Restart;
                }
                if (cmd == "-reset")
                {
                    if (con != null)
                        con.Disconnect();
                    con = null;
                    client = null;
                    connectedToLobby = false;
                    connectedToServer = false;
                    connectedToWorld = false;
                    Console.Clear();
                    goto Reset;
                }
            }
			Console.ReadLine();
		}
       
        static void con_OnMessage(object sender, Message e)
		{
            try
            {
                if (lua.onMsg != null)
                    lua.onMsg(e.Type, e);
                //
                if (lua["on" + e.Type] != null)
                    lua["on" + e.Type](e);
                if (lua["on" + Uppercase(e.Type)] != null)
                    lua["on" + Uppercase(e.Type)](e);
            }
            catch (NLua.Exceptions.LuaScriptException ex)
            {
                Console.WriteLine("(Lua Script Error) > " + ex.ToString());
            }
        }
		
		//https://stackoverflow.com/a/21755933
		private static string Uppercase(string k) {
			if(k != string.Empty && char.IsLower(k[0])) {
				return char.ToUpper(k[0]) + k.Substring(1);
			}
			return k;
		}

        public static void Send(string msg) {
			if(connectedToWorld) {
				con.Send(msg);
			} 
		}
		public static void SendArgs(string msg, params object[] args) {
            foreach(object v in args)
            {
                Console.WriteLine(v);
            }
			if(connectedToWorld) {
				con.Send(msg, args);
			} 
		}

        static void con_OnDisconnect(object sender, string message)
		{
            if(lua.onDisconnect != null)
                lua.onDisconnect(message);
            con = null;
            client = null;
            connectedToLobby = false;
            connectedToServer = false;
            connectedToWorld = false;
        }
		public static dynamic lua = new DynamicLua.DynamicLua();
		public static bool connectedToServer,connectedToLobby,connectedToWorld;
		public static Connection con;
		public static Client client;
	}
}