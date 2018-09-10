using System;
using System.Linq;
using System.Collections.Generic;
using PlayerIOClient;
using Yonom.EE;

// I own this :D -Melody

namespace Melody.BlockHandler
{
	public class BlockWorld
	{
		public int[,,] blockArray = new int[2,0,0];
		public bool[] globalSwitchState = new bool[1000];
		public bool[] botSwitchState = new bool[1000];
		public int[] botCoins = new int[2];
		public int botDeath = 0;
		public List<Block> blockList = new List<Block>();
		private static int worldX = 25, worldY = 25, botID = -1;
		public class Block {
			public int x { get; set; }
			public int y { get; set; }
			public int layer { get; set; }
			public int id { get; set; }
			public int placer { get; set; } // Warning, will send -1 if unknown placer.
			public string type { get; set; } // Warning, wil send "null" if init, clear or reset
			public object[] args { get; set; }
		}
		//
		public Block GetBlock(int x, int y, int layer) {
			foreach(Block b in blockList)
				if(b.x == x && b.y == y && b.layer == layer)
					return b;
			return null;
		}
		public Block[] GetBlock(int x, int y) {
			List<Block> a = new List<Block>();
			foreach(Block b in blockList)
				if(b.x == x && b.y == y)
					a.Add(b);
			return a.ToArray();
		}
		public bool isBlock(int x, int y, int layer = 0) {
			if(x < 0 || x > worldX)
				return false;
			if(y < 0 || y > worldY)
				return false;
			return blockArray[layer,x,y] != 0;
		}
		public List<Block> GetBlocksById(int id) {
			return  blockList.Where(x => x.id == id).ToList();
		}
		public List<Block> GetBlocksByLayer(int layer) {
			return blockList.Where(x => x.layer == layer).ToList();
		}
		public List<Block> GetBlocksByXPos(int x) {
			return blockList.Where(a => a.x == x).ToList();
		}
		public List<Block> GetBlocksByYPos(int y) {
			return blockList.Where(x => x.y == y).ToList();
		}
		public List<Block> GetBlocksByRegion(int startx, int endx, int starty, int endy, int layer = 0) {
			return blockList.Where(x => (x.x >= startx && x.x <= endx) && (x.y >= starty && x.y <= endy) && x.layer == layer).ToList();
		}
		public List<Block> GetBlocksByPlacer(int placerID) {
			return blockList.Where(x => x.placer == placerID).ToList();
		}
		public List<Block> GetBlocksByType(string type) {
			return blockList.Where(x => x.type == type).ToList();
		}
		public void Reset() {
			blockArray = new int[2,worldX,worldY];
			blockList = new List<Block>();
		}
		public void HandleMessage(Message e) {
			switch(e.Type) {
					case "init": {
						worldX = e.GetInt(18);
						worldY = e.GetInt(19);
						botID = e.GetInt(5);
						var worldD = new int[2,worldX,worldY];
						var chunks = InitParse.Parse(e);
						foreach(DataChunk chunk in chunks) {
							foreach(Point pos in chunk.Locations) {
								worldD[chunk.Layer, pos.X, pos.Y] = (int) chunk.Type;
								Block t = new Block();
								t.x = pos.X; t.y = pos.Y; t.layer = chunk.Layer; t.args = chunk.Args; t.placer = -1; t.type = "null";
								t.id = (int) chunk.Type;
								blockList.Add(t);
								if(chunk.Type == 467)
									globalSwitchState[int.Parse(chunk.Args[0].ToString())] = false;
								if(chunk.Type == 113)
									botSwitchState[int.Parse(chunk.Args[0].ToString())] = false;
							}
						}
						blockArray = worldD;
						var OSwtich = e.GetByteArray(37);
						foreach(var s in OSwtich)
							globalSwitchState[int.Parse(s.ToString())] = true;
					}
					break;
					case "clear": {
						var worldD = new int[2,worldX,worldY];
						var worldL = new List<Block>();
						var chunks = InitParse.HandleClear(e, worldX, worldY);
						foreach(DataChunk chunk in chunks) {
							foreach(Point pos in chunk.Locations) {
								worldD[chunk.Layer, pos.X, pos.Y] = (int) chunk.Type;
								worldL.Add(new Block() {
								           	x = pos.X,
								           	y = pos.Y,
								           	layer = chunk.Layer,
								           	id = (int) chunk.Type,
								           	args = chunk.Args,
								           	type = "null",
								           	placer = -1
								           });
							}
						}
						blockList = worldL;
						blockArray = worldD;
						globalSwitchState = new bool[1000];
						botSwitchState = new bool[1000];
						botCoins = new int[0];
						botDeath = 0;
					}
					break;
					case "reset": {
						var worldD = new int[2,worldX,worldY];
						var blockL = new List<Block>();
						var chunks = InitParse.Parse(e);
						foreach(DataChunk chunk in chunks) {
							foreach(Point pos in chunk.Locations) {
								worldD[chunk.Layer, pos.X, pos.Y] = (int) chunk.Type;
								blockL.Add(new Block() {
								           	x = pos.X,
								           	y = pos.Y,
								           	layer = chunk.Layer,
								           	id = (int) chunk.Type,
								           	args = chunk.Args,
								           	placer = -1,
								           	type = "null"
								           });
							}
						}
						blockArray = worldD;
						blockList = blockL;
						botSwitchState = new bool[1000];
					}
					break;
					case "resetGlobalSwitches": {
						globalSwitchState = new bool[1000];
					}
					break;
					case "tele": {
						bool containsBot = false;
						for(uint x = 2; x + 4 <= e.Count; x += 4) {
							if(e.GetInt(x) == botID) {
								containsBot = true;
								break;
							}
						}
						if(containsBot) {
							if(e.GetBoolean(0)) {
								botCoins = new int[0];
								botSwitchState = new bool[1000];
								botDeath = 0;
							}
							if(e.GetBoolean(1))
								globalSwitchState = new bool[1000];
						}
					}
					break;
					case "kill": {
						if(e.GetInt(0) == botID)
							botDeath++;
					}
					break;
					case "c": {
						if(e.GetInt(0) == botID) {
							botCoins[0] += e.GetInt(1);
							botCoins[1] += e.GetInt(2);
						}
					}
					break;
					/*case "ps" : {
						if(e.GetInt(0) == botID) {
							if(e.GetInt(1U) == 1) {
								globalSwitchState[e.GetInt(2)] = e.GetBoolean(3);
							} else {
								botSwitchState[e.GetInt(2)] = e.GetBoolean(3);
							}
						}
					}
					break;*/ // Bitch is useless
					case "b":
					case "bc":
					case "br":
					case "bs":
					case "lb":
					case "pt":
					case "ts":
					case "wp": {
						int layer = 0, x = 0, y = 0, id = 0, u = -1;
						string type = e.Type;
						object[] args = {};
						#region Type Adding
						if(e.Type == "b") {
							layer = e.GetInt(0);
							x = e.GetInt(1U);
							y = e.GetInt(2U);
							id = e.GetInt(3U);
							u = e.GetInt(4U);
						} // Regular Block
						else if(e.Type == "bc") {
							layer = 0;
							x = e.GetInt(0U);
							y = e.GetInt(1U);
							id = e.GetInt(2U);
							args = new object[] {e.GetUInt(3)};
							u = e.GetInt(4U);
						} // Block with number value
						else if(e.Type == "br") {
							layer = e.GetInt(4U);
							x = e.GetInt(0U);
							y = e.GetInt(1U);
							id = e.GetInt(2U);
							args = new object[] {e.GetUInt(3)};
							u = e.GetInt(5U);
						} // Morphable block
						else if(e.Type == "bs") {
							layer = 0;
							x = e.GetInt(0U);
							y = e.GetInt(1U);
							id = e.GetInt(2U);
							args = new object[] {e.GetInt(3)};
							u = e.GetInt(4U);
						} // Sound block
						else if(e.Type == "lb") {
							layer = 0;
							x = e.GetInt(0U);
							y = e.GetInt(1U);
							id = e.GetInt(2);
							args = new object[] {e.GetString(3), e.GetString(4)};
							u = e.GetInt(5U);
						} // Label (admin)
						else if(e.Type == "pt") {
							layer = 0;
							x = e.GetInt(0U);
							y = e.GetInt(1U);
							id = e.GetInt(2U);
							args = new object[] {e.GetUInt(3), e.GetUInt(4), e.GetUInt(5)};
							u = e.GetInt(6);
						} // Portal
						else if(e.Type == "ts") {
							layer = 0;
							x = e.GetInt(0U);
							y = e.GetInt(1U);
							id = e.GetInt(2U);
							args = new object[] {e.GetString(3), e.GetUInt(4)};
							u = e.GetInt(5U);
						} // Sign block 
						else if(e.Type == "wp") {
							layer = 0;
							x = e.GetInt(0U);
							y = e.GetInt(1U);
							id = e.GetInt(2U);
							args = new object[] {e.GetString(3)};
							u = e.GetInt(4U);
						} // World Portal
						#endregion
						#region Add to list
						blockArray[layer, x, y] = id;
						List<Block> temp = new List<Block>();
						temp.AddRange(blockList);
						if(id == 0) {
							foreach(Block b in temp)
								if(b.x == x && b.y == y && b.layer == layer)
									blockList.Remove(b);
						} else {
							foreach(Block b in temp) {
								if(b.x == x && b.y == y && b.layer == layer && b.id != id) {
									blockList.Remove(b);
									blockList.Add(new Block() {
									              	x = x,
									              	y = y,
									              	id = id,
									              	layer = layer,
									              	args = args,
									              	placer = u,
									              	type = e.Type
									              });
								}
							}
						}
						#endregion
						if(OnBlockPlace != null)
							OnBlockPlace.Invoke(u, x, y, id, layer, args);
					}
					break;
			}
		}
		public delegate void BlockMessage(int userid, int x, int y, int id, int layer, object[] args);
		public event BlockMessage OnBlockPlace;
	}
}