using SharedAssemblies.DAL;
using SharedAssemblies.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace SharedAssemblies.Content.AppCode
{
    public class CustomerHelper
    {
        private static MyDbContext _db;

        public static MyDbContext custDB
        {
            get { return _db; }
        }

        static CustomerHelper()
        {
            _db = new MyDbContext();
        }

        public static int GetCustomerIDFromClientID(int clientId)
        {
            int customerId = 0;
            var lookClient = _db.Client.Where(c => c.ID == clientId).FirstOrDefault();
            if (lookClient != null)
            {
                customerId = lookClient.CustomerID;
            }

            return customerId;
        }

        public static int GetCustomerIDFromCustomerCode(int customerCode)
        {
            int customerId = 0;
            var lookCustomer = _db.Customer.Where(c => c.CustomerCode == customerCode).FirstOrDefault();
            if (lookCustomer != null)
            {
                customerId = lookCustomer.ID;
            }

            return customerId;
        }
    }
}