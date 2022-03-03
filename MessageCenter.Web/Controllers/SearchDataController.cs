using MessageCenter.Web.Models;
using SSO.Util.Client;
using SSO.Util.Client.ElasticLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MongoDB.Bson;
using System.Text.RegularExpressions;

namespace MessageCenter.Web.Controllers
{
    public class SearchDataController : Controller
    {
        protected MsQueue<SearchDataModel> searchDataMsqueue = new MsQueue<SearchDataModel>(AppSettings.GetValue("search_data_msqueue"));
        ElasticConnection elasticConnection = new ElasticConnection(AppSettings.GetValue("esUrl"));
        public static string suggest = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"template\suggest.json");
        public static string search = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"template\search.json");
        public static string searchLike = System.IO.File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + @"template\search-like.json");

        public ActionResult Insert(SearchDataModel searchDataModel)
        {
            searchDataMsqueue.SendMessageTransactional(searchDataModel, "search", true);
            return new ResponseModel<string>(ErrorCode.success, searchDataModel.id);
        }
        public ActionResult Suggest(string word, string database = "", string table = "")
        {
            string suggestJson = suggest.Replace("{prefix}", word);
            var result = BsonDocument.Parse(elasticConnection.Post("sso-home-search/_search?filter_path=suggest.suggest-result.options.text,suggest.suggest-result.options._id,suggest.suggest-result.options._source", suggestJson));
            if (result.Contains("suggest"))
            {
                var datas = result["suggest"]["suggest-result"].AsBsonArray.First()["options"].AsBsonArray;
                var predicate = new Func<BsonValue, bool>((doc) =>
                {
                    BsonDocument source = doc["_source"].AsBsonDocument;
                    if (!database.IsNullOrEmpty() && source["database"].AsString.ToLower() != database.ToLower()) return false;
                    if (!table.IsNullOrEmpty() && source["table"].AsString.ToLower() != table.ToLower()) return false;
                    return true;
                });
                return new ResponseModel<string>(ErrorCode.success, datas.Where(predicate).ToJson());
            }
            else
            {
                return new ResponseModel<string>(ErrorCode.success, "[]");
            }
        }
        public ActionResult Search(string word, bool highlight = false, string database = "", string table = "", int pageIndex = 1, int pageSize = 10)
        {
            int from = (pageIndex - 1) * pageSize;
            string searchJson = search;
            BsonArray filters = new BsonArray();
            if (!database.IsNullOrEmpty()) filters.Add(new BsonDocument("term", new BsonDocument("database", database.ToLower())));
            if (!table.IsNullOrEmpty()) filters.Add(new BsonDocument("term", new BsonDocument("table", table.ToLower())));
            searchJson = searchJson.Replace("100", filters.ToJson()).Replace("{keyword}", word).Replace("{from}", from.ToString()).Replace("{size}", pageSize.ToString());
            var resp = BsonDocument.Parse(elasticConnection.Post("sso-home-search/_search", searchJson));
            int count = Convert.ToInt32(resp["hits"]["total"]["value"].ToString());
            List<SearchDataModel> result = new List<SearchDataModel>();
            if (count > 0)
            {
                foreach (var item in resp["hits"]["hits"].AsBsonArray)
                {
                    BsonDocument hl = item["highlight"].AsBsonDocument;
                    BsonDocument source = item["_source"].AsBsonDocument;
                    DataBaseType dataBase = (DataBaseType)Enum.Parse(typeof(DataBaseType), source["database"].AsString);
                    result.Add(new SearchDataModel()
                    {
                        database = dataBase,
                        table = source["table"].AsString,
                        key = source["key"].AsString,
                        title = (hl.Contains("title") && highlight) ? hl["title"].AsBsonArray.First().ToString() : source["title"].ToString(),
                        description = (hl.Contains("description") && highlight) ? hl["description"].AsBsonArray.First().ToString() : source.Contains("description") ? source["description"].ToString() : "",
                        doc_time = DateTime.Parse(source["doc_time"].AsString),
                        create_time = DateTime.Parse(source["create_time"].AsString),
                    });
                }
                return new ResponseModel<string>(ErrorCode.success, JsonSerializerHelper.Serialize(result), count);
            }
            else
            {
                string searchLikeJson = searchLike.Replace("100", filters.ToJson()).Replace("{keyword}", word).Replace("{from}", from.ToString()).Replace("{size}", pageSize.ToString());
                var respLike = BsonDocument.Parse(elasticConnection.Post("sso-home-search/_search", searchLikeJson));
                count = Convert.ToInt32(respLike["hits"]["total"]["value"].ToString());
                Regex reg = new Regex("(" + word + ")", RegexOptions.IgnoreCase);
                foreach (var item in respLike["hits"]["hits"].AsBsonArray)
                {
                    BsonDocument source = item["_source"].AsBsonDocument;
                    DataBaseType dataBase = (DataBaseType)Enum.Parse(typeof(DataBaseType), source["database"].AsString);
                    result.Add(new SearchDataModel()
                    {
                        database = dataBase,
                        table = source["table"].AsString,
                        key = source["key"].AsString,
                        title = highlight ? reg.Replace(source["title"].ToString(), "<em>$0</em>") : source["title"].ToString(),
                        description = source.Contains("description") ? (highlight ? reg.Replace(source["description"]?.ToString(), "<em>$0</em>") : source["description"]?.ToString()) : "",
                        doc_time = DateTime.Parse(source["doc_time"].AsString),
                        create_time = DateTime.Parse(source["create_time"].AsString),
                    });
                }
                return new ResponseModel<string>(ErrorCode.success, JsonSerializerHelper.Serialize(result), count);
            }
        }
    }
}