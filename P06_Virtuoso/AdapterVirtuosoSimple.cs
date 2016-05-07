using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using OpenLink.Data.Virtuoso;
using Polar.Common;

namespace P06_Virtuoso
{
    public class AdapterVirtuosoSimple
    {
        private string graph = "http://fogid.net/t/";

        private VirtuosoConnection connection = null;
        public AdapterVirtuosoSimple(string connectionstring, string graph)
        {
            this.connection = new OpenLink.Data.Virtuoso.VirtuosoConnection(connectionstring);
            this.graph = graph;
        }
        public int Execute(string sparql)
        {
            if (connection.State == System.Data.ConnectionState.Open) connection.Close();
            connection.Open();
            var command = connection.CreateCommand();
            //var db = connection.Database;
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sparql;
            int val = command.ExecuteNonQuery();
            connection.Close();
            return val;
        }
        public object ExecuteScalar(string sparql)
        {
            if (connection.State == System.Data.ConnectionState.Open) connection.Close();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sparql;
            object val = command.ExecuteScalar();
            connection.Close();
            return val;
        }
        public IEnumerable<object[]> Query(string sparql)
        {
            if (connection.State == System.Data.ConnectionState.Open) connection.Close();
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandType = System.Data.CommandType.Text;
            command.CommandText = sparql;
            var reader = command.ExecuteReader();

            int ncols = reader.FieldCount;
            object[] data = new object[ncols];
            while (reader.Read())
            {
                reader.GetValues(data);
                yield return data;
            }
            reader.Close();
            connection.Close();
        }
        // Начальная и конечная "скобки" транзакции. В серединке должны использоваться команды ТОЛЬКО на основе команды runcommand
        public VirtuosoCommand RunStart()
        {
            if (connection.State == System.Data.ConnectionState.Open) connection.Close();
            connection.Open();
            VirtuosoCommand runcommand = connection.CreateCommand();
            runcommand.CommandType = System.Data.CommandType.Text;
            var transaction = connection.BeginTransaction();
            runcommand.Transaction = transaction;
            return runcommand;
        }
        public void RunStop(VirtuosoCommand runcommand)
        {
            runcommand.Transaction.Commit();
            connection.Close();
        }
        // Используется для организации запросов внутри транзакции
        public IEnumerable<object[]> RunQuery(string sql, VirtuosoCommand runcommand)
        {
            runcommand.CommandText = sql;
            var reader = runcommand.ExecuteReader();

            int ncols = reader.FieldCount;
            object[] data = new object[ncols];
            while (reader.Read())
            {
                reader.GetValues(data);
                yield return data;
            }
            reader.Close();
        }
        private BufferForInsertCommand buffer;
        public void PrepareToLoad()
        {
            Execute("SPARQL CLEAR GRAPH <" + graph + ">");
        }
        public void Load(IEnumerable<XElement> xtriples)
        {
            BufferForInsertCommand buffer = new BufferForInsertCommand(this, this.graph);
            buffer.InitBuffer();
            foreach (var xtriple in xtriples)
            {
                string ty = xtriple.Name.LocalName;
                string id = ty + xtriple.Attribute("id").Value;
                buffer.AddCommandToBuffer("<"+ id + "> a <" + ty + ">"); // Тип
                foreach (XElement xprop in xtriple.Elements())
                {
                    string prop = xprop.Name.LocalName;
                    XAttribute oobj = xprop.Attribute("ref");
                    if (oobj != null) // Object Property
                    {
                        string nid = oobj.Name.LocalName;
                        string obj_id = prop == "reflected" ? "person" + nid : "photo_doc" + nid; 
                        buffer.AddCommandToBuffer("<" + id + "> <" + prop + "> <" + obj_id + ">");
                    }
                    else
                    {
                        buffer.AddCommandToBuffer("<" + id + "> <" + prop + "> \"" + xprop.Value + "\"");
                    }
                }
            }
            buffer.FlushBuffer();
        }

        public IEnumerable<object[]> GetReflections(int code, VirtuosoCommand runcommand)
        {
            string sparql = @"SPARQL SELECT ?doc WHERE {?reflection <reflected> <person" + code + "> . ?reflection <in_doc> ?doc . }";
            return RunQuery(sparql, runcommand);
        }

    }

    public class BufferForInsertCommand
    {
        private AdapterVirtuosoSimple engine;
        private string graph;
        public BufferForInsertCommand(AdapterVirtuosoSimple engine, string graph)
        {
            this.engine = engine;
            this.graph = graph;
        }
        private BufferredProcessing<string> b_entities;
        public void InitBuffer()
        {
            // размер буфера
            int bufferportion = 1000;
            // размер порции для внедрения данных
            int portion = 20;

            b_entities = new BufferredProcessing<string>(bufferportion, flow =>
            {
                var query = flow.Select((ent, i) => new { e = ent, i = i }).GroupBy(ei => ei.i / portion, ei => ei);

                VirtuosoCommand trcommand = engine.RunStart();
                foreach (var q in query)
                {
                    string data = q.Select(ei => ei.e + " . ")
                        .Aggregate((sum, s) => sum + " " + s);
                    //bool found = q.Any(ei => ei.e.s == "Gury_Marchuk");
                    trcommand.CommandText = "SPARQL INSERT INTO GRAPH <" + graph + "> {" + data + "}\n";
                    try
                    {
                        trcommand.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                engine.RunStop(trcommand);
            });
        }
        private static int cnt = 0;
        public void AddCommandToBuffer(string comm)
        {
            b_entities.Add(comm);
            cnt++;
            if (cnt % 10000 == 0) Console.Write("{0} ", cnt);
        }
        public void FlushBuffer()
        {
            b_entities.Flush();
            Console.WriteLine("{0} ", cnt);
        }
    }

}
