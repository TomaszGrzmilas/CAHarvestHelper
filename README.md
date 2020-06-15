# CAHarvestHelper
CA Harvest Helper - wrapper for CA Harvest SCM Commands

https://techdocs.broadcom.com/content/broadcom/techdocs/us/en/ca-enterprise-software/business-management/harvest-scm/13-0/command-reference/get-started-with-ca-harvest-scm-commands.html

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

static void Main(string[] args)
{
    HarvestClient harvest = new HarvestClient("brocker", "user", "password", @"C:\Program Files\CA\SCM", @"C:\Program Files (x86)\CA\SharedComponents\PEC\bin");

    foreach (var item in harvest.RunHSQL("select 1 as col_name1, 2 col_name2 from dual"))
    {
        string dummy;
        item.TryGetValue("col_name1".ToUpper(), out dummy);
        Console.WriteLine($"col_name1 - { dummy}");

        item.TryGetValue("col_name2".ToUpper(), out dummy);
        Console.WriteLine($"col_name2 - { dummy}");
    }

    Console.ReadLine();
}

static void Main(string[] args)
{
    HarvestClient harvest = new HarvestClient("brocker", "user", "password", @"C:\Program Files\CA\SCM", @"C:\Program Files (x86)\CA\SharedComponents\PEC\bin");

    var node = harvest.LoadNodeDetails(harvest.GetEnvironmentsList().FirstOrDefault().Id);

    foreach (var item in (node as HarvestDirectoryNode).GetSubDirectories())
    {
        Console.WriteLine(item.Name);
    }

    Console.ReadLine();
}
```