﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Net;
using System.Net.Mail;
using System.Collections;

namespace MediaBazar
{
    public class MediaBazaar
    {
        private static MediaBazaar instance = null;
        // Current user
        private string currentUser;

        List<Person> people = new List<Person>();
        List<Schedule> schedules = new List<Schedule>();
        // Person person = new Person();
        List<Department> departments = new List<Department>();
        List<Request> requests = new List<Request>();
        List<Stock> stocks = new List<Stock>();
        List<Product> products = new List<Product>();

        Person person = new Person();
        string connectionString = "Server=studmysql01.fhict.local;Uid=dbi435688;Database=dbi435688;Pwd=webhosting54;SslMode=none";

        MySqlConnection conn;

        Database_handler database;

        public string CurrentUser
        {
            get { return this.currentUser; }
        }

        /* Reset code variables */
        private string to;

        public MediaBazaar()
        {
            database = new Database_handler();
            conn = new MySqlConnection(connectionString);
        }

        public static MediaBazaar Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MediaBazaar();
                }
                return instance;
            }
        }


        /* LOGIN */
        /* Get User Type */
        public string GetUserType(string email)
        {
            string userType = database.GetUserType(email);

            return userType;
        }

        /* Check users credentials */
        public bool CheckCredentials(string email, string password)
        {
            bool areCredentialsCorrect = database.CheckCredentials(email, password);

            // Save user name
            if (areCredentialsCorrect)
            {
                string name = GetUserName(email);

                SaveCurrentUser(name);
            }

            return areCredentialsCorrect;
        }

        private void SaveCurrentUser(string name)
        {
            this.currentUser = name;
            Console.WriteLine(name);
        }

        /* LOGOUT */
        public void LogOut()
        {
            this.currentUser = null;
        }

        /* RESET PASSWORD */
        public string ResetPassword(string email, string password)
        {
            string result = database.ResetPassword(email, password);

            return result;
        }

        /* Send reset code */
        public string SendResetCode(string email, string randomCode)
        {
            string from, password, messageBody;

            // Send code per email
            MailMessage message = new MailMessage();

            // Since email's of user isn't real, I'm using my own email

            to = "rawan.ad7@gmail.com";
            // For testing purposes
            //to = email;
            // Email sender
            from = "mediabazaar2@gmail.com";
            password = "Sendcode43";
            messageBody = $"<h4>Hello {GetUserName(email)},</h4> <p> You recently requested to reset your password for your Media Bazaar account. <p> Here is your reset code {randomCode}</p> <p>If you did not request a password reset, please ignore this email or reply to let us know.</p> <p> Best Regards, </p> <p>Media Bazaar</p>";
            // For testing purposes
            //messageBody = $"<h4>Hello {GetUserName("CheyenneConway@mediabazaar.com")},</h4> <p> You recently requested to reset your password for your Media Bazaar account. <p> Here is your reset code {randomCode}</p> <p>If you did not request a password reset, please ignore this email or reply to let us know.</p> <p> Best Regards, </p> <p>Media Bazaar</p>";


            message.To.Add(to);
            message.From = new MailAddress(from);
            message.IsBodyHtml = true;
            message.Body = messageBody;


            message.Subject = "Password resetting code";
            // Creating the smtp object
            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.EnableSsl = true;
            smtp.Port = 587;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(from, password);
            // Send email
            try
            {
                smtp.Send(message);
                return "code sent successfully";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /* Get user name */
        public string GetUserName(string email)
        {
            string userName = database.GetUserName(email);

            return userName;
        }

        /* Check if user exists */
        public string DoesUserExist(string email)
        {
            string doesUserExist = database.DoesUserExist(email);

            return doesUserExist;
        }

        /* STATISTICS */
        public ArrayList GetStatistics(string type)
        {
            ArrayList statistics = database.GetStatistics(type);

            return statistics;
        }

        public ArrayList GetStatistics(string dateFrom, string type)
        {
            ArrayList statistics = database.GetStatistics(dateFrom, type);

            return statistics;
        }

        public ArrayList GetStatistics(string dateFrom, string dateTo, string type)
        {
            ArrayList statistics = database.GetStatistics(dateFrom, dateTo, type);

            return statistics;
        }

        /* GET EMPLOYEE FOR SHIFT, STATISTICS */
        public string GetEmployeesPerShift(DateTime date, string shiftType)
        {
            string employees = database.GetEmployeesPerShift(date, shiftType);
            return employees;
        }


        // to add a person in a database
        public void AddPerson(string givenFirstName, string givenSecondName, DateTime givenDOB, string givenStreetName, int givenHouseNr, string givenZipcode, string givenCity, double givenHourlyWage, string roles)
        {
            database.AddPersonToDatabase(givenFirstName, givenSecondName, givenDOB, givenStreetName, givenHouseNr, givenZipcode, givenCity, givenHourlyWage, roles);
        }

        // to remove a person
        public void RemovePerson(int personId)
        {
            database.PersonToRemoveFromDataBase(personId);
        }

        // to search a person by name
        public Person foundedPerson(string givenName)
        {
            return database.foundedPersonFromDatabase(givenName);
        }

        // to get the list of people from database
        public List<Person> ReturnPeopleFromDB()
        {
            return database.ReturnPeopleFromDB();
        }

        // to modify the data of an existing employee
        public void UpdateData(int id, string givenFirstName, string givenSecondName, DateTime givenDOB, string givenStreetName, int givenHouseNr, string givenZipcode, string givenCity, double givenHourlyWage, string roles)
        {
            database.ModifyData(id, givenFirstName, givenSecondName, givenDOB, givenStreetName, givenHouseNr, givenZipcode, givenCity, givenHourlyWage, roles);
        }

        // to get the existing data of an existing employee to modify
        public Person ReturnPerson(int id)
        {
            return database.ReturnPersonFromList(id);
        }

        /* SCHEDULE */
        public void ReadSchedule()
        {
            this.schedules = database.ReadSchedule();


        }
        public List<Schedule> GetSchedulesList()
        {
            return this.schedules;
        }

        public List<Schedule> GetScheduleByShift(Shift givenShift)
        {
            List<Schedule> newSchedule = new List<Schedule>();
            foreach (Schedule s in schedules)
            {
                if (s.ShiftType == givenShift)
                {
                    newSchedule.Add(s);
                }
            }
            return newSchedule;
        }
        public List<Schedule> GetScheduleByName(string name)
        {
            List<Schedule> newSchedule = new List<Schedule>();
            foreach (Schedule s in schedules)
            {
                if (s.EmployeeId == GetPersonIdByName(name))
                {
                    newSchedule.Add(s);
                }
            }
            return newSchedule;
        }
        public List<Schedule> GetScheduleByNameAndShift(string name, Shift givenShift)
        {
            List<Schedule> newSchedule = new List<Schedule>();
            foreach (Schedule s in schedules)
            {
                if (s.EmployeeId == GetPersonIdByName(name) && s.ShiftType == givenShift)
                {
                    newSchedule.Add(s);
                }
            }
            return newSchedule;
        }

        public String GetPersonNameById(int id)
        {
            string s = "";
            foreach (Person p in ReturnPeopleFromDB())
            {
                if (id == p.Id)
                {
                    s = p.FirstName + " " + p.LastName;
                }
            }
            return s;
        }
        public List<Person> GetPeopleList()
        {
            return this.ReturnPeopleFromDB();
        }




        public int GetPersonIdByName(string name)
        {
            int i = 0;
            foreach (Person p in ReturnPeopleFromDB())
            {
                if (p.GetFullName() == name)
                {
                    i = p.Id;
                }
            }
            return i;
        }

        public int GetSchedule(string name, string date, string type)
        {
            int i = 0;
            Shift shift = Shift.MORNING;
            if (type == "AFTERNOON")
            {
                shift = Shift.AFTERNOON;
            }
            else if (type == "EVENING")
            {
                shift = Shift.EVENING;
            }
            else if (type == "MORNING")
            {
                shift = Shift.MORNING;
            }
            int c = GetPersonIdByName(name);
            foreach (Schedule s in schedules)
            {

                if (s.EmployeeId == c && s.DATETime == Convert.ToDateTime(date).Date && s.ShiftType == shift)
                {
                    i = s.SheduleId;
                }
            }
            return i;
        }

        //to get the departments from the db
        public List<Department> GetDepartments()
        {
            return database.GetDepartments();
        }

        // to add a new product in the system
        public void AddProduct(int departmentId, string productName, double productPrice, double sellingPrice)
        {
            database.AddProduct(departmentId, productName, productPrice, sellingPrice);
        }
        //to get the products
        public List<Product> GetProducts()
        {
            return database.GetProducts();
        }

        //to modify the existing product
        public void ModifyProduct(int id, string productName, double productPrice, double sellingPrice)
        {
            database.ModifyProduct(id, productName, productPrice, sellingPrice);
        }

        //to get the exisitng product in order to modify
        public Product ReturnExistingProduct(int id)
        {
            return database.ReturnExistingProduct(id);
        }

        //to remove a product from the database
        public void ProductToRemove(int id)
        {
            database.ProductToRemove(id);
        }

        // to search a product by name 
        public Product ProductToSearch(string givenName)
        {
            return database.GetProductByName(givenName);
        }

        public void ReadProducts()
        {
            this.products = database.ReadProduct();
        }
        public List<Product> GetProductsList()
        {
            return this.products;
        }
        public List<Product> GetProductsListByName(string name)
        {
            List<Product> newProducts = new List<Product>();
            foreach (Product p in products)
            {
                if (p.ProductName == name)
                {
                    newProducts.Add(p);
                }
            }
            return newProducts;
        }
        public void SendDepoRequest(int productId, int quantity)
        {
            database.SendStockRequest(productId, quantity, Roles.DepotWorker);
        }
        public void SendManagerRequest(int productId, int quantity)
        {
            database.SendStockRequest(productId, quantity, Roles.Manager);
        }
        public void SendAdminRequest(int productId, int quantity)
        {
            database.SendStockRequest(productId, quantity, Roles.Administrator);
        }
        public void AddToStock(int productId, int quantity)
        {
            database.AddToStock(productId, quantity);
        }
        public void ReadDepartment()
        {
            this.departments = database.ReadDepartments();
        }
        public string GetDepartmentNameById(int id)
        {
            string name = "";
            foreach (Department d in departments)
            {
                if (d.Id == id)
                {
                    name = d.Name;
                }
            }
            return name;
        }
        public string GetProductNameById(int id)
        {
            
            string name = "";
            foreach (Product d in database.ReadAllProduct())
            {
                if (d.ProductId == id)
                {
                    name = d.ProductName;
                }
            }
            return name;
        }
        public int GetProductIntByName(string name)
        {
            ReadProducts();
            int id = 0;
            foreach (Product d in products)
            {
                if (d.ProductName == name)
                {
                    id = d.ProductId;
                }
            }
            return id;
        }
        public void ReadRequests()
        {
            this.requests = database.ReadRequests();
        }
        public void ReadStocks()
        {
            this.stocks = database.ReadStock();
        }
        public List<Stock> GetStockList()
        {
            return this.stocks;
        }
        public List<Request> GetRequestsList()
        {
            return this.requests;
        }
        public void ApproveRequest(int id, int productId, int quantity)
        {
            database.ApproveRequest(id, productId, quantity);
        }


    }


}

