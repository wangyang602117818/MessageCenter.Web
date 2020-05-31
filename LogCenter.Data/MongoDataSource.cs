using MongoDB.Bson;
using MongoDB.Driver;
using SSO.Util.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LogCenter.Data
{
    public class MongoDataSource
    {
        public static MongoClient MongoClient;   //mongo数据库操作
        static MongoDataSource()
        {
            MongoClient = new MongoClient(AppSettings.GetValue("mongodb"));
        }

    }
}
