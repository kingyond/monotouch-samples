using System;
using UIKit;

namespace FontList
{
	public class Application
	{
		public static void Main (string[] args)
		{
			try
			{
				UIApplication.Main (args, null, "AppDelegate");
			}
			catch (Exception e)
			{
				Console.WriteLine (e.ToString ());
			}
		}
	}
}
