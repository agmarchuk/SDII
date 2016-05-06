using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace P03_SQL_Phototeka
{
    class SQLdatabase : ISimpleAccess
    {
        private DbConnection connection = null;
        public SQLdatabase(string connectionstring)
        {
            string dataprovider = "System.Data.SqlClient";
            DbProviderFactory factory = DbProviderFactories.GetFactory(dataprovider);
            connection = factory.CreateConnection();
            connection.ConnectionString = connectionstring;
        }
        public void PrepareToLoad() 
        {
            connection.Open();
            DbCommand comm = connection.CreateCommand();
            comm.CommandText = "DROP TABLE person; DROP TABLE photo_doc; DROP TABLE reflection;";
            string message = null;
            try { comm.ExecuteNonQuery(); } catch (Exception ex) { message = ex.Message; }
            comm.CommandText =
@"CREATE TABLE person (id INT NOT NULL, name NVARCHAR(400), age INT, PRIMARY KEY(id));
CREATE TABLE photo_doc (id INT NOT NULL, name NVARCHAR(400), PRIMARY KEY(id));
CREATE TABLE reflection (id INT NOT NULL, reflected INT NOT NULL, in_doc INT NOT NULL);";
            try { comm.ExecuteNonQuery(); }
            catch (Exception ex) { message = ex.Message; }
            connection.Close();
            if (message != null) Console.WriteLine(message);

        }

        public void MakeIndexes()
        {
            connection.Open();
            DbCommand comm = connection.CreateCommand();
            comm.CommandTimeout = 2000;
            comm.CommandText =
@"CREATE INDEX person_name ON person(name);
CREATE INDEX reflection_reflected ON reflection(reflected);
CREATE INDEX reflection_indoc ON reflection(in_doc);";
            try
            {
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            connection.Close();
        }

        public void LoadElementFlow(IEnumerable<XElement> element_flow)
        {
            DbCommand runcomm = RunStart();
            int i = 1;
            foreach (XElement element in element_flow)
            {
                if (i % 1000 == 0) Console.Write("{0} ", i / 1000); 
                i++;
                string table = element.Name.LocalName;
                string aaa = null;
                if (table == "person")
                    aaa = "(" + element.Attribute("id").Value + ", " +
                        "N'" + element.Element("name").Value.Replace('\'', '"') + "', " +
                        "" + element.Element("age").Value + ");";
                else if (table == "photo_doc")
                    aaa = "(" + element.Attribute("id").Value + ", " +
                        "N'" + element.Element("name").Value.Replace('\'', '"') + "'" +
                        ")";
                else if (table == "reflection")
                    aaa = "(" + element.Attribute("id").Value + ", " +
                        "" + element.Element("reflected").Attribute("ref").Value + ", " +
                        "" + element.Element("in_doc").Attribute("ref").Value + "" +
                        ")";
                runcomm.CommandText = "INSERT INTO " + table + " VALUES " + aaa + ";";
                runcomm.ExecuteNonQuery();
            }
            RunStop(runcomm);
            Console.WriteLine();
        }

        public long Count(string table)
        {
            connection.Open();
            var comm = connection.CreateCommand();
            comm.CommandTimeout = 1000;
            comm.CommandText = "SELECT COUNT(*) FROM " + table +";";
            var obj = comm.ExecuteScalar();
            //Console.WriteLine("Count()={0}", obj);
            connection.Close();
            return (long)obj;
        }
        public object[] GetById(int id, string table)
        {
            connection.Open();
            var comm = connection.CreateCommand();
            comm.CommandText = "SELECT * FROM " + table + " WHERE id=" + id + ";";
            object[] res = null;
            var reader = comm.ExecuteReader();
            while (reader.Read())
            {
                int ncols = reader.FieldCount;
                res = new object[ncols];
                for (int i = 0; i < ncols; i++) res[i] = reader.GetValue(i);
            }
            connection.Close();
            return res;
        }
        public IEnumerable<object[]> SearchByName(string searchstring, string table)
        {
            connection.Open();
            var comm = connection.CreateCommand();
            comm.CommandText = "SELECT * FROM " + table + " WHERE name LIKE '" + searchstring + "%'";
            var reader = comm.ExecuteReader();
            object[] row = new object[reader.FieldCount];
            while (reader.Read())
            {
                var qu = reader.GetValues(row);
                yield return row.Select(v => v).ToArray();
            }
            connection.Close();
        }
        public IEnumerable<object[]> GetPhotosOfPersonUsingRelation(int id)
        {
            connection.Open();
            var comm = connection.CreateCommand();
            comm.CommandText = "SELECT photo_doc.id,photo_doc.name FROM reflection INNER JOIN photo_doc ON reflection.in_doc=photo_doc.id WHERE reflection.reflected=" + id + ";";
            var reader = comm.ExecuteReader();
            object[] row = new object[reader.FieldCount];
            int cnt = 0;
            while (reader.Read())
            {
                cnt++;
                var c = reader.GetValues(row);
                yield return row.Select(v => v).ToArray();
            }
            connection.Close();
        }


        // Начальная и конечная "скобки" транзакции. В серединке должны использоваться SQL-команды ТОЛЬКО на основе команды runcommand
        private DbCommand RunStart()
        {
            if (connection.State == System.Data.ConnectionState.Open) connection.Close();
            connection.Open();
            DbCommand runcommand = connection.CreateCommand();
            runcommand.CommandType = System.Data.CommandType.Text;
            DbTransaction transaction = connection.BeginTransaction();
            runcommand.Transaction = transaction;
            runcommand.CommandTimeout = 10000;
            return runcommand;
        }
        private void RunStop(DbCommand runcommand)
        {
            runcommand.Transaction.Commit();
            connection.Close();
        }
    }
}
