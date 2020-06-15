using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAHarvestHelper.Services
{
    class HarvestConnectionService
    {
        private readonly HarvestConnectionInfo CONNECTION_INFO;
        private readonly CMDHelper.Instance CMD;
        private readonly List<string> HARVEST_EXE = new List<string> { "hco.exe", "hsql.exe" };

        public HarvestConnectionService(HarvestConnectionInfo connectInfo)
        {
            CONNECTION_INFO = connectInfo;
            TestHarvestDir___();

            CMD = new CMDHelper.Instance(new[] { ("PATH", $"{CONNECTION_INFO.HarvestPath};{CONNECTION_INFO.HarvestPecBinPath}") });
            TestConnection___();
        }

        public List<Dictionary<string, string>> RunHSQL(string command)
        {
            bool headelLine = true;
            List<string> header = new List<string>();
            int idx;
            Dictionary<string, string> dic;
            List<Dictionary<string, string>> ret = new List<Dictionary<string, string>>();

            var queryResult = RunHSQL___(command);

            foreach (var line in queryResult)
            {
                if (headelLine)
                {
                    header.AddRange(line.Split("\t"));
                    headelLine = false;
                    continue;
                }

                idx = 0;
                dic = new Dictionary<string, string>();
                foreach (var columnValue in line.Split("\t"))
                {
                    dic.Add(header[idx], columnValue);
                    idx += 1;
                }
                ret.Add(dic);
            }

            return ret;
        }


        private List<string> RunHSQL___(string command)
        {
            string locCommand = $"echo {command} | hsql -b {CONNECTION_INFO.BrokerName} -usr {CONNECTION_INFO.Username} -pw {CONNECTION_INFO.Password} -s -t";

            // prepare string for echo comman
            // there is ^^^< because there is double parsing
            // echo must return ^< in order | to have only <
            locCommand = locCommand.Replace("<", "^^^<").Replace(">", "^^^>");
            return CMD.Run(locCommand);
        }

        private void TestConnection___()
        {
            RunHSQL___("SELECT 1 FROM DUAL");
        }

        private void TestHarvestDir___()
        {
            if (!Directory.Exists(CONNECTION_INFO.HarvestPath))
            {
                throw new DirectoryNotFoundException("Harvest instalation path is not valid");
            }

            var fileList = Directory.GetFileSystemEntries(CONNECTION_INFO.HarvestPath, "*.exe").Select((x) => Path.GetFileName(x)).ToArray();

            var missingFiles = HARVEST_EXE.Except(fileList);

            if (missingFiles.Count() > 0)
            {
                throw new Exception($"Missing file: {string.Join(", ", missingFiles)} in {CONNECTION_INFO.HarvestPecBinPath}");
            }
        }
    }
}
