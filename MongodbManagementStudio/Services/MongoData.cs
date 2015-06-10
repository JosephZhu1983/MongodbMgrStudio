using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongodbManagementStudio.Services
{
    public class MongoData : System.Web.UI.Page
    {
        /// <summary>
        /// 公共信息列表
        /// </summary>
        public static Dictionary<Guid, object> objectList = new Dictionary<Guid, object>();


        /// <summary>
        /// 获取公共信息列表中的某个对象实例
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public object GetInfoList(Guid id)
        {
            Dictionary<Guid, object> objList = new Dictionary<Guid, object>();
            object obj = new object();
            if (Session["DbInfoList"] != null)
            {
                objList = Session["DbInfoList"] as Dictionary<Guid, object>;
                if (objList != null && objList.Count > 0)
                {
                    obj = objList[id];
                    if (obj == null)
                    {
                        obj = new object();
                    }
                }
            }
            return obj;
        }

    }
}