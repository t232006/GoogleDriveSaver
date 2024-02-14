using Google.Apis.Download;
using Google.Apis.Drive.v3.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GoogleDriveManipulator
{
	public class GoogleDownloader : GoogleHelper
	{
		FileList response;
		//public string[] list;
		//string output;
		//public MemoryStream stream;
		GoogleDownloader(string _token, string _filePath, string _fileName) : base(_token, _filePath, _fileName)
		{
		}
		public static async Task<GoogleDownloader> Download(string token, string whereTo, string fileName)
		{
			var instance = new GoogleDownloader(token, whereTo, fileName);
			await instance.Start();
			string ID = "";
			//instance.output = "file not found"; //initial value
			var request = instance.driveService.Files.List();
			instance.response = request.Execute();
			foreach (var file in instance.response.Files)
			{
				if (file.Name == fileName)
				{
					ID = file.Id;
					//instance.output = "found";
					break;
				}
			}
			var request1 = instance.driveService.Files.Get(ID);
			MemoryStream stream = new MemoryStream();
			request1.Download(stream);
			using (FileStream fs = new FileStream(Path.Combine(whereTo, fileName), FileMode.Create, FileAccess.Write))
				stream.WriteTo(fs);
			return instance;
		}
	}
}
