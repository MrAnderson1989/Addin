using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AddIn.Web.Routing
{
    public class RouteTable
    {
        //静态构造函数，约束这个静态对象是一个不被释放的全局变量
        static RouteTable()
        {
            routes = new RouteCollection();
        }
        private static RouteCollection routes;
        public static RouteCollection Routes
        {
            get
            {
                return routes;
            }
        }
    }
}