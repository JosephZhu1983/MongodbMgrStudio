using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MongodbManagementStudio.Entities;
using MongodbManagementStudio.Services;
using MongoDB.Driver;
using gudusoft.gsqlparser;
using System.Text.RegularExpressions;

namespace MongodbManagementStudio.DbAdmin
{
    public partial class SqlManager : System.Web.UI.Page
    {
        private MongoData mdb = new MongoData();
        string infoId = "";
        private readonly  string connStringTemplate = "Server={0}:{1}";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["id"] != null)
            {
                infoId = Request.QueryString["id"];
            }
            if (!IsPostBack)
            {
                ShowData();
            }
        }

        protected void ShowData()
        {
            if (!string.IsNullOrWhiteSpace(infoId))
            {
                DBMongo obj =mdb.GetInfoList(Guid.Parse(infoId)) as DBMongo;
                if (obj != null)
                {
                    liDbName.Text = "Server:" + obj.Server.Name + " DataBase:" + obj.Name;
                }
            }


        }

        protected void tbRunSql_Click(object sender, EventArgs e)
        {
            string strContent = tbSql.Text;
            if (!string.IsNullOrWhiteSpace(strContent))
            {
                if (strContent.ToLowerInvariant().IndexOf("select") != -1)
                {
                    RunSelcet(tbSql.Text);
                }
                else if (strContent.ToLowerInvariant().IndexOf("update") != -1)
                {
                    RunUpdate(strContent);
                }
                else if (strContent.ToLowerInvariant().IndexOf("insert") != -1)
                {
                    RunInsert(strContent);
                }
                else if (strContent.ToLowerInvariant().IndexOf("delete") != -1)
                {
                    RunDelete(strContent);
                }
                else
                {
                    liMessage.Text = "<span style='color:red;'>Unrecognized command!</span>";
                }
            }
        }

        /// <summary>
        /// 执行insert
        /// </summary>
        /// <param name="sqlContent"></param>
        protected void RunInsert(string sqlContent)
        {
            TGSqlParser parser = new TGSqlParser(TDbVendor.DbVMssql);
            parser.SqlText.Text = sqlContent;
            parser.Parse();
            if (parser.ErrorCount > 0)
            {
                liMessage.Text = "<span style='color:red;'>" + parser.ErrorMessages + "</span>";
                return;
            }

            TInsertSqlStatement sql = parser.SqlStatements[0] as TInsertSqlStatement;
            if (!string.IsNullOrWhiteSpace(infoId) && sql.Tables != null)
            {
                DBMongo obj =mdb.GetInfoList(Guid.Parse(infoId)) as DBMongo;

                using (Mongo mongo = new Mongo(string.Format(connStringTemplate, obj.Server.IP, obj.Server.Port)))
                {
                    mongo.Connect();
                    Database db = mongo.GetDatabase(obj.Name);
                    var table = db.GetCollection(sql.Tables[0].AsText);
                    Document doc = new Document();
                    for (int i = 0; i < sql.Fields.Count(); i++)
                    {
                        string key = sql.Fields[i].AsText;
                        string value = (sql.MultiValues[0] as TLzFieldList)[i].AsText;
                        if (value.StartsWith("'"))
                        {
                            DateTime dt;
                            if (DateTime.TryParse(value.Replace("'", ""), out dt))
                            {
                                doc.Add(key, dt);
                            }
                            else
                            {
                                doc.Add(key, value.Replace("'", ""));
                            }
                        }
                        else
                        {
                            Int32 ii = 0;
                            if (Int32.TryParse(value, out ii))
                                doc.Add(key, ii);
                            else
                                doc.Add(key, value);
                        }
                    }
                    table.Insert(doc);

                }
            }
        }


        /// <summary>
        /// 执行delete
        /// </summary>
        /// <param name="sqlContent"></param>
        protected void RunDelete(string sqlContent)
        {


            TGSqlParser parser = new TGSqlParser(TDbVendor.DbVMssql);
            parser.SqlText.Text = sqlContent;
            parser.Parse();
            if (parser.ErrorCount > 0)
            {
                liMessage.Text = "<span style='color:red;'>" + parser.ErrorMessages + "</span>";
                return;
            }

            TDeleteSqlStatement sql = parser.SqlStatements[0] as TDeleteSqlStatement;
            ChangeSqlToJs(sql.WhereClause);

            if (!string.IsNullOrWhiteSpace(infoId) && sql.Tables != null)
            {
                DBMongo obj =mdb.GetInfoList(Guid.Parse(infoId)) as DBMongo;

                using (Mongo mongo = new Mongo(string.Format(connStringTemplate, obj.Server.IP, obj.Server.Port)))
                {
                    mongo.Connect();
                    Database db = mongo.GetDatabase(obj.Name);
                    var table = db.GetCollection(sql.Tables[0].AsText);
                    Document doc = new Document();
                    if (!string.IsNullOrEmpty(sql.WhereClauseAsPrettyText))
                        doc.Add("$where", new Code(sql.WhereClauseAsPrettyText.Replace("\r\n", "")));
                    table.Delete(doc);
                }
            }

        }

        /// <summary>
        /// 执行update
        /// </summary>
        /// <param name="sqlContent"></param>
        protected void RunUpdate(string sqlContent)
        {
            TGSqlParser parser = new TGSqlParser(TDbVendor.DbVMssql);
            parser.SqlText.Text = sqlContent;
            parser.Parse();
            if (parser.ErrorCount > 0)
            {
                liMessage.Text = "<span style='color:red;'>" + parser.ErrorMessages + "</span>";
                return;
            }

            TUpdateSqlStatement sql = parser.SqlStatements[0] as TUpdateSqlStatement;

            ChangeSqlToJs(sql.WhereClause);


            if (!string.IsNullOrWhiteSpace(infoId) && sql.Tables != null)
            {
                DBMongo obj =mdb.GetInfoList(Guid.Parse(infoId)) as DBMongo;

                using (Mongo mongo = new Mongo(string.Format(connStringTemplate, obj.Server.IP, obj.Server.Port)))
                {
                    mongo.Connect();
                    Database db = mongo.GetDatabase(obj.Name);
                    var table = db.GetCollection(sql.Tables[0].AsText);

                    Document doc = new Document();
                    for (int i = 0; i < sql.Fields.Count(); i++)
                    {
                        string key = sql.Fields[i].AsText.Split('=')[0].Trim();
                        string value = sql.Fields[i].AsText.Split('=')[1].Trim();
                        if (value.StartsWith("'"))
                        {
                            DateTime dt;
                            if (DateTime.TryParse(value.Replace("'", ""), out dt))
                            {
                                doc.Add(key, dt);
                            }
                            else
                            {
                                doc.Add(key, value.Replace("'", ""));
                            }
                        }
                        else
                        {
                            Int32 ii = 0;
                            if (Int32.TryParse(value, out ii))
                                doc.Add(key, ii);
                            else
                                doc.Add(key, value);
                        }
                    }

                    Document where = new Document();
                    if (!string.IsNullOrEmpty(sql.WhereClauseAsPrettyText))
                        where.Add("$where", new Code(sql.WhereClauseAsPrettyText.Replace("\r\n", "")));
                    table.UpdateAll(doc, where);
                }
            }

        }

        /// <summary>
        /// 执行sql select
        /// </summary>
        /// <param name="sql"></param>
        protected void RunSelcet(string sqlContent)
        {
            bool isPaged = false;
            int pageLimit = 1;
            int pageSkip = 0;

            if (sqlContent.ToLowerInvariant().IndexOf("$rowid") != -1)
            {
                string[] newcontent = sqlContent.Split('$');
                sqlContent = newcontent[0];
                string newpage = newcontent[1].Replace("rowid(", "").Replace(")", "");
                string[] newpage2 = newpage.Split(',');
                int starts = int.Parse(newpage2[0]);
                int ends = int.Parse(newpage2[1]);

                pageSkip = starts;
                pageLimit = ends - starts + 1;
                isPaged = true;
            }

            TGSqlParser parser = new TGSqlParser(TDbVendor.DbVMssql);
            parser.SqlText.Text = sqlContent;
            parser.Parse();
            if (parser.ErrorCount > 0)
            {
                liMessage.Text = "<span style='color:red;'>" + parser.ErrorMessages + "</span>";
                //MessageBox.Show(parser.ErrorMessages);
                return;
            }
            TSelectSqlStatement sql = parser.SqlStatements[0] as TSelectSqlStatement;

            Document orders = new Document();
            if (sql.SortClause != null)
            {
                for (int i = 0; i < sql.SortClause.Count(); i++)
                {
                    string s = sql.SortClause[i].AsText;
                    if (s.IndexOf(" desc", StringComparison.InvariantCultureIgnoreCase) > 0)
                    {
                        s = Regex.Replace(s, " desc", "", RegexOptions.IgnoreCase);
                        orders.Add(s, IndexOrder.Descending);
                    }
                    else
                    {
                        s = Regex.Replace(s, " asc", "", RegexOptions.IgnoreCase);
                        orders.Add(s, IndexOrder.Ascending);
                    }
                }
            }

            bool iscount = false;
            Document fields = new Document();
            if (sql.Fields != null)
            {
                for (int i = 0; i < sql.Fields.Count(); i++)
                {
                    if (sql.Fields[i].AsText.ToLower().Contains("count"))
                    {
                        iscount = true;
                        fields.Clear();
                        break;
                    }
                    if (sql.Fields[i].AsText == "*")
                    {
                        fields.Clear();
                        break;
                    }
                    fields.Add(sql.Fields[i].AsText, "1");
                }
            }

            int limit = -1;
            if (!string.IsNullOrEmpty(sql.topclauseText))
                limit = Convert.ToInt32(sql.topclauseText.Substring(3));




            ChangeSqlToJs(sql.WhereClause);

            if (!string.IsNullOrEmpty(infoId) && sql.Tables != null)
            {
                DBMongo obj =mdb.GetInfoList(Guid.Parse(infoId)) as DBMongo;

                using (Mongo mongo = new Mongo(string.Format(connStringTemplate, obj.Server.IP, obj.Server.Port)))
                {
                    mongo.Connect();
                    Database db = mongo.GetDatabase(obj.Name);
                    if (!iscount)
                    {
                        var query = db.GetCollection(sql.Tables[0].AsText).FindAll();
                        if (!string.IsNullOrEmpty(sql.WhereClauseAsPrettyText))
                        {
                            string code = sql.WhereClauseAsPrettyText.Replace("\r\n", "").HandleLike();
                            query = query.Spec(new Document().Append("$where", new Code(code)));
                        }
                        if (fields.Count > 0)
                            query = query.Fields(fields);
                        if (isPaged)
                        {//分页的处理方法
                            query = query.Limit(pageLimit).Skip(pageSkip);
                        }
                        else
                        {
                            //top 的处理方法
                            if (limit != -1)
                                query = query.Limit(limit);
                        }
                        if (orders.Count > 0)
                            query = query.Sort(orders);
                        var docs = query.Documents.ToList();
                        Document doc = new Document();
                        for (int i = 0; i < docs.Count; i++)
                        {
                            doc.Add(i.ToString(), docs[i]);
                        }
                        BindDocumentToTreeView("数据列表", doc, dataList);
                    }
                    else
                    {
                        Document doc = new Document();
                        var table = db.GetCollection(sql.Tables[0].AsText);
                        long c = 0;
                        if (!string.IsNullOrEmpty(sql.WhereClauseAsPrettyText))
                        {
                            string code = sql.WhereClauseAsPrettyText.Replace("\r\n", "").HandleLike();
                            c = table.Count(new Document().Append("$where", new Code(code)));
                        }
                        else
                            c = table.Count();
                        doc.Add("Count:", c);
                        BindDocumentToTreeView("数据列表", doc, dataList);
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

        private void ChangeSqlToJs(TLzCustomExpression pExpr)
        {
            if (pExpr != null)
            {
                pExpr.PreOrderTraverse(ChangeSqlToJsVisitor);
            }
        }


        private bool ChangeSqlToJsVisitor(TLz_Node pnode, Boolean pIsLeafNode)
        {
            TLzCustomExpression lcexpr = (TLzCustomExpression)pnode;
            if (lcexpr.oper == TLzOpType.Expr_AND)
            {
                lcexpr.opname.AsText = "&&";
            }
            else if (lcexpr.oper == TLzOpType.Expr_Comparison)
            {

                if (lcexpr.opname.AsText == "=")
                    lcexpr.opname.AsText = "==";
            }
            else if (lcexpr.oper == TLzOpType.Expr_OR)
            {
                lcexpr.opname.AsText = "||";
            }
            else if (lcexpr.oper == TLzOpType.Expr_Attr)
            {
                lcexpr.AsText = "this." + lcexpr.AsText;
            }

            return true;
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string strContent = tbSql.Text;
            if (!string.IsNullOrWhiteSpace(strContent))
            {
                if (strContent.ToLowerInvariant().IndexOf("select") != -1)
                {
                    RunSelcetExplain(tbSql.Text);
                }
                else
                {
                    liMessage.Text = "<span style='color:red;'>Unrecognized command!</span>";
                }
            }
        }


        /// <summary>
        /// 执行sql select
        /// </summary>
        /// <param name="sql"></param>
        protected void RunSelcetExplain(string sqlContent)
        {
            bool isPaged = false;
            int pageLimit = 1;
            int pageSkip = 0;

            if (sqlContent.ToLowerInvariant().IndexOf("$rowid") != -1)
            {
                string[] newcontent = sqlContent.Split('$');
                sqlContent = newcontent[0];
                string newpage = newcontent[1].Replace("rowid(", "").Replace(")", "");
                string[] newpage2 = newpage.Split(',');
                int starts = int.Parse(newpage2[0]);
                int ends = int.Parse(newpage2[1]);

                pageSkip = starts;
                pageLimit = ends - starts + 1;
                isPaged = true;
            }

            TGSqlParser parser = new TGSqlParser(TDbVendor.DbVMssql);
            parser.SqlText.Text = sqlContent;
            parser.Parse();
            if (parser.ErrorCount > 0)
            {
                liMessage.Text = "<span style='color:red;'>" + parser.ErrorMessages + "</span>";
                //MessageBox.Show(parser.ErrorMessages);
                return;
            }
            TSelectSqlStatement sql = parser.SqlStatements[0] as TSelectSqlStatement;

            Document orders = new Document();
            if (sql.SortClause != null)
            {
                for (int i = 0; i < sql.SortClause.Count(); i++)
                {
                    string s = sql.SortClause[i].AsText;
                    if (s.IndexOf(" desc", StringComparison.InvariantCultureIgnoreCase) > 0)
                    {
                        s = Regex.Replace(s, " desc", "", RegexOptions.IgnoreCase);
                        orders.Add(s, IndexOrder.Descending);
                    }
                    else
                    {
                        s = Regex.Replace(s, " asc", "", RegexOptions.IgnoreCase);
                        orders.Add(s, IndexOrder.Ascending);
                    }
                }
            }

            bool iscount = false;
            Document fields = new Document();
            if (sql.Fields != null)
            {
                for (int i = 0; i < sql.Fields.Count(); i++)
                {
                    if (sql.Fields[i].AsText.ToLower().Contains("count"))
                    {
                        iscount = true;
                        fields.Clear();
                        break;
                    }
                    if (sql.Fields[i].AsText == "*")
                    {
                        fields.Clear();
                        break;
                    }
                    fields.Add(sql.Fields[i].AsText, "1");
                }
            }

            int limit = -1;
            if (!string.IsNullOrEmpty(sql.topclauseText))
                limit = Convert.ToInt32(sql.topclauseText.Substring(3));




            ChangeSqlToJs(sql.WhereClause);

            if (!string.IsNullOrEmpty(infoId) && sql.Tables != null)
            {
                DBMongo obj =mdb.GetInfoList(Guid.Parse(infoId)) as DBMongo;

                using (Mongo mongo = new Mongo(string.Format(connStringTemplate, obj.Server.IP, obj.Server.Port)))
                {
                    mongo.Connect();
                    Database db = mongo.GetDatabase(obj.Name);
                    if (!iscount)
                    {
                        var query = db.GetCollection(sql.Tables[0].AsText).FindAll();
                        if (!string.IsNullOrEmpty(sql.WhereClauseAsPrettyText))
                        {
                            string code = sql.WhereClauseAsPrettyText.Replace("\r\n", "").HandleLike();
                            query = query.Spec(new Document().Append("$where", new Code(code)));
                        }
                        if (fields.Count > 0)
                            query = query.Fields(fields);
                        if (isPaged)
                        {//分页的处理方法
                            query = query.Limit(pageLimit).Skip(pageSkip);
                        }
                        else
                        {
                            //top 的处理方法
                            if (limit != -1)
                                query = query.Limit(limit);
                        }
                        if (orders.Count > 0)
                        {
                            query = query.Sort(orders);
                        }

                        Document doc = query.Explain();

                        BindDocumentToTreeView("数据列表", doc, dataList);
                    }
                    else
                    {
                        Document doc = new Document();
                        var table = db.GetCollection(sql.Tables[0].AsText);
                        long c = 0;
                        if (!string.IsNullOrEmpty(sql.WhereClauseAsPrettyText))
                        {
                            string code = sql.WhereClauseAsPrettyText.Replace("\r\n", "").HandleLike();
                            c = table.Count(new Document().Append("$where", new Code(code)));
                        }
                        else
                            c = table.Count();
                        doc.Add("Count:", c);
                        BindDocumentToTreeView("数据列表", doc, dataList);
                    }
                }
            }
        }

    }

    public static class util
    {
        public static string HandleLike(this string s)
        {
            while (s.ToLower().Contains("like"))
            {
                List<string> arr = s.Split(' ').ToList();
                arr.RemoveAll(d => d == "");
                string a = "", b = "";
                for (int i = 0; i < arr.Count; i++)
                {
                    if (arr[i].ToLower() == "like")
                    {
                        a = arr[i - 1];
                        b = arr[i + 1];
                        arr.RemoveAt(i);
                        arr.Remove(a);
                        arr.Remove(b);
                        break;
                    }
                }
                b = b.Replace("'", "").Replace("%", @".*");
                string ss = string.Concat(arr.ToArray()).Trim('&');
                ss = string.Concat(ss, string.Format("&& new RegExp('^{0}$', 'gi').test({1})", b, a));
                s = ss;
            }
            return s.TrimStart('&');
        }
    }
}