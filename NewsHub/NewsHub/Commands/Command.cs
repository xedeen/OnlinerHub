using System;
using System.Windows.Input;

namespace NewsHub.Commands
{
    /// <summary>
    /// Класс ViewModelCommand – реализующий интерфейс ICommand, вызывает нужную функцию.
    /// </summary>
    public class Command : ICommand
    {
        private Action _action;
        private bool _canExecute;
        private Action<object> _parameterizedAction;
        /// <summary>
        /// Инициализация нового экземпляра класса без параметров <see cref="Command"/>.
        /// </summary>
        /// <param name="action">Действие.</param>
        /// <param name="canExecute">Если установлено в<c>true</c> [can execute] (выполнение разрешено).</param>
        public Command(Action action, bool canExecute = true)
        {
            //  Set the action.
            this._action = action;
            this._canExecute = canExecute;
        }

        /// <summary>
        /// Инициализация нового экземпляра класса с параметрами <see cref="Command"/> class.
        /// </summary>
        /// <param name="parameterizedAction">Параметризированное действие.</param>
        /// <param name="canExecute"> Если установлено в <c>true</c> [can execute](выполнение разрешено).</param>
        public Command(Action<object> parameterizedAction, bool canExecute = true)
        {
            //  Set the action.
            this._parameterizedAction = parameterizedAction;
            this._canExecute = canExecute;
        }

        public bool CanExecute
        {
            get { return _canExecute; }
            set
            {
                if (_canExecute != value)
                {
                    _canExecute = value;
                    EventHandler canExecuteChanged = CanExecuteChanged;
                    if (canExecuteChanged != null)
                        canExecuteChanged(this, EventArgs.Empty);
                }
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return _canExecute;
        }

        void ICommand.Execute(object parameter)
        {
            this.DoExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public event CancelCommandEventHandler Executing;

        public event CommandEventHandler Executed;

        protected void InvokeAction(object param)
        {
            Action theAction = _action;
            Action<object> theParameterizedAction = _parameterizedAction;
            if (theAction != null)
                theAction();
            else if (theParameterizedAction != null)
                theParameterizedAction(param);
        }

        protected void InvokeExecuted(CommandEventArgs args)
        {
            CommandEventHandler executed = Executed;

            //  Вызвать все события
            if (executed != null)
                executed(this, args);
        }

        protected void InvokeExecuting(CancelCommandEventArgs args)
        {
            CancelCommandEventHandler executing = Executing;

            //  Call the executed event.
            if (executing != null)
                executing(this, args);
        }

        /// <summary>
        /// Выполнение команды
        /// </summary>
        /// <param name="param">The param.</param>
        public virtual void DoExecute(object param)
        {
            //  Вызывает выполнении команды с возможностью отмены
            CancelCommandEventArgs args =
               new CancelCommandEventArgs() { Parameter = param, Cancel = false };
            InvokeExecuting(args);

            //  Если событие было отменено -  останавливаем.
            if (args.Cancel)
                return;

            //  Вызываем действие с / без параметров, в зависимости от того. Какое было устанвленно.
            InvokeAction(param);

            //  Call the executed function.
            InvokeExecuted(new CommandEventArgs() { Parameter = param });
        }
    }

    public delegate void CommandEventHandler(object sender, CommandEventArgs args);

    public class CommandEventArgs
    {
        public object Parameter { get; set; }
    }

    public delegate void CancelCommandEventHandler(object sender, CancelCommandEventArgs args);

    public class CancelCommandEventArgs
    {
        public object Parameter { get; set; }
        public bool Cancel { get; set; }
    }
}