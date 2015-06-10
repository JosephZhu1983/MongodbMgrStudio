using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using MongoDB.Driver;
using MongodbManagementStudio.Entities;
using MongodbManagementStudio.Services;

namespace MongodbManagementStudio.DbAdmin
{
    public partial class ShowData : System.Web.UI.Page
    {
        private readonly  string connStringTemplate = "Server={0}:{1}";
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
            try
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
                            Document stats = admindb.SendCommand(new Document().Append("serverStatus", 1));
                            BindDocumentToTreeView(objServer.Name, stats, DataTreeList);
                        }
                    }
                }
                else if (infoType.ToLowerInvariant() == "database")
                {

                    DBMongo db = mdb.GetInfoList(Guid.Parse(infoId)) as DBMongo;
                    if (db != null)
                    {
                        liDbName.Text = "Server:" + db.Server.Name + " Database:" + db.Name;
                        using (Mongo mongo = new Mongo(string.Format(connStringTemplate, db.Server.IP, db.Server.Port)))
                        {
                            mongo.Connect();
                            Database database = mongo.GetDatabase(db.Name);
                            Document stats = database.SendCommand(new Document().Append("dbstats", 1));
                            BindDocumentToTreeView(db.Name, stats, DataTreeList);
                        }

                        divProfile.Visible = true;
                    }

                }
                else if (infoType.ToLowerInvariant() == "table")
                {
                    TableMongo myTable = mdb.GetInfoList(Guid.Parse(infoId)) as TableMongo;
                    if (myTable != null)
                    {
                        liDbName.Text = "Server:" + myTable.DB.Server.Name + " Database:" + myTable.DB.Name + " Table:" + myTable.Name;
                        using (Mongo mongo = new Mongo(string.Format(connStringTemplate, myTable.DB.Server.IP, myTable.DB.Server.Port)))
                        {
                            mongo.Connect();
                            Database database = mongo.GetDatabase(myTable.DB.Name);
                            Document stats = database.SendCommand(new Document().Append("collstats", myTable.Name));
                            BindDocumentToTreeView(myTable.Name, stats, DataTreeList);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Alert(this.Page, ex.Message);
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
    }







}