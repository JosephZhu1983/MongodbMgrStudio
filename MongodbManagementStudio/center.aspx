<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="center.aspx.cs" Inherits="MongodbManagementStudio.center" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>无标题文档</title>
    <style type="text/css">
        body
        {
            margin: 0;
            padding: 0;
        }
        .navPoint
        {
            cursor: hand;
        }
        .tdBG
        {
            background: #FFFFFF url('images/bg2.gif') repeat-y left top;
        }
    </style>

    <script type="text/javascript">
        function switchSysBar() {
            if (document.getElementById("frmTitle").style.display == "") {
                document.getElementById("frmTitle").style.display = "none"
            } else {
                document.getElementById("frmTitle").style.display = ""
            }
        } 
    </script>

</head>
<body><table width="100%" height="100%" border="0" cellpadding="0" cellspacing="0">
        <tr>
            <td width="400" id="frmTitle"  align="center" valign="top">
                <iframe name="mylift" height="100%" width="400" src="left.aspx" frameborder="0">
                </iframe>
            </td>
            <td width="8" valign="middle"  class="tdBG" onclick="switchSysBar()">
                <span class="navPoint"><img src="images/bg1.gif"  /></span>
            </td>
            <td valign="top">
                <iframe name="Main" height="100%" width="100%" frameborder="0" src="blank.aspx">
                </iframe>
            </td>
        </tr>
    </table>
</body>
</html>

