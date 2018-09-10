using System;
using System.Collections.Generic;
using PlayerIOClient;

// THIS IS YONOM'S INITPARSE
// I JUST ADDED A CLEAR MESSAGE HELPER
// SINCE I'M TOO LAZY TO PUT IT ON MY BLOCKHANDLER
// AND HOW YONOM DOES IT IS P RADs
// NOTHING WAS CHANGED HERE. ONLY STUFFS ARE ADDED
// SO, UH, CREDITS TO YONOM FOR THE ORIGINAL PARTS
// PLS GET PERMISSION FROM ME AND YONOM (BUT MOSTLY YONOM) BEFOER USING THIS

namespace Yonom.EE
{
    public static class InitParse
    {
        public static DataChunk[] Parse(Message m)
        {
            if (m == null) throw new ArgumentNullException("m");
            if (m.Type != "init" && m.Type != "reset") throw new ArgumentException("Invalid message type.", "m");
            
            // Get world data
	        var p = 0u;
	        var data = new Stack<object>();
	        while (m[p++] as string != "ws") { }
	        while (m[p] as string != "we") { data.Push(m[p++]); }
	        
	        // Parse world data
	        var chunks = new List<DataChunk>();
	        while (data.Count > 0)
	        {
	            var args = new Stack<object>();
	            while (!(data.Peek() is byte[]))
	            args.Push(data.Pop());
	      
	            var ys = (byte[])data.Pop();
                var xs = (byte[])data.Pop();
                var layer = (int)data.Pop();
                var type = (uint)data.Pop();

                chunks.Add(new DataChunk(layer, type, xs, ys, args.ToArray()));
            }
	
            return chunks.ToArray();
        }
        public static DataChunk[] HandleClear(Message m, int worldx, int worldy) {
        	// hi, i'm melody
        	if (m == null) throw new ArgumentNullException("m");
            if (m.Type != "clear") throw new ArgumentException("Invalid message type.", "m");
            
            var chunks = new List<DataChunk>();
            var points = new List<Point>();
            for(int x = 0; x < worldx; x++) {
            	points.Add(new Point(x, 0));
            	points.Add(new Point(x, worldy-1));
            }
            for(int y = 1; y < worldy-1; y++) {
            	points.Add(new Point(0, y));
            	points.Add(new Point(worldx-1, y));
            }
            
            chunks.Add(new DataChunk(0, 9, points.ToArray(), new object[] {}));
            
            return chunks.ToArray();
        }
    }
    
    public class DataChunk
    {
        public int Layer { get; set; }
        public uint Type { get; set; }
        public Point[] Locations { get; set; }
        public object[] Args { get; set; }

        public DataChunk(int layer, uint type, byte[] xs, byte[] ys, object[] args)
        {
            this.Layer = layer;
            this.Type = type;
            this.Args = args;
            this.Locations = GetLocations(xs, ys);
        }
        
        // I don't know how byte[] works so I'll just add the Point[] directly -Melody
        public DataChunk(int layer, uint type, Point[] pos, object[] args)
        {
            this.Layer = layer;
            this.Type = type;
            this.Args = args;
            this.Locations = pos;
        }

        private static Point[] GetLocations(byte[] xs, byte[] ys)
        {
            var points = new List<Point>();
            for (var i = 0; i < xs.Length; i += 2)
                points.Add(new Point(
                    (xs[i] << 8) | xs[i + 1],
                    (ys[i] << 8) | ys[i + 1]));
            return points.ToArray();
        }
    }

    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y) : this()
        {
            this.X = x;
            this.Y = y;
        }
    }
}