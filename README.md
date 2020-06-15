# CAHarvestHelper
CA Harvest Helper - wrapper for CA Harvest cmd tools

#Usage
```
static void Main(string[] args)
{
    HarvestClient harvest = new HarvestClient("brocker", "user", "password", @"C:\Program Files\CA\SCM", @"C:\Program Files (x86)\CA\SharedComponents\PEC\bin");

    foreach (var proj in harvest.GetEnvironmentsList())
    {
        Console.WriteLine(proj.Name);
    }

    Console.ReadLine();
}
```