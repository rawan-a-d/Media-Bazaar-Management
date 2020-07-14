using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Windows.Forms;
using System.Data;

namespace MediaBazar
{
    class Database_handler
    {
        MySqlConnection conn;
        Connection connect = new Connection();
        List<Schedule> schedules = new List<Schedule>();
        Person person = new Person();
        List<Person> people = new List<Person>();
        List<string> departments = new List<string>();
        List<Product> products;
        List<Department> newDepartments = new List<Department>();
        List<Department> Departments;
        List<Request> requests = new List<Request>();
        List<Stock> stocks = new List<Stock>();


        public Database_handler()
        {
            conn = new MySqlConnection(connect.ConnectionString);
        }

        public List<Schedule> ReadSchedule()
        {
            this.schedules = new List<Schedule>();
            try
            {
                string sql = "SELECT `id`, `employeeId`, `shiftType`, `date`, `statusOfShift` FROM `schedule`";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Shift a = Shift.MORNING;
                    if (dr[2].ToString() == "Morning")
                    {
                        a = Shift.MORNING;
                    }
                    else if (dr[2].ToString() == "Afternoon")
                    {
                        a = Shift.AFTERNOON;
                    }
                    else if (dr[2].ToString() == "Evening")
                    {
                        a = Shift.EVENING;
                    }

                    ShiftStatus b = ShiftStatus.ASSIGNED;
                    if (dr[4].ToString() == "Assigned")
                    {
                        b = ShiftStatus.ASSIGNED;
                    }
                    else if (dr[4].ToString() == "Proposed")
                    {
                        b = ShiftStatus.PROPOSED;
                    }
                    else if (dr[4].ToString() == "Accepted")
                    {
                        b = ShiftStatus.ACCEPTED;
                    }
                    else if (dr[4].ToString() == "Rejected")
                    {
                        b = ShiftStatus.REJECTED;
                    }




                    Schedule g = new Schedule(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]), a, Convert.ToDateTime(dr[3]), b);
                    schedules.Add(g);
                }
            }
            finally
            {
                conn.Close();
            }
            return schedules;
        }

 
        public List<Product> ReadProduct()
        {
            this.products = new List<Product>();
            try
            {
                string sql = "SELECT `productId`, `departmentId`, `productName`, `price`, `selling_price` FROM `product` WHERE Exist = 1";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Product product = new Product(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]), dr[2].ToString(), Convert.ToDouble(dr[3]), Convert.ToDouble(dr[4]));
                    products.Add(product);
                }
            }
            finally
            {
                conn.Close();
            }
            return products;
        }


        public List<Product> ReadAllProduct()
        {
            this.products = new List<Product>();
            try
            {
                string sql = "SELECT `productId`, `departmentId`, `productName`, `price`,`selling_price` FROM `product`";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Product product = new Product(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]), dr[2].ToString(), Convert.ToDouble(dr[3]), Convert.ToDouble(dr[4]));
                    products.Add(product);
                }
            }
            finally
            {
                conn.Close();
            }
            return products;
        }

        public List<Department> ReadDepartments()
        {
            this.newDepartments = new List<Department>();
            try
            {
                string sql = "SELECT `id`, `name`, `personId`, `minEmployees` FROM `department` ORDER BY id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    int pId = 0;
                    int minEmp = 0;
                    if (dr[2] != DBNull.Value)
                    {
                        pId = Convert.ToInt32(dr[2]);
                    }
                    if (dr[3] != DBNull.Value)
                    {
                        minEmp = Convert.ToInt32(dr[3]);
                    }
                    Department department = new Department(Convert.ToInt32(dr[0]), Convert.ToString(dr[1]), pId, minEmp);
                    newDepartments.Add(department);
                }
            }
            finally
            {
                conn.Close();
            }
            return newDepartments;
        }

        public void AddDepartment(string name, int personId, int minEmp, int lastId)
        {


            try
            {

                string sql = "INSERT INTO department(name, personId, minEmployees) VALUES(@Name, @pId, @MinEmp)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@pId", personId);
                cmd.Parameters.AddWithValue("@MinEmp", minEmp);
                conn.Open();
                cmd.ExecuteNonQuery();

                conn.Close();
                int id = 0;
                foreach (Department d in ReadDepartments())
                {
                    id = d.Id;
                }

                string sql2 = "UPDATE person SET department_id = @dId WHERE id ='" + personId + "';";
                MySqlCommand cmd2 = new MySqlCommand(sql2, conn);

                cmd2.Parameters.AddWithValue("@dId", id);

                conn.Open();
                cmd2.ExecuteNonQuery();
                System.Windows.Forms.MessageBox.Show("Department has been Added to the System");
            }

            finally
            {
                conn.Close();
            }
        }
        public List<Request> ReadRequests()
        {
            this.requests = new List<Request>();
            try
            {
                string sql = "SELECT `id`, `productId`, `quantity`, `status`, `requestedBy`, `requestDate` FROM `stock_request`";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Request request = new Request(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]), Convert.ToInt32(dr[2]), Convert.ToString(dr[3]), Convert.ToString(dr[4]), Convert.ToString(dr[5]));
                    requests.Add(request);
                }
            }
            finally
            {
                conn.Close();
            }
            return requests;
        }
        public List<Stock> ReadStock()
        {
            this.stocks = new List<Stock>();
            try
            {
                string sql = "SELECT `id`, `quantity`, `productId` FROM `stock` ORDER BY `quantity`";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Stock stock = new Stock(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]), Convert.ToInt32(dr[2]));
                    stocks.Add(stock);
                }
            }
            finally
            {
                conn.Close();
            }
            return stocks;
        }
        public void SendStockRequest(int productId, int quantity, Roles role)
        {

            try
            {
                string sql = "INSERT INTO stock_request(productId, quantity, status, requestedBy, requestDate) VALUES(@pId, @quantity, @status, @rBy, @rDate)";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                string status = "Pending";
                if (role == Roles.Administrator)
                {
                    status = "Approved";
                    int x = 0;
                    ReadStock();
                    foreach (Stock s in stocks)
                    {
                        if (s.ProductId == productId)
                        {
                            x = s.Quantity + quantity;

                        }
                    }
                    try
                    {
                        string sql2 = "UPDATE stock SET quantity = @Quantity WHERE productId = @Id";
                        MySqlCommand cmd2 = new MySqlCommand(sql2, conn);

                        cmd2.Parameters.AddWithValue("@Quantity", x);
                        cmd2.Parameters.AddWithValue("@Id", productId);
                        conn.Open();
                        cmd2.ExecuteNonQuery();
                    }
                    finally
                    {
                        conn.Close();
                    }

                }

                cmd.Parameters.AddWithValue("@pId", productId);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@rBy", role.ToString());
                cmd.Parameters.AddWithValue("@rDate", DateTime.Now.Date);
                conn.Open();
                cmd.ExecuteNonQuery();
                if (role == Roles.Administrator)
                {
                    System.Windows.Forms.MessageBox.Show("Stock has been updated");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Request has been sent");
                }

            }
            finally
            {
                conn.Close();
            }
        }

        public void AddToStock(int productId, int quantity)
        {
            int stockId = 0;
            try
            {
                int x = 0;
                ReadStock();
                foreach (Stock s in stocks)
                {
                    if (s.ProductId == productId)
                    {
                        x = s.Quantity + quantity;
                        stockId = s.Id;
                    }
                }
                if (stockId == 0)
                {
                    string sql = "INSERT INTO stock(quantity, productId) VALUES( @quantity, @productid)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@productid", productId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Stock is updated!");



                    string sql2 = "INSERT INTO stock_request(productId, quantity, status, requestedBy, requestDate) VALUES(@pId, @quantity, @status, @rBy, @rDate)";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                    string status = "Approved";
                    cmd2.Parameters.AddWithValue("@pId", productId);
                    cmd2.Parameters.AddWithValue("@quantity", quantity);
                    cmd2.Parameters.AddWithValue("@status", status);
                    cmd2.Parameters.AddWithValue("@rBy", Roles.Administrator.ToString());
                    cmd2.Parameters.AddWithValue("@rDate", DateTime.Now.Date);
                    cmd2.ExecuteNonQuery();



                }
                else
                {
                    string sql = "UPDATE stock SET quantity = @Quantity WHERE id = @Id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@Quantity", x);
                    cmd.Parameters.AddWithValue("@Id", stockId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Stock is updated!");

                    string sql2 = "INSERT INTO stock_request(productId, quantity, status, requestedBy, requestDate) VALUES(@pId, @quantity, @status, @rBy, @rDate)";
                    MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                    string status = "Approved";
                    cmd2.Parameters.AddWithValue("@pId", productId);
                    cmd2.Parameters.AddWithValue("@quantity", quantity);
                    cmd2.Parameters.AddWithValue("@status", status);
                    cmd2.Parameters.AddWithValue("@rBy", Roles.Administrator.ToString());
                    cmd2.Parameters.AddWithValue("@rDate", DateTime.Now.Date);
                    cmd2.ExecuteNonQuery();
                }

            }
            finally
            {
                conn.Close();
            }
        }
        public void SellStockItem(int pId, int pQuantity, int soldItems)
        {
            try
            {
                string sql = "UPDATE stock SET quantity = @Quantity WHERE productId = @Id";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Quantity", pQuantity);
                cmd.Parameters.AddWithValue("@Id", pId);
                conn.Open();
                cmd.ExecuteNonQuery();


                string sql2 = "INSERT INTO sale_history(productId, date, quantity) VALUES(@pId,@date, @quantity)";
                MySqlCommand cmd2 = new MySqlCommand(sql2, conn);
                cmd2.Parameters.AddWithValue("@pId", pId);
                cmd2.Parameters.AddWithValue("@date", DateTime.Now.Date);
                cmd2.Parameters.AddWithValue("@quantity", soldItems);

                cmd2.ExecuteNonQuery();
                MessageBox.Show("Stock is updated!");

            }
            finally
            {
                conn.Close();
            }
        }
        public void ApproveRequest(int id, int productId, int quantity)
        {
            int StockId = 0;
            bool itemInStock = true;
            try
            {
                string Status = "Approved";
                string sql = "UPDATE stock_request SET status = @Status WHERE id ='" + id + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Status", Status);
                conn.Open();
                cmd.ExecuteNonQuery();
                System.Windows.Forms.MessageBox.Show("Request has been approved!");

            }

            finally
            {
                conn.Close();
            }
            try
            {
                int x = 0;
                ReadStock();
                foreach (Stock s in stocks)
                {
                    if (s.ProductId == productId)
                    {
                        x = s.Quantity + quantity;
                        StockId = s.Id;
                    }
                }
                if (StockId == 0)
                {

                    string sql = "INSERT INTO stock(quantity, productId) VALUES( @quantity, @productid)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@productid", productId);
                    conn.Open();
                    cmd.ExecuteNonQuery();

                }
                else
                {



                    string sql = "UPDATE stock SET quantity = @Quantity WHERE id = @Id";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@Quantity", x);
                    cmd.Parameters.AddWithValue("@Id", StockId);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }


            }

            finally
            {
                conn.Close();
            }
        }

        /* EMPLOYEE MANAGEMENT */
        public void AddPersonToDatabase(string givenFirstName, string givenSecondName, DateTime givenDOB, string givenStreetName, int givenHouseNr, string givenZipcode, string givenCity, double givenHourlyWage, string roles)
        {
            string givenEmail;
            string newPassword;
            bool personExist = false;
            try
            {
                foreach (Person item in ReturnPeopleFromDB()) // to check if the Person with the same name already exists
                {
                    if (item.FirstName + item.LastName == givenFirstName + givenSecondName)
                    {
                        personExist = true;
                    }
                }

                if (personExist == false)
                {
                    givenEmail = person.Email(givenFirstName, givenSecondName);
                    newPassword = person.SetPassword();
                    string sql = "INSERT INTO person(firstName, lastName, email, dateOfBirth, streetName, houseNr,zipcode, city, hourlyWage, password, role) VALUES(@firstName, @lastName, @email, @DOB, @streetName, @houseNr, @zipcode, @city, @hourlyWage, @password, @role)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    if (givenFirstName == "" || givenSecondName == "" || givenStreetName == "" || givenZipcode == "" || givenCity == "" || givenHourlyWage == 0 || givenHouseNr == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("None of the above requirements should be empty");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@firstName", givenFirstName);
                        cmd.Parameters.AddWithValue("@lastName", givenSecondName);
                        cmd.Parameters.AddWithValue("@email", givenEmail);
                        cmd.Parameters.AddWithValue("@DOB", givenDOB);
                        cmd.Parameters.AddWithValue("@streetName", givenStreetName);
                        cmd.Parameters.AddWithValue("@houseNr", givenHouseNr);
                        cmd.Parameters.AddWithValue("@zipcode", givenZipcode);
                        cmd.Parameters.AddWithValue("@city", givenCity);
                        cmd.Parameters.AddWithValue("@role", roles);
                        cmd.Parameters.AddWithValue("@hourlyWage", givenHourlyWage);
                        cmd.Parameters.AddWithValue("@password", newPassword);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        System.Windows.Forms.MessageBox.Show("Person has been Added to the System");
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Person with the same name already exists");
                }
            }

            finally
            {
                conn.Close();
            }
        }

        // to remove a person
        public void PersonToRemoveFromDataBase(int personId)
        {
            string sql = "";
            bool fkDataDeleted = false;
            try
            {
                MySqlCommand cmd;
                // to remove the data first from schedule otherwise becuase of the foreing key. Otherwise it wont work. First the data from the child has to be removed
                if (sql != "DELETE FROM schedule WHERE employeeId = @id")
                {
                    sql = "DELETE FROM schedule WHERE employeeId = @id";
                    cmd = new MySqlCommand(sql, conn);  // first parameter has to be the query and the second one should be the connection
                    cmd.Parameters.AddWithValue("@id", personId);
                    conn.Open();  // this must be before the execution which is just under this
                    cmd.ExecuteNonQuery();
                    fkDataDeleted = true;
                }
                if (fkDataDeleted) // removing the data from the main table
                {
                    sql = "DELETE FROM person WHERE id = @id"; // a query of what we want
                    cmd = new MySqlCommand(sql, conn);  // first parameter has to be the query and the second one should be the connection
                    cmd.Parameters.AddWithValue("@id", personId);
                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                conn.Close();
            }
        }
        public Person foundedPersonFromDatabase(string givenName)
        {
            Person g = null;
            try
            {
                string sql = "SELECT id, firstName, lastName, department_id, dateOfBirth, streetName, houseNr, city, zipcode, hourlyWage, role FROM person"; // a query of what we want
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@name", givenName);
                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Roles r = Roles.Employee;
                    if (dr[10].ToString() == "Administrator")
                    {
                        r = Roles.Administrator;
                    }
                    else if (dr[10].ToString() == "Manager")
                    {
                        r = Roles.Manager;
                    }
                    else if (dr[10].ToString() == "DepotWorker")
                    {
                        r = Roles.DepotWorker;
                    }

                    g = new Person(Convert.ToInt32(dr[0]), dr[1].ToString(), dr[2].ToString(), Convert.ToInt32(dr[3]), Convert.ToDateTime(dr[4]), dr[5].ToString(), Convert.ToInt32(dr[6]), dr[7].ToString(), dr[8].ToString(), Convert.ToDouble(dr[9]), r); // has to specify the order like this
                }
                return g;
            }
            finally
            {
                conn.Close();
            }
        }


        public List<Person> ReturnPeopleFromDB()
        {
           people = new List<Person>();
            int dpId = 0;
            try
            {
                string sql = "SELECT id, firstName, lastName, department_id, dateOfBirth, streetName, houseNr, city, zipcode, hourlyWage, role FROM person"; // a query of what we want
                MySqlCommand cmd = new MySqlCommand(sql, conn);  // first parameter has to be the query and the second one should be the connection

                conn.Open();  // this must be before the execution which is just under this
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Roles r = Roles.Employee;
                    if (dr[10].ToString() == "Administrator")
                    {
                        r = Roles.Administrator;
                    }
                    else if (dr[10].ToString() == "Manager")
                    {
                        r = Roles.Manager;
                    }
                    else if (dr[10].ToString() == "DepotWorker")
                    {
                        r = Roles.DepotWorker;
                    }

                    if (dr[3] == DBNull.Value)
                    {
                        Person g = new Person(Convert.ToInt32(dr[0]), dr[1].ToString(), dr[2].ToString(), dpId, Convert.ToDateTime(dr[4]), dr[5].ToString(), Convert.ToInt32(dr[6]), dr[7].ToString(), dr[8].ToString(), Convert.ToDouble(dr[9]), r); // has to specify the order like this
                        people.Add(g);
                    }
                    else
                    {
                        Person g = new Person(Convert.ToInt32(dr[0]), dr[1].ToString(), dr[2].ToString(), Convert.ToInt32(dr[3]), Convert.ToDateTime(dr[4]), dr[5].ToString(), Convert.ToInt32(dr[6]), dr[7].ToString(), dr[8].ToString(), Convert.ToDouble(dr[9]), r); // has to specify the order like this
                        people.Add(g);
                    }
                }
            }
            finally
            {
                conn.Close();
            }
            return people;
        }

        public List<Person> ReadPersons()
        {
            people = new List<Person>();
            try
            {
                string sql = "SELECT id, firstName, lastName, department_id, dateOfBirth, streetName, houseNr, city, zipcode, hourlyWage, role FROM person"; // a query of what we want
                MySqlCommand cmd = new MySqlCommand(sql, conn);  // first parameter has to be the query and the second one should be the connection

                conn.Open();  // this must be before the execution which is just under this
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Roles r = Roles.Employee;
                    if (dr[10].ToString() == "Administrator")
                    {
                        r = Roles.Administrator;
                    }
                    else if (dr[10].ToString() == "Manager")
                    {
                        r = Roles.Manager;
                    }
                    else if (dr[10].ToString() == "DepotWorker")
                    {
                        r = Roles.DepotWorker;
                    }
                    int dpId = 0;
                    if (dr[3] != DBNull.Value)
                    {
                        dpId = Convert.ToInt32(dr[3]);
                    }
                    Person g = new Person(Convert.ToInt32(dr[0]), dr[1].ToString(), dr[2].ToString(), dpId, Convert.ToDateTime(dr[4]), dr[5].ToString(), Convert.ToInt32(dr[6]), dr[7].ToString(), dr[8].ToString(), Convert.ToDouble(dr[9]), r); // has to specify the order like this
                    people.Add(g);
                }
            }
            finally
            {
                conn.Close();
            }
            return people;
        }

        // a method to modify the data
        public void ModifyData(int id, string givenFirstName, string givenSecondName, DateTime givenDOB, string givenStreetName, int givenHouseNr, string givenZipcode, string givenCity, double givenHourlyWage, string roles)
        {
            bool personExist = false;
            try
            {
                foreach (Person item in people) // to check if the Person with the same name already exists
                {
                    if (item.FirstName + item.LastName == givenFirstName + givenSecondName)
                    {
                        personExist = true;
                    }
                }
                if (!personExist)
                {
                    string sql = "UPDATE person SET firstName = @firstName, lastName = @lastName, dateOfBirth = @DOB, streetName = @streetName, houseNr = @houseNr, city = @city, zipcode = @zipcode, hourlyWage = @hourlyWage, role = @role WHERE id ='" + id + "';";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    if (givenFirstName == "" || givenSecondName == "" || givenStreetName == "" || givenZipcode == "" || givenCity == "" || givenHourlyWage == 0 || givenHouseNr == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("None of the above requirements should be empty");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@firstName", givenFirstName);
                        cmd.Parameters.AddWithValue("@lastName", givenSecondName);
                        cmd.Parameters.AddWithValue("@DOB", givenDOB);
                        cmd.Parameters.AddWithValue("@streetName", givenStreetName);
                        cmd.Parameters.AddWithValue("@houseNr", givenHouseNr);
                        cmd.Parameters.AddWithValue("@zipcode", givenZipcode);
                        cmd.Parameters.AddWithValue("@city", givenCity);
                        cmd.Parameters.AddWithValue("@role", roles);
                        cmd.Parameters.AddWithValue("@hourlyWage", givenHourlyWage);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        System.Windows.Forms.MessageBox.Show("The information has been updated");
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Person with the same name already exists");
                }
            }

            finally
            {
                conn.Close();
            }
        }
        public Person ReturnPersonFromList(int id)
        {
            Person foundPerson = null;
            foreach (Person item in people)
            {
                if (item.Id == id)
                {
                    foundPerson = item;
                }
            }
            return foundPerson;
        }

        /* LOGIN */
        /* Check users credentials */
        public bool CheckCredentials(string email, string password)
        {
            try
            {
                using (conn)
                {
                    string sql = $"SELECT firstName, lastName, email, password " +
                        $"FROM person WHERE email = @email AND password = @password";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    // Parameters
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);

                    conn.Open();

                    MySqlDataReader dr = cmd.ExecuteReader();

                    bool areCredentialsCorrect = false;
                    if (dr.Read())
                    {
                        // Save current user's name
                        areCredentialsCorrect = true;
                    }
                    else
                    {
                        areCredentialsCorrect = false;
                    }
                    return areCredentialsCorrect;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        /* Get User Type */
        public string GetUserType(string email)
        {
            string role = "";
            try
            {
                using (conn)
                {
                    // Get user role
                    string sql = $"SELECT role FROM person WHERE email = @email AND role != 'Employee'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    // Parameters
                    cmd.Parameters.AddWithValue("@email", email);

                    conn.Open();

                    Object result = cmd.ExecuteScalar();

                    if (result != null)
                    {
                        role = result.ToString();
                    }
                    else
                    {
                        role = "User does not exist";
                    }
                    return role;
                }
            }
            catch (MySqlException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }



        /* STATISTICS */
        public ArrayList GetStatistics(string dateFrom, string dateTo, string type, string department)
        {
            // Salary per employee between two dates
            if (type == "Salary per employee")
            {
                try
                {

                    using (conn)
                    {
                        string sql;
                        // All departments
                        if (department == "All")
                        {
                             sql = "SELECT p.firstName, p.lastName, p.hourlyWage * Count(s.date) * 4 FROM schedule s " +
                                "INNER JOIN person p ON p.id = s.employeeId " +
                                "WHERE date BETWEEN @dateFrom AND @dateTo " +
                                "GROUP BY s.employeeId";
                        }
                        // A specific department
                        else
                        {
                             sql = "SELECT p.firstName, p.lastName, p.hourlyWage* Count(s.date) *4 FROM(schedule s " +
                                "INNER JOIN person p ON p.id = s.employeeId) " +
                                "INNER JOIN department d ON d.id = p.department_id " +
                                "WHERE date BETWEEN @dateFrom AND @dateTo AND d.name = @department " +
                                "GROUP BY s.employeeId";
                        }


                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        conn.Open();

                        // Parameters
                        cmd.Parameters.AddWithValue("@dateFrom", dateFrom);
                        cmd.Parameters.AddWithValue("@dateTo", dateTo);
                        cmd.Parameters.AddWithValue("@department", department);

                        ArrayList statistics = GatherStatisticData(cmd);

                        return statistics;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            // Number employees per shift between two dates
            else if (type == "Number of employees per shift")
            {
                try
                {
                    using (conn)
                    {
                        string sql;
                        // All departments
                        if (department == "All")
                        {
                            sql = "SELECT COUNT(*) AS nrEmployees, date, shiftType FROM schedule " +
                            "WHERE date BETWEEN @dateFrom AND @dateTo " +
                            "GROUP BY date, shiftType ORDER BY date;";
                        }
                        // A specific department
                        else
                        {
                            sql = "SELECT COUNT(*) AS nrEmployees, date, shiftType FROM (schedule AS s " +
                           "INNER JOIN person p ON s.employeeId = p.id) " +
                           "INNER JOIN department d ON d.id = p.department_id " +
                           "WHERE (d.name = @department) AND " +
                           "(date BETWEEN @dateFrom AND @dateTo) " +
                           "GROUP BY date, shiftType ORDER BY date;";
                        }

                        // Create command object
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        // Parameters

                        cmd.Parameters.AddWithValue("@dateFrom", dateFrom);
                        cmd.Parameters.AddWithValue("@dateTo", dateTo);
                        cmd.Parameters.AddWithValue("@department", department);

                        // Open db connection
                        conn.Open();

                        ArrayList statistics = GatherStatisticData(cmd);

                        return statistics;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //The items get stock request the most(for a specific timeslot)
            else if (type == "Most Restocked Items")
            {
                try
                {
                    using (conn)
                    {
                        string sql;
                        // All departments
                        if (department == "All")
                        {
                            sql = "SELECT sr.productId, p.productName, SUM(sr.quantity) AS totalQuantity FROM stock_request AS sr " +
                                   "INNER JOIN product AS p ON p.productId = sr.productId " +
                                   "WHERE requestDate BETWEEN @dateFrom AND @dateTo " +
                                   "GROUP BY sr.productId ORDER BY totalQuantity DESC LIMIT 5";
                        }
                        // A specific department
                        else
                        {
                            sql = "SELECT sr.productId, p.productName, SUM(sr.quantity) AS totalQuantity FROM (stock_request AS sr " +
                                   "INNER JOIN product AS p ON p.productId = sr.productId) " +
                                   "INNER JOIN department d ON d.id = p.departmentId " +
                                   "WHERE sr.requestDate BETWEEN @dateFrom AND @dateTo AND d.name = @department " +
                                   "GROUP BY sr.productId ORDER BY totalQuantity DESC LIMIT 5";
                        }

                        // Create command object
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        // Parameters

                        cmd.Parameters.AddWithValue("@dateFrom", dateFrom);
                        cmd.Parameters.AddWithValue("@dateTo", dateTo);
                        cmd.Parameters.AddWithValue("@department", department);


                        // Open db connection
                        conn.Open();

                        ArrayList statistics = GatherStatisticData(cmd);

                        return statistics;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            //The items got sold the most(for a specific timeslot)
            else if (type == "Top Selling Products")
            {
                try
                {
                    using (conn)
                    {
                        string sql;
                        // All departments
                        if (department == "All")
                        {
                            sql = "SELECT sl.productId, p.productName, SUM(sl.quantity) AS totalQuantity FROM sale_history AS sl " +
                                "INNER JOIN product AS p ON p.productId = sl.productId " +
                                "WHERE date BETWEEN @dateFrom AND @dateTo " +
                                "GROUP BY sl.productId ORDER BY totalQuantity DESC LIMIT 5";
                        }
                        // A specific department
                        else
                        {
                            sql = "SELECT sl.productId, p.productName, SUM(sl.quantity) AS totalQuantity FROM (sale_history AS sl " +
                                "INNER JOIN product AS p ON p.productId = sl.productId) " +
                                "INNER JOIN department d ON d.id = p.departmentId " +
                                "WHERE date BETWEEN @dateFrom AND @dateTo AND d.name = @department " +
                                "GROUP BY sl.productId ORDER BY totalQuantity DESC LIMIT 5";
                        }

                        // Create command object
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        // Parameters

                        cmd.Parameters.AddWithValue("@dateFrom", dateFrom);
                        cmd.Parameters.AddWithValue("@dateTo", dateTo);
                        cmd.Parameters.AddWithValue("@department", department);


                        // Open db connection
                        conn.Open();

                        ArrayList statistics = GatherStatisticData(cmd);

                        return statistics;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return null;

        }


        public ArrayList GetStatistics(string type, string department)
        {
            // Hourly wage per employee
            if (type == "Hourly wage per employee")
            {
                try
                {
                    using (conn)
                    {
                        string sql;
                        // All departments
                        if (department == "All")
                        {
                             sql = "SELECT firstName, lastName, hourlyWage FROM person";
                        }
                        // A specific department
                        else
                        {
                            sql = "SELECT firstName, lastName, hourlyWage FROM person AS p " +
                            "INNER JOIN department AS d ON d.id = p.department_id " +
                            "WHERE d.name = @department";
                        }

                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        // Parameters

                        cmd.Parameters.AddWithValue("@department", department);

                        conn.Open();

                        ArrayList statistics = GatherStatisticData(cmd);

                        return statistics;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            // Stock requests per year
            else if (type == "Yearly stock requests")
            {
                try
                {
                    using (conn)
                    {
                        string sql;
                        // All departments
                        if (department == "All")
                        {
                            sql = "SELECT YEAR(sr.requestDate), SUM(sr.quantity) AS totalQuantity " +
                           "FROM stock_request AS sr " +
                           "GROUP BY YEAR(sr.requestDate)";
                        }
                        // A specific department
                        else
                        {
                            sql = "SELECT YEAR(sr.requestDate), SUM(sr.quantity) AS totalQuantity " +
                            "FROM (stock_request AS sr " +
                            "INNER JOIN product AS p ON p.productId = sr.productId) " +
                            "INNER JOIN department AS d ON d.id = p.departmentId " +
                            "WHERE d.name = @department " +
                            "GROUP BY YEAR(sr.requestDate)";
                        }

                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@department", department);

                        conn.Open();

                        ArrayList statistics = GatherStatisticData(cmd);

                        return statistics;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            // Profit per year
            else if (type == "Yearly profit")
            {
                try
                {
                    using (conn)
                    {
                        string sql;
                        // All departments
                        if (department == "All")
                        {
                            sql = "SELECT year, SUM(totalProfit) FROM " +
                                   "(SELECT YEAR(sh.date) AS year, SUM(sh.quantity) *(p.selling_price - p.price) AS totalProfit  " +
                                   "FROM (sale_history AS sh " +
                                   "INNER JOIN product AS p ON p.productId = sh.productId) " +
                                   "GROUP BY YEAR(sh.date), sh.productId) AS yearlyProfit GROUP BY year";
                        }
                        // A specific department
                        else
                        {
                            sql = "SELECT year, SUM(totalProfit) FROM " +
                               "(SELECT YEAR(sh.date) AS year, SUM(sh.quantity) * (p.selling_price - p.price) AS totalProfit " +
                               "FROM(sale_history AS sh " +
                               "INNER JOIN product AS p ON p.productId = sh.productId) " +
                               "INNER JOIN department AS d ON d.id = p.departmentId WHERE d.name = @department " +
                               "GROUP BY YEAR(sh.date), p.productId) AS yearlyProfit GROUP BY year";
                        }

                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@department", department);

                        conn.Open();

                        ArrayList statistics = GatherStatisticData(cmd);

                        return statistics;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            // Number of employees per department
            else if (type == "Number of employees per department")
            {
                try
                {
                    using (conn)
                    {
                        string sql = "SELECT d.name, COUNT(*) AS numberOfEmployees FROM person AS p " +
                            "INNER JOIN department d ON d.id = p.department_id " +
                            "GROUP BY department_id";

                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        conn.Open();

                        ArrayList statistics = GatherStatisticData(cmd);

                        return statistics;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return null;
        }

        public ArrayList GetStatistics(string date, string type, string department)
        {
            // Restocked Items Per Date
            if (type == "Restocked Items On Date")
            {
                try
                {
                    using (conn)
                    {
                        string sql;
                        // All departments
                        if (department == "All")
                        {
                            sql = "SELECT p.productName, SUM(sr.quantity) AS totalQuantity " +
                            "FROM stock_request AS sr " +
                            "INNER JOIN product AS p ON p.productId = sr.productId " +
                            "WHERE requestDate = @date " +
                            "GROUP BY sr.productId ORDER BY totalQuantity";
                        }
                        // A specific department
                        else
                        {
                            sql = "SELECT p.productName, SUM(sr.quantity) AS totalQuantity " +
                            "FROM (stock_request AS sr " +
                            "INNER JOIN product AS p ON p.productId = sr.productId) " +
                            "INNER JOIN department AS d ON d.id = p.departmentId " +
                            "WHERE requestDate = @date AND d.name = @department " +
                            "GROUP BY sr.productId ORDER BY totalQuantity";
                        }

                        MySqlCommand cmd = new MySqlCommand(sql, conn);

                        cmd.Parameters.AddWithValue("@date", date);
                        cmd.Parameters.AddWithValue("@department", department);


                        conn.Open();

                        ArrayList statistics = GatherStatisticData(cmd);

                        return statistics;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return null;
        }


        // Gather statistics and save it in an ArrayList and return it
        private ArrayList GatherStatisticData(MySqlCommand cmd)
        {
            ArrayList statistics = new ArrayList();

            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                object[] values = new object[dr.FieldCount];
                dr.GetValues(values);
                statistics.Add(values);
            }

            return statistics;
        }


        /* GET EMPLOYEES PER SHIFT PER DAY */
        public string GetEmployeesPerShift(DateTime date, string shiftType, string department)
        {
            string employees = "";
            try
            {
                using (conn)
                {
                    string sql;
                    // All departments
                    if (department == "All")
                    {
                        sql = $"SELECT p.firstName, p.lastName FROM (`person` AS p " +
                        $"INNER JOIN schedule AS s ON s.employeeId = p.id) " +
                        $"WHERE s.date = '{date:yyyy-MM-dd}' AND s.shiftType = '{shiftType}'";
                    }
                    // A specific department
                    else
                    {
                        sql = $"SELECT p.firstName, p.lastName FROM (`person` AS p " +
                        $"INNER JOIN schedule AS s ON s.employeeId = p.id) " +
                        $"INNER JOIN department AS d ON d.id = p.department_id " +
                        $"WHERE s.date = '{date:yyyy-MM-dd}' AND s.shiftType = '{shiftType}' AND d.name = '{department}'";
                    }

                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        employees += $"{dr[0]} {dr[1]} {Environment.NewLine}";
                    }
                }
                return employees;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return "";

            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
                return "";
            }
        }

        // ***********************************
        /* GET ALL DEPARTMENTS */
        public ArrayList GetDepartments()
        {
            try
            {
                using (conn)
                {
                    ArrayList departments = new ArrayList();

                    string sql = $"SELECT * FROM department";


                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    conn.Open();
                    MySqlDataReader dr = cmd.ExecuteReader();


                    while (dr.Read())
                    {
                        object[] values = new object[dr.FieldCount];
                        dr.GetValues(values);
                        departments.Add(values);
                    }
                    return departments;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public List<Department> GetAllDepartments()
        {

            Departments = new List<Department>();
            try
            {
                string sql = "SELECT name, id FROM department"; // a query of what we want
                MySqlCommand cmd = new MySqlCommand(sql, conn);  // first parameter has to be the query and the second one should be the connection

                conn.Open();  // this must be before the execution which is just under this
                MySqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Department department = new Department(dr[0].ToString(), Convert.ToInt32(dr[1]));
                    Departments.Add(department);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            finally
            {
                conn.Close();
            }
            return Departments;
        }

        /* RESET PASSWORD */
        public string ResetPassword(string email, string password)
        {
            try
            {
                using (conn)
                {
                    // Find employee
                    string sql = "UPDATE person SET password= @password WHERE email = @email;";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    // Parameters
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);

                    conn.Open();

                    cmd.ExecuteNonQuery();
                    return "Password was successfully updated";
                }
            }
            catch (MySqlException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        /* Get user name */
        public string GetUserName(string email)
        {
            try
            {
                using (conn)
                {
                    // Get user name
                    string sql = $"SELECT firstName FROM person WHERE email = @email";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    // Parameters
                    cmd.Parameters.AddWithValue("@email", email);

                    conn.Open();

                    object result = cmd.ExecuteScalar();

                    string username = "";

                    if (result != null)
                    {
                        username = result.ToString();
                    }
                    return username;
                }
            }
            catch (MySqlException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Get user department
        public string GetUserDepartment(string email)
        {
            try
            {
                using (conn)
                {
                    // Get user name
                    string sql = $"SELECT d.name FROM person AS p " +
                        $"INNER JOIN department AS d ON p.department_id = d.id " +
                        $"WHERE p.email = @email";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    // Parameters
                    cmd.Parameters.AddWithValue("@email", email);

                    conn.Open();

                    object result = cmd.ExecuteScalar();

                    string department = "";

                    if (result != null)
                    {
                        department = result.ToString();
                    }
                    return department;
                }
            }
            catch (MySqlException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Check if user exists
        public string DoesUserExist(string email)
        {
            try
            {
                using (conn)
                {
                    // Get user name
                    string sql = "SELECT email FROM person WHERE email = @email && role != 'Employee'";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    // Parameters
                    cmd.Parameters.AddWithValue("@email", email);

                    conn.Open();

                    object result = cmd.ExecuteScalar();

                    // if result is empty so user doesn't exists
                    if (result != null)
                    {
                        return "User found";
                    }
                    else
                    {
                        return "User not found";
                    }
                }
            }
            catch (MySqlException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        // to add the products
        public void AddProduct(int departmentId, string name, double price, double sellingPrice)
        {
            bool productExist = false;
            try
            {
                foreach (Product item in products) // to check if the Person with the same name already exists
                {
                    if (item.Name == name)
                    {
                        productExist = true;
                    }
                }
                if (!productExist)
                {
                    string sql = "INSERT INTO product(departmentId, productName, price, selling_price) VALUES(@departmentId, @productName, @productPrice, @sellingPrice)";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    if (!System.Text.RegularExpressions.Regex.IsMatch(name, "[^0-9]"))
                    {
                        System.Windows.Forms.MessageBox.Show("Name Cannot be in Numbers");
                    }
                    else if (price <= 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Price Cannot be 0 or Less than 0");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@departmentId", departmentId);
                        cmd.Parameters.AddWithValue("@productName", name);
                        cmd.Parameters.AddWithValue("@productPrice", price);
                        cmd.Parameters.AddWithValue("@sellingPrice", sellingPrice);
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        System.Windows.Forms.MessageBox.Show("Product has been Added to the System");
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Product Exists");
                }
            }

            finally
            {
                conn.Close();
            }
        }
        public List<Product> GetProducts()
        {
            products = new List<Product>();
            try
            {
                string sql = "SELECT productId, departmentId, productName, price, exist, selling_price FROM product"; // a query of what we want
                MySqlCommand cmd = new MySqlCommand(sql, conn);  // first parameter has to be the query and the second one should be the connection

                conn.Open();  // this must be before the execution which is just under this
                MySqlDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    if (Convert.ToBoolean(dr[4]) == true)
                    {
                        Product g = new Product(Convert.ToInt32(dr[0]), dr[2].ToString(), Convert.ToDouble(dr[3]), Convert.ToInt32(dr[1]), Convert.ToDouble(dr[5])); // has to specify the order like this
                        products.Add(g);
                    }
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
            return products;
        }

        public void ModifyProduct(int id, string givenProductName, double givenProductPrice, double sellingPrice)
        {
            try
            {
                string sql = "UPDATE product SET productName = @productName, price = @productPrice, selling_price = @sellingPrice WHERE productId ='" + id + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                if (!System.Text.RegularExpressions.Regex.IsMatch(givenProductName, "[^0-9]"))
                {
                    System.Windows.Forms.MessageBox.Show("None of the above requirements should be empty");
                }
                else if (givenProductPrice <= 0)
                {
                    System.Windows.Forms.MessageBox.Show("Price Cannot Be 0 or Negative");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@productName", givenProductName);
                    cmd.Parameters.AddWithValue("@productPrice", givenProductPrice);
                    cmd.Parameters.AddWithValue("@sellingPrice", sellingPrice);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    System.Windows.Forms.MessageBox.Show("The information has been updated");
                }

            }

            finally
            {
                conn.Close();
            }
        }


        public void ModifyDepartment(int id, string name, int personId, int minEmp)
        {
            try
            {
                string sql = "UPDATE department SET name = @Name, personId = @PersonId, minEmployees = @MinEmployees WHERE id ='" + id + "';";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@PersonId", personId);
                cmd.Parameters.AddWithValue("@MinEmployees", minEmp);
                conn.Open();
                cmd.ExecuteNonQuery();


                string sql2 = "UPDATE person SET department_id = @dId WHERE id ='" + personId + "';";
                MySqlCommand cmd2 = new MySqlCommand(sql2, conn);

                cmd2.Parameters.AddWithValue("@dId", id);


                cmd2.ExecuteNonQuery();
                System.Windows.Forms.MessageBox.Show("The information has been updated");

            }

            finally
            {
                conn.Close();
            }
        }

        public Product ReturnExistingProduct(int id)
        {
            Product foundProduct = null;
            foreach (Product item in products)
            {
                if (item.ProductId == id)
                {
                    foundProduct = item;
                }
            }
            return foundProduct;
        }
        public void ProductToRemove(int productId)
        {
            string sql = "";
            bool fkStock = false;
            // bool fkProductDelete = true;
            try
            {
                MySqlCommand cmd;
                //sql = "DELETE FROM stock WHERE productId = @id";
                sql = "UPDATE product SET exist = @exist WHERE productId ='" + productId + "';";
                cmd = new MySqlCommand(sql, conn);  // first parameter has to be the query and the second one should be the connection
                cmd.Parameters.AddWithValue("@exist", fkStock);
                conn.Open();  // this must be before the execution which is just under this
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }
        public Product GetProductByName(string givenName)
        {
            Product foundProduct = null;
            foreach (Product p in products)
            {
                if (p.Name.Contains(givenName))
                {
                    foundProduct = p;
                }
            }
            return foundProduct;
        }

        //Read data from Database as object
        public Object ExecuteScalar(string sql)
        {
            try
            {
                using (conn)
                {
                    Object reader;
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();
                    reader = cmd.ExecuteScalar();
                    conn.Close();
                    return reader;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        public DataSet ExecuteDataSet(string sql)
        {
            try
            {
                DataSet ds = new DataSet();
                MySqlDataAdapter da = new MySqlDataAdapter(sql, conn);
                da.Fill(ds, "result");
                return ds;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        //Read data from Database as reader
        public MySqlDataReader ExecuteReader(string sql)
        {
            try
            {
                using (conn)
                {
                    MySqlDataReader reader;
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();
                    reader = cmd.ExecuteReader();
                    conn.Close();
                    return reader;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        
        public int ExecuteNonQuery(string sql)
        {
            try
            {
                using (conn)
                {
                    int affected;
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;
                    conn.Open();
                    affected = cmd.ExecuteNonQuery();
                    conn.Close();
                    return affected;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return -1;
        }

        //Check the number of shifts in one day
        public int checknrshift(string shifttype, string date)
        {
            int nr = 0;
            try
            {
                using (conn)
                {
                    string sql = "SELECT * FROM schedule WHERE (shiftType='" + shifttype + "' AND date='" + date + "');";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        nr++;
                    }
                    rdr.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return nr;
        }

        //Check the number of shift in a day for an employee
        public int checknrperson(int employeeid, string date)
        {
            int nr = 0;
            try
            {
                using (conn)
                {

                    string sql = "SELECT * FROM schedule WHERE (employeeId='" + employeeid + "' AND date='" + date + "');";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        nr++;
                    }
                    rdr.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return nr;
        }

        //Check the number of accepted shifts in one day
        public int checkproposalnrshift(string shifttype, string date)
        {
            int nr = 0;
            try
            {
                using (conn)
                {
                    string sql = "SELECT * FROM schedule WHERE (shiftType='" + shifttype + "' AND date='" + date + "' AND statusOfShift <> 'Rejected' AND statusOfShift <> 'Cancelled' AND statusOfShift <> 'Proposed');";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        nr++;
                    }
                    rdr.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return nr;
        }


        //Check the number of accepted shifts in a day for an employee
        public int checkemployee(int employeeid, string date)
        {
            int nr = 0;
            try
            {
                using (conn)
                {

                    string sql = "SELECT * FROM schedule WHERE (employeeId='" + employeeid + "' AND date='" + date + "' AND statusOfShift<>'Proposed' AND statusOfShift<>'Cancelled' AND statusOfShift<>'Rejected');";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        nr++;
                    }
                    rdr.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return nr;
        }

        //update schedule status
        public void changeschedulestatusbyid(int id, string status)
        {
            try
            {
                using (conn)
                {
                    string sql = "UPDATE schedule SET statusOfShift = '" + status + "' WHERE (id = " + id.ToString() + ")";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //check the shifts in one day
        public int[] checkshiftsinday(string date)
        {
            int nrM = 0, nrA = 0, nrE = 0;
            int[] shifts = new int[3];
            try
            {
                using (conn)
                {

                    string sql = "SELECT shiftType FROM schedule WHERE (date='" + date + "' AND statusOfShift <> 'Rejected' AND statusOfShift <> 'Cancelled' AND statusOfShift <> 'Proposed');";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        if (rdr[0].ToString() == "Morning")
                        {
                            nrM++;
                        }
                        if (rdr[0].ToString() == "Afternoon")
                        {
                            nrA++;
                        }
                        if (rdr[0].ToString() == "Evening")
                        {
                            nrE++;
                        }
                    }
                    rdr.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //MessageBox.Show(nrM.ToString() + " - " + nrA.ToString() + " - " + nrE.ToString());
            shifts[0] = nrM;
            shifts[1] = nrA;
            shifts[2] = nrE;
            return shifts;
        }

        public List<Schedule> ReadProposalByDay(string date, string shifttype)
        {
            this.schedules = new List<Schedule>();
            try
            {
                string sql = "SELECT `id`, `employeeId`, `shiftType`, `date`, `statusOfShift` FROM `schedule` WHERE (date='" + date + "' AND statusOfShift='Proposed' AND shiftType='" + shifttype + "') ORDER BY id ASC;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Shift a = Shift.MORNING;
                    if (dr[2].ToString() == "Morning")
                    {
                        a = Shift.MORNING;
                    }
                    else if (dr[2].ToString() == "Afternoon")
                    {
                        a = Shift.AFTERNOON;
                    }
                    else if (dr[2].ToString() == "Evening")
                    {
                        a = Shift.EVENING;
                    }

                    ShiftStatus b = ShiftStatus.PROPOSED;


                    Schedule g = new Schedule(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]), a, Convert.ToDateTime(dr[3]), b);
                    schedules.Add(g);
                }
            }
            finally
            {
                conn.Close();
            }
            return schedules;
        }


        public List<Person> FindAvailablePeopleByDay(string date)
        {
            List<Person> availablePeople = new List<Person>();
            try
            {

                using (conn)
                {
                    //string sql = "SELECT `id`, `employeeId`, `shiftType`, `date`, `statusOfShift` FROM `schedule` WHERE (date='" + date + "' AND statusOfShift='Proposed') ORDER BY id ASC;";

                    string sql = "SELECT id, firstName, lastName FROM person AS p " +
                    "WHERE id NOT IN(SELECT per.id FROM person AS per " +
                    "INNER JOIN schedule s ON s.employeeId = per.id " +
                    "WHERE s.date = @date AND (s.statusOfShift = 'Assigned' " +
                    "OR s.statusOfShift = 'Accepted' OR s.statusOfShift = 'Confirmed' " +
                    "OR s.statusOfShift = 'Rejected' OR s.statusOfShift = 'AutoAssigned'))";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);

                    cmd.Parameters.AddWithValue("@date", date);


                    conn.Open();

                    MySqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        //Schedule g = new Schedule(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]), a, Convert.ToDateTime(dr[3]), b);
                        Person p = new Person(Convert.ToInt32(dr[0]), dr[1].ToString(), dr[2].ToString());
                        availablePeople.Add(p);
                    }
                }
                return availablePeople;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }


        public List<Schedule> ReadAllProposalByDay(string date)
        {
            this.schedules = new List<Schedule>();
            try
            {
                string sql = "SELECT `id`, `employeeId`, `shiftType`, `date`, `statusOfShift` FROM `schedule` WHERE (date='" + date + "' AND statusOfShift='Proposed') ORDER BY id ASC;";
                MySqlCommand cmd = new MySqlCommand(sql, conn);

                conn.Open();
                MySqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Shift a = Shift.MORNING;
                    if (dr[2].ToString() == "Morning")
                    {
                        a = Shift.MORNING;
                    }
                    else if (dr[2].ToString() == "Afternoon")
                    {
                        a = Shift.AFTERNOON;
                    }
                    else if (dr[2].ToString() == "Evening")
                    {
                        a = Shift.EVENING;
                    }

                    ShiftStatus b = ShiftStatus.PROPOSED;


                    Schedule g = new Schedule(Convert.ToInt32(dr[0]), Convert.ToInt32(dr[1]), a, Convert.ToDateTime(dr[3]), b);
                    schedules.Add(g);
                }
            }
            finally
            {
                conn.Close();
            }
            return schedules;
        }
        public List<ComboboxItem> AddToCombobox()
        {
            List<ComboboxItem> cmbxlist = new List<ComboboxItem>();
            try
            {
                using (conn)
                {

                    string sql = "SELECT * FROM person WHERE (role='Employee' OR role='DepotWorker');";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    conn.Open();

                    MySqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        ComboboxItem item = new ComboboxItem();
                        item.Text = rdr.GetString("firstName") + " " + rdr.GetString("lastName") + " - " + rdr.GetString("role");
                        item.Value = rdr.GetString("id");
                        // MessageBox.Show(item.ToString());
                        cmbxlist.Add(item);
                    }

                    conn.Close();
                    return cmbxlist;
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }
    }
}
