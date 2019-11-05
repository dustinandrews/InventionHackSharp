using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;
using Newtonsoft.Json;
using System.Collections.Generic;
using MegaDungeon.Contracts;
using SixLabors.ImageSharp.PixelFormats;

namespace Portable
{
	public class TileManager : ITileManager
	{
		Image _tileset;
		int _tileSize;
		TileData _tileData;

		public Inv.Image TransparentDarken;

		Dictionary<int, Inv.Image> _tileCache = new Dictionary<int, Inv.Image>();
		Dictionary<int, Inv.Image> _darkTileCache = new Dictionary<int, Inv.Image>();

		public TileManager(string tileImageFile, string tileDataJson)
		{
			_tileset = Image.Load(tileImageFile);
			_tileData = JsonConvert.DeserializeObject<TileData>(tileDataJson);
			_tileSize = _tileData.tile_size;
		}

		public Inv.Image GetInvImage(int index)
		{
			if(!_tileCache.ContainsKey(index))
			{
				var invImage = new Inv.Image(GetTileBmpBytes(index), "bmp");
				_tileCache.Add(index, invImage);
			}
			return _tileCache[index];
		}

		public Inv.Image GetInvImageDark(int index)
		{
			Inv.Image invImage;
			if(_darkTileCache.ContainsKey(index))
			{
				invImage = _darkTileCache[index];
			}
			else
			{
				var tile = GetTileAsSLImage(index);
				tile.Mutate(m => m.Brightness(0.5F));
				invImage = new Inv.Image(GetBytesFromImage(tile), "bmp");
				_darkTileCache[index] = invImage;
			}
			return invImage;
		}
		public string GetTileName(int index)
		{
			return _tileData.data[index.ToString()].name;
		}

		public byte[] GetTileBmpBytes(int index)
		{
			Image tile = GetTileAsSLImage(index);
			return GetBytesFromImage(tile);
		}

		public Image GetTileAsSLImage(int index)
		{
			var x = (index * _tileSize) % _tileset.Width;
			var y = _tileSize * (int)((index * _tileSize) / _tileset.Width);
			var tile = _tileset.Clone(m => m.Crop(new Rectangle(x, y, _tileSize, _tileSize)));
			return tile;
		}

		private static byte[] GetBytesFromImage(Image tile)
		{
			byte[] imgBytes;
			using (var memstream = new MemoryStream())
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

		Image DrawImageOnImageExample(Image tile)
		{
			var borderSize = 8;
			var borderRect = new Rectangle(borderSize, 0, tile.Width - (borderSize * 2), borderSize);
			var borderImage = tile.Clone(c => c.Crop(borderRect));
			var point = new Point(borderSize, 0);
			var newTile = new Image<Rgba32>(tile.Width, tile.Height);
			newTile.Mutate( i => i.DrawImage(borderImage,point, 1.0F));
			
			return newTile;
		}
	}
}