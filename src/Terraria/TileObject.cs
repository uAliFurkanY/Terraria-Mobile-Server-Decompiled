using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ObjectData;

namespace Terraria
{
	public struct TileObject
	{
		public int xCoord;

		public int yCoord;

		public int type;

		public int style;

		public int alternate;

		public int random;

		public static TileObject Empty = default(TileObject);

		public static TileObjectPreviewData objectPreview = new TileObjectPreviewData();

		public static bool Place(TileObject toBePlaced)
		{
			TileObjectData tileData = TileObjectData.GetTileData(toBePlaced.type, toBePlaced.style, toBePlaced.alternate);
			if (tileData == null)
			{
				return false;
			}
			if (tileData.HookPlaceOverride.hook != null)
			{
				int arg;
				int arg2;
				if (tileData.HookPlaceOverride.processedCoordinates)
				{
					arg = toBePlaced.xCoord;
					arg2 = toBePlaced.yCoord;
				}
				else
				{
					arg = toBePlaced.xCoord + tileData.Origin.X;
					arg2 = toBePlaced.yCoord + tileData.Origin.Y;
				}
				if (tileData.HookPlaceOverride.hook(arg, arg2, toBePlaced.type, toBePlaced.style, 1) == tileData.HookPlaceOverride.badReturn)
				{
					return false;
				}
			}
			else
			{
				ushort num = (ushort)toBePlaced.type;
				int num2 = 0;
				int num3 = 0;
				int num4 = tileData.CalculatePlacementStyle(toBePlaced.style, toBePlaced.alternate, toBePlaced.random);
				int num5 = 0;
				if (tileData.StyleWrapLimit > 0)
				{
					num5 = num4 / tileData.StyleWrapLimit;
					num4 %= tileData.StyleWrapLimit;
				}
				if (tileData.StyleHorizontal)
				{
					num2 = tileData.CoordinateFullWidth * num4;
					num3 = tileData.CoordinateFullHeight * num5;
				}
				else
				{
					num2 = tileData.CoordinateFullWidth * num5;
					num3 = tileData.CoordinateFullHeight * num4;
				}
				int num6 = toBePlaced.xCoord;
				int num7 = toBePlaced.yCoord;
				for (int i = 0; i < tileData.Width; i++)
				{
					for (int j = 0; j < tileData.Height; j++)
					{
						Tile tileSafely = Framing.GetTileSafely(num6 + i, num7 + j);
						if (tileSafely.active() && Main.tileCut[tileSafely.type])
						{
							WorldGen.KillTile(num6 + i, num7 + j);
						}
					}
				}
				for (int k = 0; k < tileData.Width; k++)
				{
					int num8 = num2 + k * (tileData.CoordinateWidth + tileData.CoordinatePadding);
					int num9 = num3;
					for (int l = 0; l < tileData.Height; l++)
					{
						Tile tileSafely2 = Framing.GetTileSafely(num6 + k, num7 + l);
						if (!tileSafely2.active())
						{
							tileSafely2.active(true);
							tileSafely2.frameX = (short)num8;
							tileSafely2.frameY = (short)num9;
							tileSafely2.type = num;
						}
						num9 += tileData.CoordinateHeights[l] + tileData.CoordinatePadding;
					}
				}
			}
			if (tileData.FlattenAnchors)
			{
				AnchorData anchorBottom = tileData.AnchorBottom;
				if (anchorBottom.tileCount != 0 && (anchorBottom.type & AnchorType.SolidTile) == AnchorType.SolidTile)
				{
					int num10 = toBePlaced.xCoord + anchorBottom.checkStart;
					int j2 = toBePlaced.yCoord + tileData.Height;
					for (int m = 0; m < anchorBottom.tileCount; m++)
					{
						Tile tileSafely3 = Framing.GetTileSafely(num10 + m, j2);
						if (Main.tileSolid[tileSafely3.type] && !Main.tileSolidTop[tileSafely3.type] && tileSafely3.blockType() != 0)
						{
							WorldGen.SlopeTile(num10 + m, j2);
						}
					}
				}
				anchorBottom = tileData.AnchorTop;
				if (anchorBottom.tileCount != 0 && (anchorBottom.type & AnchorType.SolidTile) == AnchorType.SolidTile)
				{
					int num11 = toBePlaced.xCoord + anchorBottom.checkStart;
					int j3 = toBePlaced.yCoord - 1;
					for (int n = 0; n < anchorBottom.tileCount; n++)
					{
						Tile tileSafely4 = Framing.GetTileSafely(num11 + n, j3);
						if (Main.tileSolid[tileSafely4.type] && !Main.tileSolidTop[tileSafely4.type] && tileSafely4.blockType() != 0)
						{
							WorldGen.SlopeTile(num11 + n, j3);
						}
					}
				}
				anchorBottom = tileData.AnchorRight;
				if (anchorBottom.tileCount != 0 && (anchorBottom.type & AnchorType.SolidTile) == AnchorType.SolidTile)
				{
					int i2 = toBePlaced.xCoord + tileData.Width;
					int num12 = toBePlaced.yCoord + anchorBottom.checkStart;
					for (int num13 = 0; num13 < anchorBottom.tileCount; num13++)
					{
						Tile tileSafely5 = Framing.GetTileSafely(i2, num12 + num13);
						if (Main.tileSolid[tileSafely5.type] && !Main.tileSolidTop[tileSafely5.type] && tileSafely5.blockType() != 0)
						{
							WorldGen.SlopeTile(i2, num12 + num13);
						}
					}
				}
				anchorBottom = tileData.AnchorLeft;
				if (anchorBottom.tileCount != 0 && (anchorBottom.type & AnchorType.SolidTile) == AnchorType.SolidTile)
				{
					int i3 = toBePlaced.xCoord - 1;
					int num14 = toBePlaced.yCoord + anchorBottom.checkStart;
					for (int num15 = 0; num15 < anchorBottom.tileCount; num15++)
					{
						Tile tileSafely6 = Framing.GetTileSafely(i3, num14 + num15);
						if (Main.tileSolid[tileSafely6.type] && !Main.tileSolidTop[tileSafely6.type] && tileSafely6.blockType() != 0)
						{
							WorldGen.SlopeTile(i3, num14 + num15);
						}
					}
				}
			}
			return true;
		}

