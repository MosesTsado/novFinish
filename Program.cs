using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace DmvAppointmentScheduler
{
    class Program
    {
        public static Random random = new Random();
        public static List<Appointment> appointmentList = new List<Appointment>();
        static void Main(string[] args)
        {
            CustomerList customers = ReadCustomerData();
            TellerList tellers = ReadTellerData();
            Calculation(customers, tellers);
            OutputTotalLengthToConsole();

        }
        private static CustomerList ReadCustomerData()
        {
            string fileName = "CustomerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            CustomerList customerData = JsonConvert.DeserializeObject<CustomerList>(jsonString);
            return customerData;

        }
        private static TellerList ReadTellerData()
        {
            string fileName = "TellerData.json";
            string path = Path.Combine(Environment.CurrentDirectory, @"InputData\", fileName);
            string jsonString = File.ReadAllText(path);
            TellerList tellerData = JsonConvert.DeserializeObject<TellerList>(jsonString);
            return tellerData;

        }
        static void Calculation(CustomerList customers, TellerList tellers)
        {
            // Your code goes here .....
            // Re-write this method to be more efficient instead of a assigning all customers to the same teller

            // used a hash table to store all teller types as values 
            int x = 0;
            Hashtable teller_dict = new Hashtable();
            foreach(Teller teller in tellers.Teller){
                teller_dict.Add(x,teller.specialtyType);
                x++;  
            }
            foreach(Customer customer in customers.Customer){
                // created variable that checks if customer type is equal to any teller types if not they are processed by the first teller
                bool noTellerType = teller_dict.ContainsValue(customer.type);
                if (noTellerType == false) {
                    var appointment = new Appointment(customer, tellers.Teller[0]);
                }
                // If customer type is equal to a teller type this creates an appointment with that teller
                else{
                    int key_num = 0;
                    while(key_num > -1){
                        if(teller_dict[key_num] == customer.type){
                            var appointment = new Appointment(customer, tellers.Teller[key_num]);
                            break;
                        }
                        // Since the teller types are next to each other in the teller array this resets key num to ensure that we loop through entire array and not just part of the array
                        else if(key_num == 148){
                            key_num = 0;
                        }
                        else{
                            key_num++;
                        }

                    }
                    
                }              
            } 
        }
        static void OutputTotalLengthToConsole()
        {
            var tellerAppointments =
                from appointment in appointmentList
                group appointment by appointment.teller into tellerGroup
                select new
                {
                    teller = tellerGroup.Key,
                    totalDuration = tellerGroup.Sum(x => x.duration),
                };
            var max = tellerAppointments.OrderBy(i => i.totalDuration).LastOrDefault();
            Console.WriteLine("Teller " + max.teller.id + " will work for " + max.totalDuration + " minutes!");
        }

    }
}
