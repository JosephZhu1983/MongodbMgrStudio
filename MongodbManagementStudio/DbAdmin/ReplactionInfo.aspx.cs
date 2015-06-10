using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongoDB.Driver;
using MongodbManagementStudio.Entities;
using MongodbManagementStudio.Services;

namespace MongodbManagementStudio.DbAdmin
{
    public partial class ReplactionInfo : System.Web.UI.Page
    {
        private readonly string connStringTemplate = "Server={0}:{1}";
        private MongoData mdb = new MongoData();
        public string infoType = "";
        public string infoId = "";

        bool isServer = false;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["type"] != null)
            {
                infoType = Request.QueryString["type"];
            }
            if (Request.QueryString["Id"] != null)
            {
                infoId = Request.QueryString["id"];
            }
            if (!IsPostBack)
            {
                SetInfo();
            }
        }
        private void SetInfo()
        {

            if (infoType.ToLowerInvariant() == "server")
            {
                ServerMongo objServer = mdb.GetInfoList(Guid.Parse(infoId)) as ServerMongo;
                if (objServer != null)
                {
                    liDbName.Text = "Server:" + objServer.Name;
                    using (Mongo mongo = new Mongo(string.Format(connStringTemplate, objServer.IP, objServer.Port)))
                    {
                        mongo.Connect();
                        Database admindb = mongo.GetDatabase("admin");
                        //Document stats = admindb.SendCommand(new Document().Append("serverStatus", 1));
                        //BindDocumentToTreeView(objServer.Name, stats, DataTreeList);

                        Document stats = admindb["$cmd"].FindOne(new Document().Append("ismaster", 1));
                        BindDocumentToTreeView(objServer.Name, stats, DataTreeList);

                        string masterStats = stats["ismaster"].ToString();
                        if (masterStats == "1")
                        {
                            //是master
                            Database myDb = mongo.GetDatabase("local");
                            var query = myDb.GetCollection("oplog.$main").FindAll();

                            //分页
                            query.Limit(10);
                            //if (tbPageSize.Text.Trim() != "")
                            //{
                            //    query.Limit(int.Parse(tbPageSize.Text));
                            //}
                           
                            var docs = query.Documents.ToList();
                            Document doc = new Document();
                            for (int i = 0; i < docs.Count; i++)
                            {
                                doc.Add((i + 1).ToString(), docs[i]);
                            }
                            BindDocumentToTreeView("数据列表", doc, DataTreeList2);

                        }
                        else
                        {
                            //是slave

                            Database myDb = mongo.GetDatabase("local");
                            //ICursor cursor = myDb["oplog.$main"].FindAll();
                            Document doc = myDb.GetCollection("sources").FindOne(new Document());
                            if (doc["syncedTo"] != null)
                            {
                                DateTime logTime = DateTime.Parse(doc["syncedTo"].ToString());
                                DateTime now = DateTime.Now;
                                double v = (now - logTime).TotalSeconds;

                                doc["syncedTo"] = doc["syncedTo"] + "  =" + v + "s ago";
                            }
                            
                            BindDocumentToTreeView("数据列表", doc, DataTreeList2);

                        }
                    }
                }
            }

        }

        private void BindDocumentToTreeView(string name, Document doc, TreeView tv)
        {

            tv.Nodes.Clear();
            tv.Nodes.Add(new TreeNode(name));
            BindDocumentToNode(doc, tv.Nodes[0]);
            tv.ExpandAll();

        }

        private void BindDocumentToNode(Document doc, TreeNode node)
        {
            foreach (string key in doc.Keys)
            {
                if (doc[key] is Document)
                {
                    TreeNode newNode = new TreeNode(key);
                    BindDocumentToNode(doc[key] as Document, newNode);
                    node.ChildNodes.Add(newNode);
                }
                else
                {
                    node.ChildNodes.Add(new TreeNode(string.Format("{0} : {1}", key, doc[key].ToString())));
                }
            }
        }
       
    }
}