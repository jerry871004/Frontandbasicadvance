using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Library.Models
{
    public class BookService
    {
        private string GetDBConnectionString()
        {
            return
                System.Configuration.ConfigurationManager.ConnectionStrings["DBConn"].ConnectionString.ToString();
        }

        /// <summary>
        /// Dropdownlist 圖書類別
        /// </summary>
        public List<SelectListItem> GetClassTable()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT BOOK_CLASS_ID AS BookClassId,
		                          BOOK_CLASS_NAME AS BookClassName
                           FROM BOOK_CLASS
                           ORDER BY BookClassName";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapClassData(dt);
        }

        private List<SelectListItem> MapClassData(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem()
            {
                Value = "",
                Text = "請選擇"
            });
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Value = row["BookClassId"].ToString(),
                    Text = row["BookClassName"].ToString()                
                });
            }
            return result;
        }
        /// <summary>
        /// Dropdownlist 借閱人
        /// </summary>
        public List<SelectListItem> GetUserTable()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT [USER_ID] AS KeeperId,
		                          [USER_ENAME] AS Keeper
		                   FROM MEMBER_M";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapUserData(dt);
        }

        private List<SelectListItem> MapUserData(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem()
            {
                Value = "",
                Text = "請選擇"
            });
            foreach (DataRow row in dt.Rows)
            {
                result.Add(new SelectListItem()
                {
                    Value = row["KeeperId"].ToString(),
                    Text = row["Keeper"].ToString()
                });
            }
            return result;
        }
        /// <summary>
        /// Dropdownlist 借閱狀態
        /// </summary>
        public List<SelectListItem> GetStatusTable()
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT   CODE_ID AS StatusId,
		                            CODE_NAME AS StatusName
                             FROM BOOK_CODE
                             WHERE CODE_TYPE = 'BOOK_STATUS'";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapStatusData(dt);
        }

        private List<SelectListItem> MapStatusData(DataTable dt)
        {
            List<SelectListItem> result = new List<SelectListItem>();
            result.Add(new SelectListItem()
            {
                Value = "",
                Text = "請選擇"
            });
            foreach (DataRow row in dt.Rows)
            {

                result.Add(new SelectListItem()
                {
                    Value = row["StatusId"].ToString(),
                    Text = row["StatusName"].ToString()              
                });
            }
            return result;
        }

        /// <summary>
        /// 把書的資料抓下來
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public List<Models.BOOK> GetBookData(Models.BookSearch arg)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT   BD.BOOK_CLASS_ID AS BookClassId,
		                            BCL.BOOK_CLASS_NAME AS BookClassName,
		                            BD.BOOK_ID AS BookId,
		                            BD.BOOK_NAME AS BookName,
		                            ISNULL(BD.BOOK_KEEPER, '') AS KeeperId,
		                            ISNULL(MM.USER_ENAME, '') AS Keeper,
		                            BD.BOOK_STATUS AS BookStatusId,
		                            BCO.CODE_NAME AS BookStatus,
		                            FORMAT(BD.BOOK_BOUGHT_DATE, 'yyyy/MM/dd') AS BuyDate
                            FROM BOOK_DATA AS BD
                            INNER JOIN BOOK_CLASS BCL ON BD.BOOK_CLASS_ID = BCL.BOOK_CLASS_ID
                            INNER JOIN BOOK_CODE AS BCO ON BD.BOOK_STATUS = BCO.CODE_ID AND BCO.CODE_TYPE = 'BOOK_STATUS'
                            LEFT JOIN MEMBER_M AS MM ON BD.BOOK_KEEPER = MM.[USER_ID]
                            WHERE(UPPER(BD.BOOK_NAME)LIKE UPPER('%' + @BookName + '%') OR @BookName = '') AND
                                 (BCL.BOOK_CLASS_ID = @BookClassId OR @BookClassId = '') AND
                                 (isnull(MM.USER_ID,'') = @KeeperId OR @KeeperId = '') AND
                                 (BD.BOOK_STATUS = @BookStatusId OR @BookStatusId = '')
                            ORDER BY BuyDate DESC";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName == null ? string.Empty : arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", arg.BookClassId == null ? string.Empty : arg.BookClassId));
                cmd.Parameters.Add(new SqlParameter("@KeeperId", arg.KeeperId == null ? string.Empty : arg.KeeperId));
                cmd.Parameters.Add(new SqlParameter("@BookStatusId", arg.BookStatusId == null ? string.Empty : arg.BookStatusId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return this.MapBookData(dt);
        }

        /// <summary>
        /// 把書的資料給BOOK.CS裡宣告的變數
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        private List<Models.BOOK> MapBookData(DataTable table)
        {
            List<Models.BOOK> result = new List<BOOK>();
            foreach(DataRow row in table.Rows)
            {
                result.Add(new BOOK()
                {
                    BookName = row["BookName"].ToString(),
                    BookId = (int)row["BookId"],
                    BookClassName = row["BookClassName"].ToString(),
                    BookClassId = row["BookClassId"].ToString(),
                    BookStatusId = row["BookStatusId"].ToString(),
                    BookStatus = row["BookStatus"].ToString(),
                    KeeperId = row["KeeperId"].ToString(),
                    Keeper = row["Keeper"].ToString(),
                    BuyDate = row["BuyDate"].ToString()
                });
            }
            return result;
        }

        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="arg"></param>
        public bool DeleteBook(int arg)
        {
            DataTable dt = new DataTable();
            string sql = @"DELETE BOOK_DATA WHERE BOOK_ID = @BookId";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId",(int)arg));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return true;
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public bool InsertBook(Models.BOOK arg)
        {
            DataTable dt = new DataTable();
            string sql = @"INSERT INTO BOOK_DATA (BOOK_NAME, BOOK_AUTHOR, BOOK_PUBLISHER, BOOK_NOTE, BOOK_BOUGHT_DATE, BOOK_CLASS_ID, BOOK_STATUS)
		                         VALUES (@BookName, @Author, @Publisher, @Introduction, CONVERT(DATETIME, @BuyDate), @BookClassId, 'A')";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookName", arg.BookName == null ? string.Empty : arg.BookName));
                cmd.Parameters.Add(new SqlParameter("@Author", arg.Author == null ? string.Empty : arg.Author));
                cmd.Parameters.Add(new SqlParameter("@Publisher", arg.Publisher == null ? string.Empty : arg.Publisher));
                cmd.Parameters.Add(new SqlParameter("@Introduction", arg.Introduction == null ? string.Empty : arg.Introduction));
                cmd.Parameters.Add(new SqlParameter("@BuyDate", arg.BuyDate == null ? string.Empty : arg.BuyDate));
                cmd.Parameters.Add(new SqlParameter("@BookClassId", arg.BookClassId == null ? string.Empty : arg.BookClassId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return true;
        }

        ///<summary>
        /// 借閱紀錄
        /// </summary>
        /// <returns>圖書編號</returns>
        public List<Models.BookRecord> GetBookLendRecord(int BookID)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT FORMAT(BLR.LEND_DATE, 'yyyy/MM/dd') AS LendDate,
                                    BLR.KEEPER_ID AS KeeperId,
                                    MM.USER_ENAME AS KeeperEname,
                                    MM.USER_CNAME as KeeperCname,
                                    BLR.BOOK_ID
                            FROM BOOK_LEND_RECORD AS BLR
                            INNER JOIN MEMBER_M AS MM ON BLR.KEEPER_ID = MM.[USER_ID]
                            WHERE BLR.BOOK_ID = @Id
                            ORDER BY LendDate DESC";
            //@"SELECT blr.BOOK_ID AS BookID,
            //                   mm.[USER_ENAME] as UserEname
            //                      mm.[USER_CNAME] AS UserCname,
            //                      blr.KEEPER_ID AS UserID,
            //                   CONVERT(char(10),blr.LEND_DATE,111) AS LendDate
            //               FROM BOOK_LEND_RECORD blr
            //               INNER JOIN BOOK_DATA bd
            //                ON blr.BOOK_ID = bd.BOOK_ID
            //               INNER JOIN MEMBER_M mm
            //                ON blr.KEEPER_ID=mm.[USER_ID]
            //               WHERE blr.BOOK_ID= @ID
            //               ORDER BY blr.LEND_DATE DESC";
            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@ID", BookID));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }
            return MapBookDataToListForLendRecord(dt);
        }

        /// <summary>
        /// Map查詢資料進List
        /// </summary>
        /// <param name="BookData"></param>
        /// <returns></returns>
        private List<Models.BookRecord> MapBookDataToListForLendRecord(DataTable BookData)
        {
            List<Models.BookRecord> result = new List<BookRecord>();
            foreach (DataRow row in BookData.Rows)
            {
                result.Add(new BookRecord()
                {
                    LendDate = row["LendDate"].ToString(),
                    KeeperId = row["KeeperId"].ToString(),
                    KeeperEname = row["KeeperEname"].ToString(),
                    KeeperCname = row["KeeperCname"].ToString()
                });
            }
            return result;
        }

        /// <summary>
        /// 明細
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        public Models.BOOK GetOriginData(int BookId)
        {
            DataTable dt = new DataTable();
            string sql = @"SELECT BD.BOOK_ID AS BookId,
                                    BD.BOOK_NAME AS BookName,
		                            BD.BOOK_AUTHOR AS Author,
		                            BD.BOOK_PUBLISHER AS Publisher,
		                            BD.BOOK_NOTE AS Introduction,
		                            FORMAT(BD.BOOK_BOUGHT_DATE, 'yyyy/MM/dd') AS BuyDate,
		                            BCL.BOOK_CLASS_NAME AS BookClassName
                            FROM BOOK_DATA AS BD
                            INNER JOIN BOOK_CLASS BCL ON BD.BOOK_CLASS_ID = BCL.BOOK_CLASS_ID
                            WHERE BOOK_ID = @BookId";

            using (SqlConnection conn = new SqlConnection(this.GetDBConnectionString()))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add(new SqlParameter("@BookId", BookId));
                SqlDataAdapter sqlAdapter = new SqlDataAdapter(cmd);
                sqlAdapter.Fill(dt);
                conn.Close();
            }

            return this.MapEditData(dt);
        }

        /// <summary>
        /// Map明細資料進List
        /// </summary>
        /// <param name="BookData"></param>
        /// <returns></returns>
        private Models.BOOK MapEditData(DataTable dt)
        {
            Models.BOOK result = new Models.BOOK();

            result.BookId = (int)dt.Rows[0]["BookId"];
            result.BookName = dt.Rows[0]["BookName"].ToString();
            result.Author = dt.Rows[0]["Author"].ToString();
            result.Publisher = dt.Rows[0]["Publisher"].ToString();
            result.Introduction = dt.Rows[0]["Introduction"].ToString();
            result.BuyDate = dt.Rows[0]["BuyDate"].ToString();
            result.BookClassName = dt.Rows[0]["BookClassName"].ToString();
            return result;
        }
    }
}