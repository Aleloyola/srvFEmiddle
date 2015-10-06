using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Odbc;
using comBusinessBE.BD;

namespace srvFEmiddle.BD
{
    public class ooConnectionBD: IConnectionBD
    {
        public string sDSNServer { get; set; }
        public string sUser { get; set; }
        public string sPassword { get; set; }
        public string sTable { get; set; }
        private string sConnection { 
            get { return "DSN=" + sDSNServer + ";UID=" + sUser + ";PWD=" + sPassword; }
        }

        public string select()
        {
            //Test method for connection issues.
            StringBuilder sb = new StringBuilder();
            OdbcConnection conn = new OdbcConnection();
            conn.ConnectionString = this.sConnection;
            try
            {
                conn.Open();
                string sQuery = "SELECT * FROM " + sTable;
                using (OdbcCommand com = new OdbcCommand(sQuery, conn))
                {
                    using (OdbcDataReader reader = com.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string word = reader.GetString(0);
                            sb.Append(word);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.Append(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return sb.ToString();
        }

        public bool existDoc(string sSIIDOC, string sSIINUM)
        {
            bool bResp = false;
            OdbcConnection conn = new OdbcConnection();
            conn.ConnectionString = this.sConnection;
            try
            {
                conn.Open();
                string sQuery = "SELECT 1 FROM " + sTable + "WHERE SIIDOC=@val1 WHERE SIINUM=@val2";
                using (OdbcCommand cmd = new OdbcCommand(sQuery, conn))
                {
                    cmd.Parameters.Add("@val1", OdbcType.VarChar).Value = sSIIDOC;
                    cmd.Parameters.Add("@val2", OdbcType.VarChar).Value = sSIINUM;
                    using (OdbcDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string sResp = reader.GetString(0);
                            if (sResp.Substring(0) == "1") bResp = true;
                            else bResp = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bResp = false;
            }
            finally
            {
                conn.Close();
            }
            return bResp;
        }

        public bool updateOK( string sSIIDOC, string sSIINUM )
        {
            bool bResp = true;
            OdbcConnection conn = new OdbcConnection(this.sConnection);
            try
            {
                conn.Open();
                OdbcCommand cmd = new OdbcCommand("UPDATE " + this.sTable + " SET SIISTS=@valOk WHERE SIIDOC=@val1 WHERE SIINUM=@val2", conn);
                cmd.Parameters.Add("@valOk", OdbcType.VarChar).Value = "OK";
                cmd.Parameters.Add("@val1", OdbcType.VarChar).Value = sSIIDOC;
                cmd.Parameters.Add("@val2", OdbcType.VarChar).Value = sSIINUM;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                bResp = false;
            }
            finally
            {
                conn.Close();
            }
            return bResp;
        }

        public bool updateError(string sSIIDOC, string sSIINUM, string sObservation)
        {
            bool bResp = true;
            OdbcConnection conn = new OdbcConnection(this.sConnection);
            try
            {
                conn.Open();
                OdbcCommand cmd = new OdbcCommand("UPDATE " + this.sTable + " SET SIISTS=@valEr SIIOBS=@valOb WHERE SIIDOC=@val1 WHERE SIINUM=@val2", conn);
                cmd.Parameters.Add("@valEr", OdbcType.VarChar).Value = "ERR";
                cmd.Parameters.Add("@val1", OdbcType.VarChar).Value = sSIIDOC;
                cmd.Parameters.Add("@val2", OdbcType.VarChar).Value = sSIINUM;
                cmd.Parameters.Add("@valOb", OdbcType.VarChar).Value = sObservation;

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                bResp = false;
            }
            finally
            {
                conn.Close();
            }
            return bResp;
        }

    }
}
