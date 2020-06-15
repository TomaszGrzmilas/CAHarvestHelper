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
```