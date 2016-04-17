﻿using MLearning.Core.Configuration;
using MLearning.Core.Services;
using MLearning.Web.Singleton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MLearning.Web.Models;

namespace MLearning.Web.Controllers
{
    public class MLController : Controller
    {
        protected IMLearningService _mLearningService;

        public MLController()
            : base()
        {
            _mLearningService = ServiceManager.GetService();

            InstitutionID = System.Web.HttpContext.Current.Session["InstitutionID"] as int? ?? default(int);
            UserID = System.Web.HttpContext.Current.Session["UserID"] as int? ?? default(int);
            PublisherID = System.Web.HttpContext.Current.Session["PublisherID"] as int? ?? default(int);
            CircleID = System.Web.HttpContext.Current.Session["CircleID"] as int? ?? default(int);
            LOID = System.Web.HttpContext.Current.Session["LOID"] as int? ?? default(int);
            PageID = System.Web.HttpContext.Current.Session["PageID"] as int? ?? default(int);
            QuizID = System.Web.HttpContext.Current.Session["QuizID"] as int? ?? default(int);
            QuestionID = System.Web.HttpContext.Current.Session["QuestionID"] as int? ?? default(int);
            userType = System.Web.HttpContext.Current.Session["UserType"] as UserType?;
            Password = System.Web.HttpContext.Current.Session["Password"] as string;
            Toastr = new Toastr();
        }


        int _institutionID;
        protected int InstitutionID
        {
            get { return _institutionID; }
            set { System.Web.HttpContext.Current.Session["InstitutionID"] = _institutionID = value; }
        }



        int _userID;
        protected int UserID
        {
            get { return _userID; }
            set { System.Web.HttpContext.Current.Session["UserID"] = _userID = value; }
        }

        UserType? _userType;
        protected UserType? userType
        {
            get { return _userType; }
            set { System.Web.HttpContext.Current.Session["UserType"] = _userType = value; }
        }
        string _password;
        protected string Password
        {
            get { return _password; }
            set { System.Web.HttpContext.Current.Session["Password"] = _password = value; }
        }

        int _publisherID;
        protected int PublisherID
        {
            get { return _publisherID; }
            set { System.Web.HttpContext.Current.Session["PublisherID"] = _publisherID = value; }
        }




        int _circleID;
        protected int CircleID
        {
            get { return _circleID; }
            set { System.Web.HttpContext.Current.Session["CircleID"] = _circleID = value; }
        }




        int _lOID;
        protected int LOID
        {
            get { return _lOID; }
            set { System.Web.HttpContext.Current.Session["LOID"] = _lOID = value; }
        }


        int _pageID;
        public int PageID
        {
            get { return _pageID; }
            set { System.Web.HttpContext.Current.Session["PageID"] = _pageID = value; }
        }

        int _quizID;
        public int QuizID
        {
            get { return _quizID; }
            set { System.Web.HttpContext.Current.Session["QuizID"] = _quizID = value; }
        }

        int _questionID;
        public int QuestionID
        {
            get { return _questionID; }
            set { System.Web.HttpContext.Current.Session["QuestionID"] = _questionID = value; }
        }

        public Toastr Toastr { get; set; }

        public ToastMessage AddToastMessage(string title, string message, ToastType toastType)
        {
            return Toastr.AddToastMessage(title, message, toastType);
        }

    }
}