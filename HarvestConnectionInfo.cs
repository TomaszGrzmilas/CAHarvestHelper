namespace CAHarvestHelper
{
    public class HarvestConnectionInfo
    {
        public string Username { get; }
        public string Password { get; }
        public string BrokerName { get; }

        public string HarvestPath { get; }
        public string HarvestPecBinPath { get; }

        public HarvestConnectionInfo(string brocker, string user, string pass, string harvestPath, string harvestBinPath)
        {
            BrokerName = brocker;
            Username = user;
            Password = pass;

            HarvestPath = harvestPath;
            HarvestPecBinPath = harvestBinPath;
        }
    }
}
