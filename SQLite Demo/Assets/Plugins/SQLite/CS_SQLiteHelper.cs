using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

/**
* Sqlite helper
*@author Jean-Milost Reymond
*/
public class CS_SQLiteHelper
{
    private IDbConnection m_DbConnection;
    private IDbCommand    m_DbCmd;

    /**
    * Opens a connection to a SQLite database
    *@param databaseName - the database name
    *@return true on success, otherwise false
    */
    public bool OpenConnection(string databaseName)
    {
        try
        {
            // another connection was already opened?
            if (IsConnectionOpened())
                return false;

            // build the connection name
            string connectionName = "URI=file:" + Application.persistentDataPath + "/" + databaseName + ".db";

            // open a SQLite database, creates one if still not exists
            m_DbConnection = new SqliteConnection(connectionName);
            m_DbConnection.Open();

            // succeeded?
            if (!IsConnectionOpened())
                return false;

            // create an empty command
            m_DbCmd            = m_DbConnection.CreateCommand();
            m_DbCmd.Connection = m_DbConnection;

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return false;
    }

    /**
    * Closes a previously opened SQLite database connection
    */
    public void CloseConnection()
    {
        try
        {
            // connection was already closed?
            if (!IsConnectionOpened())
                return;

            m_DbConnection.Close();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    /**
    * Checks if a connection to a SQLite database is opened
    *@param databaseName - the database name
    *@return true on success, otherwise false
    */
    public bool IsConnectionOpened()
    {
        try
        {
            return (m_DbConnection != null && m_DbConnection.State != ConnectionState.Closed);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return false;
    }

    /**
    * Begins a transaction
    *@param addInCommand - if true, the transaction will be added in the internal command object
    *@return the transaction on success, otherwise null
    */
    public IDbTransaction BeginTransaction(bool addInCommand = true)
    {
        try
        {
            // another internal command transaction was already started?
            if (addInCommand && m_DbCmd.Transaction != null)
                return m_DbCmd.Transaction;

            IDbTransaction dbTransaction = m_DbConnection.BeginTransaction();

            if (dbTransaction == null)
                return null;

            // add the transaction in the internal command
            if (addInCommand)
                m_DbCmd.Transaction = dbTransaction;

            return dbTransaction;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return null;
    }

    /**
    * Ends a previously started transaction
    *@param rollBack - if true, the transaction will be rolled back, otherwise committed
    *@return true on success, otherwise false
    */
    public bool EndTransaction(bool rollback)
    {
        if (EndTransaction(m_DbCmd.Transaction, rollback))
        {
            m_DbCmd.Transaction = null;
            return true;
        }

        return false;
    }

    /**
    * Ends a previously started transaction
    *@param dbTransaction - the database transaction to end
    *@param rollBack - if true, the transaction will be rolled back, otherwise committed
    *@return true on success, otherwise false
    */
    public bool EndTransaction(IDbTransaction dbTransaction, bool rollback)
    {
        if (dbTransaction == null)
            return false;

        try
        {
            if (rollback)
                dbTransaction.Rollback();
            else
                dbTransaction.Commit();

            return true;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return false;
    }

    /**
    * Executes a command onto the database
    *@param query - the query to execute
    *@return the resulting query data reader on success, otherwise null
    */
    public IDataReader ExecuteNonQuery(string query)
    {
        try
        {
            // fill the command
            m_DbCmd.CommandText = query;

            // execute the command
            m_DbCmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        return null;
    }

    /**
    * Executes a query onto the database
    *@param query - the query to execute
    *@param[out] error - errortext
    *@return the resulting query data reader on success, otherwise null
    */
    public IDataReader ExecuteQuery(string query, out string error)
    {
        error = "";

        try
        {
            // fill the command
            m_DbCmd.CommandText = query;

            // execute the command and return the resulting data reader
            return m_DbCmd.ExecuteReader();
        }
        catch (Exception e)
        {
            error = e.ToString();
            Debug.Log(e);
        }

        return null;
    }

    /**
    * Closes a query
    */
    public void CloseQuery(IDataReader dataReader)
    {
        // no data reader?
        if (dataReader == null)
            return;

        // close the query
        dataReader.Close();
    }
}
