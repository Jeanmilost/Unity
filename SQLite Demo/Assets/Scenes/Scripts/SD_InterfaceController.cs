using System.Data;
using UnityEngine;
using UnityEngine.UI;
using Mono.Data.Sqlite;

/**
* Provides an interface controller
*@author Jean-Milost Reymond
*/
public class SD_InterfaceController : MonoBehaviour
{
    /**
    * Text instance
    */
    public class IText
    {
        /**
        * Gets the text object
        */
        public GameObject Object { get; private set; }

        /**
        * Gets the text instance
        */
        public Text Instance { get; private set; }

        /**
        * Initializes the text
        *@param name - the button name
        */
        public void Init(string name)
        {
            // get the text object
            Object = GameObject.Find(name);
            Debug.Assert(Object);

            // get the text instance
            Instance = Object.GetComponent<Text>();
            Debug.Assert(Instance);
        }
    }

    /**
    * Button instance
    */
    public class IButton
    {
        /**
        * Gets the button object
        */
        public GameObject Object { get; private set; }

        /**
        * Gets the button instance
        */
        public Button Instance { get; private set; }

        /**
        * Gets the button caption
        */
        public Text Caption { get; private set; }

        /**
        * Gets the button controller
        */
        public SD_ButtonController Controller { get; private set; }

        /**
        * Initializes the button
        *@param name - the button name
        */
        public void Init(string name)
        {
            // get the button object
            Object = GameObject.Find(name);
            Debug.Assert(Object);

            // get the button instance
            Instance = Object.GetComponent<Button>();
            Debug.Assert(Instance);

            // get the button caption
            Caption = Object.GetComponentInChildren<Text>();
            Debug.Assert(Caption);

            // get the button controller
            Controller = Object.GetComponent<SD_ButtonController>();
            Debug.Assert(Controller);
        }
    }

    /**
    * Edit instance
    */
    public class IEdit
    {
        /**
        * Gets the edit object
        */
        public GameObject Object { get; private set; }

        /**
        * Gets the edit input field
        */
        public InputField Instance { get; private set; }

        /**
        * Initializes the edit
        *@param name - the button name
        */
        public void Init(string name)
        {
            // get the edit object
            Object = GameObject.Find(name);
            Debug.Assert(Object);

            // get the edit input field
            Instance = Object.GetComponent<InputField>();
            Debug.Assert(Instance);
        }

        /**
        * Clears the edit content
        */
        public void Clear()
        {
            if (Instance == null)
                return;

            Instance.text = "";
        }
    }

    /**
    * Scroll area instance
    */
    public class IScrollArea
    {
        /**
        * Gets the scroll area object
        */
        public GameObject Object { get; private set; }

        /**
        * Gets the scroll area instance
        */
        public ScrollRect Instance { get; private set; }

        /**
        * Initializes the scroll area
        *@param name - the scroll area name
        */
        public void Init(string name)
        {
            // get the edit object
            Object = GameObject.Find(name);
            Debug.Assert(Object);

            // get the edit input field
            Instance = Object.GetComponent<ScrollRect>();
            Debug.Assert(Instance);
        }
    }

    private CS_SQLiteHelper m_SQLiteHelper     = new CS_SQLiteHelper();
    private IScrollArea     m_ScrollArea       = new IScrollArea();
    private IEdit           m_QueryEdit        = new IEdit();
    private IButton         m_OpenConnection   = new IButton();
    private IButton         m_ExecuteQuery     = new IButton();
    private IText           m_ConnectionStatus = new IText();
    private IText           m_DbFileName       = new IText();
    private IText           m_QueryOutput      = new IText();
    private Color           m_ErrorColor       = new Color(0.9173f, 0.298f, 0.298f);
    private Color           m_SuccessColor     = new Color(0.2039f, 0.7254f, 0.2431f);

    /**
    * Starts the script
    */
    void Start()
    {
        // get the scroll area
        m_ScrollArea.Init("Scroll View");

        // get the query edit component
        m_QueryEdit.Init("QueryEdit");

        // get the open connection button
        m_OpenConnection.Init("OpenConnectionBtn");
        m_OpenConnection.Controller.OnButtonClicked = OnOpenConnectionClick;

        // get the execute query button
        m_ExecuteQuery.Init("ExecuteQueryBtn");
        m_ExecuteQuery.Controller.OnButtonClicked = OnExecuteQueryClick;

        // get the connection status
        m_ConnectionStatus.Init("ConnectionStatus");

        // get the database file name
        m_DbFileName.Init("DbFile");

        // get the query output
        m_QueryOutput.Init("QueryOutput");

        // disable the interface by default, as the database is still not opened
        EnableInterface(false);
    }

