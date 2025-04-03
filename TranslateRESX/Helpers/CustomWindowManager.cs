using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using System.Windows;

namespace TranslateRESX.Helpers
{
    public class CustomWindowManager : WindowManager
    {
        private IDictionary<WeakReference, WeakReference> windows = new Dictionary<WeakReference, WeakReference>();

        public override void ShowWindow(object rootModel, object context = null, IDictionary<string, object> settings = null)
        {
            NavigationWindow navWindow = null;

            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                navWindow = Application.Current.MainWindow as NavigationWindow;
            }

            if (navWindow != null)
            {
                var window = CreatePage(rootModel, context, settings);
                navWindow.Navigate(window);
            }
            else
            {
                var window = GetExistingWindow(rootModel);
                Debug.WriteLine(window?.Title);
                if (window == null)
                {
                    try
                    {
                        window = CreateWindow(rootModel, false, context, settings);
                        windows.Add(new WeakReference(rootModel), new WeakReference(window));
                        window.Show();
                    }
                    catch (InvalidOperationException)
                    {
                    }
                }
                else
                {
                    window.Focus();
                }
            }
        }

        private readonly PropertyInfo _propertyInfo = typeof(Window).GetProperty("IsDisposed", BindingFlags.NonPublic | BindingFlags.Instance);


        protected virtual Window GetExistingWindow(object model)
        {
            if (!windows.Any(d => d.Key.IsAlive && d.Key.Target == model))
                return null;

            var pair = windows.FirstOrDefault(d => d.Key.Target == model);


            if (pair.Value == null)
            {
                if (pair.Key != null)
                    windows.Remove(pair.Key);
                return null;
            }

            var window = pair.Value.Target as Window;
            if (window == null)
                return null;

            var isDisposed = (bool)_propertyInfo.GetValue(window);

            if (isDisposed)
            {
                windows.Remove(pair.Key);
                return null;
            }

            return window;
        }
    }
}
