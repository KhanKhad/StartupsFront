using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace StartupsFront.MVVM
{
    public class Cmd : ICommand
    {
        public Cmd()
        {
        }

        public Cmd(Action executed)
        {
            this.Executed = (a, b) => executed();
            CanExecuteChecker = (f, o) => true;

        }
        public static Cmd CreateAsync(Func<Task> executeAsync) => new Cmd(executeAsync);
        private Cmd(Func<Task> executeAsync)
        {
            this.Executed += (_, __) => executeAsync();
            CanExecuteChecker = (f, o) => true;

        }
        public Cmd(Action<Cmd, object> Executed)
        {
            this.Executed += Executed;
            CanExecuteChecker = (f, o) => true;
        }

        public Func<Cmd, object, bool> CanExecuteChecker
        {
            get { return _CanExecuteChecker; }
            set
            {
                if (value != null)
                    _CanExecuteChecker = value;
                else
                    _CanExecuteChecker = (f, o) => true;
                CanExecuteChanged?.Invoke(this, new EventArgs());
            }
        }

        private Func<Cmd, object, bool> _CanExecuteChecker;
        public event Action<Cmd, object> Executed;

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            if (CanExecuteChecker != null)
            {
                return CanExecuteChecker(this, parameter);
            }
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void Execute(object parameter)
        {
            Executed?.Invoke(this, parameter);
        }

        #endregion

        public void SetExecutability(bool val)
        {
            CanExecuteChecker = (c, o) => val;
        }
    }

    public class Cmd<T> : Cmd
    {
        public Cmd()
        { }

        public Cmd(Action<Cmd, T> ExecutedT)
        {
            this.ExecutedT += ExecutedT;
        }
        public event Action<Cmd, T> ExecutedT;
        public override void Execute(object parameter)
        {
            base.Execute(parameter);
            if (ExecutedT != null)
            {
                if (parameter is T)
                {
                    ExecutedT(this, (T)parameter);
                }
                else
                {
                    ExecutedT(this, default(T));
                }
            }
        }
    }
}
