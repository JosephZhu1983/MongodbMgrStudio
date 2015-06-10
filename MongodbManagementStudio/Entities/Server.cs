using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MongodbManagementStudio.Entities
{
    /// <summary>
    /// 服务器信息
    /// </summary>
    public class ServerMongo
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public Guid ID { get; set; }

        public string IP { get; set; }
        public string Port { get; set; }
        public long TotalSize { get; set; }
        public List<DBMongo> DBs { get; set; }

        /// <summary>
        /// 服务器状态
        /// </summary>
        public bool IsOK { get; set; }

        /// <summary>
        /// 服务器名称
        /// </summary>
        public string Name
        {
            get
            {
                return string.Format("{0}:{1}", IP, Port);
            }
        }
    }

    /// <summary>
    /// 数据库信息
    /// </summary>
    public class DBMongo
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public Guid ID { get; set; }
        public ServerMongo Server { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public List<TableMongo> Tables { get; set; }
    }

    /// <summary>
    /// 表信息
    /// </summary>
    public class TableMongo
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public Guid ID { get; set; }
        public DBMongo DB { get; set; }
        public string Name { get; set; }
        public string Namespace { get; set; }
        public List<IndexMongo> Indexes { get; set; }
        public List<ColumnMongo> Columnes { get; set; }
    }


    /// <summary>
    /// 表列名信息
    /// </summary>
    public class ColumnMongo
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public Guid ID { get; set; }
        public TableMongo Table { get; set; }
        public string ColumnName { get; set; }
    }


    /// <summary>
    /// 索引器信息
    /// </summary>
    public class IndexMongo
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public Guid ID { get; set; }
        public TableMongo Table { get; set; }
        public string Name { get; set; }
        public List<IndexColumnMongo> IndexColumns { get; set; }
        public bool Unique { get; set; }
    }

    /// <summary>
    /// 索引列信息
    /// </summary>
    public class IndexColumnMongo
    {
        /// <summary>
        /// 唯一值
        /// </summary>
        public Guid ID { get; set; }
        public IndexMongo Index { get; set; }
    }

}