using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dasdparser
{
    internal class dasditem
    {
        public string Filename { get; set; }
        public string Volser { get; set; }
        public string Dsname { get; set; }
        public string Created { get; set; }
        public string Org { get; set; }
        public string RecFm { get; set; }
        public string LRecl { get; set; }
        public string BlkSz { get; set; }
        public string Key { get; set; }
        public string Trks { get; set; }
        public string Use { get; set; }
        public string Ext { get; set; }
        public string Secdry { get; set; }
        public string Alloc { get; set; }

        public List<string> Members { get; set; } = new();

        public List<string> ToExcel()
        {
            List<string> list = new List<string>();

            if (this.Members.Count == 0)
            {
                list.Add(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8}", this.Volser, this.Filename, this.Org, this.RecFm, this.LRecl, this.BlkSz, this.Key, this.Trks, this.Dsname));
            }
            else
            {
                foreach (string member in this.Members)
                {
                    list.Add(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", this.Volser, this.Filename, this.Org, this.RecFm, this.LRecl, this.BlkSz, this.Key, this.Trks, this.Dsname, member));
                }
            }

            return list;
        }
    }
}
