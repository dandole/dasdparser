using System.Diagnostics;

namespace dasdparser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<dasditem> list = new List<dasditem>();

            foreach (var file in new DirectoryInfo(@"C:\Users\dando\Downloads\tk4\dasd").GetFiles())
            {
                Console.Write(DasdLS(file.FullName));
                list.AddRange(GetDsnFiles(file));
            }

            using (var sw = new StreamWriter("output.csv", false))
            {
                sw.WriteLine("VOLSER,FILE,ORG,RECFM,LRECL,BLKSZ,KEY,TRKS,DSNAME,MEMBER");
                foreach (var item in list)
                {
                    foreach (var str in item.ToExcel())
                    {
                        sw.WriteLine(str);
                    }
                }

                sw.Close();
            }

        }

        static List<dasditem> GetDsnFiles(FileInfo dasdFile)
        {
            List<dasditem> files = new List<dasditem>();

            string output = DasdLS(dasdFile.FullName);

            string filename = dasdFile.Name;
            string volser = string.Empty;

            bool foundHeader = false;
            foreach (var line in output.Split('\n'))
            {
                if (foundHeader && line.Trim() != String.Empty)
                {
                    string dsn = line.Substring(0, 45).Trim();
                    string created = line.Substring(45, 9).Trim();
                    string org = line.Substring(55, 3).Trim();
                    string recfm = line.Substring(59, 5).Trim();
                    string lrecl = line.Substring(65, 5).Trim();
                    string blksz = line.Substring(71, 5).Trim();
                    string key = line.Substring(77, 3).Trim();
                    string trks = line.Substring(81, 5).Trim();
                    string used = line.Substring(87, 3).Trim();
                    string ext = line.Substring(92, 3).Trim();
                    string sndry = line.Substring(95, 6).Trim();
                    string alloc = line.Substring(101, 5).Trim();

                    dasditem di = new()
                    {
                        Filename = filename,
                        Volser = volser,
                        Dsname = dsn,
                        Created = created,
                        Org = org,
                        RecFm = recfm,
                        LRecl = lrecl,
                        BlkSz = blksz,
                        Key = key,
                        Trks = trks,
                        Use = used,
                        Ext = ext,
                        Secdry = sndry,
                        Alloc = alloc
                    };

                    if (di.Org == "PO" && di.Dsname.EndsWith("CDS")==false)
                        PSMembers(dasdFile.FullName, di);

                    files.Add(di);
                }
                else
                {
                    if (line.StartsWith("HHC0"))
                        continue;

                    if (line.StartsWith("Dsname "))
                        foundHeader = true;

                    if (line.Contains("VOLSER="))
                    {
                        volser = line.Split("VOLSER=")[1].Trim();
                    }
                }
            }

            return files;
        }

        static void PSMembers(string filename, dasditem di)
        {
            string output = DasdCat(filename, di.Dsname);

            foreach (var line in output.Split('\n'))
            {
                if (line.StartsWith("HHC0") || line.Trim() == string.Empty)
                    continue;

                di.Members.Add(line.Trim().ToUpper());
            }
        }

        static string DasdLS(string dasdFile)
        {
            Process p = new Process();
            p.StartInfo.FileName = "dasdls";
            p.StartInfo.Arguments = string.Format("-hdr -info -caldt {0}", dasdFile);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            var output = p.StandardOutput.ReadToEnd();

            return output;
        }

        static string DasdCat(string dasdFile, string dsname)
        {
            Process p = new Process();
            p.StartInfo.FileName = "dasdcat";
            p.StartInfo.Arguments = string.Format("-i {0} {1}/?", dasdFile, dsname);
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            var output = p.StandardOutput.ReadToEnd();

            return output;
        }
    }
}