    /**
    * Enables or disables the interface
    *@param enabled - if true, the interface is enabled, otherwise disabled
    */
    void EnableInterface(bool enabled)
    {
        m_QueryEdit.Instance.interactable    = enabled;
        m_ExecuteQuery.Instance.interactable = enabled;
    }

    /**
    * Logs a line to the output console
    *@param line - the line to log
    */
    void Log(string line)
    {
        m_QueryOutput.Instance.text += line + "\n";
    }

    /**
    * Logs the content of a query to the output console
    *@param dataReader - the data reader containing the query to log
    */
    void LogQueryResult(SqliteDataReader dataReader)
    {
        // no data reader?
        if (dataReader == null)
            return;

        // query contains no rows?
        if (!dataReader.HasRows)
            return;

        // iterate through data content
        while (dataReader.Read())
        {
            string line = "";

            // iterate through cells
            for (int i = 0; i < dataReader.FieldCount; ++i)
            {
                if (line.Length != 0)
                    line += " - ";

                // get the cell data type
                string dataTypeName = dataReader.GetDataTypeName(i);

                // get the cell data and add it to log line
                if (dataTypeName == "TEXT")
                    line += dataReader.GetString(i);
                else
                if (dataTypeName == "INTEGER")
                    line += dataReader.GetInt32(i);
                else
                if (dataTypeName == "REAL")
                    line += dataReader.GetDouble(i);
                else
                if (dataTypeName == "NUMERIC")
                    line += dataReader.GetDecimal(i);
                else
                if (dataTypeName == "BLOB")
                    line += "BLOB " + i;
                else
                    line += "UNKNOWN";
            }

            // log the line
            Log(line);
        }
    }

    /**
    * Called when the open/close connection button is clicked
    *@param sender - event sender
    */
    void OnOpenConnectionClick(object sender)
    {
        // is database opened?
        if (m_SQLiteHelper.IsConnectionOpened())
        {
            // close the database
            m_SQLiteHelper.CloseConnection();

            // clear the query edit
            m_QueryEdit.Clear();

            // disable the interface
            EnableInterface(false);

            // update the status
            m_ConnectionStatus.Instance.text  = "Closed";
            m_ConnectionStatus.Instance.color = m_ErrorColor;

            // hide the file
            m_DbFileName.Instance.text = "";

            // switch to open button
            m_OpenConnection.Caption.text = "Open connection";

            return;
        }

        // open the databse file
        if (m_SQLiteHelper.OpenConnection("SqliteDemo"))
        {
            // succeeded, update the status
            m_ConnectionStatus.Instance.text  = "Opened";
            m_ConnectionStatus.Instance.color = m_SuccessColor;

            // show the file
            m_DbFileName.Instance.text = Application.persistentDataPath + "/SqliteDemo.db";

            // switch to close button
            m_OpenConnection.Caption.text = "Close connection";

            // enable the interface
            EnableInterface(true);
        }
        else
        {
            // clear the query edit
            m_QueryEdit.Instance.textComponent.text = "";

            // disable the interface
            EnableInterface(false);

            // failed, update the status
            m_ConnectionStatus.Instance.text  = "Error";
            m_ConnectionStatus.Instance.color = m_ErrorColor;

            // hide the file
            m_DbFileName.Instance.text = "";
        }
    }

    /**
    * Called when the executer query button is clicked
    *@param sender - event sender
    */
    void OnExecuteQueryClick(object sender)
    {
        // begin the transaction
        m_SQLiteHelper.BeginTransaction();

        string error;

        // execute the query
        SqliteDataReader dataReader = m_SQLiteHelper.ExecuteQuery(m_QueryEdit.Instance.text, out error) as SqliteDataReader;

        // succeeded?
        if (dataReader == null)
        {
            // log the error
            Log("Query FAILED - error occurred:\n" + error);

            // end the transaction and rollback changes from database
            m_SQLiteHelper.EndTransaction(true);
            return;
        }
        else
        {
            // log the result
            Log("Query SUCCEEDED:\n" + m_QueryEdit.Instance.text);
            LogQueryResult(dataReader);
        }

        // close the query
        dataReader.Close();

        // end the transaction and commit changes to database
        m_SQLiteHelper.EndTransaction(false);
    }
}
