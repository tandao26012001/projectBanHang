using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Models.Common
{
    public class ThongKeTruyCap
    {
        public static string strConnect = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
        public static ThongKeViewModel ThongKe()
        {
            try
            {
                using (var connect = new SqlConnection(strConnect))
                {
                    var item = connect.QueryFirstOrDefault<ThongKeViewModel>(
                        "sp_ThongKe",
                        commandType: CommandType.StoredProcedure
                    );
                    return item;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thực thi stored procedure sp_ThongKe", ex);
            }
        }
    }
}
