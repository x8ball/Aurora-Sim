using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Aurora.Framework;
using Aurora.DataManager;
using OpenMetaverse;
using OpenSim.Framework;
using Nini.Config;
using OpenSim.Services.Interfaces;

namespace Aurora.Services.DataService
{
    public class LocalUserAccountConnector : IUserAccountData
	{
		private IGenericData GD = null;
        private string m_realm = "useraccounts";

        public void Initialize(IGenericData GenericData, IConfigSource source, IRegistryCore simBase, string defaultConnectionString)
        {
            if(source.Configs["AuroraConnectors"].GetString("AbuseReportsConnector", "LocalConnector") == "LocalConnector")
            {
                GD = GenericData;

                string connectionString = defaultConnectionString;
                if (source.Configs[Name] != null)
                    connectionString = source.Configs[Name].GetString("ConnectionString", defaultConnectionString);

                GD.ConnectToDatabase(connectionString, "UserAccounts", source.Configs["AuroraConnectors"].GetBoolean("ValidateTables", true));

                DataManager.DataManager.RegisterPlugin(Name, this);
            }
        }

        public string Name
        {
            get { return "IUserAccountData"; }
        }

        public void Dispose()
        {
        }

        public UserAccount[] Get(string[] fields, string[] values)
        {
            List<string> query = GD.Query(fields, values, m_realm, "*");
            List<UserAccount> list = new List<UserAccount>();

            ParseQuery(query, ref list);

            return list.ToArray();
        }

        private void ParseQuery(List<string> query, ref List<UserAccount> list)
        {
            for (int i = 0; i < query.Count; i += 11)
            {
                UserAccount data = new UserAccount();

                data.PrincipalID = UUID.Parse(query[i + 0]);
                data.ScopeID = UUID.Parse(query[i + 1]);
                //We keep these even though we don't always use them because we might need to create the "Name" from them
                string FirstName = query[i + 2];
                string LastName = query[i + 3];
                data.Email = query[i + 4];

                data.ServiceURLs = new Dictionary<string, object>();
                if (query[i + 5] != null)
                {
                    string[] URLs = query[i + 5].Split(new char[] { ' ' });

                    foreach (string url in URLs)
                    {
                        string[] parts = url.Split(new char[] { '=' });

                        if (parts.Length != 2)
                            continue;

                        string name = System.Web.HttpUtility.UrlDecode(parts[0]);
                        string val = System.Web.HttpUtility.UrlDecode(parts[1]);

                        data.ServiceURLs[name] = val;
                    }
                }
                data.Created = Int32.Parse(query[i + 6]);
                data.UserLevel = Int32.Parse(query[i + 7]);
                data.UserFlags = Int32.Parse(query[i + 8]);
                data.UserTitle = query[i + 9];
                data.Name = query[i + 10];
                if (data.Name == null || data.Name == "")
                {
                    data.Name = FirstName + " " + LastName;
                    //Save the change!
                    Store(data);
                }
                list.Add(data);
            }
        }

        public bool Store(UserAccount data)
        {
            List<string> parts = new List<string>();

            foreach (KeyValuePair<string, object> kvp in data.ServiceURLs)
            {
                string key = System.Web.HttpUtility.UrlEncode(kvp.Key);
                string val = System.Web.HttpUtility.UrlEncode(kvp.Value.ToString());
                parts.Add(key + "=" + val);
            }
            if (data.UserTitle == null)
                data.UserTitle = "";

            string serviceUrls = string.Join(" ", parts.ToArray());

            return GD.Replace(m_realm, new string[] { "PrincipalID", "ScopeID", "FirstName",
                "LastName", "Email", "ServiceURLs", "Created", "UserLevel", "UserFlags", "UserTitle","Name"}, new object[]{
                data.PrincipalID, data.ScopeID, data.FirstName, data.LastName, data.Email,
                serviceUrls, data.Created, data.UserLevel, data.UserFlags, data.UserTitle, data.Name});
        }

        public bool Delete(string field, string val)
        {
            return true;
        }

        public UserAccount[] GetUsers(UUID scopeID, string query)
        {
            List<UserAccount> data = new List<UserAccount>();

            string[] words = query.Split(new char[] { ' ' });

            for (int i = 0; i < words.Length; i++)
            {
                if (words[i].Length < 3)
                {
                    if (i != words.Length - 1)
                        Array.Copy(words, i + 1, words, i, words.Length - i - 1);
                    Array.Resize(ref words, words.Length - 1);
                }
            }

            if (words.Length == 0)
                return new UserAccount[0];

            if (words.Length > 2)
                return new UserAccount[0];

            List<string> retVal = GD.Query("(ScopeID='" + scopeID + "' or ScopeID='00000000-0000-0000-0000-000000000000') " +
                "and (Name like '%" + query + "%' or " + 
                "FirstName like '%" + words[0] + "%' " + 
                ((words.Length == 1) ? 
                    " or LastName like '%" + words[0]
                    : " and LastName like '%" + words[1])
                     + "%')", m_realm, " PrincipalID, ScopeID, FirstName, LastName, Email, ServiceURLs, Created, UserLevel, UserFlags, UserTitle, " + GD.IsNull("Name", GD.ConCat(new string[] { "FirstName", "' '", "LastName" })) + " as Name ");

            ParseQuery(retVal, ref data);
            
            return data.ToArray();
        }
    }
}
