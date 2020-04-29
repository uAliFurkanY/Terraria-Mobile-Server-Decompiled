using Ionic.Zlib;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.IO;
using Terraria.IO;
using Terraria.Social;
using Terraria.Utilities;

namespace Terraria.Map
{
	internal static class MapHelper
	{
		private struct OldMapHelper
		{
			public byte misc;

			public byte misc2;

			public bool active()
			{
				if ((misc & 1) == 1)
				{
					return true;
				}
				return false;
			}

			public bool water()
			{
				if ((misc & 2) == 2)
				{
					return true;
				}
				return false;
			}

			public bool lava()
			{
				if ((misc & 4) == 4)
				{
					return true;
				}
				return false;
			}

			public bool honey()
			{
				if ((misc2 & 0x40) == 64)
				{
					return true;
				}
				return false;
			}

			public bool changed()
			{
				if ((misc & 8) == 8)
				{
					return true;
				}
				return false;
			}

			public bool wall()
			{
				if ((misc & 0x10) == 16)
				{
					return true;
				}
				return false;
			}

			public byte option()
			{
				byte b = 0;
				if ((misc & 0x20) == 32)
				{
					b = (byte)(b + 1);
				}
				if ((misc & 0x40) == 64)
				{
					b = (byte)(b + 2);
				}
				if ((misc & 0x80) == 128)
				{
					b = (byte)(b + 4);
				}
				if ((misc2 & 1) == 1)
				{
					b = (byte)(b + 8);
				}
				return b;
			}

			public byte color()
			{
				return (byte)((misc2 & 0x1E) >> 1);
			}
		}

		public const int drawLoopMilliseconds = 5;

		private const int HeaderEmpty = 0;

		private const int HeaderTile = 1;

		private const int HeaderWall = 2;

		private const int HeaderWater = 3;

		private const int HeaderLava = 4;

		private const int HeaderHoney = 5;

		private const int HeaderHeavenAndHell = 6;

		private const int HeaderBackground = 7;

		private const int maxTileOptions = 12;

		private const int maxWallOptions = 2;

		private const int maxLiquidTypes = 3;

		private const int maxSkyGradients = 256;

		private const int maxDirtGradients = 256;

		private const int maxRockGradients = 256;

		public static int maxUpdateTile = 1000;

		public static int numUpdateTile = 0;

		public static short[] updateTileX = new short[maxUpdateTile];

		public static short[] updateTileY = new short[maxUpdateTile];

		private static bool saveLock = false;

		private static object padlock = new object();

		public static int[] tileOptionCounts;

		public static int[] wallOptionCounts;

		public static ushort[] tileLookup;

		public static ushort[] wallLookup;

		private static ushort tilePosition;

		private static ushort wallPosition;

		private static ushort liquidPosition;

		private static ushort skyPosition;

		private static ushort dirtPosition;

		private static ushort rockPosition;

		private static ushort hellPosition;

		private static Color[] colorLookup;

		private static ushort[] snowTypes;

		private static ushort wallRangeStart;

		private static ushort wallRangeEnd;

