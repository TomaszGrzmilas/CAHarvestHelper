using System.Collections.Generic;
using System.Linq;

namespace CAHarvestHelper.Models
{
    public class HarvestFileNode : HarvestNode
    {
        public List<HarvestNode> Versions { get; set; }

        public HarvestFileNode(string fileName) : base(fileName)
        {
            Versions = new List<HarvestNode>();
        }

        public void AddVersion(HarvestNode fileVersion)
        {
            Versions.Add(fileVersion);
            Versions = Versions.OrderByDescending(x => x.VersionNo).ToList();
        }
    }
}
