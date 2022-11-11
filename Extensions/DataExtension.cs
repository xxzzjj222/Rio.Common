using Rio.Common;
using Rio.Common.Helpers;
using Rio.Common.Logging;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;

namespace Rio.Extensions;

public static partial class DataExtension
{
    public sealed class DbParameterReadOnlyCollection : IReadOnlyCollection<DbParameter>
    {
        private readonly DbParameterCollection _paramCollection;

        public DbParameterReadOnlyCollection(DbParameterCollection parameterCollection)
        {
            _paramCollection = parameterCollection;
        }
        public int Count => _paramCollection.Count;

        public IEnumerator<DbParameter> GetEnumerator()
        {
            return (IEnumerator<DbParameter>)_paramCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    private static DbParameterReadOnlyCollection GetReadOnlyCollection(this DbParameterCollection collection) => new(collection);

    public static Func<DbCommand, string> CommandLogFormatterFunc = command => $"DbCommand log: CommandText:{command.CommandText},CommandType:{command.CommandType},Parameters:{command.Parameters.GetReadOnlyCollection().Select(p => $"{p.ParameterName}={p.Value}").StringJoin(",")},CommandTimeout:{command.CommandTimeout}s";

    public static Action<string>? CommandLogAction = log => Debug.WriteLine(log);

    #region DataTable

    public static DataTable ToDataTable<T>(this IEnumerable<T> entities)
    {
        if(null==entities)
        {
            throw new ArgumentNullException(nameof(entities));
        }
        var properties = CacheUtil.GetTypeProperties(typeof(T)).Where(_ => _.CanRead).ToArray();

        var dataTable = new DataTable();

        dataTable.Columns.AddRange(properties.Select(p => new DataColumn(p.Name, p.PropertyType)).ToArray());

        foreach(var item in entities)
        {
            var row=dataTable.NewRow();
            foreach(var property in properties)
            {
                row[property.Name] = property.GetValueGetter<T>()?.Invoke(item);
            }
            dataTable.Rows.Add(row);
        }

        return dataTable;
    }

    private static object? GetValueFromDbValue(this object? obj)
    {
        if (obj == null || obj == DBNull.Value)
        {
            return null;
        }
        return obj;
    }

    public static IEnumerable<T> ToEntities<T>(DataTable @this)
    {
        if(@this.Columns.Count>0)
        {
            if(typeof(T).IsBasicType())
            {
                foreach(DataRow row in @this.Rows)
                {
                    yield return row[0].To<T>();
                }
            }
            else
            {
                foreach(DataRow dr in @this.Rows)
                {
                    yield return dr.ToEntity<T>();
                }
            }
        }
    }

    public static IEnumerable<dynamic> ToExpandoObjects(this DataTable @this)
    {
        foreach (DataRow dr in @this.Rows)
        {
            dynamic entity = new ExpandoObject();
            var expandoDict = (IDictionary<string, object>)entity;

            foreach(DataColumn column in @this.Columns)
            {
                expandoDict.Add(column.ColumnName, dr[column]);
            }

            yield return entity;
        }
    }

    public static IEnumerable<T?> ColumnToList<T>(this DataTable dataTable,int index)
    {
        Guard.NotNull(dataTable, nameof(dataTable));
        if(dataTable.Rows.Count>index)
        {
            foreach(DataRow row in dataTable.Rows)
            {
                yield return row[index].ToOrDefault<T>();
            }
        }
    }

    #endregion DataTable

    #region DataRow

    public static T ToEntity<T>(this DataRow dr)
    {
        var type = typeof(T);
        var propertites = CacheUtil.GetTypeProperties(type).Where(p => p.CanWrite).ToArray();

        var entity = NewFuncHelper<T>.Instance()!;

        if (type.IsValueType)
        {
            object obj = entity;
            foreach(var property in propertites)
            {
                if(dr.Table.Columns.Contains(property.Name))
                {
                    property.GetValueSetter()?.Invoke(obj,dr[property.Name].GetValueFromDbValue());
                }
            }
            entity = (T)obj;
        }
        else
        {
            foreach (var property in propertites)
            {
                if(dr.Table.Columns.Contains(property.Name))
                {
                    property.GetValueSetter()?.Invoke(entity, dr[property.Name].GetValueFromDbValue());
                }
            }
        }
        return entity;
    }

    public static dynamic ToExpandoObject(this DataRow @this)
    {
        dynamic entity = new ExpandoObject();
        var expandoDict = (IDictionary<string, object>)entity;

        foreach(DataColumn column in @this.Table.Columns)
        {
            expandoDict.Add(column.ColumnName,@this[column]);
        }

        return expandoDict;
    }

    #endregion

    #region IDataReader

    public static DataTable ToDataTable(this IDataReader @this)
    {
        var dt = new DataTable();
        dt.Load(@this);
        return dt;
    }

    public static IEnumerable<T> ToEntities<T>(this IDataReader @this)
    {
        var type=typeof(T);

        if(type.IsBasicType())
        {
            while(@this.Read())
            {
                yield return @this[0].To<T>();
            }
        }
        else
        {
            while(@this.Read())
            {
                yield return @this.ToEntity<T>(true)!;
            }
        }
    }

    public static T? ToEntity<T>(this IDataReader @this,bool hadRead=false)
    {
        if(!hadRead)
        {
            hadRead = @this.Read();
        }

        if (hadRead && @this.FieldCount > 0)
        {
            var type = typeof(T);
            if (type.IsBasicType())
            {
                return @this[0].ToOrDefault<T>();
            }

            var properties = CacheUtil.GetTypeProperties(type).Where(p => p.CanRead).ToArray();

            var entity = NewFuncHelper<T>.Instance()!;

            var dic = Enumerable.Range(0, @this.FieldCount).ToDictionary(_ => @this.GetName(_).ToUpper(), _ => @this[_].GetValueFromDbValue());

            try
            {
                if(type.IsValueType)
                {
                    var obj = (object)entity;
                    foreach(var property in properties)
                    {
                        if(dic.ContainsKey(property.Name.ToUpper()))
                        {
                            property.GetValueSetter()?.Invoke(obj, dic[property.Name.ToUpper()]);
                        }
                    }
                    entity = (T)obj;
                }
                else
                {
                    foreach(var property in properties)
                    {
                        if(dic.ContainsKey(property.Name.ToUpper()))
                        {
                            property.GetValueSetter()?.Invoke(entity, dic[property.Name.ToUpper()]);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Common.Helpers.LogHelper.GetLogger(typeof(DataExtension)).Error(e);
                throw;
            }
        }

        return default;
    }

    public static dynamic ToExpandoObject(this IDataReader @this,bool hadRead=false)
    {
        dynamic entity = new ExpandoObject();
        if(!hadRead)
        {
            hadRead = @this.Read();
        }

        if(hadRead && @this.FieldCount>0)
        {
            var expandoDict = (IDictionary<string, object>)entity;
            var columnNames = Enumerable.Range(0, @this.FieldCount)
                .Select(x => new KeyValuePair<int, string>(x, @this.GetName(x)))
                .ToDictionary(pair => pair.Key);

            Enumerable.Range(0,@this.FieldCount)
                .ToList()
                .ForEach(x=>expandoDict.Add(columnNames[x].Value,@this[x]));
        }

        return entity;
    }

    public static IEnumerable<dynamic> ToExpandoObjects(this IDataReader @this)
    {
        var columnNames = Enumerable.Range(0, @this.FieldCount)
            .Select(x => new KeyValuePair<int, string>(x, @this.GetName(x)))
            .ToDictionary(pair => pair.Key);

        while(@this.Read())
        { 
            dynamic entity=new ExpandoObject();
            if(@this.FieldCount>0)
            {
                var expandoDict=(IDictionary<string,object>)entity;

                Enumerable.Range(0,@this.FieldCount)
                    .ToList()
                    .ForEach(x=>expandoDict.Add(columnNames[x].Value,@this[x]));
            }

            yield return entity;
        }
    }

    #endregion IDataReader

    #region IDbConnection

    public static void EnsureOpen(this IDbConnection connection)
    {
        if(connection.State == ConnectionState.Closed)
        {
            connection.Open();
        }
    }

    public static bool IsConnectionOpen(this IDbConnection connection)
    {
        return connection.State == ConnectionState.Open;
    }

    #endregion IDbConnection

    #region DbConnection

    public static async Task EnsureOpenAsync(this DbConnection conn)
    {
        if(conn.State==ConnectionState.Closed)
        {
            await conn.OpenAsync().ConfigureAwait(false);
        }
    }

    #endregion DbConnection

    #region DbCommand

    public static dynamic ExecuteExpandoObject(this DbCommand @this)
    {
        using IDataReader reader = @this.ExecuteReader();
        return reader.ToExpandoObject();
    }

    public static IEnumerable<dynamic> ExecuteExpandoObjects(this DbCommand @this)
    {
        using IDataReader reader = @this.ExecuteReader();
        return reader.ToExpandoObjects();
    }

    public static T ExecuteDataTable<T>(this DbCommand @this, Func<DataTable, T> func) => func(@this.ExecuteDataTable());

    public static async Task<T> ExecuteDataTableAsyn<T>(this DbCommand @this,Func<DataTable,Task<T>> func,CancellationToken cancellationToken=default)
    {
        var dataTable = await @this.ExecuteDataTableAsync(cancellationToken);
        var result = await func(dataTable);
        return result;
    }

    public static T ExecuteScalarTo<T>(this DbCommand @this) => @this.ExecuteScalar().To<T>();

    public static async Task<T> ExecuteScalarToAsync<T>(this DbCommand @this, CancellationToken cancellationToken = default) => (await @this.ExecuteScalarAsync(cancellationToken)).To<T>();

    public static T? ExecuteScalarToOrDefault<T>(this DbCommand @this) => @this.ExecuteScalar().ToOrDefault<T>();

    public static async Task<T?> ExecuteScalarToOrDefaultAsync<T>(this DbCommand @this, CancellationToken cancellationToken = default) => (await @this.ExecuteScalarAsync(cancellationToken)).ToOrDefault<T>();

    public static T? ExecuteScalarTo<T>(this DbCommand @this,Func<object?,T> func)
    {
        return func(@this.ExecuteScalar());
    }

    private static DbCommand GetDbCommand(this DbConnection conn,string cmdText,CommandType commandType=CommandType.Text,object? paramInfo=null,DbParameter[]? parameters=null,DbTransaction? transaction=null,int commandTimeout=60)
    {
        conn.EnsureOpen();
        var command = conn.CreateCommand();

        command.CommandText = cmdText;
        command.CommandType = commandType;
        command.Transaction = transaction;
        command.CommandTimeout = commandTimeout;

        if (parameters != null)
        {
            command.Parameters.AddRange(parameters);
        }
        command.AttachDbParameters(paramInfo);

        commnad
    }

    #endregion DbCommand

    #region DbParameter

    public static bool ContainsParam(this DbParameterCollection @this,string paramName)
    {
        var originName = GetParameterName(paramName);
        return @this.Contains(originName)
            || @this.Contains("@" + originName)
            || @this.Contains("?" + originName);
    }

    public static void AttachDbParameters(this DbCommand command,object? paramInfo)
    {
        if(paramInfo!=null)
        {
            if(!(paramInfo is IDictionary<string,object?> parameters))
            {
                if (paramInfo.IsValueTuple()) //Tuple
                {
                    parameters = paramInfo.GetFields().ToDictionary(f => f.Name, f => f?.GetValue(paramInfo));
                }
                else // get properties
                {
                    parameters = CacheUtil.GetTypeProperties(paramInfo.GetType()).ToDictionary(x => x.Name, x => x.GetValueGetter()?.Invoke(paramInfo));
                }
            }
            foreach (var parameter in parameters)
            {
                var param = command.CreateParameter();
                param.ParameterName = GetParameterName(parameter.Key);
                param.Value = parameter.Value ?? DBNull.Value;
                param.DbType = param.Value?.GetType().ToDbType() ?? DbType.String;
                command.Parameters.Add(param);
            }
        }
        
    }

    private static string GetParameterName(string originName)
    {
        if(!string.IsNullOrEmpty(originName))
        {
            switch(originName[0])
            {
                case '@':
                case ':':
                case '?':
                    return originName.Substring(1);
            }
        }
        return originName;
    }

    public static readonly Dictionary<Type, DbType> TypeMap = new()
    {
        [typeof(byte)] = DbType.Byte,
        [typeof(sbyte)] = DbType.SByte,
        [typeof(short)] = DbType.Int16,
        [typeof(ushort)] = DbType.UInt16,
        [typeof(int)] = DbType.Int32,
        [typeof(uint)] = DbType.UInt32,
        [typeof(long)] = DbType.Int64,
        [typeof(ulong)] = DbType.UInt64,
        [typeof(float)] = DbType.Single,
        [typeof(double)] = DbType.Double,
        [typeof(decimal)] = DbType.Decimal,
        [typeof(bool)] = DbType.Boolean,
        [typeof(string)] = DbType.String,
        [typeof(char)] = DbType.StringFixedLength,
        [typeof(Guid)] = DbType.Guid,
        [typeof(DateTime)] = DbType.DateTime2,
        [typeof(DateTimeOffset)] = DbType.DateTimeOffset,
        [typeof(TimeSpan)] = DbType.Time,
        [typeof(byte[])] = DbType.Binary,
        [typeof(object)] = DbType.Object,
#if NET6_0_OR_GREATER
        [typeof(DateOnly)] = DbType.Date,
        [typeof(TimeOnly)] = DbType.Time,
#endif
    };

    public static DbType ToDbType(this Type type)
    {
        if(type.IsEnum() && !TypeMap.ContainsKey(type))
        {
            type = Enum.GetUnderlyingType(type);
        }
        if(TypeMap.TryGetValue(type.Unwrap(),out var dbType))
        {
            return dbType;
        }
        if(type.FullName=="System.Data.Linq.Binary")
        {
            return DbType.Binary;
        }
        return DbType.Object;
    }

#endregion DbParameter
}

