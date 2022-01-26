using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SSO.Util.Client;
using SSO.Util.Client.ElasticLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogService
{
    public class SearchProcessor
    {
        public static string esUrl = null;
        public static string mapping = null;
        ElasticConnection elasticConnection = new ElasticConnection(esUrl);
        public List<Task> tasks = new List<Task>();
        static string indexName = "sso-home-search";
        static SearchProcessor()
        {
            esUrl = AppSettings.GetValue("esUrl");
            mapping = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "template\\sso-search-mapping.json");
            ElasticConnection elasticConnection = new ElasticConnection(esUrl);
            try
            {
                if (!elasticConnection.Head(indexName))
                {
                    var res = elasticConnection.Put(indexName, mapping);
                    Log4Net.InfoLog("新建index:" + res);
                }
            }
            catch (Exception ex)
            {
                Log4Net.ErrorLog(ex);
            }
        }
        public void StartWork()
        {
            Task task = Task.Factory.StartNew(() =>
            {
                MsQueue<SearchDataModel> msQueue = new MsQueue<SearchDataModel>(AppSettings.GetValue("search_data_msqueue"));
                msQueue.ReceiveMessageTransactional(Worker);
            });
            tasks.Add(task);
        }
        public bool Worker(SearchDataModel searchDataModel)
        {
            var result = "";
            if (searchDataModel.operationType == OperationType.delete)
            {
                result = elasticConnection.Delete(indexName + "/_doc/" + searchDataModel.Id);
                Log4Net.InfoLog("delete:" + result);
                if (result.Contains("\"found\":false")) return true;
            }
            else
            {
                result = elasticConnection.Post(indexName + "/_doc/" + searchDataModel.Id, JsonSerializerHelper.Serialize(searchDataModel));
                Log4Net.InfoLog("update:" + result);
            }
            if (result.Contains("\"successful\":1")) return true;
            return false;
        }
    }

    public class SearchDataModel
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public DataBaseType database { get; set; }
        public string table { get; set; }
        public string key { get; set; }
        [JsonIgnore]
        public OperationType operationType { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime doc_time { get; set; }
        public DateTime create_time { get; set; }

        [JsonIgnore]
        public string Id { get { return (database + table + key).ToLower().ToMD5(); } }
    }
}
