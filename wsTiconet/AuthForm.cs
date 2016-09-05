using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wsTiconet
{
    public partial class AuthForm : Form
    {
        private delegate void onCallDispose();
        private event onCallDispose callDispose;

        public AuthForm()
        {
            InitializeComponent();
            callDispose += AuthForm_callDispose;
        }

        public void Get(out string service_name, out string p)
        {
            service_name = serviceName.Text;
            p = pwd.Text;

            ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
            {
                callDispose();
            }));
        }

        private void flush()
        {
            pwd.Text = "";
            serviceName.Text = "";
        }


        void AuthForm_callDispose()
        {
            flush();
            this.Dispose();
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void close_Click(object sender, EventArgs e)
        {
            flush();
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        private void pwd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
                ok_button.PerformClick();
        }
    }
}
