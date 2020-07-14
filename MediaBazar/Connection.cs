using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaBazar
{
    class Connection
    {
        //this is the class which will provide the connection between the database and the application.
        //beacuse we are using the local site to debugging so it will be convenient for us if we user a class like this
        //so when you want to change the location of the database all you need to do is changing the location here
        //instead of changing the connection in every class.
        public string ConnectionString { get; private set; }

        public Connection()
        {
            FileStream fs = new FileStream("../../../connection.txt", FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            this.ConnectionString = sr.ReadLine();
            sr.Dispose();
            fs.Dispose();
        }
    }
}
