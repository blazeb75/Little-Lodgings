using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Data;

public static class CSVReader
{
	public static DataTable CsvToTable(TextAsset csv)
	{
		DataTable table = new DataTable(csv.name);
		string[] lines = csv.text.Split("\n"[0]);

		foreach (string header in SplitCsvLine(lines[0]))
		{
			System.Type type;
			if (header.Contains("Vector2[]"))
            {
				type = typeof(Vector2[]);
            }
			else if(header.Contains("Vector2"))

			{
				type = typeof(Vector2);

			}
			else if(header.Contains("Vector3"))

			{
				type = typeof(Vector3);

			}
			else
            {
				type = typeof(string);
            }
			DataColumn column = new DataColumn(header.Replace("\r",""), type);
			table.Columns.Add(column);
		}

		foreach (string line in lines.Skip(1))
		{
			DataRow row = table.NewRow();
			object[] elements = SplitCsvLine(line);
			for(int i = 0; i < table.Columns.Count; i++)
            {
				if(table.Columns[i].DataType == typeof(Vector2))
                {
					string[] xy = ((string)elements[i]).Split(' ');
					elements[i] = new Vector2(float.Parse(xy[0]), float.Parse(xy[1]));
                }
				else if(table.Columns[i].DataType == typeof(Vector3))
                {
					string[] xy = ((string)elements[i]).Split(' ');
					elements[i] = new Vector3(float.Parse(xy[0]), float.Parse(xy[1]), float.Parse(xy[2]));
                }
				else if (table.Columns[i].DataType == typeof(Vector2[]))
                {
					//TODO
                }

			}
			row.ItemArray = elements;
			if (row.ItemArray[0].ToString() == "")
			{
				continue;
			}
			else
			{
				table.Rows.Add(row);
			}
		}
		return table;
	}

	//static public string[][] SplitCsvGrid(string csvText)
	//{
	//	string[] lines = csvText.Split("\n"[0]);

	//	for (int i = 0; i < lines.Length; i++)
	//	{
	//		string[] row = SplitCsvLine(lines[i]);
	//	}
	//	int width = lines.Max(x => x.Length);

	//	string[][] outputGrid = {new string[lines.Length + 1], new string[width + 1] };
	//	for (int y = 0; y < lines.Length; y++)
	//	{
	//		string[] row = SplitCsvLine(lines[y]);
	//		for (int x = 0; x < row.Length; x++)
	//		{
	//			outputGrid[x][y] = row[x];
	//			//outputGrid[x][y] = outputGrid[x][y].Replace("\"\"", "\"");
	//		}
	//	}

	//	return outputGrid;
	//}
	public static object[] SplitCsvLine(string line)
    {
		return line.Split(',');
    }
}
