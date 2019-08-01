using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }

        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            switch(crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    break;
                case "read":
                    Employee foundEmployeeRead = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Where(e => e.Email == employee.Email).Where(e => e.FirstName == employee.FirstName).Where(e => e.LastName == employee.LastName).FirstOrDefault();
                    string output = "";
                    output += "Name: " + foundEmployeeRead.FirstName + " " + foundEmployeeRead.LastName + "\n";
                    output += "Employee Number: " + foundEmployeeRead.EmployeeNumber + "\n";
                    output += "Email: " + foundEmployeeRead.Email + "\n";
                    output += "Username: " + foundEmployeeRead.UserName + "\n";
                    output += "Password: " + foundEmployeeRead.Password;
                    UserInterface.DisplayUserOptions(output);
                    Console.ReadKey();
                    break;
                case "update":
                    Employee foundEmployeeUpdate = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Where(e => e.Email == employee.Email).Where(e => e.FirstName == employee.FirstName).Where(e => e.LastName == employee.LastName).FirstOrDefault();

                    employee.FirstName = UserInterface.GetStringData("new first name", "the employee's");
                    employee.LastName = UserInterface.GetStringData("new last name", "the employee's");
                    employee.EmployeeNumber = int.Parse(UserInterface.GetStringData("new employee number", "the employee's"));
                    employee.Email = UserInterface.GetStringData("new email", "the employee's");
                    employee.UserName = UserInterface.GetStringData("new username", "the employee's");
                    employee.Password = UserInterface.GetStringData("new password", "the employee's");
                    
                    foundEmployeeUpdate.FirstName = employee.FirstName;
                    foundEmployeeUpdate.LastName = employee.LastName;
                    foundEmployeeUpdate.Email = employee.Email;
                    foundEmployeeUpdate.UserName = employee.UserName;
                    foundEmployeeUpdate.Password = employee.Password;
                    break;
                case "delete":
                    Employee FoundEmployeeDelete = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Where(e => e.Email == employee.Email).Where(e => e.FirstName == employee.FirstName).Where(e => e.LastName == employee.LastName).FirstOrDefault();
                    db.Employees.DeleteOnSubmit(FoundEmployeeDelete);
                    var changedAnimals = db.Animals.Where(a => a.EmployeeId == FoundEmployeeDelete.EmployeeId);
                    foreach (Animal animal in changedAnimals)
                    {
                        animal.EmployeeId = null;
                    }
                    break;
            }
            db.SubmitChanges();
        }
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
            db.SubmitChanges();
        }

        internal static Animal GetAnimalByID(int id)
        {
            Animal foundAnimal = db.Animals.Where(a => a.AnimalId == id).FirstOrDefault();
            return foundAnimal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            Animal foundAnimal = GetAnimalByID(animalId);
            if (updates.ContainsKey(1))
            {
                foundAnimal.Category = db.Categories.Where(c => c.Name == updates[1]).FirstOrDefault();
            }
            if (updates.ContainsKey(2))
            {
                foundAnimal.Name = updates[2];
            }
            if (updates.ContainsKey(3))
            {
                foundAnimal.Age = Convert.ToInt32(updates[3]);
            }
            if (updates.ContainsKey(4))
            {
                foundAnimal.Demeanor = updates[4];
            }
            if (updates.ContainsKey(5))
            {
                foundAnimal.KidFriendly = Convert.ToBoolean(updates[5]);
            }
            if (updates.ContainsKey(6))
            {
                foundAnimal.PetFriendly = Convert.ToBoolean(updates[6]);
            }
            if (updates.ContainsKey(7))
            {
                foundAnimal.Weight = Convert.ToInt32(updates[7]);
            }
            db.SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
            var rooms = db.Rooms.Where(a => a.AnimalId == animal.AnimalId);
            foreach (Room room in rooms)
            {
                room.AnimalId = null;
            }
            db.SubmitChanges();
        }

        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            IQueryable<Animal> animals = db.Animals.Select(a => a);
            if (updates.ContainsKey(1))
            {
                animals = animals.Where(a => a.Category.Name == updates[1]);
            }
            if (updates.ContainsKey(2))
            {
                animals = animals.Where(a => a.Name == updates[2]);
            }
            if (updates.ContainsKey(3))
            {
                animals = animals.Where(a => a.Age == Convert.ToInt32(updates[3]));
            }
            if (updates.ContainsKey(4))
            {
                animals = animals.Where(a => a.Demeanor == updates[4]);
            }
            if (updates.ContainsKey(5))
            {
                animals = animals.Where(a => a.KidFriendly == Convert.ToBoolean(updates[5]));
            }
            if (updates.ContainsKey(6))
            {
                animals = animals.Where(a => a.PetFriendly == Convert.ToBoolean(updates[6]));
            }
            if (updates.ContainsKey(7))
            {
                animals = animals.Where(a => a.Weight == Convert.ToInt32(updates[7]));
            }
            if (updates.ContainsKey(8))
            {
                animals = animals.Where(a => a.AnimalId == Convert.ToInt32(updates[8]));
            }
            return animals;
        }

        internal static int GetCategoryId(string categoryName)
        {
            var categoryConnectId = db.Categories.Where(e => e.Name == categoryName).Single();
            var categoryFromDb = db.Animals.Where(e => e.CategoryId == categoryConnectId.CategoryId).FirstOrDefault();
            if (categoryFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return categoryFromDb.AnimalId;
            }
        }

        internal static Room GetRoom(int animalId)
        {
            var roomConnectId = db.Rooms.Where(e => e.AnimalId == animalId).Single();
            if (roomConnectId == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return roomConnectId;
            }
        }

        internal static int GetDietPlanId(string dietPlanName)
        {
            var dietPlanConnectId = db.DietPlans.Where(e => e.Name == dietPlanName).Single();
            if (dietPlanConnectId == null)
            {
                throw new NullReferenceException();
            }
            else
            {

                return dietPlanConnectId.DietPlanId;
            }
        }



        internal static void Adopt(Animal animal, Client client)
        {
            Adoption adoption = new Adoption();
            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.ApprovalStatus = "Pending";
            adoption.AdoptionFee = 75;
            adoption.PaymentCollected = true;
            db.Adoptions.InsertOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            IQueryable<Adoption> adoptions = db.Adoptions.Where(a => a.ApprovalStatus == "Pending");
            return adoptions;
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            if (adoption.PaymentCollected == true)
            {
                if (isAdopted == true)
                {
                    adoption.ApprovalStatus = "Approved";
                    Animal adoptedAnimal = db.Animals.Where(a => a.AnimalId == adoption.AnimalId).FirstOrDefault();
                    RemoveAdoption(adoption.AnimalId, adoption.ClientId);
                    RemoveAnimal(adoptedAnimal);
                }
                else
                {
                    adoption.ApprovalStatus = "Denied";
                    RemoveAdoption(adoption.AnimalId, adoption.ClientId);
                }
                db.SubmitChanges();
            }
            else
            {
                UserInterface.DisplayUserOptions("Client has not paid fees.");
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var adoption = db.Adoptions.Where(a => a.AnimalId == animalId).Where(a => a.ClientId == clientId).FirstOrDefault();
            db.Adoptions.DeleteOnSubmit(adoption);
            db.SubmitChanges();
        }

        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> animalShotsQuery = db.AnimalShots.Where(e => e.AnimalId == animal.AnimalId);
            return animalShotsQuery;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot UpdateAnimalShot = db.AnimalShots.Where(e => e.AnimalId == animal.AnimalId).FirstOrDefault();
            var typeOfShot = db.Shots.Where(e => e.Name == shotName).FirstOrDefault();
            UpdateAnimalShot.ShotId = typeOfShot.ShotId;

        }

        internal static void ImportAnimalDataFromCSV(string filePath)
        {
            if (File.Exists(filePath))
            {
                string line;

                StreamReader file = new StreamReader(filePath);
                while ((line = file.ReadLine()) != null)
                {
                    string[] currentLineItems = line.Split(',');
                    // name, weight, age, demeanor, kidfriendly, petfriendly, gender, adoptionstatus, categoryid, dietplanid, employeeid
                    Animal currentAnimal = new Animal();
                    currentAnimal.Name = currentLineItems[0].Replace("\"", "") ?? null;
                    if(currentLineItems[1].Replace("\"", "") == "null" || string.IsNullOrEmpty(currentLineItems[1].Replace("\"", "")))
                    {
                        currentAnimal.Weight = null;
                    }
                    else
                    {
                        currentAnimal.Weight = Convert.ToInt32(currentLineItems[1]);
                    }
                    if (currentLineItems[2].Replace("\"", "") == "null" || string.IsNullOrEmpty(currentLineItems[2].Replace("\"", "")))
                    {
                        currentAnimal.Age = null;
                    }
                    else
                    {
                        currentAnimal.Age = Convert.ToInt32(currentLineItems[2]);
                    }
                    currentAnimal.Demeanor = currentLineItems[3].Replace("\"", "") ?? null;
                    if (currentLineItems[4].Replace("\"", "") == "null" || string.IsNullOrEmpty(currentLineItems[4].Replace("\"", "")))
                    {
                        currentAnimal.KidFriendly = null;
                    }
                    else
                    {
                        currentAnimal.KidFriendly = (Convert.ToInt32(currentLineItems[4]) != 0);
                    }
                    if (currentLineItems[5].Replace("\"", "") == "null" || string.IsNullOrEmpty(currentLineItems[5].Replace("\"", "")))
                    {
                        currentAnimal.PetFriendly = null;
                    }
                    else
                    {
                        currentAnimal.PetFriendly = (Convert.ToInt32(currentLineItems[5]) != 0);
                    }
                    currentAnimal.Gender = currentLineItems[6].Replace("\"", "") ?? null;
                    currentAnimal.AdoptionStatus = currentLineItems[7].Replace("\"", "") ?? null;
                    if (currentLineItems[8].Replace("\"", "") == "null" || string.IsNullOrEmpty(currentLineItems[8].Replace("\"", "")))
                    {
                        currentAnimal.CategoryId = null;
                    }
                    else
                    {
                        currentAnimal.CategoryId = Convert.ToInt32(currentLineItems[8]);
                    }
                    if (currentLineItems[9].Replace("\"", "") == "null" || string.IsNullOrEmpty(currentLineItems[9].Replace("\"", "")))
                    {
                        currentAnimal.DietPlanId = null;
                    }
                    else
                    {
                        currentAnimal.DietPlanId = Convert.ToInt32(currentLineItems[9]);
                    }
                    if (currentLineItems[10].Replace("\"", "") == "null" || string.IsNullOrEmpty(currentLineItems[10].Replace("\"", "")))
                    {
                        currentAnimal.EmployeeId = null;
                    }
                    else
                    {
                        currentAnimal.EmployeeId = Convert.ToInt32(currentLineItems[10]);
                    }
                    AddAnimal(currentAnimal);
                }

                file.Close();
                Console.WriteLine("Animals have been imported.");
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
            Console.ReadKey();
        }
    }
}