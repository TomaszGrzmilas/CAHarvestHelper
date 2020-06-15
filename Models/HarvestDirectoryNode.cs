using System.Collections.Generic;
using System.Linq;

namespace CAHarvestHelper.Models
{
    public class HarvestDirectoryNode : HarvestNode
    {
        private List<HarvestDirectoryNode> _directories;
        private List<HarvestFileNode> _files;

        public HarvestDirectoryNode(string name) : base(name)
        {
            _directories = new List<HarvestDirectoryNode>();
            _files = new List<HarvestFileNode>();
        }

        public IEnumerable<HarvestDirectoryNode> Descendants()
        {
            return GetSubDirectories()
                .Concat(GetSubDirectories().SelectMany(n => n.Descendants())).ToList();
        }

        public HarvestDirectoryNode GetDirectoryNode(int NodeId)
        {
            HarvestDirectoryNode ret;

            if (NodeId == Id)
            {
                return this;
            }

            ret = _directories.Find(x => x.Id == NodeId);

            if (ret != null)
            {
                return ret;
            }

            foreach (var child in _directories)
            {
                ret = child.GetDirectoryNode(NodeId);
                if (ret != null)
                {
                    return ret;
                }
            }

            return null;
        }

        public HarvestNode GetChildNode(HarvestNode search)
        {
            HarvestNode ret;

            if (search.Hash == Hash)
            {
                return this;
            }

            ret = _directories.Find(x =>
                x.Hash == search.Hash
            );

            return ret;
        }

        public void Add(HarvestNode node)
        {
            if (node.Type == Enums.NodeType.DIRECTORY)
            {
                AddSubDdirectory___(node as HarvestDirectoryNode);
                return;
            }
            AddFile___(node as HarvestFileNode);
        }

        public HarvestDirectoryNode Clone()
        {
            return this.MemberwiseClone() as HarvestDirectoryNode;
        }

        public HarvestDirectoryNode[] GetSubDirectories()
        {
            return _directories.ToArray();
        }

        public HarvestFileNode[] GetFiles()
        {
            SortFiles___();
            return _files.ToArray();
        }

        public void ReplaceFile(HarvestFileNode oldFile, HarvestFileNode newFile)
        {
            if (_files.Exists(f => f.Hash == oldFile.Hash))
            {
                _files[_files.FindIndex(f => f.Hash == oldFile.Hash)] = newFile;
            }
        }

        public void DeleteFiles()
        {
            _files = new List<HarvestFileNode>();
        }

        public void DeleteFile(HarvestFileNode File)
        {
            _files.Remove(File);
        }

        private void AddSubDdirectory___(HarvestDirectoryNode dir)
        {
            _directories.Add(dir);
        }

        private void AddFile___(HarvestFileNode file)
        {
            _files.Add(file);
        }

        private void SortFiles___()
        {
            _files = _files.OrderBy(x => x.Name).ToList();
        }

    }
}
