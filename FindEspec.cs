using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
namespace Win11Check
{
    class FindEspec
    {
       public ManagementObjectSearcher seacherComp = new ManagementObjectSearcher();

        public void InsertInfo(string root, string querry) 
        {
            seacherComp = new ManagementObjectSearcher(root, "SELECT * FROM " + querry);

            return;
        }

    }
}