		public static void Initialize()
		{
			Color[][] array = new Color[419][];
			for (int i = 0; i < 419; i++)
			{
				array[i] = new Color[12];
			}
			Color color = new Color(151, 107, 75);
			array[0][0] = color;
			array[5][0] = color;
			array[30][0] = color;
			array[191][0] = color;
			array[272][0] = new Color(121, 119, 101);
			color = new Color(128, 128, 128);
			array[1][0] = color;
			array[38][0] = color;
			array[48][0] = color;
			array[130][0] = color;
			array[138][0] = color;
			array[273][0] = color;
			array[283][0] = color;
			array[2][0] = new Color(28, 216, 94);
			color = new Color(26, 196, 84);
			array[3][0] = color;
			array[192][0] = color;
			array[73][0] = new Color(27, 197, 109);
			array[52][0] = new Color(23, 177, 76);
			array[353][0] = new Color(28, 216, 94);
			array[20][0] = new Color(163, 116, 81);
			array[6][0] = new Color(140, 101, 80);
			color = new Color(150, 67, 22);
			array[7][0] = color;
			array[47][0] = color;
			array[284][0] = color;
			color = new Color(185, 164, 23);
			array[8][0] = color;
			array[45][0] = color;
			color = new Color(185, 194, 195);
			array[9][0] = color;
			array[46][0] = color;
			color = new Color(98, 95, 167);
			array[22][0] = color;
			array[140][0] = color;
			array[23][0] = new Color(141, 137, 223);
			array[24][0] = new Color(122, 116, 218);
			array[25][0] = new Color(109, 90, 128);
			array[37][0] = new Color(104, 86, 84);
			array[39][0] = new Color(181, 62, 59);
			array[40][0] = new Color(146, 81, 68);
			array[41][0] = new Color(66, 84, 109);
			array[43][0] = new Color(84, 100, 63);
			array[44][0] = new Color(107, 68, 99);
			array[53][0] = new Color(186, 168, 84);
			color = new Color(190, 171, 94);
			array[151][0] = color;
			array[154][0] = color;
			array[274][0] = color;
			array[328][0] = new Color(200, 246, 254);
			array[329][0] = new Color(15, 15, 15);
			array[54][0] = new Color(200, 246, 254);
			array[56][0] = new Color(43, 40, 84);
			array[75][0] = new Color(26, 26, 26);
			array[57][0] = new Color(68, 68, 76);
			color = new Color(142, 66, 66);
			array[58][0] = color;
			array[76][0] = color;
			color = new Color(92, 68, 73);
			array[59][0] = color;
			array[120][0] = color;
			array[60][0] = new Color(143, 215, 29);
			array[61][0] = new Color(135, 196, 26);
			array[74][0] = new Color(96, 197, 27);
			array[62][0] = new Color(121, 176, 24);
			array[233][0] = new Color(107, 182, 29);
			array[63][0] = new Color(110, 140, 182);
			array[64][0] = new Color(196, 96, 114);
			array[65][0] = new Color(56, 150, 97);
			array[66][0] = new Color(160, 118, 58);
			array[67][0] = new Color(140, 58, 166);
			array[68][0] = new Color(125, 191, 197);
			array[70][0] = new Color(93, 127, 255);
			color = new Color(182, 175, 130);
			array[71][0] = color;
			array[72][0] = color;
			array[190][0] = color;
			color = new Color(73, 120, 17);
			array[80][0] = color;
			array[188][0] = color;
			color = new Color(11, 80, 143);
			array[107][0] = color;
			array[121][0] = color;
			color = new Color(91, 169, 169);
			array[108][0] = color;
			array[122][0] = color;
			color = new Color(128, 26, 52);
			array[111][0] = color;
			array[150][0] = color;
			array[109][0] = new Color(78, 193, 227);
			array[110][0] = new Color(48, 186, 135);
			array[113][0] = new Color(48, 208, 234);
			array[115][0] = new Color(33, 171, 207);
			array[112][0] = new Color(103, 98, 122);
			color = new Color(238, 225, 218);
			array[116][0] = color;
			array[118][0] = color;
			array[117][0] = new Color(181, 172, 190);
			array[119][0] = new Color(107, 92, 108);
			array[123][0] = new Color(106, 107, 118);
			array[124][0] = new Color(73, 51, 36);
			array[131][0] = new Color(52, 52, 52);
			array[145][0] = new Color(192, 30, 30);
			array[146][0] = new Color(43, 192, 30);
			color = new Color(211, 236, 241);
			array[147][0] = color;
			array[148][0] = color;
			array[152][0] = new Color(128, 133, 184);
			array[153][0] = new Color(239, 141, 126);
			array[155][0] = new Color(131, 162, 161);
			array[156][0] = new Color(170, 171, 157);
			array[157][0] = new Color(104, 100, 126);
			color = new Color(145, 81, 85);
			array[158][0] = color;
			array[232][0] = color;
			array[159][0] = new Color(148, 133, 98);
			array[160][0] = new Color(200, 0, 0);
			array[160][1] = new Color(0, 200, 0);
			array[160][2] = new Color(0, 0, 200);
			array[161][0] = new Color(144, 195, 232);
			array[162][0] = new Color(184, 219, 240);
			array[163][0] = new Color(174, 145, 214);
			array[164][0] = new Color(218, 182, 204);
			array[170][0] = new Color(27, 109, 69);
			array[171][0] = new Color(33, 135, 85);
			color = new Color(129, 125, 93);
			array[166][0] = color;
			array[175][0] = color;
			array[167][0] = new Color(62, 82, 114);
			color = new Color(132, 157, 127);
			array[168][0] = color;
			array[176][0] = color;
			color = new Color(152, 171, 198);
			array[169][0] = color;
			array[177][0] = color;
			array[179][0] = new Color(49, 134, 114);
			array[180][0] = new Color(126, 134, 49);
			array[181][0] = new Color(134, 59, 49);
			array[182][0] = new Color(43, 86, 140);
			array[183][0] = new Color(121, 49, 134);
			array[381][0] = new Color(254, 121, 2);
			array[189][0] = new Color(223, 255, 255);
			array[193][0] = new Color(56, 121, 255);
			array[194][0] = new Color(157, 157, 107);
			array[195][0] = new Color(134, 22, 34);
			array[196][0] = new Color(147, 144, 178);
			array[197][0] = new Color(97, 200, 225);
			array[198][0] = new Color(62, 61, 52);
			array[199][0] = new Color(208, 80, 80);
			array[201][0] = new Color(203, 61, 64);
			array[205][0] = new Color(186, 50, 52);
			array[200][0] = new Color(216, 152, 144);
			array[202][0] = new Color(213, 178, 28);
			array[203][0] = new Color(128, 44, 45);
			array[204][0] = new Color(125, 55, 65);
			array[206][0] = new Color(124, 175, 201);
			array[208][0] = new Color(88, 105, 118);
			array[211][0] = new Color(191, 233, 115);
			array[213][0] = new Color(137, 120, 67);
			array[214][0] = new Color(103, 103, 103);
			array[221][0] = new Color(239, 90, 50);
			array[222][0] = new Color(231, 96, 228);
			array[223][0] = new Color(57, 85, 101);
			array[224][0] = new Color(107, 132, 139);
			array[225][0] = new Color(227, 125, 22);
			array[226][0] = new Color(141, 56, 0);
			array[229][0] = new Color(255, 156, 12);
			array[230][0] = new Color(131, 79, 13);
			array[234][0] = new Color(53, 44, 41);
			array[235][0] = new Color(214, 184, 46);
			array[236][0] = new Color(149, 232, 87);
			array[237][0] = new Color(255, 241, 51);
			array[238][0] = new Color(225, 128, 206);
			array[243][0] = new Color(198, 196, 170);
			array[248][0] = new Color(219, 71, 38);
			array[249][0] = new Color(235, 38, 231);
			array[250][0] = new Color(86, 85, 92);
			array[251][0] = new Color(235, 150, 23);
			array[252][0] = new Color(153, 131, 44);
			array[253][0] = new Color(57, 48, 97);
			array[254][0] = new Color(248, 158, 92);
			array[255][0] = new Color(107, 49, 154);
			array[256][0] = new Color(154, 148, 49);
			array[257][0] = new Color(49, 49, 154);
			array[258][0] = new Color(49, 154, 68);
			array[259][0] = new Color(154, 49, 77);
			array[260][0] = new Color(85, 89, 118);
			array[261][0] = new Color(154, 83, 49);
			array[262][0] = new Color(221, 79, 255);
			array[263][0] = new Color(250, 255, 79);
			array[264][0] = new Color(79, 102, 255);
			array[265][0] = new Color(79, 255, 89);
			array[266][0] = new Color(255, 79, 79);
			array[267][0] = new Color(240, 240, 247);
			array[268][0] = new Color(255, 145, 79);
			array[287][0] = new Color(79, 128, 17);
			color = new Color(122, 217, 232);
			array[275][0] = color;
			array[276][0] = color;
			array[277][0] = color;
			array[278][0] = color;
			array[279][0] = color;
			array[280][0] = color;
			array[281][0] = color;
			array[282][0] = color;
			array[285][0] = color;
			array[286][0] = color;
			array[288][0] = color;
			array[289][0] = color;
			array[290][0] = color;
			array[291][0] = color;
			array[292][0] = color;
			array[293][0] = color;
			array[294][0] = color;
			array[295][0] = color;
			array[296][0] = color;
			array[297][0] = color;
			array[298][0] = color;
			array[299][0] = color;
			array[309][0] = color;
			array[310][0] = color;
			array[413][0] = color;
			array[339][0] = color;
			array[358][0] = color;
			array[359][0] = color;
			array[360][0] = color;
			array[361][0] = color;
			array[362][0] = color;
			array[363][0] = color;
			array[364][0] = color;
			array[391][0] = color;
			array[392][0] = color;
			array[393][0] = color;
			array[394][0] = color;
			array[414][0] = color;
			array[408][0] = new Color(85, 83, 82);
			array[409][0] = new Color(85, 83, 82);
			array[415][0] = new Color(249, 75, 7);
			array[416][0] = new Color(0, 160, 170);
			array[417][0] = new Color(160, 87, 234);
			array[418][0] = new Color(22, 173, 254);
			array[311][0] = new Color(117, 61, 25);
			array[312][0] = new Color(204, 93, 73);
			array[313][0] = new Color(87, 150, 154);
			array[4][0] = new Color(253, 221, 3);
			array[4][1] = new Color(253, 221, 3);
			color = new Color(253, 221, 3);
			array[93][0] = color;
			array[33][0] = color;
			array[174][0] = color;
			array[100][0] = color;
			array[98][0] = color;
			array[173][0] = color;
			color = new Color(119, 105, 79);
			array[11][0] = color;
			array[10][0] = color;
			color = new Color(191, 142, 111);
			array[14][0] = color;
			array[15][0] = color;
			array[18][0] = color;
			array[19][0] = color;
			array[55][0] = color;
			array[79][0] = color;
			array[86][0] = color;
			array[87][0] = color;
			array[88][0] = color;
			array[89][0] = color;
			array[94][0] = color;
			array[101][0] = color;
			array[104][0] = color;
			array[106][0] = color;
			array[114][0] = color;
			array[128][0] = color;
			array[139][0] = color;
			array[172][0] = color;
			array[216][0] = color;
			array[269][0] = color;
			array[334][0] = color;
			array[377][0] = color;
			array[380][0] = color;
			array[395][0] = color;
			array[12][0] = new Color(174, 24, 69);
			array[13][0] = new Color(133, 213, 247);
			color = new Color(144, 148, 144);
			array[17][0] = color;
			array[90][0] = color;
			array[96][0] = color;
			array[97][0] = color;
			array[99][0] = color;
			array[132][0] = color;
			array[142][0] = color;
			array[143][0] = color;
			array[144][0] = color;
			array[207][0] = color;
			array[209][0] = color;
			array[212][0] = color;
			array[217][0] = color;
			array[218][0] = color;
			array[219][0] = color;
			array[220][0] = color;
			array[228][0] = color;
			array[300][0] = color;
			array[301][0] = color;
			array[302][0] = color;
			array[303][0] = color;
			array[304][0] = color;
			array[305][0] = color;
			array[306][0] = color;
			array[307][0] = color;
			array[308][0] = color;
			array[349][0] = new Color(144, 148, 144);
			array[105][0] = new Color(144, 148, 144);
			array[105][1] = new Color(177, 92, 31);
			array[105][2] = new Color(201, 188, 170);
			array[137][0] = new Color(144, 148, 144);
			array[137][1] = new Color(141, 56, 0);
			array[16][0] = new Color(140, 130, 116);
			array[26][0] = new Color(119, 101, 125);
			array[26][1] = new Color(214, 127, 133);
			array[36][0] = new Color(230, 89, 92);
			array[28][0] = new Color(151, 79, 80);
			array[28][1] = new Color(90, 139, 140);
			array[28][2] = new Color(192, 136, 70);
			array[28][3] = new Color(203, 185, 151);
			array[28][4] = new Color(73, 56, 41);
			array[28][5] = new Color(148, 159, 67);
			array[28][6] = new Color(138, 172, 67);
			array[28][7] = new Color(226, 122, 47);
			array[28][8] = new Color(198, 87, 93);
			array[29][0] = new Color(175, 105, 128);
			array[51][0] = new Color(192, 202, 203);
			array[31][0] = new Color(141, 120, 168);
			array[31][1] = new Color(212, 105, 105);
			array[32][0] = new Color(151, 135, 183);
			array[42][0] = new Color(251, 235, 127);
			array[50][0] = new Color(170, 48, 114);
			array[85][0] = new Color(192, 192, 192);
			array[69][0] = new Color(190, 150, 92);
			array[77][0] = new Color(238, 85, 70);
			array[81][0] = new Color(245, 133, 191);
			array[78][0] = new Color(121, 110, 97);
			array[141][0] = new Color(192, 59, 59);
			array[129][0] = new Color(255, 117, 224);
			array[126][0] = new Color(159, 209, 229);
			array[125][0] = new Color(141, 175, 255);
			array[103][0] = new Color(141, 98, 77);
			array[95][0] = new Color(255, 162, 31);
			array[92][0] = new Color(213, 229, 237);
			array[91][0] = new Color(13, 88, 130);
			array[215][0] = new Color(254, 121, 2);
			array[316][0] = new Color(157, 176, 226);
			array[317][0] = new Color(118, 227, 129);
			array[318][0] = new Color(227, 118, 215);
			array[319][0] = new Color(96, 68, 48);
			array[320][0] = new Color(203, 185, 151);
			array[321][0] = new Color(96, 77, 64);
			array[322][0] = new Color(198, 170, 104);
			array[149][0] = new Color(220, 50, 50);
			array[149][1] = new Color(0, 220, 50);
			array[149][2] = new Color(50, 50, 220);
			array[133][0] = new Color(231, 53, 56);
			array[133][1] = new Color(192, 189, 221);
			array[134][0] = new Color(166, 187, 153);
			array[134][1] = new Color(241, 129, 249);
			array[102][0] = new Color(229, 212, 73);
			array[49][0] = new Color(89, 201, 255);
			array[35][0] = new Color(226, 145, 30);
			array[34][0] = new Color(235, 166, 135);
			array[136][0] = new Color(213, 203, 204);
			array[231][0] = new Color(224, 194, 101);
			array[239][0] = new Color(224, 194, 101);
			array[240][0] = new Color(120, 85, 60);
			array[240][1] = new Color(99, 50, 30);
			array[240][2] = new Color(153, 153, 117);
			array[240][3] = new Color(112, 84, 56);
			array[240][4] = new Color(234, 231, 226);
			array[241][0] = new Color(77, 74, 72);
			array[244][0] = new Color(200, 245, 253);
			color = new Color(99, 50, 30);
			array[242][0] = color;
			array[245][0] = color;
			array[246][0] = color;
			array[242][1] = new Color(185, 142, 97);
			array[247][0] = new Color(140, 150, 150);
			array[271][0] = new Color(107, 250, 255);
			array[270][0] = new Color(187, 255, 107);
			array[314][0] = new Color(181, 164, 125);
			array[324][0] = new Color(228, 213, 173);
			array[351][0] = new Color(31, 31, 31);
			array[21][0] = new Color(174, 129, 92);
			array[21][1] = new Color(233, 207, 94);
			array[21][2] = new Color(137, 128, 200);
			array[21][3] = new Color(160, 160, 160);
			array[21][4] = new Color(106, 210, 255);
			array[27][0] = new Color(54, 154, 54);
			array[27][1] = new Color(226, 196, 49);
			color = new Color(246, 197, 26);
			array[82][0] = color;
			array[83][0] = color;
			array[84][0] = color;
			color = new Color(76, 150, 216);
			array[82][1] = color;
			array[83][1] = color;
			array[84][1] = color;
			color = new Color(185, 214, 42);
			array[82][2] = color;
			array[83][2] = color;
			array[84][2] = color;
			color = new Color(167, 203, 37);
			array[82][3] = color;
			array[83][3] = color;
			array[84][3] = color;
			color = new Color(72, 145, 125);
			array[82][4] = color;
			array[83][4] = color;
			array[84][4] = color;
			color = new Color(177, 69, 49);
			array[82][5] = color;
			array[83][5] = color;
			array[84][5] = color;
			color = new Color(40, 152, 240);
			array[82][6] = color;
			array[83][6] = color;
			array[84][6] = color;
			array[165][0] = new Color(115, 173, 229);
			array[165][1] = new Color(100, 100, 100);
			array[165][2] = new Color(152, 152, 152);
			array[165][3] = new Color(227, 125, 22);
			array[178][0] = new Color(208, 94, 201);
			array[178][1] = new Color(233, 146, 69);
			array[178][2] = new Color(71, 146, 251);
			array[178][3] = new Color(60, 226, 133);
			array[178][4] = new Color(250, 30, 71);
			array[178][5] = new Color(166, 176, 204);
			array[178][6] = new Color(255, 217, 120);
			array[184][0] = new Color(29, 106, 88);
			array[184][1] = new Color(94, 100, 36);
			array[184][2] = new Color(96, 44, 40);
			array[184][3] = new Color(34, 63, 102);
			array[184][4] = new Color(79, 35, 95);
			array[184][5] = new Color(253, 62, 3);
			color = new Color(99, 99, 99);
			array[185][0] = color;
			array[186][0] = color;
			array[187][0] = color;
			color = new Color(114, 81, 56);
			array[185][1] = color;
			array[186][1] = color;
			array[187][1] = color;
			color = new Color(133, 133, 101);
			array[185][2] = color;
			array[186][2] = color;
			array[187][2] = color;
			color = new Color(151, 200, 211);
			array[185][3] = color;
			array[186][3] = color;
			array[187][3] = color;
			color = new Color(177, 183, 161);
			array[185][4] = color;
			array[186][4] = color;
			array[187][4] = color;
			color = new Color(134, 114, 38);
			array[185][5] = color;
			array[186][5] = color;
			array[187][5] = color;
			color = new Color(82, 62, 66);
			array[185][6] = color;
			array[186][6] = color;
			array[187][6] = color;
			color = new Color(143, 117, 121);
			array[185][7] = color;
			array[186][7] = color;
			array[187][7] = color;
			color = new Color(177, 92, 31);
			array[185][8] = color;
			array[186][8] = color;
			array[187][8] = color;
			color = new Color(85, 73, 87);
			array[185][9] = color;
			array[186][9] = color;
			array[187][9] = color;
			array[227][0] = new Color(74, 197, 155);
			array[227][1] = new Color(54, 153, 88);
			array[227][2] = new Color(63, 126, 207);
			array[227][3] = new Color(240, 180, 4);
			array[227][4] = new Color(45, 68, 168);
			array[227][5] = new Color(61, 92, 0);
			array[227][6] = new Color(216, 112, 152);
			array[227][7] = new Color(200, 40, 24);
			array[227][8] = new Color(113, 45, 133);
			array[227][9] = new Color(235, 137, 2);
			array[227][10] = new Color(41, 152, 135);
			array[227][11] = new Color(198, 19, 78);
			array[373][0] = new Color(9, 61, 191);
			array[374][0] = new Color(253, 32, 3);
			array[375][0] = new Color(255, 156, 12);
			array[323][0] = new Color(182, 141, 86);
			array[325][0] = new Color(129, 125, 93);
			array[326][0] = new Color(9, 61, 191);
			array[327][0] = new Color(253, 32, 3);
			array[330][0] = new Color(226, 118, 76);
			array[331][0] = new Color(161, 172, 173);
			array[332][0] = new Color(204, 181, 72);
			array[333][0] = new Color(190, 190, 178);
			array[335][0] = new Color(217, 174, 137);
			array[336][0] = new Color(253, 62, 3);
			array[337][0] = new Color(144, 148, 144);
			array[338][0] = new Color(85, 255, 160);
			array[315][0] = new Color(235, 114, 80);
			array[340][0] = new Color(96, 248, 2);
			array[341][0] = new Color(105, 74, 202);
			array[342][0] = new Color(29, 240, 255);
			array[343][0] = new Color(254, 202, 80);
			array[344][0] = new Color(131, 252, 245);
			array[345][0] = new Color(255, 156, 12);
			array[346][0] = new Color(149, 212, 89);
			array[347][0] = new Color(236, 74, 79);
			array[348][0] = new Color(44, 26, 233);
			array[350][0] = new Color(55, 97, 155);
			array[352][0] = new Color(238, 97, 94);
			array[354][0] = new Color(141, 107, 89);
			array[355][0] = new Color(141, 107, 89);
			array[356][0] = new Color(233, 203, 24);
			array[357][0] = new Color(168, 178, 204);
			array[367][0] = new Color(168, 178, 204);
			array[365][0] = new Color(146, 136, 205);
			array[366][0] = new Color(223, 232, 233);
			array[368][0] = new Color(50, 46, 104);
			array[369][0] = new Color(50, 46, 104);
			array[370][0] = new Color(127, 116, 194);
			array[372][0] = new Color(252, 128, 201);
			array[371][0] = new Color(249, 101, 189);
			array[376][0] = new Color(160, 120, 92);
			array[378][0] = new Color(160, 120, 100);
			array[379][0] = new Color(251, 209, 240);
			array[382][0] = new Color(28, 216, 94);
			array[383][0] = new Color(221, 136, 144);
			array[384][0] = new Color(131, 206, 12);
			array[385][0] = new Color(87, 21, 144);
			array[386][0] = new Color(127, 92, 69);
			array[387][0] = new Color(127, 92, 69);
			array[388][0] = new Color(127, 92, 69);
			array[389][0] = new Color(127, 92, 69);
			array[390][0] = new Color(253, 32, 3);
			array[397][0] = new Color(212, 192, 100);
			array[396][0] = new Color(198, 124, 78);
			array[398][0] = new Color(100, 82, 126);
			array[399][0] = new Color(77, 76, 66);
			array[400][0] = new Color(96, 68, 117);
			array[401][0] = new Color(68, 60, 51);
			array[402][0] = new Color(174, 168, 186);
			array[403][0] = new Color(205, 152, 186);
			array[404][0] = new Color(140, 84, 60);
			array[405][0] = new Color(140, 140, 140);
			array[406][0] = new Color(120, 120, 120);
			array[407][0] = new Color(255, 227, 132);
			array[411][0] = new Color(227, 46, 46);
			array[410][0] = new Color(75, 139, 166);
			array[412][0] = new Color(75, 139, 166);
			Color[] array2 = new Color[3]
			{
				new Color(9, 61, 191),
				new Color(253, 32, 3),
				new Color(254, 194, 20)
			};
			Color[][] array3 = new Color[225][];
			for (int j = 0; j < 225; j++)
			{
				array3[j] = new Color[2];
			}
			array3[158][0] = new Color(107, 49, 154);
			array3[163][0] = new Color(154, 148, 49);
			array3[162][0] = new Color(49, 49, 154);
			array3[160][0] = new Color(49, 154, 68);
			array3[161][0] = new Color(154, 49, 77);
			array3[159][0] = new Color(85, 89, 118);
			array3[157][0] = new Color(154, 83, 49);
			array3[154][0] = new Color(221, 79, 255);
			array3[166][0] = new Color(250, 255, 79);
			array3[165][0] = new Color(79, 102, 255);
			array3[156][0] = new Color(79, 255, 89);
			array3[164][0] = new Color(255, 79, 79);
			array3[155][0] = new Color(240, 240, 247);
			array3[153][0] = new Color(255, 145, 79);
			array3[169][0] = new Color(5, 5, 5);
			array3[224][0] = new Color(57, 55, 52);
			array3[170][0] = new Color(59, 39, 22);
			array3[171][0] = new Color(59, 39, 22);
			color = new Color(52, 52, 52);
			array3[1][0] = color;
			array3[53][0] = color;
			array3[52][0] = color;
			array3[51][0] = color;
			array3[50][0] = color;
			array3[49][0] = color;
			array3[48][0] = color;
			array3[44][0] = color;
			array3[5][0] = color;
			color = new Color(88, 61, 46);
			array3[2][0] = color;
			array3[16][0] = color;
			array3[59][0] = color;
			array3[3][0] = new Color(61, 58, 78);
			array3[4][0] = new Color(73, 51, 36);
			array3[6][0] = new Color(91, 30, 30);
			color = new Color(27, 31, 42);
			array3[7][0] = color;
			array3[17][0] = color;
			color = new Color(32, 40, 45);
			array3[94][0] = color;
			array3[100][0] = color;
			color = new Color(44, 41, 50);
			array3[95][0] = color;
			array3[101][0] = color;
			color = new Color(31, 39, 26);
			array3[8][0] = color;
			array3[18][0] = color;
			color = new Color(36, 45, 44);
			array3[98][0] = color;
			array3[104][0] = color;
			color = new Color(38, 49, 50);
			array3[99][0] = color;
			array3[105][0] = color;
			color = new Color(41, 28, 36);
			array3[9][0] = color;
			array3[19][0] = color;
			color = new Color(72, 50, 77);
			array3[96][0] = color;
			array3[102][0] = color;
			color = new Color(78, 50, 69);
			array3[97][0] = color;
			array3[103][0] = color;
			array3[10][0] = new Color(74, 62, 12);
			array3[11][0] = new Color(46, 56, 59);
			array3[12][0] = new Color(75, 32, 11);
			array3[13][0] = new Color(67, 37, 37);
			color = new Color(15, 15, 15);
			array3[14][0] = color;
			array3[20][0] = color;
			array3[15][0] = new Color(52, 43, 45);
			array3[22][0] = new Color(113, 99, 99);
			array3[23][0] = new Color(38, 38, 43);
			array3[24][0] = new Color(53, 39, 41);
			array3[25][0] = new Color(11, 35, 62);
			array3[26][0] = new Color(21, 63, 70);
			array3[27][0] = new Color(88, 61, 46);
			array3[27][1] = new Color(52, 52, 52);
			array3[28][0] = new Color(81, 84, 101);
			array3[29][0] = new Color(88, 23, 23);
			array3[30][0] = new Color(28, 88, 23);
			array3[31][0] = new Color(78, 87, 99);
			color = new Color(69, 67, 41);
			array3[34][0] = color;
			array3[37][0] = color;
			array3[32][0] = new Color(86, 17, 40);
			array3[33][0] = new Color(49, 47, 83);
			array3[35][0] = new Color(51, 51, 70);
			array3[36][0] = new Color(87, 59, 55);
			array3[38][0] = new Color(49, 57, 49);
			array3[39][0] = new Color(78, 79, 73);
			array3[45][0] = new Color(60, 59, 51);
			array3[46][0] = new Color(48, 57, 47);
			array3[47][0] = new Color(71, 77, 85);
			array3[40][0] = new Color(85, 102, 103);
			array3[41][0] = new Color(52, 50, 62);
			array3[42][0] = new Color(71, 42, 44);
			array3[43][0] = new Color(73, 66, 50);
			array3[54][0] = new Color(40, 56, 50);
			array3[55][0] = new Color(49, 48, 36);
			array3[56][0] = new Color(43, 33, 32);
			array3[57][0] = new Color(31, 40, 49);
			array3[58][0] = new Color(48, 35, 52);
			array3[60][0] = new Color(1, 52, 20);
			array3[61][0] = new Color(55, 39, 26);
			array3[62][0] = new Color(39, 33, 26);
			array3[69][0] = new Color(43, 42, 68);
			array3[70][0] = new Color(30, 70, 80);
			color = new Color(30, 80, 48);
			array3[63][0] = color;
			array3[65][0] = color;
			array3[66][0] = color;
			array3[68][0] = color;
			color = new Color(53, 80, 30);
			array3[64][0] = color;
			array3[67][0] = color;
			array3[78][0] = new Color(63, 39, 26);
			array3[71][0] = new Color(78, 105, 135);
			array3[72][0] = new Color(52, 84, 12);
			array3[73][0] = new Color(190, 204, 223);
			color = new Color(64, 62, 80);
			array3[74][0] = color;
			array3[80][0] = color;
			array3[75][0] = new Color(65, 65, 35);
			array3[76][0] = new Color(20, 46, 104);
			array3[77][0] = new Color(61, 13, 16);
			array3[79][0] = new Color(51, 47, 96);
			array3[81][0] = new Color(101, 51, 51);
			array3[82][0] = new Color(77, 64, 34);
			array3[83][0] = new Color(62, 38, 41);
			array3[84][0] = new Color(48, 78, 93);
			array3[85][0] = new Color(54, 63, 69);
			color = new Color(138, 73, 38);
			array3[86][0] = color;
			array3[108][0] = color;
			color = new Color(50, 15, 8);
			array3[87][0] = color;
			array3[112][0] = color;
			array3[109][0] = new Color(94, 25, 17);
			array3[110][0] = new Color(125, 36, 122);
			array3[111][0] = new Color(51, 35, 27);
			array3[113][0] = new Color(135, 58, 0);
			array3[114][0] = new Color(65, 52, 15);
			array3[115][0] = new Color(39, 42, 51);
			array3[116][0] = new Color(89, 26, 27);
			array3[117][0] = new Color(126, 123, 115);
			array3[118][0] = new Color(8, 50, 19);
			array3[119][0] = new Color(95, 21, 24);
			array3[120][0] = new Color(17, 31, 65);
			array3[121][0] = new Color(192, 173, 143);
			array3[122][0] = new Color(114, 114, 131);
			array3[123][0] = new Color(136, 119, 7);
			array3[124][0] = new Color(8, 72, 3);
			array3[125][0] = new Color(117, 132, 82);
			array3[126][0] = new Color(100, 102, 114);
			array3[127][0] = new Color(30, 118, 226);
			array3[128][0] = new Color(93, 6, 102);
			array3[129][0] = new Color(64, 40, 169);
			array3[130][0] = new Color(39, 34, 180);
			array3[131][0] = new Color(87, 94, 125);
			array3[132][0] = new Color(6, 6, 6);
			array3[133][0] = new Color(69, 72, 186);
			array3[134][0] = new Color(130, 62, 16);
			array3[135][0] = new Color(22, 123, 163);
			array3[136][0] = new Color(40, 86, 151);
			array3[137][0] = new Color(183, 75, 15);
			array3[138][0] = new Color(83, 80, 100);
			array3[139][0] = new Color(115, 65, 68);
			array3[140][0] = new Color(119, 108, 81);
			array3[141][0] = new Color(59, 67, 71);
			array3[142][0] = new Color(17, 172, 143);
			array3[143][0] = new Color(90, 112, 105);
			array3[144][0] = new Color(62, 28, 87);
			array3[146][0] = new Color(120, 59, 19);
			array3[147][0] = new Color(59, 59, 59);
			array3[148][0] = new Color(229, 218, 161);
			array3[149][0] = new Color(73, 59, 50);
			array3[151][0] = new Color(102, 75, 34);
			array3[167][0] = new Color(70, 68, 51);
			array3[172][0] = new Color(163, 96, 0);
			array3[173][0] = new Color(94, 163, 46);
			array3[174][0] = new Color(117, 32, 59);
			array3[175][0] = new Color(20, 11, 203);
			array3[176][0] = new Color(74, 69, 88);
			array3[177][0] = new Color(60, 30, 30);
			array3[183][0] = new Color(111, 117, 135);
			array3[179][0] = new Color(111, 117, 135);
			array3[178][0] = new Color(111, 117, 135);
			array3[184][0] = new Color(25, 23, 54);
			array3[181][0] = new Color(25, 23, 54);
			array3[180][0] = new Color(25, 23, 54);
			array3[182][0] = new Color(74, 71, 129);
			array3[185][0] = new Color(52, 52, 52);
			array3[186][0] = new Color(38, 9, 66);
			array3[216][0] = new Color(158, 100, 64);
			array3[217][0] = new Color(62, 45, 75);
			array3[218][0] = new Color(57, 14, 12);
			array3[219][0] = new Color(96, 72, 133);
			array3[187][0] = new Color(149, 80, 51);
			array3[220][0] = new Color(67, 55, 80);
			array3[221][0] = new Color(64, 37, 29);
			array3[222][0] = new Color(70, 51, 91);
			array3[188][0] = new Color(82, 63, 80);
			array3[189][0] = new Color(65, 61, 77);
			array3[190][0] = new Color(64, 65, 92);
			array3[191][0] = new Color(76, 53, 84);
			array3[192][0] = new Color(144, 67, 52);
			array3[193][0] = new Color(149, 48, 48);
			array3[194][0] = new Color(111, 32, 36);
			array3[195][0] = new Color(147, 48, 55);
			array3[196][0] = new Color(97, 67, 51);
			array3[197][0] = new Color(112, 80, 62);
			array3[198][0] = new Color(88, 61, 46);
			array3[199][0] = new Color(127, 94, 76);
			array3[200][0] = new Color(143, 50, 123);
			array3[201][0] = new Color(136, 120, 131);
			array3[202][0] = new Color(219, 92, 143);
			array3[203][0] = new Color(113, 64, 150);
			array3[204][0] = new Color(74, 67, 60);
			array3[205][0] = new Color(60, 78, 59);
			array3[206][0] = new Color(0, 54, 21);
			array3[207][0] = new Color(74, 97, 72);
			array3[208][0] = new Color(40, 37, 35);
			array3[209][0] = new Color(77, 63, 66);
			array3[210][0] = new Color(111, 6, 6);
			array3[211][0] = new Color(88, 67, 59);
			array3[212][0] = new Color(88, 87, 80);
			array3[213][0] = new Color(71, 71, 67);
			array3[214][0] = new Color(76, 52, 60);
			array3[215][0] = new Color(89, 48, 59);
			array3[223][0] = new Color(51, 18, 4);
			Color[] array4 = new Color[256];
			Color color2 = new Color(50, 40, 255);
			Color color3 = new Color(145, 185, 255);
			for (int k = 0; k < array4.Length; k++)
			{
				float num = (float)k / (float)array4.Length;
				float num2 = 1f - num;
				array4[k] = new Color((byte)((float)(int)color2.R * num2 + (float)(int)color3.R * num), (byte)((float)(int)color2.G * num2 + (float)(int)color3.G * num), (byte)((float)(int)color2.B * num2 + (float)(int)color3.B * num));
			}
			Color[] array5 = new Color[256];
			Color color4 = new Color(88, 61, 46);
			Color color5 = new Color(37, 78, 123);
			for (int l = 0; l < array5.Length; l++)
			{
				float num3 = (float)l / 255f;
				float num4 = 1f - num3;
				array5[l] = new Color((byte)((float)(int)color4.R * num4 + (float)(int)color5.R * num3), (byte)((float)(int)color4.G * num4 + (float)(int)color5.G * num3), (byte)((float)(int)color4.B * num4 + (float)(int)color5.B * num3));
			}
			Color[] array6 = new Color[256];
			Color color6 = new Color(74, 67, 60);
			color5 = new Color(53, 70, 97);
			for (int m = 0; m < array6.Length; m++)
			{
				float num5 = (float)m / 255f;
				float num6 = 1f - num5;
				array6[m] = new Color((byte)((float)(int)color6.R * num6 + (float)(int)color5.R * num5), (byte)((float)(int)color6.G * num6 + (float)(int)color5.G * num5), (byte)((float)(int)color6.B * num6 + (float)(int)color5.B * num5));
			}
			Color color7 = new Color(50, 44, 38);
			int num7 = 0;
			tileOptionCounts = new int[419];
			for (int n = 0; n < 419; n++)
			{
				Color[] array7 = array[n];
				int num8;
				for (num8 = 0; num8 < 12 && !(array7[num8] == Color.Transparent); num8++)
				{
				}
				tileOptionCounts[n] = num8;
				num7 += num8;
			}
			wallOptionCounts = new int[225];
			for (int num9 = 0; num9 < 225; num9++)
			{
				Color[] array8 = array3[num9];
				int num10;
				for (num10 = 0; num10 < 2 && !(array8[num10] == Color.Transparent); num10++)
				{
				}
				wallOptionCounts[num9] = num10;
				num7 += num10;
			}
			num7 += 773;
			colorLookup = new Color[num7];
			colorLookup[0] = Color.Transparent;
			ushort num11 = tilePosition = 1;
			tileLookup = new ushort[419];
			for (int num12 = 0; num12 < 419; num12++)
			{
				if (tileOptionCounts[num12] > 0)
				{
					Color[] array9 = array[num12];
					tileLookup[num12] = num11;
					for (int num13 = 0; num13 < tileOptionCounts[num12]; num13++)
					{
						colorLookup[num11] = array[num12][num13];
						num11 = (ushort)(num11 + 1);
					}
				}
				else
				{
					tileLookup[num12] = 0;
				}
			}
			wallPosition = num11;
			wallLookup = new ushort[225];
			wallRangeStart = num11;
			for (int num14 = 0; num14 < 225; num14++)
			{
				if (wallOptionCounts[num14] > 0)
				{
					Color[] array10 = array3[num14];
					wallLookup[num14] = num11;
					for (int num15 = 0; num15 < wallOptionCounts[num14]; num15++)
					{
						colorLookup[num11] = array3[num14][num15];
						num11 = (ushort)(num11 + 1);
					}
				}
				else
				{
					wallLookup[num14] = 0;
				}
			}
			wallRangeEnd = num11;
			liquidPosition = num11;
			for (int num16 = 0; num16 < 3; num16++)
			{
				colorLookup[num11] = array2[num16];
				num11 = (ushort)(num11 + 1);
			}
			skyPosition = num11;
			for (int num17 = 0; num17 < 256; num17++)
			{
				colorLookup[num11] = array4[num17];
				num11 = (ushort)(num11 + 1);
			}
			dirtPosition = num11;
			for (int num18 = 0; num18 < 256; num18++)
			{
				colorLookup[num11] = array5[num18];
				num11 = (ushort)(num11 + 1);
			}
			rockPosition = num11;
			for (int num19 = 0; num19 < 256; num19++)
			{
				colorLookup[num11] = array6[num19];
				num11 = (ushort)(num11 + 1);
			}
			hellPosition = num11;
			colorLookup[num11] = color7;
			snowTypes = new ushort[6];
			snowTypes[0] = tileLookup[147];
			snowTypes[1] = tileLookup[161];
			snowTypes[2] = tileLookup[162];
			snowTypes[3] = tileLookup[163];
			snowTypes[4] = tileLookup[164];
			snowTypes[5] = tileLookup[200];
		}

