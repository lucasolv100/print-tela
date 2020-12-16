using System;
using System.Drawing;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using OpenCvSharp;

namespace PrintTela
{
	class Program
	{
		//{"X":1141,"Y":225,"Width":311,"Height":539,"Top":225,"Bottom":764,"Left":1141,"Right":1452,"Location":{"X":1141,"Y":225},"Size":{"Width":311,"Height":539},"TopLeft":{"X":1141,"Y":225},"BottomRight":{"X":1452,"Y":764}}

		public static Rect ROI { get; set; } = new Rect {
			X = 1141,
			Y = 225,
			Width = 331-15,
			Height = 539
		};
		private static Random random = new Random();
		private static Config cfg { get; set; }


		static void Main(string[] args)
		{
			// Environment.CurrentDirectory + "config.json";
			string datafile = File.ReadAllText(@"config.json");
			cfg = JsonConvert.DeserializeObject<Config>(datafile);
			PrintScreen(cfg.LarguraTela, cfg.AlturaTela);
			Console.WriteLine("Classificado");
		}

		private static void PrintScreen(int width, int height)
		{
			Bitmap printscreen = new Bitmap(width, height);
			Graphics graphics = Graphics.FromImage(printscreen as Image);
			graphics.CopyFromScreen(0, 0, 0, 0, printscreen.Size);
			using (Mat imagem = MatToBitmap(printscreen))
			{
				// ROI = Cv2.SelectROI(imagem);
				// Console.WriteLine(JsonConvert.SerializeObject(ROI));
				var imgFrag = new Mat(imagem, ROI);
				Console.WriteLine("Classificação: ");
				Console.WriteLine("1 - Vermelho, 2 - Verde, 3 - Predição");
				
				var resp = Console.ReadLine();
				int classific = int.TryParse(resp, out _) ? int.Parse(resp) : 0;
				
				if (classific > 0)
				{
					switch (classific)
					{
						case 1:
							Cv2.ImWrite(Path.Combine(cfg.PathVermelho + RandomString(20) + ".jpg"), imgFrag);
							break;
						case 2:
							Cv2.ImWrite(Path.Combine(cfg.PathVerde + RandomString(20) + ".jpg"), imgFrag);
							break;
						case 3:
							Cv2.ImWrite(Path.Combine(cfg.PathPred + RandomString(20) + ".jpg"), imgFrag);
							break;
					}
				}
					
			}
			// printscreen.Save(@"C:\temp\printscreen.jpg", ImageFormat.Jpeg);
		}

		public static Mat MatToBitmap(Bitmap image)
		{
			return OpenCvSharp.Extensions.BitmapConverter.ToMat(image);
		}

		public static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
