<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="left.aspx.cs" Inherits="MongodbManagementStudio.left" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link id="Link1" href="css/base.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        .menu
        {
            width: 100%;
            overflow-x: hidden;
            overflow-y: auto;
            margin: 0 auto;
        }
        * html .menu
        {
            width: 100%;
        }
        .menuFd
        {
            width: 100%;
            margin: 0 0 0 8px;
            padding: 0px 0px;
        }
        .menuTit
        {
            background-image: url(images/menuitem.gif);
            background-repeat: no-repeat;
            height: 30px;
            font-size: 12px;
            text-decoration: none;
            padding-left: 15px;
            padding-top: 10px;
            padding-bottom: 0px;
            font-weight: bold;
        }
        .menuItem
        {
            margin: 5px 0 0 20px;
            text-align: left;
            line-height: 20px;
        }
        .menuTop
        {
            padding: 10px 6px;
            text-align: left;
        }
        .menuTop span
        {
            width: 80px;
            background-image: url(images/menuico.gif);
            background-repeat: no-repeat;
            padding-left: 28px;
            font-weight: bold;
        }
    </style>
</head>
<body style="background-color: #f3f8fc; padding: 0px;">
    <form runat="server" id="form1">
    <div style="width: 100%; height: 32px; background-image: url(images/left_top.gif);
        margin-top: 1px;">
        <div class="menuTop">
            <span><a href="DbAdmin/ServerManager.aspx" target="Main">Servers</a></span>&nbsp;<asp:ImageButton ID="ibRefresh" runat="server" ToolTip="Refresh"
                ImageUrl="~/images/refresh.gif" OnClick="ibRefresh_Click"></asp:ImageButton>&nbsp;<asp:ImageButton
                    ID="ibExpandAll" ToolTip="Expand All" runat="server" ImageUrl="~/images/minimize.gif"
                    OnClick="ibExpandAll_Click" ></asp:ImageButton>&nbsp;<asp:ImageButton
                        ID="ibCollapse" runat="server" ToolTip="Collapse All" ImageUrl="~/images/maximize.gif"
                        OnClick="ibCollapse_Click"></asp:ImageButton>&nbsp;
        </div>
    </div>
    <div id="menu" class="menu">
        <div class="menuFd">
            <asp:TreeView ID="list" runat="server" ExpandDepth="1">
            </asp:TreeView>
        </div>
    </div>
    <a id="MyShowData" href="javascript:void;" target="Main"></a>
    <script type="text/javascript">
        function SendId(type, id) {
            var myherf = document.getElementById("MyShowData");
            myherf.href = "DbAdmin/ShowData.aspx?id=" + id + "&type=" + type;
            myherf.click();

        }
        function SendReplactionInfoId(type, id) {
            var myherf = document.getElementById("MyShowData");
            myherf.href = "DbAdmin/ReplactionInfo.aspx?id=" + id + "&type=" + type;
            myherf.click();

        }
        function SendHref(id) {
            var myherf = document.getElementById("MyShowData");
            myherf.href = "DbAdmin/SqlManager.aspx?id=" + id;
            myherf.click();
        }
        function SendHrefIndex(id) {
            var myherf = document.getElementById("MyShowData");
            myherf.href = "DbAdmin/IndexManager.aspx?id=" + id;
            myherf.click();
        }
        function setPageHeight() {
            document.getElementById("menu").style.height = (document.body.clientHeight - 39) + "px";
        }
        setPageHeight();
        window.onresize = setPageHeight;
    </script>
    </form>
</body>
</html>
