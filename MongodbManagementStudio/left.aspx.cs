using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongodbManagementStudio.Entities;
using System.Xml.Linq;
using System.IO;
using System.ComponentModel;
using System.Threading;
using MongoDB.Driver;
using MongodbManagementStudio.Services;

namespace MongodbManagementStudio
{
    public partial class left : System.Web.UI.Page
    {
        private   List<ServerMongo> servers = new List<ServerMongo>();
        private   Dictionary<Guid, object> infoList = new Dictionary<Guid, object>();

        private readonly static string connStringTemplate = "Server={0}:{1}";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                load_DoWork();
                load_RunWorkerCompleted();
            }
        }



        private void load_RunWorkerCompleted()
        {
            list.Nodes.Clear();
            foreach (var server in servers)
            {
                TreeNode serverNode = new TreeNode(server.IsOK ? ("<span onclick=\"SendId('server','" + server.ID + "');return false;\">" + server.Name + "</span><span><img src='images/tongbu.gif'onclick=\"SendReplactionInfoId('server','" + server.ID + "');return false;\" /></span>") : ("<span onclick=\"SendId('server','" + server.ID + "');return false;\"><span style=\"color:red;\" >" + server.Name + "</span></span>"));

                TreeNode dbNode = new TreeNode("<span onclik='return false;'>Database</span>");
                foreach (var db in server.DBs)//数据库
                {
                    if (IsShowDatabase(db.Name))
                    {
                        var dbNode2 = new TreeNode("<span onclick=\"SendId('database','" + db.ID + "');return false;\">" + db.Name + "</span>");
                        TreeNode dbTable = new TreeNode("Table");
                        foreach (var tb in db.Tables)
                        {
                            if (IsShowTable(tb.Name))
                            {
                                TreeNode dbTbale2 = new TreeNode("<span onclick=\"SendId('table','" + tb.ID + "');return false;\">" + tb.Name + "</span>&nbsp;<span><img src='images/sql.gif'onclick=\"SendHref('" + db.ID + "');return false;\" /></span>&nbsp;&nbsp;<span><img src='images/indexs.gif'onclick=\"SendHrefIndex('" + tb.ID + "');return false;\" /></span>");
                                TreeNode dbColumn = new TreeNode("Column");
                                if (tb.Columnes != null)
                                {
                                    foreach (ColumnMongo myColumn in tb.Columnes)
                                    {
                                        TreeNode dbColumn2 = new TreeNode("<span onclick=\"SendId('Column','" + myColumn.ID + "');return false;\">" + myColumn.ColumnName + "</span>");
                                        dbColumn.ChildNodes.Add(dbColumn2);
                                    }
                                }
                                dbTbale2.ChildNodes.Add(dbColumn);


                                TreeNode dbindex = new TreeNode("Index");
                                foreach (var myIndex in tb.Indexes)
                                {
                                    TreeNode dbindex2 = new TreeNode("<span onclick=\"SendId('index','" + myIndex.ID + "');return false;\">" + myIndex.Name + "</span>");
                                    dbindex.ChildNodes.Add(dbindex2);
                                }
                                dbTbale2.ChildNodes.Add(dbindex);
                                dbTable.ChildNodes.Add(dbTbale2);
                            }
                        }
                        dbNode2.ChildNodes.Add(dbTable);
                        dbNode.ChildNodes.Add(dbNode2);
                    }
                }
                serverNode.ChildNodes.Add(dbNode);
                list.Nodes.Add(serverNode);
            }
        }

        /// <summary>
        /// 判断表是否能显示,true表示可以显示,false表示不显示
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected bool IsShowTable(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) return false;
            List<string> listTable = new List<string>();
            listTable.Add("system.indexes");
            listTable.Add("system.js");
            listTable.Add("system.profile");
            return !listTable.Contains(tableName.ToLowerInvariant());
        }

        /// <summary>
        /// 判断表是否能显示,true表示可以显示,false表示不显示
        /// </summary>
        /// <param name="DbName"></param>
        /// <returns></returns>
        protected bool IsShowDatabase(string DbName)
        {
            if (string.IsNullOrEmpty(DbName)) return false;
            List<string> listDb = new List<string>();
            listDb.Add("admin");
            listDb.Add("local");
            return !listDb.Contains(DbName.ToLowerInvariant());
        }


        private void load_DoWork()
        {
            servers.Clear();
            infoList.Clear();//清空数据
            XDocument xml = XDocument.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config/servers.config"));
            servers.AddRange(xml.Descendants("Server").Select(node => new ServerMongo
            {
                ID = Guid.NewGuid(),
                DBs = new List<DBMongo>(),
                IP = node.Attribute("IP").Value,
                Port = node.Attribute("Port").Value
            }));
            //把服务器信息添加到公共信息列表中
            servers.ForEach(s => infoList.Add(s.ID, s));

            Thread[] threads = new Thread[servers.Count];

            for (int i = 0; i < servers.Count; i++)
            {
                threads[i] = new Thread(obj =>
                {

                    ServerMongo server = servers[(int)obj];
                    using (Mongo mongo = new Mongo(string.Format(connStringTemplate, server.IP, server.Port)))
                    {
                        try
                        {
                            mongo.Connect();
                            server.IsOK = true;
                            Database admindb = mongo.GetDatabase("admin");
                            Document dbs = admindb.SendCommand(new Document().Append("listDatabases", 1));
                            server.TotalSize = Convert.ToInt64(dbs["totalSize"]);//数据库总大小
                            List<Document> dblist = dbs["databases"] as List<Document>;
                            server.DBs.AddRange(dblist.Select(d => new DBMongo
                            {
                                ID = Guid.NewGuid(),
                                Name = d["name"].ToString(),
                                Size = Convert.ToInt64(d["sizeOnDisk"]),
                                Tables = new List<TableMongo>(),
                                Server = server,
                            }));
                            //讲数据库信息添加表公共信息列表中
                            server.DBs.ForEach(myDB => infoList.Add(myDB.ID, myDB));
                            server.DBs.ForEach(d =>
                            {
                                Database db = mongo.GetDatabase(d.Name);
                                IMongoCollection namespaces = db.GetCollection("system.namespaces");
                                ICursor cursor = namespaces.Find(new Document());

                                d.Tables.AddRange(db.GetCollectionNames().Where(t => !t.Contains("$")).Select(t => new TableMongo
                                {
                                    ID = Guid.NewGuid(),
                                    Namespace = t,
                                    Name = t.Substring(t.IndexOf('.')).TrimStart('.'),
                                    Indexes = new List<IndexMongo>(),
                                    Columnes = new List<ColumnMongo>(),
                                    DB = d,
                                }));
                                //将表信息添加到公共信息列表中
                                d.Tables.ForEach(myTable => infoList.Add(myTable.ID, myTable));


                                d.Tables.ForEach(t =>
                                    {
                                        Document doc = db[t.Name].FindOne(new Document());
                                        if (doc != null)
                                        {
                                            foreach (string item in doc.Keys)
                                            {
                                                t.Columnes.Add(
                                                    new ColumnMongo
                                                    {
                                                        ID = Guid.NewGuid(),
                                                        ColumnName = item,
                                                        Table = t,
                                                    }
                                                );
                                            }
                                        }
                                        //t.Columnes.ForEach(myColumn => infoList.Add(myColumn.ID, myColumn));

                                    });


                                d.Tables.ForEach(t =>
                                {
                                    ICursor index = db["system.indexes"].Find(new Document().Append("ns", t.Namespace));
                                    foreach (Document doc in index.Documents)
                                    {
                                        t.Indexes.Add(new IndexMongo
                                        {
                                            ID = Guid.NewGuid(),
                                            Name = doc["name"].ToString(),
                                            Unique = Convert.ToBoolean(doc["unique"]),
                                            Table = t,
                                        });
                                    }
                                    //将索引信息添加到公共信息列表中
                                    t.Indexes.ForEach(myIndex => infoList.Add(myIndex.ID, myIndex));
                                });
                            });

                        }
                        catch (Exception ex)
                        {
                            server.IsOK = false;
                        }
                    }
                });
                threads[i].Start(i);
            }

            foreach (Thread t in threads)
            {
                t.Join(1000);
            }

            Session["DbInfoList"] = infoList;

        }

        /// <summary>
        /// 展开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ibExpandAll_Click(object sender, ImageClickEventArgs e)
        {
            this.list.ExpandAll();
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ibCollapse_Click(object sender, ImageClickEventArgs e)
        {
            this.list.CollapseAll();
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ibRefresh_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("left.aspx");
        }

    }
}