		public static int TileToLookup(int tileType, int option)
		{
			int num = tileLookup[tileType];
			return num + option;
		}

		public static int LookupCount()
		{
			return colorLookup.Length;
		}

		private static void MapColor(ushort type, ref Color oldColor, byte colorType)
		{
			Color color = WorldGen.paintColor(colorType);
			float num = (float)(int)oldColor.R / 255f;
			float num2 = (float)(int)oldColor.G / 255f;
			float num3 = (float)(int)oldColor.B / 255f;
			if (num2 > num)
			{
				float num4 = num;
				num = num2;
				num2 = num4;
			}
			if (num3 > num)
			{
				float num5 = num;
				num = num3;
				num3 = num5;
			}
			switch (colorType)
			{
			case 29:
			{
				float num7 = num3 * 0.3f;
				oldColor.R = (byte)((float)(int)color.R * num7);
				oldColor.G = (byte)((float)(int)color.G * num7);
				oldColor.B = (byte)((float)(int)color.B * num7);
				break;
			}
			case 30:
				if (type >= wallRangeStart && type <= wallRangeEnd)
				{
					oldColor.R = (byte)((float)(255 - oldColor.R) * 0.5f);
					oldColor.G = (byte)((float)(255 - oldColor.G) * 0.5f);
					oldColor.B = (byte)((float)(255 - oldColor.B) * 0.5f);
				}
				else
				{
					oldColor.R = (byte)(255 - oldColor.R);
					oldColor.G = (byte)(255 - oldColor.G);
					oldColor.B = (byte)(255 - oldColor.B);
				}
				break;
			default:
			{
				float num6 = num;
				oldColor.R = (byte)((float)(int)color.R * num6);
				oldColor.G = (byte)((float)(int)color.G * num6);
				oldColor.B = (byte)((float)(int)color.B * num6);
				break;
			}
			}
		}

