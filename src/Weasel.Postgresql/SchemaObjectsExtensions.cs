using System.IO;
using System.Threading.Tasks;
using Baseline;
using Npgsql;

namespace Weasel.Postgresql
{
    public static class SchemaObjectsExtensions
    {
        internal static string ToIndexName(this DbObjectName name, string prefix, params string[] columnNames)
        {
            return $"{prefix}_{name.Name}_{columnNames.Join("_")}";
        }
        
        public static Task<SchemaPatch> CreatePatch(this ISchemaObject schemaObject, NpgsqlConnection conn)
        {
            var patch = new SchemaPatch(new DdlRules());
            patch.Apply(conn, AutoCreate.All, schemaObject);

            return Task.FromResult(patch);
        }

        public static Task Drop(this ISchemaObject schemaObject, NpgsqlConnection conn)
        {
            var writer = new StringWriter();
            schemaObject.WriteDropStatement(new DdlRules(), writer);

            return conn.CreateCommand(writer.ToString()).ExecuteNonQueryAsync();
        }

        public static Task Create(this ISchemaObject schemaObject, NpgsqlConnection conn)
        {
            var writer = new StringWriter();
            schemaObject.Write(new DdlRules(), writer);
            
            return conn.CreateCommand(writer.ToString()).ExecuteNonQueryAsync();
        }
    }
}