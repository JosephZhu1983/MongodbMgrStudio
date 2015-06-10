using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongodbManagementStudio.Entities;
using MongodbManagementStudio.Services;
using MongoDB.Driver;
using System.Threading;

namespace MongodbManagementStudio.DbAdmin
{
    public partial class IndexManager : System.Web.UI.Page
    {
        private MongoData mdb = new MongoData();
        public string infoId = "";
        string indexInfo = "";
        bool isUnique = false;
        bool isBackground = false;
        bool isDropDups = false;
        private readonly string connStringTemplate = "Server={0}:{1}";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Form["hdIndexInfo"] != null)
            {
                indexInfo = Request.Form["hdIndexInfo"];
            }
            if (Request.Form["cbUnique"] != null)
            {
                isUnique = bool.Parse(Request.Form["cbUnique"]);
            }
            if (Request.Form["cbBackground"] != null)
            {
                isBackground = bool.Parse(Request.Form["cbBackground"]);
            }
            if (Request.Form["cbDropDups"] != null)
            {
                isDropDups = bool.Parse(Request.Form["cbDropDups"]);
            }

            if (Request.QueryString["id"] != null)
            {
                infoId = Request.QueryString["id"];
            }
            if (!IsPostBack)
            {
                ShowData();
            }

            AjaxPro.Utility.RegisterTypeForAjax(typeof(IndexManager));
        }

        protected void ShowData()
        {
            if (!string.IsNullOrWhiteSpace(infoId))
            {

                TableMongo obj =mdb.GetInfoList(Guid.Parse(infoId)) as TableMongo;
                if (obj != null)
                {
                    liDbName.Text = "Server:" + obj.DB.Server.Name + " DataBase:" + obj.DB.Name + " Table:" + obj.Name;
                    string showColumnName = "";

                    if (!string.IsNullOrWhiteSpace(infoId))
                    {

                        using (Mongo mongo = new Mongo(string.Format(connStringTemplate, obj.DB.Server.IP, obj.DB.Server.Port)))
                        {
                            mongo.Connect();
                            Database db = mongo.GetDatabase(obj.DB.Name);
                            IMongoCollection table = db.GetCollection(obj.Name);


                            //获取列名
                            Document doc = table.FindOne(new Document());
                            if (doc != null && doc.Keys != null)
                            {
                                foreach (string item in doc.Keys)
                                {
                                    showColumnName += "<th onclick=\"setColumnName('" + item + "')\"><span>&nbsp;" + item + "&nbsp;</span></th>";//读取列名
                                }
                            }
                            //获取索引
                            dataList.Nodes.Clear();
                            ICursor index = db.GetCollection("system.indexes").Find(new Document().Append("ns", obj.DB.Name + "." + obj.Name));
                            foreach (Document myDoc in index.Documents)
                            {
                                BindDocumentToTreeView("索引列表", myDoc, dataList);
                            }
                            //关闭链接
                            mongo.Disconnect();

                        }

                    }
                    liColumnName.Text = showColumnName;

                }
            }


        }


        /// <summary>
        /// 把字符串转换成docment
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private Document StringToDoc(string info)
        {
            Document doc = new Document();
            if (!string.IsNullOrEmpty(info))
            {
                string[] items = info.TrimEnd('|').Split('|');
                foreach (string small in items)
                {
                    string[] smallitem = small.Split(':');
                    if (smallitem[0] != null && smallitem[1] != null)
                    {
                        doc.Append(smallitem[0], int.Parse(smallitem[1]));
                    }
                }
            }
            return doc;
        }

        private void BindDocumentToTreeView(string name, Document doc, TreeView tv)
        {
            TreeNode tn = new TreeNode(name);
            BindDocumentToNode(doc, tn);
            tv.Nodes.Add(tn);
            tv.ExpandAll();
            //tv.SelectedNode = tv.Nodes[0];

        }

        /// <summary>
        /// 删除指定索引
        /// </summary>
        /// <param name="objName"></param>
        [AjaxPro.AjaxMethod]
        public void DelIndex(object objName, string objid)
        {
            if (!string.IsNullOrWhiteSpace(objid))
            {
                TableMongo obj = MongoData.objectList[Guid.Parse(objid)] as TableMongo;
                if (obj != null)
                {
                    using (Mongo mongo = new Mongo(string.Format(connStringTemplate, obj.DB.Server.IP, obj.DB.Server.Port)))
                    {
                        mongo.Connect();
                        Database db = mongo.GetDatabase(obj.DB.Name);
                        IMongoCollection table = db.GetCollection(obj.Name);
                        //删除索引
                        table.MetaData.DropIndex(objName.ToString());
                        mongo.Disconnect();
                    }
                }
            }

        }

        /// <summary>
        /// 绑定索引1
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="node"></param>
        private void BindDocumentToNode(Document doc, TreeNode node)
        {
            foreach (string key in doc.Keys)
            {

                if (doc[key] is Document)
                {
                    TreeNode newNode = new TreeNode(key);
                    BindDocumentToNodeTwo(doc[key] as Document, newNode);
                    node.ChildNodes.Add(newNode);
                }
                else
                {

                    if (key.ToLowerInvariant() == "name")
                    {
                        node.ChildNodes.Add(new TreeNode(key + ":" + doc[key].ToString() + " <span onclick=\"deleteIndex('" + doc[key].ToString() + "');return false;\" >删除</span>"));
                    }
                    else
                    {
                        node.ChildNodes.Add(new TreeNode(string.Format("{0} : {1}", key, doc[key].ToString())));
                    }

                }
            }
        }
        /// <summary>
        /// 绑定索引2
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="node"></param>
        private void BindDocumentToNodeTwo(Document doc, TreeNode node)
        {
            foreach (string key in doc.Keys)
            {

                if (doc[key] is Document)
                {
                    TreeNode newNode = new TreeNode(key);
                    BindDocumentToNodeTwo(doc[key] as Document, newNode);
                    node.ChildNodes.Add(newNode);
                }
                else
                {
                    node.ChildNodes.Add(new TreeNode(string.Format("{0} : {1}", key, doc[key].ToString())));
                }
            }
        }



        protected void btCreateIndex_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(indexInfo))
            {
                CreateIndex();
                ShowData();
            }
        }

        protected void CreateIndex()
        {
            Thread thread = new Thread(newObj =>
            {

                if (!string.IsNullOrWhiteSpace(infoId))
                {
                    TableMongo obj =mdb.GetInfoList(Guid.Parse(infoId)) as TableMongo;
                    if (obj != null)
                    {
                        using (Mongo mongo = new Mongo(string.Format(connStringTemplate, obj.DB.Server.IP, obj.DB.Server.Port)))
                        {
                            mongo.Connect();
                            Database db = mongo.GetDatabase(obj.DB.Name);
                            IMongoCollection table = db.GetCollection(obj.Name);
                            table.MetaData.CreateIndex(StringToDoc(indexInfo), isUnique, isBackground, isDropDups);//创建索引
                            mongo.Disconnect();
                        }
                    }
                }

            });
            thread.Start();
        }

    }
}