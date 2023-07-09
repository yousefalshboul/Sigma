using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Sockets;
using System.Web;
using System.Web.Mvc;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Sigma.Models;

namespace Sigma.Controllers
{
    public class SearchController : Controller
    {
        SqlConnection con = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;
        List<Search> search = new List<Search>();
        public void ConnectionString()
        {
            if (con.ConnectionString == "")
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
            }
        }

        public ActionResult Data()
        {
            ConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from tmast T INNER JOIN YEARS Y on T.NO = Y.MMEM order by 1 desc ";
            com.ExecuteNonQuery();
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                search.Add(new Search()
                {
                    id = Convert.ToInt32(dr["NO"]),
                    Name = dr["NAME"].ToString(),
                    NationalID = dr["FAMNO"].ToString(),
                    Year = Convert.ToDouble(dr["YR"])
                });
            }
            dr.Close();
            con.Close();
            return View("Result");

        }
        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult Result()
        {
            Data();
            return View(search);
        }
        public ActionResult searchMethod(string categories, string searchQuery)
        {
            List<Search> search = new List<Search>();
            ConnectionString();
            if (string.Equals(categories, "Id"))
            {
                try
                {
                    Convert.ToInt32(searchQuery);
                    using (con)
                    {
                        using (com = new SqlCommand("select * from tmast T INNER JOIN YEARS Y on T.NO = '" + searchQuery + "' AND Y.MMEM = '" + searchQuery + "'", con))
                        {
                            con.Open();
                            SqlDataReader dr = com.ExecuteReader();
                            DataTable dt = new DataTable();
                            dt.Load(dr);
                            foreach (DataRow row in dt.Rows)
                            {
                                search.Add(new Search
                                {
                                    id = Convert.ToInt32(row["NO"]),
                                    Name = row["NAME"].ToString(),
                                    NationalID = row["FAMNO"].ToString(),
                                    Year = Convert.ToDouble(row["YR"])
                                });
                            }
                            con.Close();
                            return View("Result", search);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Content("Something went wrong " + ex.Message);
                }
            }
            else
            {
                try
                {
                    using (con)
                    {
                        using (com = new SqlCommand("select * from tmast T INNER JOIN YEARS Y on T.Name LIKE N'%" + searchQuery + "%' AND T.NO = Y.MMEM", con))
                        {
                            con.Open();
                            SqlDataReader dr = com.ExecuteReader();
                            DataTable dt = new DataTable();
                            dt.Load(dr);
                            foreach (DataRow row in dt.Rows)
                            {
                                search.Add(new Search
                                {
                                    id = Convert.ToInt32(row["NO"]),
                                    Name = row["NAME"].ToString(),
                                    NationalID = row["FAMNO"].ToString(),
                                    Year = Convert.ToDouble(row["YR"])
                                });
                            }
                            con.Close();
                            return View("Result", search);
                        }
                    }
                }
                catch (Exception ex)
                {
                    return Content("Something went wrong " + ex.Message);
                }


                //    List<Search> search = new List<Search>();
                //ConnectionString();
                //Convert.ToInt32(searchQuery);
                //using (con)
                //{
                //    //select * from tmast T inner join YEARS Y on T.NO = 1 and y.MMEM=1 order by 1 desc
                //    using (com = new SqlCommand("select * from tmast T INNER JOIN YEARS Y on T.NO = '" + searchQuery + "' AND Y.MMEM = '" + searchQuery+"'", con))
                //    {
                //        con.Open();
                //        dr = com.ExecuteReader();
                //        DataTable dt = new DataTable();
                //        dt.Load(dr);
                //        foreach(DataRow row in dt.Rows)
                //        {
                //            search.Add(new Search
                //            {
                //                id = Convert.ToInt32(row["NO"]),
                //                Name = row["NAME"].ToString(),
                //                NationalID = row["FAMNO"].ToString(),
                //                Year = Convert.ToDouble(row["YR"])
                //            });
                //        }
                //        con.Close();
                //    }
                //}
                //    return View("Result", search);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            ConnectionString();
            //con.Open();
            com.Connection = con;
            com = new SqlCommand("dbo.DeleteElements", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@ID", SqlDbType.VarChar).Value = id;
            //com.Parameters.Add("@YEAR", SqlDbType.VarChar).Value = year;
            con.Open();
            com.ExecuteNonQuery();
            con.Close();
            TempData["AlertMessageDelete"] = "The client number " + id + " is deleted.";
            return RedirectToAction("Result");
        }


        [HttpPost]
        public ActionResult Created(Search s)
        {
            Int32 MaxId;
            ConnectionString();
            //con.Open();
            com.Connection = con;
            con.Open();
            com.CommandText = "select max (NO) from TMAST";
            MaxId = Convert.ToInt32(com.ExecuteScalar());
            con.Close();
            s.id = MaxId + 1;
            com = new SqlCommand("dbo.AddElements", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@ID", SqlDbType.VarChar).Value = s.id;
            com.Parameters.Add("@Name", SqlDbType.VarChar).Value = s.Name;
            com.Parameters.Add("@National", SqlDbType.VarChar).Value = s.NationalID;
            com.Parameters.Add("@Year", SqlDbType.VarChar).Value = s.Year;
            con.Open();
            com.ExecuteNonQuery();
            TempData["AlertMessageEdit"] = "The client number " + s.id + " is Added successfully.";
            con.Close();
            return RedirectToAction("Result");
        }

        [HttpPost]
        public ActionResult Create(Search s)
        {

            ConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from tmast T INNER JOIN YEARS Y on T.NO = Y.MMEM order by 1 desc ";
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                s.id = Convert.ToInt32(dr["NO"]);
                s.Name = dr["NAME"].ToString();
                s.NationalID = dr["FAMNO"].ToString();
                s.Year = Convert.ToDouble(dr["YR"]);
            }
            con.Close();
            return View("Add", s);
        }

        [HttpPost]
        public ActionResult UpdateEdit(Search s)
        {
            ConnectionString();
            //con.Open();
            com.Connection = con;
            com = new SqlCommand("dbo.EditElements", con);
            com.CommandType = CommandType.StoredProcedure;
            com.Parameters.Add("@ID", SqlDbType.VarChar).Value = s.id;
            com.Parameters.Add("@Name", SqlDbType.VarChar).Value = s.Name;
            com.Parameters.Add("@National", SqlDbType.VarChar).Value = s.NationalID;
            com.Parameters.Add("@Year", SqlDbType.VarChar).Value = s.Year;

            con.Open();
            com.ExecuteNonQuery();
            TempData["AlertMessageEdit"] = "The client number " + s.id + " is updated successfully.";
            con.Close();
            //Data();
            return RedirectToAction("Result");
        }

        [HttpPost]
        public ActionResult Edit(Search s)
        {

            ConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from tmast T INNER JOIN YEARS Y on T.NO = '"+s.id+"' AND Y.MMEM='"+s.id+"' order by 1 desc ";
            dr = com.ExecuteReader();
            while (dr.Read())
            {
                s.id = Convert.ToInt32(dr["NO"]);
                s.Name = dr["NAME"].ToString();
                s.NationalID = dr["FAMNO"].ToString();
                s.Year = Convert.ToDouble(dr["YR"]);
            }
            con.Close();
            return View("Edit", s);
        }
    }
}
