using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using ClassControlsAndStyle.Dialogs;

namespace TerminalRbkm.Logik
{
    class SingleInstance
    {
        /// <summary>
        /// Processing single instance in <see cref="SingleInstanceModes"/> <see cref="SingleInstanceModes.ForEveryUser"/> mode.
        /// </summary>
        internal static void Make()
        {
            Make(SingleInstanceModes.ForEveryUser);
        }

        /// <summary>
        /// Processing single instance.
        /// </summary>
        /// <param name="singleInstanceModes"></param>
        internal static void Make(SingleInstanceModes singleInstanceModes)
        {



            var appName = Application.Current.GetType().Assembly.ManifestModule.ScopeName;

            var windowsIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            if (windowsIdentity != null && windowsIdentity.User != null)
            {
                var keyUserName = windowsIdentity.User.ToString();

                // Be careful! Max 260 chars!
                var eventWaitHandleName = string.Format(
                    "{0}",
                    appName
                    //singleInstanceModes == SingleInstanceModes.ForEveryUser ? keyUserName : String.Empty
                    );

                try
                {

                    if (RunningInstance())
                    {
                        using (var eventWaitHandle = EventWaitHandle.OpenExisting(eventWaitHandleName))
                        {
                            new DialogOkCancel("Приложение уже запущено.", "Внимание!");
                            eventWaitHandle.Set();
                        }

                        // Let's terminate this posterior startup.
                        // For that exit no interceptions.
                        Environment.Exit(0);
                    }
                    else
                    {
                        using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventWaitHandleName))
                        {
                            ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, OtherInstanceAttemptedToStart, null, Timeout.Infinite, false);
                        }

                        RemoveApplicationsStartupDeadlockForStartupCrushedWindows();
                    }
                }
                catch
                {
                    // It's first instance.

                    // Register EventWaitHandle.
                    using (var eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, eventWaitHandleName))
                    {
                        ThreadPool.RegisterWaitForSingleObject(eventWaitHandle, OtherInstanceAttemptedToStart, null, Timeout.Infinite, false);
                    }

                    RemoveApplicationsStartupDeadlockForStartupCrushedWindows();
                }
            }
        }

        public static bool  RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            var appName = Application.Current.GetType().Assembly.ManifestModule.ScopeName;
            var proc = Process.GetProcesses();

            //Loop through the running processes in with the same name
            foreach (Process process in proc)
            {
                //Ignore the current process
                if (process.ProcessName.Contains("ETS") && process.Id != current.Id)
                {
                    return true;
                }
            }

            return false;
        }

        private static void OtherInstanceAttemptedToStart(Object state, Boolean timedOut)
        {
            RemoveApplicationsStartupDeadlockForStartupCrushedWindows();
            Application.Current.Dispatcher.BeginInvoke(new Action(() => { try { Application.Current.MainWindow.Activate(); } catch { } }));
        }

        internal static DispatcherTimer AutoExitAplicationIfStartupDeadlock;

        /// <summary>
        /// Бывают случаи, когда при старте произошла ошибка и ни одно окно не появилось.
        /// При этом второй инстанс приложения уже не запустить, а этот не закрыть, кроме как через панель задач. Deedlock своего рода получился.
        /// </summary>
        public static void RemoveApplicationsStartupDeadlockForStartupCrushedWindows()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                AutoExitAplicationIfStartupDeadlock =
                    new DispatcherTimer(
                        TimeSpan.FromSeconds(6),
                        DispatcherPriority.ApplicationIdle,
                        (o, args) =>
                        {
                            if (Application.Current.Windows.Cast<Window>().Count(window => !Double.IsNaN(window.Left)) == 0)
                            {
                                // For that exit no interceptions.
                                Environment.Exit(0);
                            }
                        },
                        Application.Current.Dispatcher
                    );
            }),
                DispatcherPriority.ApplicationIdle
                );
        }
    }

    public enum SingleInstanceModes
    {
        /// <summary>
        /// Do nothing.
        /// </summary>
        NotInited = 0,

        /// <summary>
        /// Every user can have own single instance.
        /// </summary>
        ForEveryUser,
    }
}