		public static Color GetMapTileXnaColor(ref MapTile tile)
		{
			Color oldColor = colorLookup[tile.Type];
			byte color = tile.Color;
			if (color > 0)
			{
				MapColor(tile.Type, ref oldColor, color);
			}
			if (tile.Light == byte.MaxValue)
			{
				return oldColor;
			}
			float num = (float)(int)tile.Light / 255f;
			oldColor.R = (byte)((float)(int)oldColor.R * num);
			oldColor.G = (byte)((float)(int)oldColor.G * num);
			oldColor.B = (byte)((float)(int)oldColor.B * num);
			return oldColor;
		}

		public static MapTile CreateMapTile(int i, int j, byte Light)
		{
			Tile tile = Main.tile[i, j];
			if (tile == null)
			{
				tile = (Main.tile[i, j] = new Tile());
			}
			int num = 0;
			int num2 = Light;
			ushort type = Main.Map[i, j].Type;
			int num3 = 0;
			int num4 = 0;
			if (tile.active())
			{
				int type2 = tile.type;
				num3 = tileLookup[type2];
				if (type2 == 51 && (i + j) % 2 == 0)
				{
					num3 = 0;
				}
				if (num3 != 0)
				{
					num = ((type2 != 160) ? tile.color() : 0);
					switch (type2)
					{
					case 4:
						if (tile.frameX < 66)
						{
							num4 = 1;
						}
						num4 = 0;
						break;
					case 21:
						switch (tile.frameX / 36)
						{
						case 1:
						case 2:
						case 10:
						case 13:
						case 15:
							num4 = 1;
							break;
						case 3:
						case 4:
							num4 = 2;
							break;
						case 6:
							num4 = 3;
							break;
						case 11:
						case 17:
							num4 = 4;
							break;
						default:
							num4 = 0;
							break;
						}
						break;
					case 28:
						num4 = ((tile.frameY >= 144) ? ((tile.frameY < 252) ? 1 : ((tile.frameY >= 360 && (tile.frameY <= 900 || tile.frameY >= 1008)) ? ((tile.frameY >= 468) ? ((tile.frameY >= 576) ? ((tile.frameY >= 684) ? ((tile.frameY >= 792) ? ((tile.frameY >= 898) ? ((tile.frameY >= 1006) ? ((tile.frameY >= 1114) ? ((tile.frameY >= 1222) ? 7 : 3) : 0) : 7) : 8) : 6) : 5) : 4) : 3) : 2)) : 0);
						break;
					case 27:
						num4 = ((tile.frameY < 34) ? 1 : 0);
						break;
					case 31:
						num4 = ((tile.frameX >= 36) ? 1 : 0);
						break;
					case 26:
						num4 = ((tile.frameX >= 54) ? 1 : 0);
						break;
					case 137:
						num4 = ((tile.frameY != 0) ? 1 : 0);
						break;
					case 82:
					case 83:
					case 84:
						num4 = ((tile.frameX >= 18) ? ((tile.frameX < 36) ? 1 : ((tile.frameX >= 54) ? ((tile.frameX >= 72) ? ((tile.frameX >= 90) ? ((tile.frameX >= 108) ? 6 : 5) : 4) : 3) : 2)) : 0);
						break;
					case 105:
						num4 = ((tile.frameX >= 1548 && tile.frameX <= 1654) ? 1 : ((tile.frameX >= 1656 && tile.frameX <= 1798) ? 2 : 0));
						break;
					case 133:
						num4 = ((tile.frameX >= 52) ? 1 : 0);
						break;
					case 134:
						num4 = ((tile.frameX >= 28) ? 1 : 0);
						break;
					case 149:
						num4 = j % 3;
						break;
					case 160:
						num4 = j % 3;
						break;
					case 165:
						num4 = ((tile.frameX >= 54) ? ((tile.frameX < 106) ? 1 : ((tile.frameX >= 216) ? 1 : ((tile.frameX >= 162) ? 3 : 2))) : 0);
						break;
					case 178:
						num4 = ((tile.frameX >= 18) ? ((tile.frameX < 36) ? 1 : ((tile.frameX >= 54) ? ((tile.frameX >= 72) ? ((tile.frameX >= 90) ? ((tile.frameX >= 108) ? 6 : 5) : 4) : 3) : 2)) : 0);
						break;
					case 184:
						num4 = ((tile.frameX >= 22) ? ((tile.frameX < 44) ? 1 : ((tile.frameX >= 66) ? ((tile.frameX >= 88) ? ((tile.frameX >= 110) ? 5 : 4) : 3) : 2)) : 0);
						break;
					case 185:
						if (tile.frameY < 18)
						{
							int num5 = tile.frameX / 18;
							if (num5 < 6 || num5 == 28 || num5 == 29 || num5 == 30 || num5 == 31 || num5 == 32)
							{
								num4 = 0;
							}
							else if (num5 < 12 || num5 == 33 || num5 == 34 || num5 == 35)
							{
								num4 = 1;
							}
							else if (num5 < 28)
							{
								num4 = 2;
							}
							else if (num5 < 48)
							{
								num4 = 3;
							}
							else if (num5 < 54)
							{
								num4 = 4;
							}
						}
						else
						{
							int num5 = tile.frameX / 36;
							if (num5 < 6 || num5 == 19 || num5 == 20 || num5 == 21 || num5 == 22 || num5 == 23 || num5 == 24 || num5 == 33 || num5 == 38 || num5 == 39 || num5 == 40)
							{
								num4 = 0;
							}
							else if (num5 < 16)
							{
								num4 = 2;
							}
							else if (num5 < 19 || num5 == 31 || num5 == 32)
							{
								num4 = 1;
							}
							else if (num5 < 31)
							{
								num4 = 3;
							}
							else if (num5 < 38)
							{
								num4 = 4;
							}
						}
						break;
					case 186:
					{
						int num5 = tile.frameX / 54;
						if (num5 < 7)
						{
							num4 = 2;
						}
						else if (num5 < 22 || num5 == 33 || num5 == 34 || num5 == 35)
						{
							num4 = 0;
						}
						else if (num5 < 25)
						{
							num4 = 1;
						}
						else if (num5 == 25)
						{
							num4 = 5;
						}
						else if (num5 < 32)
						{
							num4 = 3;
						}
						break;
					}
					case 187:
					{
						int num5 = tile.frameX / 54;
						if (num5 < 3 || num5 == 14 || num5 == 15 || num5 == 16)
						{
							num4 = 0;
						}
						else if (num5 < 6)
						{
							num4 = 6;
						}
						else if (num5 < 9)
						{
							num4 = 7;
						}
						else if (num5 < 14)
						{
							num4 = 4;
						}
						else if (num5 < 18)
						{
							num4 = 4;
						}
						else if (num5 < 23)
						{
							num4 = 8;
						}
						else if (num5 < 25)
						{
							num4 = 0;
						}
						else if (num5 < 29)
						{
							num4 = 1;
						}
						break;
					}
					case 227:
						num4 = tile.frameX / 34;
						break;
					case 240:
					{
						int num5 = tile.frameX / 54;
						int num6 = tile.frameY / 54;
						num5 += num6 * 36;
						if ((num5 >= 0 && num5 <= 11) || (num5 >= 47 && num5 <= 53))
						{
							num4 = 0;
							break;
						}
						if (num5 >= 12 && num5 <= 15)
						{
							num4 = 1;
							break;
						}
						switch (num5)
						{
						case 16:
						case 17:
							num4 = 2;
							break;
						case 18:
						case 19:
						case 20:
						case 21:
						case 22:
						case 23:
						case 24:
						case 25:
						case 26:
						case 27:
						case 28:
						case 29:
						case 30:
						case 31:
						case 32:
						case 33:
						case 34:
						case 35:
							num4 = 1;
							break;
						default:
							if (num5 >= 41 && num5 <= 45)
							{
								num4 = 3;
							}
							else if (num5 == 46)
							{
								num4 = 4;
							}
							break;
						}
						break;
					}
					case 242:
					{
						int num5 = tile.frameY / 72;
						num4 = ((num5 >= 22 && num5 <= 24) ? 1 : 0);
						break;
					}
					default:
						num4 = 0;
						break;
					}
				}
			}
			if (num3 == 0)
			{
				if (tile.liquid > 32)
				{
					int num7 = tile.liquidType();
					num3 = liquidPosition + num7;
				}
				else if (tile.wall > 0)
				{
					int wall = tile.wall;
					num3 = wallLookup[wall];
					num = tile.wallColor();
					switch (wall)
					{
					case 21:
					case 88:
					case 89:
					case 90:
					case 91:
					case 92:
					case 93:
					case 168:
						num = 0;
						break;
					case 27:
						num4 = i % 2;
						break;
					default:
						num4 = 0;
						break;
					}
				}
			}
			if (num3 == 0)
			{
				if ((double)j < Main.worldSurface)
				{
					int num8 = (byte)(255.0 * ((double)j / Main.worldSurface));
					num3 = skyPosition + num8;
					num2 = 255;
					num = 0;
				}
				else if (j < Main.maxTilesY - 200)
				{
					num = 0;
					bool flag = (type < dirtPosition || type >= hellPosition) ? true : false;
					byte b = 0;
					float num9 = Main.screenPosition.X / 16f - 5f;
					float num10 = (Main.screenPosition.X + (float)Main.screenWidth) / 16f + 5f;
					float num11 = Main.screenPosition.Y / 16f - 5f;
					float num12 = (Main.screenPosition.Y + (float)Main.screenHeight) / 16f + 5f;
					if (((float)i < num9 || (float)i > num10 || (float)j < num11 || (float)j > num12) && i > 40 && i < Main.maxTilesX - 40 && j > 40 && j < Main.maxTilesY - 40 && flag)
					{
						for (int k = i - 36; k <= i + 30; k += 10)
						{
							for (int l = j - 36; l <= j + 30; l += 10)
							{
								for (int m = 0; m < snowTypes.Length; m++)
								{
									if (snowTypes[m] == type)
									{
										b = byte.MaxValue;
										k = i + 31;
										l = j + 31;
										break;
									}
								}
							}
						}
					}
					else
					{
						float num13 = (float)Main.snowTiles / 1000f;
						num13 *= 255f;
						if (num13 > 255f)
						{
							num13 = 255f;
						}
						b = (byte)num13;
					}
					num3 = ((!((double)j < Main.rockLayer)) ? (rockPosition + b) : (dirtPosition + b));
				}
				else
				{
					num3 = hellPosition;
				}
			}
			int num14 = num3 + num4;
			return MapTile.Create((ushort)num14, (byte)num2, (byte)num);
		}

