using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace File_Types_Utility
{
    public class FileType
    {
        public string Extention { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

        public FileType(string ext, string descr, string type)
        {
            Extention = ext;
            Name = ext.Substring(1); //everything after the period
            Description = descr;
            Type = type;
        }
    }
}
