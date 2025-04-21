using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;

namespace WebBanHangOnline.App_Start
{
    public class NotHomeIndexConstraint : IRouteConstraint
    {
        public bool Match(HttpContextBase httpContext,
                      Route route,
                      string parameterName,
                      RouteValueDictionary values,
                      RouteDirection routeDirection)
        {
            // Kiểm tra nếu controller là "Home" và action là "Index"
            if (values["controller"]?.ToString().ToLower() == "Home" &&
                values["action"]?.ToString().ToLower() == "Index")
            {
                return false; // Không cho phép truy cập
            }

            return true; // Cho phép route này
        }
    }
}