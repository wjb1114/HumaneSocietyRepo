﻿using System;
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

                try
                {
                    db.Addresses.InsertOnSubmit(newAddress);
                }
                catch
                {
                    Console.WriteLine("Unable to access database.");
                }
                SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;
            try
            {
                db.Clients.InsertOnSubmit(newClient);
            }
            catch
            {
                Console.WriteLine("Unable to access database.");
            }

            SubmitChanges();
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

                try
                {
                    db.Addresses.InsertOnSubmit(newAddress);
                }
                catch
                {
                    Console.WriteLine("Unable to access database.");
                }

                SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            SubmitChanges();
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
                    RunEmployeeQueriesAdd(employee);
                    break;
                case "read":
                    RunEmployeeQueriesRead(employee);
                    break;
                case "update":
                    RunEmployeeQueriesUpdate(employee);
                    break;
                case "delete":
                    RunEmployeeQueriesDelete(employee);
                    break;
            }
            SubmitChanges();
        }
        internal static void RunEmployeeQueriesAdd(Employee employee)
        {
            try
            {
                db.Employees.InsertOnSubmit(employee);
            }
            catch
            {
                Console.WriteLine("Unable to access database.");
            }
        }
        internal static void RunEmployeeQueriesRead(Employee employee)
        {
            Employee foundEmployeeRead = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Where(e => e.Email == employee.Email).Where(e => e.FirstName == employee.FirstName).Where(e => e.LastName == employee.LastName).FirstOrDefault();
            string output = "";
            output += "Name: " + foundEmployeeRead.FirstName + " " + foundEmployeeRead.LastName + "\n";
            output += "Employee Number: " + foundEmployeeRead.EmployeeNumber + "\n";
            output += "Email: " + foundEmployeeRead.Email + "\n";
            output += "Username: " + foundEmployeeRead.UserName + "\n";
            output += "Password: " + foundEmployeeRead.Password;
            UserInterface.DisplayUserOptions(output);
            Console.ReadKey();
        }
        internal static void RunEmployeeQueriesUpdate(Employee employee)
        {
            Employee foundEmployeeUpdate = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Where(e => e.Email == employee.Email).Where(e => e.FirstName == employee.FirstName).Where(e => e.LastName == employee.LastName).FirstOrDefault();

            employee.FirstName = UserInterface.GetStringData("new first name", "the employee's");
            employee.LastName = UserInterface.GetStringData("new last name", "the employee's");
            try
            {
                employee.EmployeeNumber = int.Parse(UserInterface.GetStringData("new employee number", "the employee's"));
            }
            catch
            {
                Console.WriteLine("Invalid input, ignoring employee number update.");
            }
            employee.Email = UserInterface.GetStringData("new email", "the employee's");
            employee.UserName = UserInterface.GetStringData("new username", "the employee's");
            employee.Password = UserInterface.GetStringData("new password", "the employee's");

            foundEmployeeUpdate.FirstName = employee.FirstName;
            foundEmployeeUpdate.LastName = employee.LastName;
            foundEmployeeUpdate.Email = employee.Email;
            foundEmployeeUpdate.UserName = employee.UserName;
            foundEmployeeUpdate.Password = employee.Password;
        }
        internal static void RunEmployeeQueriesDelete(Employee employee)
        {
            Employee FoundEmployeeDelete = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).Where(e => e.Email == employee.Email).Where(e => e.FirstName == employee.FirstName).Where(e => e.LastName == employee.LastName).FirstOrDefault();
            try
            {
                db.Employees.DeleteOnSubmit(FoundEmployeeDelete);
            }
            catch
            {
                Console.WriteLine("Unable to access database.");
            }
            var changedAnimals = db.Animals.Where(a => a.EmployeeId == FoundEmployeeDelete.EmployeeId);
            foreach (Animal animal in changedAnimals)
            {
                animal.EmployeeId = null;
            }
        }

        internal static void AddAnimal(Animal animal)
        {
            try
            {
                db.Animals.InsertOnSubmit(animal);
            }
            catch
            {
                Console.WriteLine("Unable to access database.");
            }
            SubmitChanges();
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
                try
                {
                    foundAnimal.Age = Convert.ToInt32(updates[3]);
                }
                catch
                {
                    Console.WriteLine("Invalid input, ignoring Age input.");
                }
            }
            if (updates.ContainsKey(4))
            {
                foundAnimal.Demeanor = updates[4];
            }
            if (updates.ContainsKey(5))
            {
                try
                {
                    foundAnimal.KidFriendly = Convert.ToBoolean(updates[5]);
                }
                catch
                {
                    Console.WriteLine("Invalid input, ignoring Kid Friendly input.");
                }
            }
            if (updates.ContainsKey(6))
            {
                try
                {
                    foundAnimal.PetFriendly = Convert.ToBoolean(updates[6]);
                }
                catch
                {
                    Console.WriteLine("Invalid input, ignoring Pet Friendly input.");
                }
            }
            if (updates.ContainsKey(7))
            {
                try
                {
                    foundAnimal.Weight = Convert.ToInt32(updates[7]);
                }
                catch
                {
                    Console.WriteLine("Invalid input, ignoring Weight input.");
                }
            }
            SubmitChanges();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            try
            {
                db.Animals.DeleteOnSubmit(animal);
            }
            catch
            {
                Console.WriteLine("Unable to access database.");
            }
            var rooms = db.Rooms.Where(a => a.AnimalId == animal.AnimalId);
            foreach (Room room in rooms)
            {
                room.AnimalId = null;
            }
            SubmitChanges();
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
                try
                {
                    int value = Convert.ToInt32(updates[3]);
                    animals = animals.Where(a => a.Age == value);
                }
                catch
                {
                    Console.WriteLine("Input is invalid. Ignoring search criteria \'Age\'");
                }
            }
            if (updates.ContainsKey(4))
            {
                animals = animals.Where(a => a.Demeanor == updates[4]);
            }
            if (updates.ContainsKey(5))
            {
                try
                {
                    bool value = Convert.ToBoolean(updates[5]);
                    animals = animals.Where(a => a.KidFriendly == value);
                }
                catch
                {
                    Console.WriteLine("Input is invalid. Ignoring search criteria \'Kid Friendly\'");
                }
                
            }
            if (updates.ContainsKey(6))
            {
                try
                {
                    bool value = Convert.ToBoolean(updates[6]);
                    animals = animals.Where(a => a.PetFriendly == value);
                }
                catch
                {
                    Console.WriteLine("Input is invalid. Ignoring search criteria \'Pet Friendly\'");
                }
            }
            if (updates.ContainsKey(7))
            {
                try
                {
                    int value = Convert.ToInt32(updates[7]);
                    animals = animals.Where(a => a.Weight == value);
                }
                catch
                {
                    Console.WriteLine("Input is invalid. Ignoring search criteria \'Weight\'");
                }
            }
            if (updates.ContainsKey(8))
            {
                try
                {
                    int value = Convert.ToInt32(updates[8]);
                    animals = animals.Where(a => a.AnimalId == value);
                }
                catch
                {
                    Console.WriteLine("Input is invalid. Ignoring search criteria \'Animal ID\'");
                }
            }
            return animals;
        }

        internal static int GetCategoryId(string categoryName)
        {
            var categoryConnectId = db.Categories.Where(e => e.Name == categoryName).FirstOrDefault();
            if (categoryConnectId == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return categoryConnectId.CategoryId;
            }
        }

        internal static Room GetRoom(int animalId)
        {
            var roomConnectId = db.Rooms.Where(e => e.AnimalId == animalId).FirstOrDefault();
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
            var dietPlanConnectId = db.DietPlans.Where(e => e.Name == dietPlanName).FirstOrDefault();
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
            try
            {
                db.Adoptions.InsertOnSubmit(adoption);
            }
            catch
            {
                Console.WriteLine("Unable to access database.");
            }
            SubmitChanges();
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
                SubmitChanges();
            }
            else
            {
                UserInterface.DisplayUserOptions("Client has not paid fees.");
            }
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            var adoption = db.Adoptions.Where(a => a.AnimalId == animalId).Where(a => a.ClientId == clientId).FirstOrDefault();
            try
            {
                db.Adoptions.DeleteOnSubmit(adoption);
            }
            catch
            {
                Console.WriteLine("Unable to access database.");
            }
            SubmitChanges();
        }

        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            IQueryable<AnimalShot> animalShotsQuery = db.AnimalShots.Where(e => e.AnimalId == animal.AnimalId);
            return animalShotsQuery;
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            AnimalShot updateAnimalShot = db.AnimalShots.Where(e => e.AnimalId == animal.AnimalId).FirstOrDefault();
            var typeOfShot = db.Shots.Where(e => e.Name == shotName).FirstOrDefault();
            updateAnimalShot.ShotId = typeOfShot.ShotId;

        }

        internal static void ImportAnimalDataFromCSV(string filePath)
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath).Select(x => x.Split(','));

                foreach (var data in lines)
                {
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = data[i].Replace("\"", "").Trim();
                    }

                    Animal currentAnimal = new Animal
                    {
                        Name = data[0] ?? null,
                        Weight = data[1].ToNullableInt() ?? null,
                        Age = data[2].ToNullableInt() ?? null,
                        Demeanor = data[3] ?? null,
                        KidFriendly = data[4].ToNullableBool() ?? null,
                        PetFriendly = data[5].ToNullableBool() ?? null,
                        Gender = data[6] ?? null,
                        AdoptionStatus = data[7] ?? null,
                        CategoryId = data[8].ToNullableInt() ?? null,
                        DietPlanId = data[9].ToNullableInt() ?? null,
                        EmployeeId = data[10].ToNullableInt() ?? null
                    };
                    AddAnimal(currentAnimal);
                }
                Console.WriteLine("Animals have been imported.");
            }
            else
            {
                Console.WriteLine("File does not exist!");
            }
            
            Console.ReadKey();
            Console.Clear();
        }

        public static int? ToNullableInt(this string line)
        {
            int value;
            if (int.TryParse(line, out value))
            {
                return value;
            }
            return null;
        }

        public static bool? ToNullableBool(this string line)
        {
            int? initialValue = line.ToNullableInt();
            if (initialValue == null)
            {
                return null;
            }
            else
            {
                if (initialValue == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        static void SubmitChanges()
        {
            try
            {
                db.SubmitChanges();
            }
            catch
            {
                Console.WriteLine("Unable to access database.");
            }
        }

    }
}