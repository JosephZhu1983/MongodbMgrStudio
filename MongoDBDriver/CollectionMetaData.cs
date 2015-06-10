using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Driver.Connections;

namespace MongoDB.Driver
{
    /// <summary>
    /// Lazily loaded meta data on the collection.
    /// </summary>
    public class CollectionMetaData
    {
        private string fullName;
        private string name;
        private Database db;

        public CollectionMetaData (string dbName, string name, Connection conn){
            this.fullName = dbName + "." + name;
            this.name = name;
            this.db = new Database (conn, dbName);
        }

        private Document options = null;
        public Document Options {
            get {
                if (options != null)
                    return options;
                Document doc = db["system.namespaces"].FindOne (new Document ().Append ("name", this.fullName));
                if (doc == null)
                    doc = new Document ();
                if (doc.Contains ("create"))
                    doc.Remove ("create");
                //Not sure why this is here.  The python driver has it.
                this.options = doc;
                return this.options;
            }
        }

        private bool gotIndexes = false;
        private Dictionary<string, Document> indexes = new Dictionary<string, Document> ();
        public Dictionary<string, Document> Indexes {
            get {
                if (gotIndexes)
                    return indexes;
                
                indexes.Clear ();
                
                ICursor docs = db["system.indexes"].Find (new Document ().Append ("ns", this.fullName));
                foreach (Document doc in docs.Documents) {
                    indexes.Add ((string)doc["name"], doc);
                }
                
                return indexes;
            }
        }

        public void CreateIndex (string name, Document fieldsAndDirections, bool unique){
            Document index = new Document ();
            index["name"] = name;
            index["ns"] = this.fullName;
            index["key"] = fieldsAndDirections;
            index["unique"] = unique;
            db["system.indexes"].Insert (index);
            this.refresh ();
        }
        #region 新添加的删除索引的方法

        public void CreateIndex(string name, Document fieldsAndDirections, bool unique, bool background, bool dropDups)
        {
            Document index = new Document();
            index["name"] = name;
            index["ns"] = this.fullName;
            index["key"] = fieldsAndDirections;
            index["unique"] = unique;
            index["background"] = background;
            index["dropDups"] = dropDups;
            db["system.indexes"].Insert(index);
            this.refresh();
        }
        protected string generateIndexName(Document fieldsAndDirections, bool unique, bool background, bool dropDups)
        {
            StringBuilder sb = new StringBuilder("_");
            foreach (string key in fieldsAndDirections.Keys)
            {
                sb.Append(key).Append("_");
            }
            if (unique)
                sb.Append("unique_");
            if (background)
                sb.Append("background_");
            if (dropDups)
                sb.Append("dropDups_");

            return sb.ToString();
        }

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="fieldsAndDirections"></param>
        /// <param name="unique">是否唯一</param>
        /// <param name="background">是否后台创建索引</param>
        /// <param name="dropDups">是否删除同名索引</param>
        public void CreateIndex(Document fieldsAndDirections, bool unique, bool background, bool dropDups)
        {
            string name = this.generateIndexName(fieldsAndDirections, unique, background, dropDups);
            this.CreateIndex(name, fieldsAndDirections, unique, background, dropDups);
        }

        #endregion

        public void CreateIndex (Document fieldsAndDirections, bool unique){
            string name = this.generateIndexName (fieldsAndDirections, unique);
            this.CreateIndex (name, fieldsAndDirections, unique);
        }

        public void DropIndex (string name){
            Document cmd = new Document ();
            cmd.Append ("deleteIndexes", this.name).Append ("index", name);
            db.SendCommand (cmd);
            this.refresh ();
        }

        public void Rename (string newName){
            if (string.IsNullOrEmpty (newName))
                throw new ArgumentException ("Name must not be null or empty", "newName");
            
            Document cmd = new Document ();
            cmd.Append ("renameCollection", fullName).Append ("to", db.Name + "." + newName);
            db.GetSisterDatabase ("admin").SendCommand (cmd);
            this.refresh ();
        }

        public void refresh (){
            indexes.Clear ();
            gotIndexes = false;
            options = null;
        }

        protected string generateIndexName (Document fieldsAndDirections, bool unique){
            StringBuilder sb = new StringBuilder ("_");
            foreach (string key in fieldsAndDirections.Keys) {
                sb.Append (key).Append ("_");
            }
            if (unique)
                sb.Append ("unique_");
            
            return sb.ToString ();
        }
        
    }
}
