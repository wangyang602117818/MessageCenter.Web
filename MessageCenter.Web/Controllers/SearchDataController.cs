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
        public ActionResult Suggest(string word)
        {
            string searchJson = suggest.Replace("{prefix}", word);
            var result = BsonDocument.Parse(elasticConnection.Post("sso-home-search/_doc/_search?filter_path=suggest.suggest-result.options.text,suggest.suggest-result.options._id", searchJson));
            if (result.Contains("suggest"))
            {
                var resp = result["suggest"]["suggest-result"].AsBsonArray.First()["options"].AsBsonArray.ToJson();
                return new ResponseModel<string>(ErrorCode.success, resp);
            }
            else
            {
                return new ResponseModel<string>(ErrorCode.success, "[]");
            }
        }
        public ActionResult Search(string word, int pageIndex = 1, int pageSize = 10)
        {
            int from = (pageIndex - 1) * pageSize;
            string searchJson = search.Replace("{keyword}", word).Replace("{from}", from.ToString()).Replace("{size}", pageSize.ToString());
            var resp = BsonDocument.Parse(elasticConnection.Post("sso-home-search/_doc/_search", searchJson));
            int count = Convert.ToInt32(resp["hits"]["total"]["value"].ToString());
            List<SearchDataModel> result = new List<SearchDataModel>();
            if (count > 0)
            {
                foreach (var item in resp["hits"]["hits"].AsBsonArray)
                {
                    BsonDocument highlight = item["highlight"].AsBsonDocument;
                    BsonDocument source = item["_source"].AsBsonDocument;
                    result.Add(new SearchDataModel()
                    {
                        id = item["_id"].AsString,
                        title = highlight.Contains("title") ? highlight["title"].AsBsonArray.First().ToString() : source["title"].ToString(),
                        description = highlight.Contains("description") ? highlight["description"].AsBsonArray.First().ToString() : source.Contains("description") ? source["description"].ToString() : "",
                        doc_time = DateTime.Parse(source["doc_time"].AsString),
                        create_time = DateTime.Parse(source["create_time"].AsString),
                    });
                }
                return new ResponseModel<string>(ErrorCode.success, JsonSerializerHelper.Serialize(result), count);
            }
            else
            {
                string searchLikeJson = searchLike.Replace("{keyword}", "*" + word + "*").Replace("{from}", from.ToString()).Replace("{size}", pageSize.ToString());
                var respLike = BsonDocument.Parse(elasticConnection.Post("sso-home-search/_doc/_search", searchLikeJson));
                count = Convert.ToInt32(respLike["hits"]["total"]["value"].ToString());
                Regex reg = new Regex("(" + word + ")", RegexOptions.IgnoreCase);
                foreach (var item in respLike["hits"]["hits"].AsBsonArray)
                {
                    BsonDocument source = item["_source"].AsBsonDocument;
                    result.Add(new SearchDataModel()
                    {
                        id = item["_id"].AsString,
                        title = reg.Replace(source["title"].ToString(), "<em>$0</em>"),
                        description = source.Contains("description") ? reg.Replace(source["description"]?.ToString(), "<em>$0</em>") : "",
                        doc_time = DateTime.Parse(source["doc_time"].AsString),
                        create_time = DateTime.Parse(source["create_time"].AsString),
                    });
                }
                return new ResponseModel<string>(ErrorCode.success, JsonSerializerHelper.Serialize(result), count);
            }
        }
    }
}