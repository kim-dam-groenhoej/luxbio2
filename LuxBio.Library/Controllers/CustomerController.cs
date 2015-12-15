// Author: Nick S. Reese
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxBio.Library.Models;
using LuxBio.Library.DAL;

namespace LuxBio.Library.Controllers
{
    public class CustomerController
    {

        public CustomerController()
        {


        }

        // TODO Refactor this
        public Customer GetCustomer(int id)
        {
            return new Customer() { ID = 1, Name ="Jørn", Phone = "12345678" };
        }

    }


}
