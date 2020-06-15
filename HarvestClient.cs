using CAHarvestHelper.Models;
using CAHarvestHelper.Models.Enums;
using CAHarvestHelper.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CAHarvestHelper
{
    public class HarvestClient
    {
        private static readonly string DEFAULT_HARVEST_PATH = @"C:\Program Files\CA\SCM";
        private static readonly string DEFAULT_HARVEST_PEC_PATH = @"C:\Program Files (x86)\CA\SharedComponents\PEC\bin";
        
        private readonly HarvestConnectionService CONNECTION;
        private IEnumerable<HarvestEnvironment> environments;

        public HarvestClient(string brocker, string user, string pass, string harvestInstallationPath = null, string harvestPecBinPath = null)
            : this(new HarvestConnectionInfo(brocker, user, pass, harvestInstallationPath ?? DEFAULT_HARVEST_PATH, harvestPecBinPath ?? DEFAULT_HARVEST_PEC_PATH))
        {
        }

        public HarvestClient(HarvestConnectionInfo connectionInfo)
        {
            CONNECTION = new HarvestConnectionService(connectionInfo);
        }

        public IEnumerable<HarvestEnvironment> GetEnvironmentsList(bool clearCache = false)
        {
            if (environments == null || clearCache)
            {
                environments = LoadEnvironments___();
            }
            return environments;
        }

        public HarvestNode LoadNodeDetails(int envId, int? startItemId = null)
        {
            return LoadEnvironmentDetails___(environments.FirstOrDefault(x => x.Id == envId), startItemId).ImplementationView.Root;
        }

        public void LoadFileVersions(HarvestFileNode file)
        {
            LoadFileVersions___(file);
        }

        private HarvestEnvironment LoadEnvironmentDetails___(HarvestEnvironment env,  int? startItemId = null, int? toLevel = 10)
        {
            if (environments.Contains(env))
            {
                int level;
                int itemId;
                int itemType;
                int parentItemId;
                string itemName;
                HarvestNode newItem = null;

                string query = HSqlQueriesService.GetQuery(
                    "Env Items Tree",
                    new Dictionary<string, string>()
                    {
                        { ":viewobjid1", env.BaseLineViewId.ToString() },
                        { ":viewobjid2", env.ImplementationView.Id.ToString() },
                        { ":maxlevel", toLevel.ToString() },
                        { ":parentobjid", startItemId == null ? "0" : startItemId.ToString() }
                    }
                );
             
                foreach (var line in CONNECTION.RunHSQL(query))
                {                
                    int.TryParse(line.GetValueOrDefault("LVL"), out level);
                    int.TryParse(line.GetValueOrDefault("ITEMOBJID"), out itemId);
                    int.TryParse(line.GetValueOrDefault("PARENTOBJID"), out parentItemId);
                    int.TryParse(line.GetValueOrDefault("ITEMTYPE"), out itemType);

                    itemName = line.GetValueOrDefault("ITEMNAME");

                    if (itemType == 0)
                    {
                        newItem = new HarvestDirectoryNode(itemName)
                        {
                            Id = itemId,
                            EnvironmentId = env.Id,
                            ParentId = parentItemId,
                            Type = NodeType.DIRECTORY,
                            Level = level,
                            PathFullName = line.GetValueOrDefault("PATHFULLNAME")
                        };
                    }
                    else if(itemName.ToUpper() != "DEPLOY.INI")
                    {
                        newItem = new HarvestFileNode(itemName)
                        {
                            Id = itemId,
                            EnvironmentId = env.Id,
                            ParentId = parentItemId,
                            Type = NodeType.FILE,
                            Level = level,
                            PathFullName = line.GetValueOrDefault("PATHFULLNAME")
                        };
                    }
    
                    if (newItem != null)
                    {
                        if (level == 0)
                        {
                            env.ImplementationView.Root = newItem as HarvestDirectoryNode;
                        }
                        else
                        {
                            env.ImplementationView.Root.GetDirectoryNode(newItem.ParentId)?.Add(newItem);
                        }
                    }
                }
            }
            return env;
        }

        private void LoadFileVersions___(HarvestFileNode file)
        {
            int itemId;
            int parentVersionId;
            int parentItemId;
            int versionid;
            int versionNo;
            string itemName;
            HarvestNode newItem;

            string query = HSqlQueriesService.GetQuery(
                "Get Versions",
                new Dictionary<string, string>()
                {
                    { ":itemobjid", file.Id.ToString() }
                }
            );

            foreach (var line in CONNECTION.RunHSQL(query))
            {
                int.TryParse(line.GetValueOrDefault("ITEMOBJID"), out itemId);
                int.TryParse(line.GetValueOrDefault("VERSIONOBJID"), out versionid);
                int.TryParse(line.GetValueOrDefault("PARENTOBJID"), out parentItemId);
                int.TryParse(line.GetValueOrDefault("PARENTVERSIONID"), out parentVersionId);
                int.TryParse(line.GetValueOrDefault("MAPPEDVERSION"), out versionNo);

                itemName = line.GetValueOrDefault("ITEMNAME");

                newItem = new HarvestNode(itemName)
                {
                    Id = itemId,
                    EnvironmentId = file.EnvironmentId,
                    ParentId = parentItemId,
                    Type = NodeType.FILE,
                    Level = file.Level,
                    VersionNo = versionNo,
                    PathFullName = line.GetValueOrDefault("PATHFULLNAME")
                };

                file.AddVersion(newItem);
            }
        }

        private IEnumerable<HarvestEnvironment> LoadEnvironments___()
        {
            List<HarvestEnvironment> ret = new List<HarvestEnvironment>();
            int EnvId;
            int ViewId;
            int baseViewId;

            foreach (var line in CONNECTION.RunHSQL(HSqlQueriesService.GetQuery("Environment List")))
            {
                int.TryParse(line.GetValueOrDefault("ENVOBJID"), out EnvId);
                int.TryParse(line.GetValueOrDefault("BASELINEVIEWID"), out baseViewId);
            
                ret.Add(new HarvestEnvironment()
                {
                    Id = EnvId,
                    Name = line.GetValueOrDefault("ENVIRONMENTNAME"),
                    BaseLineViewId = baseViewId,
                    ImplementationView = new HarvestView()
                });
            }

            foreach (var line in CONNECTION.RunHSQL(HSqlQueriesService.GetQuery("Implementation Views List")))
            {
                int.TryParse(line.GetValueOrDefault("ENVOBJID"), out EnvId);
                int.TryParse(line.GetValueOrDefault("VIEWOBJID"), out ViewId);
                int.TryParse(line.GetValueOrDefault("BASELINEVIEWID"), out baseViewId);
            
                var tmpEnv = ret.Find(x => x.Id == EnvId);
            
                tmpEnv.ImplementationView =
                    new HarvestView()
                    {
                        Id = ViewId,
                        Name = line.GetValueOrDefault("VIEWNAME"),
                        Type = line.GetValueOrDefault("VIEWTYPE"),
                        BaseLineViewId = baseViewId
                    };
            
                tmpEnv.ImplementationView.Root = new HarvestDirectoryNode(tmpEnv.Name)
                {
                    EnvironmentId = tmpEnv.Id,
                    Type = NodeType.DUMY_ROOT
                };
            }

            return ret;
        }
    }
}
