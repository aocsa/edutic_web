﻿using MLearning.Web.Controllers;
using System.Web;
using System.Web.Mvc;

namespace MLearning.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new MessagesActionFilter());
        }
    }
}
