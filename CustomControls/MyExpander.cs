using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace GraphPlotter
{
    public class MyExpander : Expander
    {
        bool DeExpand;
        readonly int DeExpandDelay = 1000;
        CancellationTokenSource tokenSource;

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            tokenSource?.Cancel();
            DeExpand = false;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            DeExpand = true;

            tokenSource = new CancellationTokenSource();
            if (DeExpand) Task.Delay(DeExpandDelay).ContinueWith(x =>
             {
                 this.Dispatcher.Invoke((Action)(() =>
                 {
                     DeExpand = false;
                     IsExpanded = false;
                     tokenSource.Dispose();
                     tokenSource = null;
                 }));
             }, tokenSource.Token);
        }
    }
}
