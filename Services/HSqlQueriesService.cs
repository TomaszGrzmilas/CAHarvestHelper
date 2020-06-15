using CAHarvestHelper.Models;
using System.Collections.Generic;

namespace CAHarvestHelper.Services
{
    class HSqlQueriesService
    {
        private static Dictionary<string, HSqlQuery> Queries;
        private static readonly string TREE_PENETRATION_LEVEL = "5";

        static HSqlQueriesService()
        {
            Queries = new Dictionary<string, HSqlQuery>();

            Queries.Add(
                "Environment List",
                new HSqlQuery(
                    @"SELECT env.envobjid, 
                             env.environmentname, 
                             env.baselineviewid
                        FROM harenvironment env
                       WHERE env.envisactive = 'Y'
                         AND env.isarchive = 'N'
                         AND env.envobjid != 0
                    ORDER BY env.envobjid"
               )
            );

            Queries.Add(
               "Implementation Views List",
               new HSqlQuery(
                   @"SELECT vi.envobjid,
                            vi.viewobjid,
                            vi.viewname, 
                            vi.viewtype,
                            vi.baselineviewid
                       FROM harview vi 
                      WHERE vi.viewname = 'Implementation'
                        AND vi.envobjid IN (SELECT env.envobjid
                                              FROM harenvironment env
                                             WHERE env.envisactive = 'Y'
                                               AND env.isarchive = 'N'
                                               AND env.envobjid != 0
                            )
                   ORDER BY vi.envobjid"
              )
           );

            Queries.Add(
               "Get Root",
               new HSqlQuery(
                   @"SELECT 0 lvl, 
                            t.itemobjid,
                            t.itemname,
                            t.itemnameupper,
                            t.parentobjid,
                            t.itemtype,
                            p.pathfullname
                       FROM harversioninview ve, 
                            harversions v
                  LEFT JOIN harpathfullname p on v.itemobjid = p.itemobjid and p.versionobjid = v.versionobjid,
                            haritems t
                      WHERE ve.viewobjid = :viewobjid
                        AND ve.versionobjid  = v.versionobjid 
                        AND v.itemobjid = t.itemobjid;"
              )
           );

            Queries.Add(
               "Env Items Tree",
               new HSqlQuery(
                   @"SELECT LEVEL - 1 lvl,
                            t.itemobjid,
                            t.itemname,
                            t.itemnameupper,
                            t.parentobjid,
                            t.itemtype,
                            t.pathfullname
                       FROM (SELECT distinct item.itemobjid,
                                    item.itemname,
                                    item.itemnameupper,
                                    item.parentobjid,
                                    item.itemtype,
                                    p.pathfullname
                               FROM harversions v
                          LEFT JOIN harpathfullname p on v.itemobjid = p.itemobjid and p.versionobjid = v.versionobjid,
                                    haritems item
                              WHERE v.itemobjid = item.itemobjid   
                                AND v.versionobjid IN (SELECT ve.versionobjid
                                                         FROM harversioninview ve
                                                        WHERE ve.viewobjid in (:viewobjid1, :viewobjid2)
                                                      )
                            ) t
                      WHERE LEVEL <= :maxlevel
                            START WITH parentobjid = :parentobjid
                            CONNECT BY PRIOR itemobjid = parentobjid
                            ORDER SIBLINGS BY itemobjid asc, parentobjid asc, itemtype asc, itemname",
                    new Dictionary<string, string>()
                    {
                        { ":maxlevel", TREE_PENETRATION_LEVEL },
                        { ":parentobjid", "0" }
                    }
              )
           );

            Queries.Add(
               "Get Versions",
               new HSqlQuery(
                   @"SELECT item.itemobjid,
                            item.itemname,
                            item.itemnameupper,
                            item.parentobjid,
                            item.itemtype,
                            v.versionobjid,
                            v.parentversionid,
                            v.mappedversion,
                            v.creationtime,
                            v.modifiedtime
                       FROM harversions v,
                            haritems item
                      WHERE v.itemobjid = item.itemobjid   
                        AND v.itemobjid = :itemobjid
                   ORDER BY v.mappedversion desc"
              )
           );
        }

        public static string GetQuery(string queryName, Dictionary<string, string> parameters = null)
        {
            return Queries.GetValueOrDefault(queryName)
                .SetParameters(parameters)
                .GetQuery();
        }

    }
}
