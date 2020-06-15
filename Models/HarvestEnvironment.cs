namespace CAHarvestHelper.Models
{
    public class HarvestEnvironment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int BaseLineViewId { get; set; }

        public HarvestView ImplementationView { get; set; } = new HarvestView();
    }
}
