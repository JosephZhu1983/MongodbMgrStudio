using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.IO;
using System.Text;
using MongodbManagementStudio.Services;

namespace MongodbManagementStudio.DBAdmin
{
    public partial class ServerManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ShowServer();
            }
            AjaxPro.Utility.RegisterTypeForAjax(typeof(ServerManager));
        }

        protected void ShowServer()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                // 加载指定的xml文件
                XDocument xml = XDocument.Load(Server.MapPath("~/config/servers.config"));

                // 使用查询语法获取Person集合
                var persons = from p in xml.Descendants("Server")
                              select new
                              {
                                  IP = p.Attribute("IP").Value,
                                  Port = p.Attribute("Port").Value
                              };
                Repeater1.DataSource = persons;
                Repeater1.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Alert(this.Page, ex.Message);
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            AddServer();
        }

        /// <summary>
        /// 添加服务器
        /// </summary>
        protected void AddServer()
        {
            try
            {
                if (tbIp.Text != "" && tbPort.Text != "")
                {
                    XDocument xml = XDocument.Load(Server.MapPath("~/config/servers.config"));

                    // 创建需要新增的XElement对象
                    XElement server = new XElement(
                        "Server",
                        new XAttribute("IP", tbIp.Text),
                        new XAttribute("Port", tbPort.Text));

                    // 添加需要新增的XElement对象
                    xml.Root.Add(server);

                    // 保存xml
                    xml.Save(Server.MapPath("~/config/servers.config"));
                    //xml.Descendants("Server").add
                }
                ShowServer();
            }
            catch (Exception ex)
            {
                MessageBox.Alert(this.Page, ex.Message);
            }
        }


        [AjaxPro.AjaxMethod]
        public void DeleteServer(string ip, string port)
        {
            try
            {
                // 加载指定的xml文件
                XDocument xml = XDocument.Load(Server.MapPath("~/config/servers.config"));

                // 使用查询语法获取指定的Person集合
                var server = from p in xml.Root.Elements("Server")
                             where p.Attribute("IP").Value == ip && p.Attribute("Port").Value == port
                             select p;
                // 删除指定的XElement对象
                server.Remove();

                // 保存xml
                xml.Save(Server.MapPath("~/config/servers.config"));
            }
            catch (Exception ex)
            {
                MessageBox.Alert(this.Page, ex.Message);
            }

        }


         protected void UpdateServer(string ip, string port)
         {

             // 加载指定的xml文件
             XDocument xml = XDocument.Load(Server.MapPath("~/config/servers.config"));

             // 使用查询语法获取指定的Person集合
             var servers = from p in xml.Root.Elements("Server")
                           where p.Attribute("IP").Value == ip && p.Attribute("Port").Value == port
                           select p;

             // 更新指定的XElement对象
             foreach (XElement xe in servers)
             {
                 xe.SetAttributeValue("IP", tbIp.Text);
                 xe.SetAttributeValue("Port", tbPort.Text);
             }

             // 保存xml
             xml.Save(Server.MapPath("~/config/servers.config"));
         }

    }
}