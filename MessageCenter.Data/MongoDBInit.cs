﻿using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MessageCenter.Data
{
    public static class MongoDBInit
    {
        public static void Init(IMongoDatabase database)
        {
            var databases = database.ListCollectionNames().ToList();
            

        }
    }
}
