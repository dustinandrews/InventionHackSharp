using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Portable
{
	public class Tiles
	{
		Image _tileset;
		int _tileSize;
		TileData _tileData;

		public Tiles(string tileImageFile, string tileDataJson)
		{
			_tileset = Image.Load(tileImageFile);
			_tileData = JsonConvert.DeserializeObject<TileData>(System.IO.File.ReadAllText(tileDataJson));
			_tileSize = _tileData.tile_size;
		}

		public string GetTileName(int index)
		{
			return _tileData.data[index.ToString()].name;
		}

		public byte[] GetTileBmpBytes(int index)
		{
			var x = (index * _tileSize) % _tileset.Width;
			var y = _tileSize * (int)((index * _tileSize) / _tileset.Width);
			var tile = _tileset.Clone( m => m.Crop(new Rectangle(x,y,_tileSize,_tileSize)));

			byte[] imgBytes;
			using(var memstream = new MemoryStream())
			{
				var enc = new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder();
				enc.BitsPerPixel = SixLabors.ImageSharp.Formats.Bmp.BmpBitsPerPixel.Pixel32;
				tile.Save(memstream, enc);
				imgBytes = new Byte[memstream.Length];
				memstream.Seek(0, SeekOrigin.Begin);
				memstream.Read(imgBytes, 0, imgBytes.Length);
			}
			return imgBytes;
		}
	}

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
}