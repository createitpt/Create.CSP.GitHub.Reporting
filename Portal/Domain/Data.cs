using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ReportingPortal.Models;

namespace ReportingPortal.Domain
{
    public class Data
    {
        public IEnumerable<Navbar> navbarItems()
        {
            var menu = new List<Navbar>();
            
            menu.Add(new Navbar { Id = 100, nameOption = "Reporting", imageClass = "fa fa-users fa-fw", status = true, isParent = true, parentId = 0 });
            menu.Add(new Navbar { Id = 101, nameOption = "Campaign Management", controller = "Reporting", action = "CampaignManagement", status = true, isParent = false, parentId = 100 });
            menu.Add(new Navbar { Id = 102, nameOption = "Activation Report", controller = "Reporting", action = "ActivationReport", status = true, isParent = false, parentId = 100 });
                      
        //http://cspfrontenddemo.azurewebsites.net/Customers/Detail/91fb1592-3c7e-45c8-93e1-db2142103025


            // Separator
            //menu.Add(new Navbar { Id = 99, nameOption = "Separator", controller = "Home", action = "Index", imageClass = "fa fa-dashboard fa-fw", status = true, isParent = false, parentId = 0 });


            // Original options
            //menu.Add(new Navbar { Id = 100, nameOption = "Dashboard", controller = "Home", action = "Index", imageClass = "fa fa-dashboard fa-fw", status = true, isParent = false, parentId = 0 });
            //menu.Add(new Navbar { Id = 200, nameOption = "Charts", imageClass = "fa fa-bar-chart-o fa-fw", status = true, isParent = true, parentId = 0 });
            //menu.Add(new Navbar { Id = 300, nameOption = "Flot Charts", controller = "Home", action = "FlotCharts", status = true, isParent = false, parentId = 200 });
            //menu.Add(new Navbar { Id = 400, nameOption = "Morris.js Charts", controller = "Home", action = "MorrisCharts", status = true, isParent = false, parentId = 200 });
            //menu.Add(new Navbar { Id = 500, nameOption = "Tables", controller = "Home", action = "Tables", imageClass = "fa fa-table fa-fw", status = true, isParent = false, parentId = 0 });
            //menu.Add(new Navbar { Id = 600, nameOption = "Forms", controller = "Home", action = "Forms", imageClass = "fa fa-edit fa-fw", status = true, isParent = false, parentId = 0 });
            //menu.Add(new Navbar { Id = 700, nameOption = "UI Elements", imageClass = "fa fa-wrench fa-fw", status = true, isParent = true, parentId = 0 });
            //menu.Add(new Navbar { Id = 800, nameOption = "Panels and Wells", controller = "Home", action = "Panels", status = true, isParent = false, parentId = 700 });
            //menu.Add(new Navbar { Id = 900, nameOption = "Buttons", controller = "Home", action = "Buttons", status = true, isParent = false, parentId = 700 });
            //menu.Add(new Navbar { Id = 1000, nameOption = "Notifications", controller = "Home", action = "Notifications", status = true, isParent = false, parentId = 700 });
            //menu.Add(new Navbar { Id = 1100, nameOption = "Typography", controller = "Home", action = "Typography", status = true, isParent = false, parentId = 700 });
            //menu.Add(new Navbar { Id = 1200, nameOption = "Icons", controller = "Home", action = "Icons", status = true, isParent = false, parentId = 700 });
            //menu.Add(new Navbar { Id = 1300, nameOption = "Grid", controller = "Home", action = "Grid", status = true, isParent = false, parentId = 700 });
            //menu.Add(new Navbar { Id = 1400, nameOption = "Multi-Level Dropdown", imageClass = "fa fa-sitemap fa-fw", status = true, isParent = true, parentId = 0 });
            //menu.Add(new Navbar { Id = 1500, nameOption = "Second Level Item", status = true, isParent = false, parentId = 1400 });
            //menu.Add(new Navbar { Id = 1600, nameOption = "Sample Pages", imageClass = "fa fa-files-o fa-fw", status = true, isParent = true, parentId = 0 });
            //menu.Add(new Navbar { Id = 1700, nameOption = "Blank Page", controller = "Home", action = "Blank", status = true, isParent = false, parentId = 1600 });
            //menu.Add(new Navbar { Id = 1800, nameOption = "Login Page", controller = "Home", action = "Login", status = true, isParent = false, parentId = 1600 });

            return menu.ToList();
        }
    }
}