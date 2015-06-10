<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="top.aspx.cs" Inherits="MongodbManagementStudio.top" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta content="text/html; charset=utf-8" http-equiv="Content-Type" />
    <link href="css/base.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        body
        {
            padding: 0px 0px;
            overflow: hidden;
        }
        td
        {
            background-color: Transparent;
        }
    </style>
<script language="javascript" type="text/javascript">
    function jsRedirect(url) {
        top.location.href = url;
    }
</script>
</head>

<body>
    <table border="0" cellspacing="0" cellpadding="0" width="100%" style="background-image: url(images/headbg.png);background-repeat: repeat-x">
        <tbody>
            <tr>
                <td style="height: 55px; width: 320px; background-image: url(images/Logo.png); background-repeat: no-repeat">
                </td>
                <td>
                    <img alt="" border="0" align="absmiddle" src="images/admin-ico.gif" />
                    &nbsp;&nbsp;[<a href="javascript:jsRedirect('index.aspx');" title="MongoDB管理中心首页">管理中心首页</a>]
                    
                </td>
                <td align="right">
                </td>
            </tr>
        </tbody>
    </table>
</body>
</html>