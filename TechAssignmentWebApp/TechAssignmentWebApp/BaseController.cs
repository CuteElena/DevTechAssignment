using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechAssignmentWebApp.ApiRepository;

namespace TechAssignmentWebApp
{
    public class BaseController : Controller
    {

        protected ApiServiceRepository apiRepository;


        public BaseController()
        {
            apiRepository = new ApiServiceRepository();


        }
    }
}