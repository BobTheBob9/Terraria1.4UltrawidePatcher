using System;
using System.IO;

using Mono.Cecil;

namespace TerrariaUltrawidePatcher
{
	class Program
	{
		static void Main(string[] args)
		{
			//Terraria::Main.DoDraw(Gametime): remove IL instructions 968-983 (starts at line 363 of c# decompile)
			if (args.Length == 0)
			{
				Console.WriteLine("Usage: Terraria1.4UltrawidePatcher.exe path/to/Terraria.exe");
				return;
			}

			string path = args[0];
			string newPath = Path.GetDirectoryName(path) + "\\Terraria-unpatched.exe"; 

			File.Move(path, newPath); //move the unpatched terraria exe

			AssemblyDefinition terraria = AssemblyDefinition.ReadAssembly(newPath); //get assembly
			MethodDefinition[] methods = terraria.MainModule.GetType("Terraria.Main").Methods.ToArray(); //get methods

			MethodDefinition doDrawMethod = null;
			foreach (var method in methods)
				if (method.FullName == "System.Void Terraria.Main::DoDraw(Microsoft.Xna.Framework.GameTime)") //find the method we want to patch
				{
					doDrawMethod = method;
					break;
				}

			for (int i = 983; i >= 968; i--)
				doDrawMethod.Body.Instructions.RemoveAt(i); //remove IL instructions 968-983

			terraria.Write(path); //write assembly to old path

			//dirty way of avoiding oom: thanks to FakeMichau
			using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite)) 
			{
				stream.Position = 150;
				stream.WriteByte(0x22);
			}
			
			Console.WriteLine("Done! Your unpatched terraria exe has been renamed to Terraria-unpatched.exe");
		}
	}
}
