using SharedAssemblies.DAL;
using SharedAssemblies.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Content.AppCode
{
    public class ClientHelper
    {
        private static MyDbContext _db;

        public static MyDbContext clientHelperDB
        {
            get { return _db; }
        }

        static ClientHelper()
        {
            _db = new MyDbContext();
        }
        // The session key value is set here: SharedAssemblies.ClientHelper.GetClientSessionObjects
        public static string GetSessionInsertOutput()
        {
            return GetSessionInsertOutput(null);
        }

        public static string GetSessionInsertOutput(string sessionKey)
        {
            var result = "";
            sessionKey = String.IsNullOrEmpty(sessionKey) ? HttpContext.Current.Session["sessionKey"].ToString() : sessionKey;

            var lookSessionInsertOutput = clientHelperDB.GetSessionInsertOutput(sessionKey);
            if (lookSessionInsertOutput != null)
            {
                var sessionInsertOutput = lookSessionInsertOutput.ToList();
                if (sessionInsertOutput.Count > 0)
                {
                    result = sessionInsertOutput.FirstOrDefault().Output;
                }
            }

            return result;
        }


    }
}