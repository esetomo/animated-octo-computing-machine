using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Lemonade
{
    public abstract class ModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            MemberExpression memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentNullException("ラムダ式の内容がプロパティになっていません", "propertyExpression");

            RaisePropertyChanged(memberExpression.Member.Name);
        }

        protected void SetError(string message, [CallerMemberName] string propertyName = "")
        {
            errors[propertyName] = message;
        }

        protected void ClearError([CallerMemberName] string propertyName = "")
        {
            errors.Remove(propertyName);
        }

        protected bool HasError()
        {
            return errors.Any();
        }

        private Dictionary<string, string> errors = new Dictionary<string, string>();

        string IDataErrorInfo.Error
        {
            get { return null; }
        }

        string IDataErrorInfo.this[string columnName]
        {
            get
            {
                if (errors.ContainsKey(columnName))
                    return errors[columnName];
                return null;
            }
        }
    }
}
