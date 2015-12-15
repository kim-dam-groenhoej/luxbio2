// Author: Kim Dam Grønhøj, Gitte Nielsen
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LuxBio.WCF;
using System.ServiceModel;
using System.Timers;
using LuxBio.Library.Models;
using System.Diagnostics;

namespace WCFConsoleHost
{
    class Program
    {
        private static System.Timers.Timer lockedTimer;
        private static IService1 service;

        static void Main(string[] args)
        {
            service = new Service1();

            ServiceHost host = new ServiceHost(typeof(Service1));
            var behaviour = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behaviour.InstanceContextMode = InstanceContextMode.PerCall;
            behaviour.ConcurrencyMode = ConcurrencyMode.Multiple;
            host.Open();

            lockedTimer = new Timer(5000);
            lockedTimer.Elapsed += OnTimerElapsed;
            lockedTimer.Start();
            lockedTimer.Enabled = true;

            Console.WriteLine("Running...");
            Console.ReadLine();

            Console.WriteLine("Closing...");
            host.Close();
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            lockedTimer.Enabled = false;

            try
            {
                var hall = service.GetHall(1);

                foreach (var moviePlayTime in hall.MoviePlayTimes)
                {
                    var chairs = new List<Chair>();
                    foreach (var row in hall.Rows)
                    {
                        chairs.AddRange(row.Chairs);
                    }

                    service.ReleaseLockedByTime(moviePlayTime, chairs);
                }


            } catch (Exception ex)
            {
                // write to windows event viewer
                EventLog.WriteEntry(ex.Source, ex.Message);
            }

            lockedTimer.Enabled = true;
        }
    }
}
