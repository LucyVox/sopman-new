using System;
using System.Data;
using System.IO;
 
namespace sopman.Models
{
    public class CSVReaderHelper
    {
        public static DataTable GetCSVData(string localDestination)
        {
            //Instantiating Data Table
            var dt = new DataTable();

            try
            {
                if (File.Exists(localDestination))
                {
                    using (StreamReader streamReader = new StreamReader(localDestination))
                    {
                        string[] headers = streamReader.ReadLine().Split(',');

                        foreach (string header in headers)
                        {
                            dt.Columns.Add(header);
                        }

                        while (!streamReader.EndOfStream)
                        {
                            string[] rows = streamReader.ReadLine().Split(',');

                            if (rows.Length > 1)
                            {
                                DataRow dr = dt.NewRow();

                                for (int i = 0; i < headers.Length; i++)
                                {
                                    dr[i] = rows[i].Trim();
                                }

                                dt.Rows.Add(dr);
                            }

                        }
                    }
                }

                return dt;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}