		public static bool CanPlace(int x, int y, int type, int style, int dir, out TileObject objectData, bool onlyCheck = false)
		{
			TileObjectData tileData = TileObjectData.GetTileData(type, style);
			objectData = Empty;
			if (tileData == null)
			{
				return false;
			}
			int num = x - tileData.Origin.X;
			int num2 = y - tileData.Origin.Y;
			if (num < 0 || num + tileData.Width >= Main.maxTilesX || num2 < 0 || num2 + tileData.Height >= Main.maxTilesY)
			{
				return false;
			}
			bool flag = tileData.RandomStyleRange > 0;
			if (TileObjectPreviewData.placementCache == null)
			{
				TileObjectPreviewData.placementCache = new TileObjectPreviewData();
			}
			TileObjectPreviewData.placementCache.Reset();
			int num3 = 0;
			int num4 = 0;
			if (tileData.AlternatesCount != 0)
			{
				num4 = tileData.AlternatesCount;
			}
			float num5 = -1f;
			float num6 = -1f;
			int num7 = 0;
			TileObjectData tileObjectData = null;
			int num8 = num3 - 1;
			while (num8 < num4)
			{
				num8++;
				TileObjectData tileData2 = TileObjectData.GetTileData(type, style, num8);
				if (tileData2.Direction != 0 && ((tileData2.Direction == TileObjectDirection.PlaceLeft && dir == 1) || (tileData2.Direction == TileObjectDirection.PlaceRight && dir == -1)))
				{
					continue;
				}
				int num9 = x - tileData2.Origin.X;
				int num10 = y - tileData2.Origin.Y;
				if (num9 < 5 || num9 + tileData2.Width > Main.maxTilesX - 5 || num10 < 5 || num10 + tileData2.Height > Main.maxTilesY - 5)
				{
					return false;
				}
				Rectangle rectangle = new Rectangle(0, 0, tileData2.Width, tileData2.Height);
				int num11 = 0;
				int num12 = 0;
				if (tileData2.AnchorTop.tileCount != 0)
				{
					if (rectangle.Y == 0)
					{
						rectangle.Y = -1;
						rectangle.Height++;
						num12++;
					}
					int checkStart = tileData2.AnchorTop.checkStart;
					if (checkStart < rectangle.X)
					{
						rectangle.Width += rectangle.X - checkStart;
						num11 += rectangle.X - checkStart;
						rectangle.X = checkStart;
					}
					int num13 = checkStart + tileData2.AnchorTop.tileCount - 1;
					int num14 = rectangle.X + rectangle.Width - 1;
					if (num13 > num14)
					{
						rectangle.Width += num13 - num14;
					}
				}
				if (tileData2.AnchorBottom.tileCount != 0)
				{
					if (rectangle.Y + rectangle.Height == tileData2.Height)
					{
						rectangle.Height++;
					}
					int checkStart2 = tileData2.AnchorBottom.checkStart;
					if (checkStart2 < rectangle.X)
					{
						rectangle.Width += rectangle.X - checkStart2;
						num11 += rectangle.X - checkStart2;
						rectangle.X = checkStart2;
					}
					int num15 = checkStart2 + tileData2.AnchorBottom.tileCount - 1;
					int num16 = rectangle.X + rectangle.Width - 1;
					if (num15 > num16)
					{
						rectangle.Width += num15 - num16;
					}
				}
				if (tileData2.AnchorLeft.tileCount != 0)
				{
					if (rectangle.X == 0)
					{
						rectangle.X = -1;
						rectangle.Width++;
						num11++;
					}
					int num17 = tileData2.AnchorLeft.checkStart;
					if ((tileData2.AnchorLeft.type & AnchorType.Tree) == AnchorType.Tree)
					{
						num17--;
					}
					if (num17 < rectangle.Y)
					{
						rectangle.Width += rectangle.Y - num17;
						num12 += rectangle.Y - num17;
						rectangle.Y = num17;
					}
					int num18 = num17 + tileData2.AnchorLeft.tileCount - 1;
					if ((tileData2.AnchorLeft.type & AnchorType.Tree) == AnchorType.Tree)
					{
						num18 += 2;
					}
					int num19 = rectangle.Y + rectangle.Height - 1;
					if (num18 > num19)
					{
						rectangle.Height += num18 - num19;
					}
				}
				if (tileData2.AnchorRight.tileCount != 0)
				{
					if (rectangle.X + rectangle.Width == tileData2.Width)
					{
						rectangle.Width++;
					}
					int num20 = tileData2.AnchorLeft.checkStart;
					if ((tileData2.AnchorRight.type & AnchorType.Tree) == AnchorType.Tree)
					{
						num20--;
					}
					if (num20 < rectangle.Y)
					{
						rectangle.Width += rectangle.Y - num20;
						num12 += rectangle.Y - num20;
						rectangle.Y = num20;
					}
					int num21 = num20 + tileData2.AnchorRight.tileCount - 1;
					if ((tileData2.AnchorRight.type & AnchorType.Tree) == AnchorType.Tree)
					{
						num21 += 2;
					}
					int num22 = rectangle.Y + rectangle.Height - 1;
					if (num21 > num22)
					{
						rectangle.Height += num21 - num22;
					}
				}
				if (onlyCheck)
				{
					objectPreview.Reset();
					objectPreview.Active = true;
					objectPreview.Type = (ushort)type;
					objectPreview.Style = (short)style;
					objectPreview.Alternate = num8;
					objectPreview.Size = new Point16(rectangle.Width, rectangle.Height);
					objectPreview.ObjectStart = new Point16(num11, num12);
					objectPreview.Coordinates = new Point16(num9 - num11, num10 - num12);
				}
				float num23 = 0f;
				float num24 = tileData2.Width * tileData2.Height;
				float num25 = 0f;
				float num26 = 0f;
				for (int i = 0; i < tileData2.Width; i++)
				{
					for (int j = 0; j < tileData2.Height; j++)
					{
						Tile tileSafely = Framing.GetTileSafely(num9 + i, num10 + j);
						bool flag2 = !tileData2.LiquidPlace(tileSafely);
						bool flag3 = false;
						if (tileData2.AnchorWall)
						{
							num26 += 1f;
							if (!tileData2.isValidWallAnchor(tileSafely.wall))
							{
								flag3 = true;
							}
							else
							{
								num25 += 1f;
							}
						}
						bool flag4 = false;
						if (tileSafely.active() && !Main.tileCut[tileSafely.type])
						{
							flag4 = true;
						}
						if (flag4 || flag2 || flag3)
						{
							if (onlyCheck)
							{
								objectPreview[i + num11, j + num12] = 2;
							}
							continue;
						}
						if (onlyCheck)
						{
							objectPreview[i + num11, j + num12] = 1;
						}
						num23 += 1f;
					}
				}
				AnchorData anchorBottom = tileData2.AnchorBottom;
				if (anchorBottom.tileCount != 0)
				{
					num26 += (float)anchorBottom.tileCount;
					int height = tileData2.Height;
					for (int k = 0; k < anchorBottom.tileCount; k++)
					{
						int num27 = anchorBottom.checkStart + k;
						Tile tileSafely = Framing.GetTileSafely(num9 + num27, num10 + height);
						bool flag5 = false;
						if (tileSafely.nactive())
						{
							if ((anchorBottom.type & AnchorType.SolidTile) == AnchorType.SolidTile && Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type] && !Main.tileNoAttach[tileSafely.type] && (tileData2.FlattenAnchors || tileSafely.blockType() == 0))
							{
								flag5 = tileData2.isValidTileAnchor(tileSafely.type);
							}
							if (!flag5 && ((anchorBottom.type & AnchorType.SolidWithTop) == AnchorType.SolidWithTop || (anchorBottom.type & AnchorType.Table) == AnchorType.Table))
							{
								if (tileSafely.type == 19)
								{
									int num28 = tileSafely.frameX / TileObjectData.PlatformFrameWidth();
									if ((!tileSafely.halfBrick() && num28 >= 0 && num28 <= 7) || (num28 >= 12 && num28 <= 16) || (num28 >= 25 && num28 <= 26))
									{
										flag5 = true;
									}
								}
								else if (Main.tileSolid[tileSafely.type] && Main.tileSolidTop[tileSafely.type])
								{
									flag5 = true;
								}
							}
							if (!flag5 && (anchorBottom.type & AnchorType.Table) == AnchorType.Table && tileSafely.type != 19 && Main.tileTable[tileSafely.type] && tileSafely.blockType() == 0)
							{
								flag5 = true;
							}
							if (!flag5 && (anchorBottom.type & AnchorType.SolidSide) == AnchorType.SolidSide && Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type])
							{
								switch (tileSafely.blockType())
								{
								case 4:
								case 5:
									flag5 = tileData2.isValidTileAnchor(tileSafely.type);
									break;
								}
							}
							if (!flag5 && (anchorBottom.type & AnchorType.AlternateTile) == AnchorType.AlternateTile && tileData2.isValidAlternateAnchor(tileSafely.type))
							{
								flag5 = true;
							}
						}
						else if (!flag5 && (anchorBottom.type & AnchorType.EmptyTile) == AnchorType.EmptyTile)
						{
							flag5 = true;
						}
						if (!flag5)
						{
							if (onlyCheck)
							{
								objectPreview[num27 + num11, height + num12] = 2;
							}
							continue;
						}
						if (onlyCheck)
						{
							objectPreview[num27 + num11, height + num12] = 1;
						}
						num25 += 1f;
					}
				}
				anchorBottom = tileData2.AnchorTop;
				if (anchorBottom.tileCount != 0)
				{
					num26 += (float)anchorBottom.tileCount;
					int num29 = -1;
					for (int l = 0; l < anchorBottom.tileCount; l++)
					{
						int num30 = anchorBottom.checkStart + l;
						Tile tileSafely = Framing.GetTileSafely(num9 + num30, num10 + num29);
						bool flag6 = false;
						if (tileSafely.nactive())
						{
							if (Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type] && !Main.tileNoAttach[tileSafely.type] && (tileData2.FlattenAnchors || tileSafely.blockType() == 0))
							{
								flag6 = tileData2.isValidTileAnchor(tileSafely.type);
							}
							if (!flag6 && (anchorBottom.type & AnchorType.SolidBottom) == AnchorType.SolidBottom && ((Main.tileSolid[tileSafely.type] && (!Main.tileSolidTop[tileSafely.type] || (tileSafely.type == 19 && (tileSafely.halfBrick() || tileSafely.topSlope())))) || tileSafely.halfBrick() || tileSafely.topSlope()) && !TileID.Sets.NotReallySolid[tileSafely.type] && !tileSafely.bottomSlope())
							{
								flag6 = tileData2.isValidTileAnchor(tileSafely.type);
							}
							if (!flag6 && (anchorBottom.type & AnchorType.SolidSide) == AnchorType.SolidSide && Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type])
							{
								switch (tileSafely.blockType())
								{
								case 2:
								case 3:
									flag6 = tileData2.isValidTileAnchor(tileSafely.type);
									break;
								}
							}
							if (!flag6 && (anchorBottom.type & AnchorType.AlternateTile) == AnchorType.AlternateTile && tileData2.isValidAlternateAnchor(tileSafely.type))
							{
								flag6 = true;
							}
						}
						else if (!flag6 && (anchorBottom.type & AnchorType.EmptyTile) == AnchorType.EmptyTile)
						{
							flag6 = true;
						}
						if (!flag6)
						{
							if (onlyCheck)
							{
								objectPreview[num30 + num11, num29 + num12] = 2;
							}
							continue;
						}
						if (onlyCheck)
						{
							objectPreview[num30 + num11, num29 + num12] = 1;
						}
						num25 += 1f;
					}
				}
				anchorBottom = tileData2.AnchorRight;
				if (anchorBottom.tileCount != 0)
				{
					num26 += (float)anchorBottom.tileCount;
					int width = tileData2.Width;
					for (int m = 0; m < anchorBottom.tileCount; m++)
					{
						int num31 = anchorBottom.checkStart + m;
						Tile tileSafely = Framing.GetTileSafely(num9 + width, num10 + num31);
						bool flag7 = false;
						if (tileSafely.nactive())
						{
							if (Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type] && !Main.tileNoAttach[tileSafely.type] && (tileData2.FlattenAnchors || tileSafely.blockType() == 0))
							{
								flag7 = tileData2.isValidTileAnchor(tileSafely.type);
							}
							if (!flag7 && (anchorBottom.type & AnchorType.SolidSide) == AnchorType.SolidSide && Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type])
							{
								switch (tileSafely.blockType())
								{
								case 2:
								case 4:
									flag7 = tileData2.isValidTileAnchor(tileSafely.type);
									break;
								}
							}
							if (!flag7 && (anchorBottom.type & AnchorType.Tree) == AnchorType.Tree && tileSafely.type == 5)
							{
								flag7 = true;
								if (m == 0)
								{
									num26 += 1f;
									Tile tileSafely2 = Framing.GetTileSafely(num9 + width, num10 + num31 - 1);
									if (tileSafely2.nactive() && tileSafely2.type == 5)
									{
										num25 += 1f;
										if (onlyCheck)
										{
											objectPreview[width + num11, num31 + num12 - 1] = 1;
										}
									}
									else if (onlyCheck)
									{
										objectPreview[width + num11, num31 + num12 - 1] = 2;
									}
								}
								if (m == anchorBottom.tileCount - 1)
								{
									num26 += 1f;
									Tile tileSafely3 = Framing.GetTileSafely(num9 + width, num10 + num31 + 1);
									if (tileSafely3.nactive() && tileSafely3.type == 5)
									{
										num25 += 1f;
										if (onlyCheck)
										{
											objectPreview[width + num11, num31 + num12 + 1] = 1;
										}
									}
									else if (onlyCheck)
									{
										objectPreview[width + num11, num31 + num12 + 1] = 2;
									}
								}
							}
							if (!flag7 && (anchorBottom.type & AnchorType.AlternateTile) == AnchorType.AlternateTile && tileData2.isValidAlternateAnchor(tileSafely.type))
							{
								flag7 = true;
							}
						}
						else if (!flag7 && (anchorBottom.type & AnchorType.EmptyTile) == AnchorType.EmptyTile)
						{
							flag7 = true;
						}
						if (!flag7)
						{
							if (onlyCheck)
							{
								objectPreview[width + num11, num31 + num12] = 2;
							}
							continue;
						}
						if (onlyCheck)
						{
							objectPreview[width + num11, num31 + num12] = 1;
						}
						num25 += 1f;
					}
				}
				anchorBottom = tileData2.AnchorLeft;
				if (anchorBottom.tileCount != 0)
				{
					num26 += (float)anchorBottom.tileCount;
					int num32 = -1;
					for (int n = 0; n < anchorBottom.tileCount; n++)
					{
						int num33 = anchorBottom.checkStart + n;
						Tile tileSafely = Framing.GetTileSafely(num9 + num32, num10 + num33);
						bool flag8 = false;
						if (tileSafely.nactive())
						{
							if (Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type] && !Main.tileNoAttach[tileSafely.type] && (tileData2.FlattenAnchors || tileSafely.blockType() == 0))
							{
								flag8 = tileData2.isValidTileAnchor(tileSafely.type);
							}
							if (!flag8 && (anchorBottom.type & AnchorType.SolidSide) == AnchorType.SolidSide && Main.tileSolid[tileSafely.type] && !Main.tileSolidTop[tileSafely.type])
							{
								switch (tileSafely.blockType())
								{
								case 3:
								case 5:
									flag8 = tileData2.isValidTileAnchor(tileSafely.type);
									break;
								}
							}
							if (!flag8 && (anchorBottom.type & AnchorType.Tree) == AnchorType.Tree && tileSafely.type == 5)
							{
								flag8 = true;
								if (n == 0)
								{
									num26 += 1f;
									Tile tileSafely4 = Framing.GetTileSafely(num9 + num32, num10 + num33 - 1);
									if (tileSafely4.nactive() && tileSafely4.type == 5)
									{
										num25 += 1f;
										if (onlyCheck)
										{
											objectPreview[num32 + num11, num33 + num12 - 1] = 1;
										}
									}
									else if (onlyCheck)
									{
										objectPreview[num32 + num11, num33 + num12 - 1] = 2;
									}
								}
								if (n == anchorBottom.tileCount - 1)
								{
									num26 += 1f;
									Tile tileSafely5 = Framing.GetTileSafely(num9 + num32, num10 + num33 + 1);
									if (tileSafely5.nactive() && tileSafely5.type == 5)
									{
										num25 += 1f;
										if (onlyCheck)
										{
											objectPreview[num32 + num11, num33 + num12 + 1] = 1;
										}
									}
									else if (onlyCheck)
									{
										objectPreview[num32 + num11, num33 + num12 + 1] = 2;
									}
								}
							}
							if (!flag8 && (anchorBottom.type & AnchorType.AlternateTile) == AnchorType.AlternateTile && tileData2.isValidAlternateAnchor(tileSafely.type))
							{
								flag8 = true;
							}
						}
						else if (!flag8 && (anchorBottom.type & AnchorType.EmptyTile) == AnchorType.EmptyTile)
						{
							flag8 = true;
						}
						if (!flag8)
						{
							if (onlyCheck)
							{
								objectPreview[num32 + num11, num33 + num12] = 2;
							}
							continue;
						}
						if (onlyCheck)
						{
							objectPreview[num32 + num11, num33 + num12] = 1;
						}
						num25 += 1f;
					}
				}
				if (tileData2.HookCheck.hook != null)
				{
					if (tileData2.HookCheck.processedCoordinates)
					{
						short x2 = tileData2.Origin.X;
						short y2 = tileData2.Origin.Y;
					}
					if (tileData2.HookCheck.hook(x, y, type, style, dir) == tileData2.HookCheck.badReturn && tileData2.HookCheck.badResponse == 0)
					{
						num25 = 0f;
						num23 = 0f;
						objectPreview.AllInvalid();
					}
				}
				float num34 = num25 / num26;
				float num35 = num23 / num24;
				if (num34 == 1f && num35 == 1f)
				{
					num5 = 1f;
					num6 = 1f;
					num7 = num8;
					tileObjectData = tileData2;
					break;
				}
				if (num34 > num5 || (num34 == num5 && num35 > num6))
				{
					TileObjectPreviewData.placementCache.CopyFrom(objectPreview);
					num5 = num34;
					num6 = num35;
					tileObjectData = tileData2;
					num7 = num8;
				}
			}
			int num36 = -1;
			if (flag)
			{
				if (TileObjectPreviewData.randomCache == null)
				{
					TileObjectPreviewData.randomCache = new TileObjectPreviewData();
				}
				bool flag9 = false;
				if (TileObjectPreviewData.randomCache.Type == type)
				{
					Point16 coordinates = TileObjectPreviewData.randomCache.Coordinates;
					Point16 objectStart = TileObjectPreviewData.randomCache.ObjectStart;
					int num37 = coordinates.X + objectStart.X;
					int num38 = coordinates.Y + objectStart.Y;
					int num39 = x - tileData.Origin.X;
					int num40 = y - tileData.Origin.Y;
					if (num37 != num39 || num38 != num40)
					{
						flag9 = true;
					}
				}
				else
				{
					flag9 = true;
				}
				num36 = ((!flag9) ? TileObjectPreviewData.randomCache.Random : Main.rand.Next(tileData.RandomStyleRange));
			}
			if (onlyCheck)
			{
				if (num5 != 1f || num6 != 1f)
				{
					objectPreview.CopyFrom(TileObjectPreviewData.placementCache);
					num8 = num7;
				}
				objectPreview.Random = num36;
				if (tileData.RandomStyleRange > 0)
				{
					TileObjectPreviewData.randomCache.CopyFrom(objectPreview);
				}
			}
			if (!onlyCheck)
			{
				objectData.xCoord = x - tileObjectData.Origin.X;
				objectData.yCoord = y - tileObjectData.Origin.Y;
				objectData.type = type;
				objectData.style = style;
				objectData.alternate = num8;
				objectData.random = num36;
			}
			if (num5 == 1f)
			{
				return num6 == 1f;
			}
			return false;
		}

		public static void DrawPreview(SpriteBatch sb, TileObjectPreviewData op, Vector2 position)
		{
			Point16 coordinates = op.Coordinates;
			Texture2D texture = Main.tileTexture[op.Type];
			TileObjectData tileData = TileObjectData.GetTileData(op.Type, op.Style, op.Alternate);
			int num = 0;
			int num2 = 0;
			int num3 = tileData.CalculatePlacementStyle(op.Style, op.Alternate, op.Random);
			int num4 = 0;
			int num5 = tileData.DrawYOffset;
			if (tileData.StyleWrapLimit > 0)
			{
				num4 = num3 / tileData.StyleWrapLimit;
				num3 %= tileData.StyleWrapLimit;
			}
			if (tileData.StyleHorizontal)
			{
				num = tileData.CoordinateFullWidth * num3;
				num2 = tileData.CoordinateFullHeight * num4;
			}
			else
			{
				num = tileData.CoordinateFullWidth * num4;
				num2 = tileData.CoordinateFullHeight * num3;
			}
			for (int i = 0; i < op.Size.X; i++)
			{
				int x = num + (i - op.ObjectStart.X) * (tileData.CoordinateWidth + tileData.CoordinatePadding);
				int num6 = num2;
				for (int j = 0; j < op.Size.Y; j++)
				{
					int num7 = coordinates.X + i;
					int num8 = coordinates.Y + j;
					if (j == 0 && tileData.DrawStepDown != 0)
					{
						Tile tileSafely = Framing.GetTileSafely(num7, num8 - 1);
						if (WorldGen.SolidTile(tileSafely))
						{
							num5 += tileData.DrawStepDown;
						}
					}
					Color color;
					switch (op[i, j])
					{
					case 2:
						color = Color.Red * 0.7f;
						break;
					case 1:
						color = Color.White;
						break;
					default:
						continue;
					}
					color *= 0.5f;
					if (i >= op.ObjectStart.X && i < op.ObjectStart.X + tileData.Width && j >= op.ObjectStart.Y && j < op.ObjectStart.Y + tileData.Height)
					{
						SpriteEffects spriteEffects = SpriteEffects.None;
						if (tileData.DrawFlipHorizontal && i % 2 == 1)
						{
							spriteEffects |= SpriteEffects.FlipHorizontally;
						}
						if (tileData.DrawFlipVertical && j % 2 == 1)
						{
							spriteEffects |= SpriteEffects.FlipVertically;
						}
						sb.Draw(sourceRectangle: new Rectangle(x, num6, tileData.CoordinateWidth, tileData.CoordinateHeights[j - op.ObjectStart.Y]), texture: texture, position: new Vector2(num7 * 16 - (int)(position.X + (float)(tileData.CoordinateWidth - 16) / 2f), num8 * 16 - (int)position.Y + num5), color: color, rotation: 0f, origin: Vector2.Zero, scale: 1f, effects: spriteEffects, layerDepth: 0f);
						num6 += tileData.CoordinateHeights[j - op.ObjectStart.Y] + tileData.CoordinatePadding;
					}
				}
			}
		}
	}
}
