using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Sigma.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sigma.Controllers
{
    public class AuthController : Controller
    {
        SqlConnection con  = new SqlConnection();
        SqlCommand com = new SqlCommand();
        SqlDataReader dr;

        public void ConnectionString()
        {
            if (con.ConnectionString == "")
            {
                con.ConnectionString = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
            }
        }
        // GET: Auth
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {

            return View();
        }
        public ActionResult Login(Auth Ath) 
        {
            ConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "select * from Users where UserName = '" + Ath.UserName + "' and Password = '" + Ath.encode(Ath.Password) + "' ";
            com.ExecuteNonQuery();
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                con.Close();
                Session["UserName"] = Ath.UserName;
                TempData["AlertMessageLogin"] = "Logged in successfully, welcome " + Ath.UserName;
                return View("~/Views/Search/Index.cshtml");
            }
            else
            {
                con.Close();
                TempData["AlertMessageFailedLogin"] = "The user name or password is incorrect, please try again!";
                return View("Index");
            }
        }

        public ActionResult LogOut()
        {
            Session["UserName"] = null;
            Session.Abandon();
            TempData["AlertMessageLogout"] = "Logged out!";
            return View("Index");
        }
        [HttpPost]
        public ActionResult Signup(Auth ath)
        {

            ConnectionString();
            con.Open();
            com.Connection = con;
            com.CommandText = "insert into Users (UserName,Password,FirstName,LastName) values ('" + ath.UserName + "','" + ath.encode(ath.Password) + "','" + ath.FirstName + "','" + ath.LastName + "')";
            dr = com.ExecuteReader();
            con.Close();
            con.Open();
            com.CommandText = "select * from Users where FirstName = '" + ath.FirstName + "' and UserName = '" + ath.UserName + "' and Password = '" + ath.encode(ath.Password) + "' ";
            dr = com.ExecuteReader();
            if (dr.Read())
            {
                Session["UserName"] = "Welcome " + ath.UserName;
                TempData["AlertMessageSignup"] = "The user " + ath.FirstName + " is registered successfully!";
                return View("~/Views/Search/Index.cshtml");
            }
            else
            {
                con.Close();
                return View("Signup");
            }

        }
    }
}