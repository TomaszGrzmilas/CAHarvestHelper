using CAHarvestHelper.Models.Enums;
using System;
using System.IO;

namespace CAHarvestHelper.Models
{
    public class HarvestNode
    {
        public int Id { get; set; }

        public int EnvironmentId { get; set; }

        public string Name { get; private set; }

        public string FullName { get => Name; }

        public int Hash { get => (Name + Level.ToString()).GetHashCode(); }

        public NodeType Type { get; set; }

        public int Level { get; set; }

        public string Extension { get; private set; }

        public int ParentId { get; set; }

        public string PathFullName { get; set; }

        public int VersionNo { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        public HarvestNode(string name)
        {
            Name = name;
            Extension = Path.GetExtension(name);
        }
    }
}
