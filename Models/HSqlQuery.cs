using System;
using System.Collections.Generic;
using System.Linq;

namespace CAHarvestHelper.Models
{
    class HSqlQuery
    {
        private readonly string STATEMENT;
        private Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private string _query;

        public string Query
        {
            get => GetQuery();
        }

        public Dictionary<string, string> Parameters
        {
            get => _parameters;
            set => SetParameters___(value);
        }

        public HSqlQuery(string statement, Dictionary<string, string> parameters = null)
        {
            STATEMENT = NormalizeString___(statement);
            SetParameters___(parameters);
        }

        public HSqlQuery SetParameters(Dictionary<string, string> parameters)
        {
            SetParameters___(parameters);
            return this;
        }

        public string GetQuery()
        {
            if (_query == null)
            {
                return STATEMENT;
            }
            return _query;
        }

        private void SetParameters___(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (var item in parameters)
            {
                if (_parameters.ContainsKey(item.Key))
                {
                    _parameters[item.Key] = item.Value;
                }
                else
                {
                    _parameters.Add(item.Key, item.Value);
                }
            }

            _query = STATEMENT;

            foreach (var item in _parameters.Keys)
            {
                _query = _query.Replace(item, _parameters[item]);
            }
        }

        private string NormalizeString___(string query)
        {
            // remove extra characters
            string[] tmpQuery = query.Replace("\r\n", "").Replace("\t", " ").Split(" ");
            // remove extra spaces
            tmpQuery = tmpQuery.Where(x => x != "").Select(x => x + " ").ToArray();

            return String.Concat(tmpQuery);
        }
    }
}
