﻿using Google.Apis.Download;
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
		public Dictionary<string, string> list;
		//string output;
		//public MemoryStream stream;
		public GoogleDownloader(string _token) : base(_token)
		{
		}
		public static async Task<GoogleDownloader> DownloadList(string token)
		{
			var instance = new GoogleDownloader(token);
			await instance.Start();
			var request = instance.driveService.Files.List();
			request.Q = "fileExtension='db' and trashed=false";
			request.Fields = "nextPageToken, files(id,name)";
			request.PageSize = 100;
			instance.response = request.Execute();
			Dictionary<string, string> templist = new Dictionary<string, string>();
			try
			{
				do
				{
					foreach (var file in instance.response.Files)
					{
						if (Path.GetExtension(file.Name) == ".db")
						{
							if (!templist.ContainsKey(file.Name))
								templist.Add(file.Name, file.Id);
						}
						
					}
					request.PageToken = instance.response.NextPageToken;
				}
				while (!string.IsNullOrEmpty(instance.response.NextPageToken));
			}
			catch(Google.GoogleApiException ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
			instance.list = templist;
			return instance;
		}
		public void DownloadFile(string FileId, string filename)
		{
			var request = driveService.Files.Get(FileId);
			var stream = new MemoryStream();
			request.Download(stream);
			using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write))
			{
				stream.WriteTo(fs);
			}
		}
	}
}
