using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Multi_IU_Check_Transaction
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thr = new Thread(()=>run());
            thr.Start();
        }

        private void run()
        {
            string constr = "Data Source=172.16.1.89;uid=secure;pwd=weishenme;database=multi_season";
            string Centralcmd = @"SELECT * FROM [dbo].[CP_infor];
                                  SELECT * FROM [dbo].[Deduction_IU];
                                  SELECT * FROM [dbo].[Total_IU];";
            DataSet ds = null;
            try
            {
                ds = SqlHelper.ExecuteDataset(constr,CommandType.Text, Centralcmd);
                LogClass.WriteLog("reading server...");
            }catch(SqlException sql)
            {
                LogClass.WriteLog($"DB error when read from server {sql.ToString()}");
            }

            foreach (DataRow dr1 in ds.Tables[0].Rows)
            {
                string CP = dr1["CP"].ToString();
                string ip = dr1["IP"].ToString();
                LogClass.WriteLog($"conecting car park {CP}...");
                string constring = "Data Source=" + ip + ";uid=sa;pwd=yzhh2007;database=" + CP + ";Connection Timeout=20";

                foreach(DataRow dr2 in ds.Tables[1].Rows)
                {
                    
                    string Deduction_IU = dr2["IU"].ToString();
                    string offset_time = dr2["exit_time"].ToString();
                    LogClass.WriteLog($"======IU={Deduction_IU},Entry_time={offset_time}======");
                    int i = 0;
                    foreach(DataRow dr3 in ds.Tables[2].Rows)
                    {
                        string list_IU = dr3["IU"].ToString();
                        string cmd = @"select * from movement_trans where (iu_tk_no=@IU and entry_time<@offset_time and exit_time>@offset_time) or (iu_tk_no=@IU and entry_time<@offset_time and exit_time is null) order by entry_time";
                        DataSet Count_ds = null;

                        SqlParameter[] para = new SqlParameter[]
                        {
                            new SqlParameter("@IU",list_IU),
                             new SqlParameter("@offset_time",offset_time)
                        };
                        try
                        {
                            Count_ds = SqlHelper.ExecuteDataset(constring,CommandType.Text,cmd,para);
                        }
                        catch (SqlException sql)
                        {
                            LogClass.WriteLog($"reading fail {sql.ToString()}");
                        }

                        if (Count_ds != null)
                        {
                            if (Count_ds.Tables[0].Rows.Count >= 1)
                            {
                                foreach(DataRow ddr in Count_ds.Tables[0].Rows)
                                {
                                    i++;
                                    string entry_time = ddr["entry_time"].ToString();
                                    string exit_time = ddr["exit_time"].ToString();
                                    LogClass.WriteLog($"IU={list_IU} is found inside car park,entry_time={entry_time},exit_time={exit_time}");
                                }
                                //i++;
                                //string entry_time = Count_ds.Tables[0].Rows[0]["entry_time"].ToString();
                                //string exit_time= Count_ds.Tables[0].Rows[0]["exit_time"].ToString();
                                //LogClass.WriteLog($"IU={list_IU} is found inside car park, its entry_time={entry_time},exit_time={exit_time}");
                            }
                        }
                    }
                    LogClass.WriteLog($"Found {i.ToString()} vehicle inside car park when {Deduction_IU} exit.");                    
                }             
            }

            Application.Exit();




        }
    }
}
