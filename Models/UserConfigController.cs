using System;
namespace BoostMyEconomy.Models
{

    public sealed class UserConfigController
    {
        // The Singleton's constructor should always be private to prevent
        // direct construction calls with the `new` operator.
        private UserConfigController() { }

        // The Singleton's instance is stored in a static field. There there are
        // multiple ways to initialize this field, all of them have various pros
        // and cons. In this example we'll show the simplest of these ways,
        // which, however, doesn't work really well in multithreaded program.
        private static readonly object _lock = new object();
        private static UserConfigController? _instance;
        private static bool _openDrawer;
        public static bool isDrawerOpen;

        // This is the static method that controls the access to the singleton
        // instance. On the first run, it creates a singleton object and places
        // it into the static field. On subsequent runs, it returns the client
        // existing object stored in the static field.
        public static UserConfigController GetInstance(string Value)
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new UserConfigController();
                        _openDrawer = true;
                        isDrawerOpen = _openDrawer;
                        Console.WriteLine(Value);
                    }
                }
            }
            return _instance;
        }



        // Finally, any singleton should define some business logic, which can
        // be executed on its instance.
        public static void ToggleDrawer()
        {
            if (!_openDrawer)
            {
                Console.WriteLine("Opening drawer");
                _openDrawer = true;
            }
            else
            {
                Console.WriteLine("Closing drawer");
                _openDrawer = false;
            }
        }
    }
}
