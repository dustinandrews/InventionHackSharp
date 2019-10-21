/*
	TileData.json Contract.
 */

using System.Collections.Generic;

namespace MegaDungeon.Contracts
{
	public class TileData
	{
		public int tile_size;
		public Dictionary<string,TileDataData> data;
		
	}
	
		public class TileDataData
	{
		public string name;
		public string type;
	}

	public interface ITileManager
	{
		string GetTileName(int index);
		byte[] GetTileBmpBytes(int index);
	}
}