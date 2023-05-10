using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NodeLookup.Methods
{
    public class NodeDetails
    {
        public string Path;
        public string Node;
        public string Description;

        public override string ToString()
        {
            return Description;
        }
    }

    public class Comspecs
    {
        public Dictionary<string, List<NodeDetails>> Comspec;
        public bool direxists;
        string ComspecLoc;

        public Comspecs()
        {
            if (GetLoc())
                GetComspecs();
        }

        private bool GetLoc()
        {
            if (Directory.Exists(@"C:\BHI\Advantage\mwdcomms\"))
            {
                ComspecLoc = @"C:\BHI\Advantage\mwdcomms\comspecs";
                return true;
            }
            else if (Directory.Exists(@"D:\Inteq\Advantage\mwdcomms\"))
            {
                ComspecLoc = @"D:\Inteq\Advantage\mwdcomms\comspecs";
                return true;
            }
            return false;
        }

        private void GetComspecs()
        {
            try
            {
                Comspec = new Dictionary<string, List<NodeDetails>>();
                foreach (var file in Directory.GetFiles(ComspecLoc))
                {
                    var name = file.Split('\\').Last().Split('.')[0].Split(' ');
                    var nodeDetails = new NodeDetails
                    {
                        Path = file,
                        Node = name[0],
                        Description = string.Join(" ", name.Skip(1))
                    };
                    if (!Comspec.ContainsKey(name[0]))
                    {
                        Comspec.Add(name[0], new List<NodeDetails>());
                        Comspec[name[0]].Add(nodeDetails);
                    }
                    else
                    {
                        Comspec[name[0]].Add(nodeDetails);
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }

    public class ComspecFileReader
    {
        public SortedDictionary<string, string> cmds;
        public Dictionary<string, string> bits;
        public bool dualStatus = false;
        public void ReadFile(string path)
        {
            dualStatus = false;
            cmds = new SortedDictionary<string, string>();
            bits = new Dictionary<string, string>();
            var lines = File.ReadAllLines(path, System.Text.Encoding.Default);
            int i = 0;
            foreach(string line in lines)
            {
                if (line.StartsWith("[") && line.Length > 3 && line.Substring(3, 1) == "]")
                {
                    cmds[line.Split(']')[0].TrimStart('[').TrimEnd(']').ToUpper()] = line.Split(']')[1].TrimStart(' ');
                }
                if (line.ToUpper().StartsWith("[BIT "))
                {
                    string details = lines[i].Split(']')[1].TrimStart(' ');
                    int k = 1;
                    while(true)
                    {
                        if (lines[i + k].StartsWith(";") || k > 30)
                            break;
                        details += (details == "" ? "" : "\n") + lines[i + k];
                        k++;
                    }

                    if (bits.ContainsKey(line.Split(']')[0].Split(' ')[1]))
                    {
                        bits[line.Split(']')[0].Split(' ')[1]] += "\n" + details;
                        if(dualStatus == false) dualStatus = true;
                        continue;
                    }
                    bits[line.Split(']')[0].Split(' ')[1]] = details;
                }
                i++;
            }
        }
    }
}
