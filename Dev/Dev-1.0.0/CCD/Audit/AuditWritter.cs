// CCD Consolidation and De-duplication Engine
// Copyright (C) 2013  CreateIT Healthcare Solutions, Inc.

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see {http://www.gnu.org/licenses/}.

// Contact Info:
//     Jonathan Meade
//     CreateIT Healthcare Solutions, Inc.
//     516 SW 15th Street
//     Richmond, IN 47374
//     jmeade@createit-inc.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;

namespace Audit
{
    public static class AuditWritter
    {
        public static void WriteAuditRecord(AuditRecord auditRecord)
        {
            //try
            //{
            //    var xTest = new AuditRecordMongo(auditRecord);

            //    const string connString = "mongodb://ec2-54-221-19-62.compute-1.amazonaws.com";
            //    var client = new MongoClient(connString);
            //    var server = client.GetServer();
            //    var database = server.GetDatabase("CcdDeDupAudit");
            //    var collection = database.GetCollection<AuditRecordMongo>("auditRecords");

            //    collection.Insert(xTest);
            //}
            //catch (Exception)
            //{
            //    //Catches if I have db server off
            //}
            


            //var xList = from e in collection.AsQueryable<AuditRecordMongo>()
            //            select e;


            //new Thread(auditRecord.SaveDynamoDb).Start();
            //auditRecord.SaveDynamoDb();

            //new Thread(auditRecord.SaveRuleList).Start();
            //new Thread(auditRecord.SaveDiscard).Start();
            //new Thread(auditSave.Save).Start();
        }
    }
}
