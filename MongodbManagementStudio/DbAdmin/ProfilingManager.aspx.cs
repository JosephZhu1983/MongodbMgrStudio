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
    public partial class ProfilingManager : System.Web.UI.Page
    {
        private MongoData mdb = new MongoData();
        private readonly string connStringTemplate = "Server={0}:{1}";

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

            if (infoType.ToLowerInvariant() == "database")
            {

                DBMongo db =mdb.GetInfoList(Guid.Parse(infoId)) as DBMongo;
                if (db != null)
                {

                    liDbName.Text = "Server:" + db.Server.Name + " DataBase:" + db.Name;
                    using (Mongo mongo = new Mongo(string.Format(connStringTemplate, db.Server.IP, db.Server.Port)))
                    {
                        mongo.Connect();
                        Database database = mongo.GetDatabase(db.Name);

                        Document newLevel = database["$cmd"].FindOne(new Document().Append("profile", -1));
                        if (newLevel != null)
                        {
                            int ok = int.Parse(newLevel["ok"].ToString());
                            if (ok == 1)
                            {
                                LiProfileStats.Text = "";
                                int profileLevel = int.Parse(newLevel["was"].ToString());
                                if (profileLevel == 2)
                                {
                                    LiProfileStats.Text = "已打开profile,记录所有操作";
                                }
                                else if (profileLevel == 1)
                                {
                                    LiProfileStats.Text = "已打开profile,记录部分操作";
                                }
                                else if (profileLevel == 0)
                                {
                                    LiProfileStats.Text = "已关闭profile.不记录任何操作";
                                }
                            }
                        }

                        var query = database.GetCollection("system.profile").FindAll();
                        if (tbPageSize.Text.Trim() != "")
                        {
                            query.Limit(int.Parse(tbPageSize.Text));
                        }

                        query.Sort(new Document().Append("ts", IndexOrder.Descending));
                        var docs = query.Documents.ToList();
                        Document doc = new Document();
                        for (int i = 0; i < docs.Count; i++)
                        {
                            doc.Add((i + 1).ToString(), docs[i]);
                        }
                        BindDocumentToTreeView("数据列表", doc, DataTreeList);

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
            //tv.SelectedNode = tv.Nodes[0];
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

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btRefresh_Click(object sender, EventArgs e)
        {
            SetInfo();
        }

        /// <summary>
        /// 设置Profile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btSetProfile_Click(object sender, EventArgs e)
        {
            Document doc = new Document();
            int stats = int.Parse(rblType.Text);
            if (stats == 0)
            {
                doc.Append("profile", 0);

            }
            else if (stats == 1)
            {
                doc.Append("profile", 1);
                doc.Append("slowms", int.Parse(tbRunTime.Text));
            }
            else if (stats == 2)
            {
                doc.Append("profile", 2);
            }

            if (infoType.ToLowerInvariant() == "database")
            {

                DBMongo db =mdb.GetInfoList(Guid.Parse(infoId)) as DBMongo;
                if (db != null)
                {
                    using (Mongo mongo = new Mongo(string.Format(connStringTemplate, db.Server.IP, db.Server.Port)))
                    {
                        mongo.Connect();
                        Database database = mongo.GetDatabase(db.Name);
                        database.SendCommand(doc);
                        mongo.Disconnect();
                    }
                }

            }
            SetInfo();
        }
    }
}