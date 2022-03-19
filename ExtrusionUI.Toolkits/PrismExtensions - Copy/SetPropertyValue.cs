using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;


namespace ExtrusionUI.Toolkits.PrismExtensions
{
    public abstract class SetPropertyValue : BindableBase
    {
        private readonly Dictionary<string, object> _propertyBackingDictionary = new Dictionary<string, object>();

        protected virtual bool SetProperty<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            if (EqualityComparer<T>.Default.Equals(newValue, GetPropertyValue<T>(propertyName))) return false;

            _propertyBackingDictionary[propertyName] = newValue;

            //If no subscribers then skip PropertyChanged
            try
            {
                RaisePropertyChanged(propertyName);
            }
            catch { }
            return true;
            }

        protected T GetPropertyValue<T>([CallerMemberName] string propertyName = null)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            object value;
            if (_propertyBackingDictionary.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }
    }
}
