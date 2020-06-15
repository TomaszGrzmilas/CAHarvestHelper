namespace CAHarvestHelper.Models
{
    public class HarvestView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int BaseLineViewId { get; set; }

        public HarvestDirectoryNode Root { get; set; }
    }
}
