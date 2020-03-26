using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace Swift.Database.Model.DataAccess
{
    public abstract class DataBaseAccess
    {
        protected string ConnectionString { get; set; }

        public DataBaseAccess(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(ConnectionString);
            if (connection.State != ConnectionState.Open)
                connection.Open();

            return connection;
        }

        private DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            SqlCommand command = new SqlCommand(commandText, connection as SqlConnection)
            {
                CommandType = commandType
            };

            return command;
        }

        protected SqlParameter GetParameter(string parameter, object value)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, value ?? DBNull.Value)
            {
                Direction = ParameterDirection.Input
            };

            return parameterObject;
        }

        protected SqlParameter GetParameter(string parameter, SqlDbType type, int size, object value, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, type, size)
            {
                Value = value ?? DBNull.Value,
                Direction = parameterDirection
            };

            return parameterObject;
        }

        protected SqlParameter GetParameter(string parameter, SqlDbType type, int size, object value, byte precision, byte scale, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            SqlParameter parameterObject = new SqlParameter(parameter, type, size)
            {
                Precision = precision,
                Scale = scale,
                Value = value ?? DBNull.Value,
                Direction = parameterDirection,
            };

            return parameterObject;
        }

        protected int ExecuteNonQuery(string procedureNameOrQuery, List<DbParameter> parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            int returnValue = 0;

            using (SqlConnection connection = GetConnection())
            {
                using (DbCommand cmd = GetCommand(connection, procedureNameOrQuery, commandType))
                {

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    returnValue = cmd.ExecuteNonQuery();
                }
            }

            return returnValue;
        }

        protected object ExecuteScalar(string procedureNameOrQuery, List<SqlParameter> parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            object returnValue = null;

            using (DbConnection connection = GetConnection())
            {
                using (DbCommand cmd = GetCommand(connection, procedureNameOrQuery, commandType))
                {

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    returnValue = cmd.ExecuteScalar();
                }
            }

            return returnValue;
        }

        protected DbDataReader ExecuteDataReader(string procedureNameOrQuery, List<DbParameter> parameters = null, CommandType commandType = CommandType.StoredProcedure)
        {
            DbDataReader dr;

            DbConnection connection = GetConnection();
            DbCommand cmd = GetCommand(connection, procedureNameOrQuery, commandType);
            if (parameters != null && parameters.Count > 0)
            {
                cmd.Parameters.AddRange(parameters.ToArray());
            }

            dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

            return dr;
        }
    }
}
