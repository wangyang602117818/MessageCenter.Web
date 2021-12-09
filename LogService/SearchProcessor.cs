using Newtonsoft.Json;
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
            if (searchDataModel.type == "delete")
            {
                result = elasticConnection.Delete(indexName + "/_doc/" + searchDataModel.id);
                Log4Net.InfoLog("delete:" + result);
                if (result.Contains("\"found\":false")) return true;
            }
            else
            {
                result = elasticConnection.Post(indexName + "/_doc/" + searchDataModel.id, JsonSerializerHelper.Serialize(searchDataModel));
                Log4Net.InfoLog("update:" + result);
            }
            if (result.Contains("\"successful\":1")) return true;
            return false;
        }
    }

    public class SearchDataModel
    {
        [JsonIgnore]
        public string id { get; set; }
        [JsonIgnore]
        public string type { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public DateTime doc_time { get; set; }
        public DateTime create_time { get; set; }
    }
}