		public static void SaveMap()
		{
			bool ısCloudSave = Main.ActivePlayerFileData.IsCloudSave;
			if ((!ısCloudSave || SocialAPI.Cloud != null) && Main.mapEnabled && !saveLock)
			{
				string text = Main.playerPathName.Substring(0, Main.playerPathName.Length - 4);
				lock (padlock)
				{
					try
					{
						saveLock = true;
						try
						{
							if (!ısCloudSave)
							{
								Directory.CreateDirectory(text);
							}
						}
						catch
						{
						}
						text = text + Path.DirectorySeparatorChar + Main.worldID + ".map";
						Stopwatch stopwatch = new Stopwatch();
						stopwatch.Start();
						bool flag = false;
						if (!Main.gameMenu)
						{
							flag = true;
						}
						using (MemoryStream memoryStream = new MemoryStream(4000))
						{
							using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
							{
								using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress))
								{
									int num = 0;
									byte[] array = new byte[16384];
									binaryWriter.Write(Main.curRelease);
									Main.MapFileMetadata.IncrementAndWrite(binaryWriter);
									binaryWriter.Write(Main.worldName);
									binaryWriter.Write(Main.worldID);
									binaryWriter.Write(Main.maxTilesY);
									binaryWriter.Write(Main.maxTilesX);
									binaryWriter.Write((short)419);
									binaryWriter.Write((short)225);
									binaryWriter.Write((short)3);
									binaryWriter.Write((short)256);
									binaryWriter.Write((short)256);
									binaryWriter.Write((short)256);
									byte b = 1;
									byte b2 = 0;
									int i;
									for (i = 0; i < 419; i++)
									{
										if (tileOptionCounts[i] != 1)
										{
											b2 = (byte)(b2 | b);
										}
										if (b == 128)
										{
											binaryWriter.Write(b2);
											b2 = 0;
											b = 1;
										}
										else
										{
											b = (byte)(b << 1);
										}
									}
									if (b != 1)
									{
										binaryWriter.Write(b2);
									}
									i = 0;
									b = 1;
									b2 = 0;
									for (; i < 225; i++)
									{
										if (wallOptionCounts[i] != 1)
										{
											b2 = (byte)(b2 | b);
										}
										if (b == 128)
										{
											binaryWriter.Write(b2);
											b2 = 0;
											b = 1;
										}
										else
										{
											b = (byte)(b << 1);
										}
									}
									if (b != 1)
									{
										binaryWriter.Write(b2);
									}
									for (i = 0; i < 419; i++)
									{
										if (tileOptionCounts[i] != 1)
										{
											binaryWriter.Write((byte)tileOptionCounts[i]);
										}
									}
									for (i = 0; i < 225; i++)
									{
										if (wallOptionCounts[i] != 1)
										{
											binaryWriter.Write((byte)wallOptionCounts[i]);
										}
									}
									binaryWriter.Flush();
									for (int j = 0; j < Main.maxTilesY; j++)
									{
										if (!flag)
										{
											float num2 = (float)j / (float)Main.maxTilesY;
											Main.statusText = string.Concat(Lang.gen[66], " ", (int)(num2 * 100f + 1f), "%");
										}
										int num3;
										for (num3 = 0; num3 < Main.maxTilesX; num3++)
										{
											MapTile mapTile = Main.Map[num3, j];
											byte b3;
											byte b4 = b3 = 0;
											int num4 = 0;
											bool flag2 = true;
											bool flag3 = true;
											int num5 = 0;
											int num6 = 0;
											byte b5 = 0;
											int num7;
											ushort num8;
											if (mapTile.Light <= 18)
											{
												flag3 = false;
												flag2 = false;
												num7 = 0;
												num8 = 0;
												num4 = 0;
												int num9 = num3 + 1;
												int num10 = Main.maxTilesX - num3 - 1;
												while (num10 > 0)
												{
													MapTile mapTile2 = Main.Map[num9, j];
													if (mapTile2.Light > 18)
													{
														break;
													}
													num4++;
													num10--;
													num9++;
												}
											}
											else
											{
												b5 = mapTile.Color;
												num8 = mapTile.Type;
												if (num8 < wallPosition)
												{
													num7 = 1;
													num8 = (ushort)(num8 - tilePosition);
												}
												else if (num8 < liquidPosition)
												{
													num7 = 2;
													num8 = (ushort)(num8 - wallPosition);
												}
												else if (num8 < skyPosition)
												{
													num7 = 3 + (num8 - liquidPosition);
													flag2 = false;
												}
												else if (num8 < dirtPosition)
												{
													num7 = 6;
													flag3 = false;
													flag2 = false;
												}
												else if (num8 < hellPosition)
												{
													num7 = 7;
													num8 = ((num8 >= rockPosition) ? ((ushort)(num8 - rockPosition)) : ((ushort)(num8 - dirtPosition)));
												}
												else
												{
													num7 = 6;
													flag2 = false;
												}
												if (mapTile.Light == byte.MaxValue)
												{
													flag3 = false;
												}
												if (flag3)
												{
													num4 = 0;
													int num9 = num3 + 1;
													int num10 = Main.maxTilesX - num3 - 1;
													num5 = num9;
													while (num10 > 0)
													{
														MapTile other = Main.Map[num9, j];
														if (!mapTile.EqualsWithoutLight(ref other))
														{
															num6 = num9;
															break;
														}
														num10--;
														num4++;
														num9++;
													}
												}
												else
												{
													num4 = 0;
													int num9 = num3 + 1;
													int num10 = Main.maxTilesX - num3 - 1;
													while (num10 > 0)
													{
														MapTile other2 = Main.Map[num9, j];
														if (!mapTile.Equals(ref other2))
														{
															break;
														}
														num10--;
														num4++;
														num9++;
													}
												}
											}
											if (b5 > 0)
											{
												b3 = (byte)(b3 | (byte)(b5 << 1));
											}
											if (b3 != 0)
											{
												b4 = (byte)(b4 | 1);
											}
											b4 = (byte)(b4 | (byte)(num7 << 1));
											if (flag2 && num8 > 255)
											{
												b4 = (byte)(b4 | 0x10);
											}
											if (flag3)
											{
												b4 = (byte)(b4 | 0x20);
											}
											if (num4 > 0)
											{
												b4 = ((num4 <= 255) ? ((byte)(b4 | 0x40)) : ((byte)(b4 | 0x80)));
											}
											array[num] = b4;
											num++;
											if (b3 != 0)
											{
												array[num] = b3;
												num++;
											}
											if (flag2)
											{
												array[num] = (byte)num8;
												num++;
												if (num8 > 255)
												{
													array[num] = (byte)(num8 >> 8);
													num++;
												}
											}
											if (flag3)
											{
												array[num] = mapTile.Light;
												num++;
											}
											if (num4 > 0)
											{
												array[num] = (byte)num4;
												num++;
												if (num4 > 255)
												{
													array[num] = (byte)(num4 >> 8);
													num++;
												}
											}
											for (int k = num5; k < num6; k++)
											{
												array[num] = Main.Map[k, j].Light;
												num++;
											}
											num3 += num4;
											if (num >= 4096)
											{
												deflateStream.Write(array, 0, num);
												num = 0;
											}
										}
									}
									if (num > 0)
									{
										deflateStream.Write(array, 0, num);
									}
									deflateStream.Dispose();
									FileUtilities.WriteAllBytes(text, memoryStream.ToArray(), ısCloudSave);
								}
							}
						}
					}
					catch (Exception value)
					{
						using (StreamWriter streamWriter = new StreamWriter("client-crashlog.txt", true))
						{
							streamWriter.WriteLine(DateTime.Now);
							streamWriter.WriteLine(value);
							streamWriter.WriteLine("");
						}
					}
					saveLock = false;
				}
			}
		}

		public static void LoadMapVersion1(BinaryReader fileIO, int release)
		{
			string a = fileIO.ReadString();
			int num = fileIO.ReadInt32();
			int num2 = fileIO.ReadInt32();
			int num3 = fileIO.ReadInt32();
			if (a != Main.worldName || num != Main.worldID || num3 != Main.maxTilesX || num2 != Main.maxTilesY)
			{
				throw new Exception("Map meta-data is invalid.");
			}
			OldMapHelper oldMapHelper = default(OldMapHelper);
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				float num4 = (float)i / (float)Main.maxTilesX;
				Main.statusText = string.Concat(Lang.gen[67], " ", (int)(num4 * 100f + 1f), "%");
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					if (fileIO.ReadBoolean())
					{
						int num5 = (release <= 77) ? fileIO.ReadByte() : fileIO.ReadUInt16();
						byte b = fileIO.ReadByte();
						oldMapHelper.misc = fileIO.ReadByte();
						if (release >= 50)
						{
							oldMapHelper.misc2 = fileIO.ReadByte();
						}
						else
						{
							oldMapHelper.misc2 = 0;
						}
						bool flag = false;
						int num6 = oldMapHelper.option();
						int num7;
						if (oldMapHelper.active())
						{
							num7 = num6 + tileLookup[num5];
						}
						else if (oldMapHelper.water())
						{
							num7 = liquidPosition;
						}
						else if (oldMapHelper.lava())
						{
							num7 = liquidPosition + 1;
						}
						else if (oldMapHelper.honey())
						{
							num7 = liquidPosition + 2;
						}
						else if (oldMapHelper.wall())
						{
							num7 = num6 + wallLookup[num5];
						}
						else if ((double)j < Main.worldSurface)
						{
							flag = true;
							int num8 = (byte)(256.0 * ((double)j / Main.worldSurface));
							num7 = skyPosition + num8;
						}
						else if ((double)j < Main.rockLayer)
						{
							flag = true;
							if (num5 > 255)
							{
								num5 = 255;
							}
							num7 = num5 + dirtPosition;
						}
						else if (j < Main.maxTilesY - 200)
						{
							flag = true;
							if (num5 > 255)
							{
								num5 = 255;
							}
							num7 = num5 + rockPosition;
						}
						else
						{
							num7 = hellPosition;
						}
						MapTile tile = MapTile.Create((ushort)num7, b, 0);
						Main.Map.SetTile(i, j, ref tile);
						int num9 = fileIO.ReadInt16();
						if (b == byte.MaxValue)
						{
							while (num9 > 0)
							{
								num9--;
								j++;
								if (flag)
								{
									if ((double)j < Main.worldSurface)
									{
										flag = true;
										int num10 = (byte)(256.0 * ((double)j / Main.worldSurface));
										num7 = skyPosition + num10;
									}
									else if ((double)j < Main.rockLayer)
									{
										flag = true;
										num7 = num5 + dirtPosition;
									}
									else if (j < Main.maxTilesY - 200)
									{
										flag = true;
										num7 = num5 + rockPosition;
									}
									else
									{
										flag = true;
										num7 = hellPosition;
									}
									tile.Type = (ushort)num7;
								}
								Main.Map.SetTile(i, j, ref tile);
							}
							continue;
						}
						while (num9 > 0)
						{
							j++;
							num9--;
							b = fileIO.ReadByte();
							if (b <= 18)
							{
								continue;
							}
							tile.Light = b;
							if (flag)
							{
								if ((double)j < Main.worldSurface)
								{
									flag = true;
									int num11 = (byte)(256.0 * ((double)j / Main.worldSurface));
									num7 = skyPosition + num11;
								}
								else if ((double)j < Main.rockLayer)
								{
									flag = true;
									num7 = num5 + dirtPosition;
								}
								else if (j < Main.maxTilesY - 200)
								{
									flag = true;
									num7 = num5 + rockPosition;
								}
								else
								{
									flag = true;
									num7 = hellPosition;
								}
								tile.Type = (ushort)num7;
							}
							Main.Map.SetTile(i, j, ref tile);
						}
					}
					else
					{
						int num12 = fileIO.ReadInt16();
						j += num12;
					}
				}
			}
		}

		public static void LoadMapVersion2(BinaryReader fileIO, int release)
		{
			if (release >= 135)
			{
				Main.MapFileMetadata = FileMetadata.Read(fileIO, FileType.Map);
			}
			else
			{
				Main.MapFileMetadata = FileMetadata.FromCurrentSettings(FileType.Map);
			}
			string a = fileIO.ReadString();
			int num = fileIO.ReadInt32();
			int num2 = fileIO.ReadInt32();
			int num3 = fileIO.ReadInt32();
			if (a != Main.worldName || num != Main.worldID || num3 != Main.maxTilesX || num2 != Main.maxTilesY)
			{
				throw new Exception("Map meta-data is invalid.");
			}
			short num4 = fileIO.ReadInt16();
			short num5 = fileIO.ReadInt16();
			short num6 = fileIO.ReadInt16();
			short num7 = fileIO.ReadInt16();
			short num8 = fileIO.ReadInt16();
			short num9 = fileIO.ReadInt16();
			bool[] array = new bool[num4];
			byte b = 0;
			byte b2 = 128;
			for (int i = 0; i < num4; i++)
			{
				if (b2 == 128)
				{
					b = fileIO.ReadByte();
					b2 = 1;
				}
				else
				{
					b2 = (byte)(b2 << 1);
				}
				if ((b & b2) == b2)
				{
					array[i] = true;
				}
			}
			bool[] array2 = new bool[num5];
			b = 0;
			b2 = 128;
			for (int i = 0; i < num5; i++)
			{
				if (b2 == 128)
				{
					b = fileIO.ReadByte();
					b2 = 1;
				}
				else
				{
					b2 = (byte)(b2 << 1);
				}
				if ((b & b2) == b2)
				{
					array2[i] = true;
				}
			}
			byte[] array3 = new byte[num4];
			ushort num10 = 0;
			for (int i = 0; i < num4; i++)
			{
				if (array[i])
				{
					array3[i] = fileIO.ReadByte();
				}
				else
				{
					array3[i] = 1;
				}
				num10 = (ushort)(num10 + array3[i]);
			}
			byte[] array4 = new byte[num5];
			ushort num11 = 0;
			for (int i = 0; i < num5; i++)
			{
				if (array2[i])
				{
					array4[i] = fileIO.ReadByte();
				}
				else
				{
					array4[i] = 1;
				}
				num11 = (ushort)(num11 + array4[i]);
			}
			int num12 = num10 + num11 + num6 + num7 + num8 + num9 + 2;
			ushort[] array5 = new ushort[num12];
			array5[0] = 0;
			ushort num13 = 1;
			ushort num14 = 1;
			ushort num15 = num14;
			for (int i = 0; i < 419; i++)
			{
				if (i < num4)
				{
					int num16 = array3[i];
					int num17 = tileOptionCounts[i];
					for (int j = 0; j < num17; j++)
					{
						if (j < num16)
						{
							array5[num14] = num13;
							num14 = (ushort)(num14 + 1);
						}
						num13 = (ushort)(num13 + 1);
					}
				}
				else
				{
					num13 = (ushort)(num13 + (ushort)tileOptionCounts[i]);
				}
			}
			ushort num18 = num14;
			for (int i = 0; i < 225; i++)
			{
				if (i < num5)
				{
					int num19 = array4[i];
					int num20 = wallOptionCounts[i];
					for (int k = 0; k < num20; k++)
					{
						if (k < num19)
						{
							array5[num14] = num13;
							num14 = (ushort)(num14 + 1);
						}
						num13 = (ushort)(num13 + 1);
					}
				}
				else
				{
					num13 = (ushort)(num13 + (ushort)wallOptionCounts[i]);
				}
			}
			ushort num21 = num14;
			for (int i = 0; i < 3; i++)
			{
				if (i < num6)
				{
					array5[num14] = num13;
					num14 = (ushort)(num14 + 1);
				}
				num13 = (ushort)(num13 + 1);
			}
			ushort num22 = num14;
			for (int i = 0; i < 256; i++)
			{
				if (i < num7)
				{
					array5[num14] = num13;
					num14 = (ushort)(num14 + 1);
				}
				num13 = (ushort)(num13 + 1);
			}
			ushort num23 = num14;
			for (int i = 0; i < 256; i++)
			{
				if (i < num8)
				{
					array5[num14] = num13;
					num14 = (ushort)(num14 + 1);
				}
				num13 = (ushort)(num13 + 1);
			}
			ushort num24 = num14;
			for (int i = 0; i < 256; i++)
			{
				if (i < num9)
				{
					array5[num14] = num13;
					num14 = (ushort)(num14 + 1);
				}
				num13 = (ushort)(num13 + 1);
			}
			ushort num25 = num14;
			array5[num14] = num13;
			BinaryReader binaryReader;
			if (release >= 93)
			{
				DeflateStream input = new DeflateStream(fileIO.BaseStream, CompressionMode.Decompress);
				binaryReader = new BinaryReader(input);
			}
			else
			{
				binaryReader = new BinaryReader(fileIO.BaseStream);
			}
			for (int l = 0; l < Main.maxTilesY; l++)
			{
				float num26 = (float)l / (float)Main.maxTilesY;
				Main.statusText = string.Concat(Lang.gen[67], " ", (int)(num26 * 100f + 1f), "%");
				for (int m = 0; m < Main.maxTilesX; m++)
				{
					byte b3 = binaryReader.ReadByte();
					byte b4 = (byte)(((b3 & 1) == 1) ? binaryReader.ReadByte() : 0);
					byte b5 = (byte)((b3 & 0xE) >> 1);
					bool flag;
					switch (b5)
					{
					case 0:
						flag = false;
						break;
					case 1:
					case 2:
					case 7:
						flag = true;
						break;
					case 3:
					case 4:
					case 5:
						flag = false;
						break;
					case 6:
						flag = false;
						break;
					default:
						flag = false;
						break;
					}
					ushort num27 = (ushort)(flag ? (((b3 & 0x10) != 16) ? binaryReader.ReadByte() : binaryReader.ReadUInt16()) : 0);
					byte b6 = ((b3 & 0x20) != 32) ? byte.MaxValue : binaryReader.ReadByte();
					int num28;
					switch ((byte)((b3 & 0xC0) >> 6))
					{
					case 0:
						num28 = 0;
						break;
					case 1:
						num28 = binaryReader.ReadByte();
						break;
					case 2:
						num28 = binaryReader.ReadInt16();
						break;
					default:
						num28 = 0;
						break;
					}
					switch (b5)
					{
					case 0:
						m += num28;
						continue;
					case 1:
						num27 = (ushort)(num27 + num15);
						break;
					case 2:
						num27 = (ushort)(num27 + num18);
						break;
					case 3:
					case 4:
					case 5:
						num27 = (ushort)(num27 + (ushort)(num21 + (b5 - 3)));
						break;
					case 6:
						if ((double)l < Main.worldSurface)
						{
							ushort num29 = (ushort)((double)num7 * ((double)l / Main.worldSurface));
							num27 = (ushort)(num27 + (ushort)(num22 + num29));
						}
						else
						{
							num27 = num25;
						}
						break;
					case 7:
						num27 = ((!((double)l < Main.rockLayer)) ? ((ushort)(num27 + num24)) : ((ushort)(num27 + num23)));
						break;
					}
					MapTile tile = MapTile.Create(array5[num27], b6, (byte)((b4 >> 1) & 0x1F));
					Main.Map.SetTile(m, l, ref tile);
					if (b6 == byte.MaxValue)
					{
						while (num28 > 0)
						{
							m++;
							Main.Map.SetTile(m, l, ref tile);
							num28--;
						}
						continue;
					}
					while (num28 > 0)
					{
						m++;
						tile = tile.WithLight(binaryReader.ReadByte());
						Main.Map.SetTile(m, l, ref tile);
						num28--;
					}
				}
			}
			binaryReader.Close();
		}
	}
}
