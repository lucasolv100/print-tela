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
		public static Rect ROI { get; set; }
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
				ROI = Cv2.SelectROI(imagem);
				var imgFrag = new Mat(imagem, ROI);
				Console.WriteLine("Classificação: ");
				Console.WriteLine("1 - Vermelho, 2 - Verde");
				
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
