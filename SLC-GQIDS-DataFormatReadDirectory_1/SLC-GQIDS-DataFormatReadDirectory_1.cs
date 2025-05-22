/*
****************************************************************************
*  Copyright (c) 2025,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

dd/mm/2025	1.0.0.1		XXX, Skyline	Initial version
****************************************************************************
*/

namespace SLC_GQIDS_DataFormatReadDirectory_1
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Automation;

	[GQIMetaData(Name = "List Files in Directory")]
	public class ListDirectoryDataSource : IGQIDataSource, IGQIInputArguments
	{
		private readonly GQIStringArgument _argDirectoryPath = new GQIStringArgument("Directory Path") { IsRequired = true };

		private readonly GQIBooleanArgument _argRecursive = new GQIBooleanArgument("Recursive") { IsRequired = true };

		private string _path;

		private bool _isRecursive;

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[]
			{
				new GQIStringColumn("Path"),
				new GQIStringColumn("File"),
				new GQIStringColumn("Type"),
			};
		}

		public GQIArgument[] GetInputArguments()
		{
			return new GQIArgument[] { _argDirectoryPath, _argRecursive };
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			var files = ListFilesInDirectory(_path, _isRecursive);
			var rows = CreateRows(files);

			return new GQIPage(rows);
		}

		private GQIRow[] CreateRows(List<string> files)
		{
			string partToRemove = @"c:\Skyline DataMiner\Webpages\";
			var rows = new List<GQIRow>();
			foreach (var file in files)
			{
				FileInfo fileInfo = new FileInfo(file);
				var row = new GQIRow(
					new GQICell[]
						{
						new GQICell() { Value = file.Replace(partToRemove, string.Empty)},
						new GQICell() { Value = Path.GetFileName(file)},
						new GQICell() { Value = Path.GetFileName(fileInfo.DirectoryName)},
						});

				rows.Add(row);
			}

			return rows.ToArray();
		}

		public List<string> ListFilesInDirectory(string path, bool recursive)
		{
			List<string> files = new List<string>();

			if (Directory.Exists(path))
			{
				if (recursive)
				{
					files.AddRange(Directory.GetFiles(path, "*.*", SearchOption.AllDirectories));
				}
				else
				{
					files.AddRange(Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly));
				}
			}
			else
			{
				Console.WriteLine($"The directory '{path}' does not exist.");
			}

			return files;
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			_path = args.GetArgumentValue(_argDirectoryPath);
			_isRecursive = args.GetArgumentValue(_argRecursive);
			return new OnArgumentsProcessedOutputArgs();
		}
	}
}