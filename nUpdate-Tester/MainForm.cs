using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using nUpdate.Internal;

namespace nUpdate_Tester
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        UpdateManager manager;

        private void button1_Click(object sender, EventArgs e)
        {
            manager = new UpdateManager(new Uri("http://www.trade-programming.de/nupdate/updates.json"), "<RSAKeyValue><Modulus>uBUAV+3bYyZEQEeliRph2KXEdQCGGCQB1LHKZ66m97yufc7XZguRxHqlmb4BIbcjtBvo3MJV21g9OVsDUSuy0RVtUDJF++LIBOpWaIICfWd1el2it4D/72x8lGX2SdpN0gT+mE/l8vXHULAdq8c/HVSBG9eGv8Zfsmxsw4m9ge0AecKjVAxSTIrlDr9SoK8uD6BD+jy81vnuE6WWEMhoSU+uwRtIiA7q9M9MRW9g5K1LkjOQtqRHTpsUqPxN66LWtBCuVbKb6RQLwuHhMocDesK6Oz/i1gJZQgoom7HDEbCQvhqIzDRAARnbwA+bytIGd9NXKjbDavMNrPKjXPpr5TqdPd2/51hS42kYPPP8plhvdb6ySx5dfU+sGnkO90GLxxVUKon85Ak4vwd8OKm2nneXOI0vW7wjx4CvzIaYtK4eiZhKd+y3tIhMCMKaJFvU/dk36mbyMEM367Itqusy3waFKhGhemj0oLNVKnkk3iFNIxOqeCvLRS5tomOVADzeqkLolzEiXr6BWDC/c+aaAWaHvCJ3ly7akPxPpnAp82HJkprqVwhg6sMn9WugTeUrXSRR/PE8mrlOlzkAyPTdDB+WiSXn6KEV2fN0upBHKSfTFvEYij6u6nttbS3SWD9mAL9nfSPqUHVRt6tz2p4Y2DuW/tdOg3qtUkG0cykI0+E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new Version(1, 0, 0, 0));
            manager.IncludeAlpha = false;
            manager.IncludeBeta = false;
            //manager.Language = nUpdate.Core.Language.Language.English;
            manager.Language = nUpdate.Core.Language.Language.German;

            var updaterUI = new UpdaterUI(manager);
            updaterUI.ShowUserInterface();

            //    var dialog = new SearchForm();
            //    manager.UpdateSearchFinished += new UpdateManager.UpdateSearchFinishedEventHandler(dialog.Finished);
            //    manager.CheckForUpdatesAsync();
            //    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            //        dialog.Close();
            //    MessageBox.Show(manager.UpdateVersion.ToString());
            //}
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //manager = new UpdateManager(new Uri("http://www.trade-programming.de/nupdate/updates.json"), "<RSAKeyValue><Modulus>uBUAV+3bYyZEQEeliRph2KXEdQCGGCQB1LHKZ66m97yufc7XZguRxHqlmb4BIbcjtBvo3MJV21g9OVsDUSuy0RVtUDJF++LIBOpWaIICfWd1el2it4D/72x8lGX2SdpN0gT+mE/l8vXHULAdq8c/HVSBG9eGv8Zfsmxsw4m9ge0AecKjVAxSTIrlDr9SoK8uD6BD+jy81vnuE6WWEMhoSU+uwRtIiA7q9M9MRW9g5K1LkjOQtqRHTpsUqPxN66LWtBCuVbKb6RQLwuHhMocDesK6Oz/i1gJZQgoom7HDEbCQvhqIzDRAARnbwA+bytIGd9NXKjbDavMNrPKjXPpr5TqdPd2/51hS42kYPPP8plhvdb6ySx5dfU+sGnkO90GLxxVUKon85Ak4vwd8OKm2nneXOI0vW7wjx4CvzIaYtK4eiZhKd+y3tIhMCMKaJFvU/dk36mbyMEM367Itqusy3waFKhGhemj0oLNVKnkk3iFNIxOqeCvLRS5tomOVADzeqkLolzEiXr6BWDC/c+aaAWaHvCJ3ly7akPxPpnAp82HJkprqVwhg6sMn9WugTeUrXSRR/PE8mrlOlzkAyPTdDB+WiSXn6KEV2fN0upBHKSfTFvEYij6u6nttbS3SWD9mAL9nfSPqUHVRt6tz2p4Y2DuW/tdOg3qtUkG0cykI0+E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new Version(1, 0, 0, 0));
            //manager.IncludeAlpha = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            //manager = new UpdateManager(new Uri("http://www.trade-programming.de/nupdate/updates.json"), "<RSAKeyValue><Modulus>uBUAV+3bYyZEQEeliRph2KXEdQCGGCQB1LHKZ66m97yufc7XZguRxHqlmb4BIbcjtBvo3MJV21g9OVsDUSuy0RVtUDJF++LIBOpWaIICfWd1el2it4D/72x8lGX2SdpN0gT+mE/l8vXHULAdq8c/HVSBG9eGv8Zfsmxsw4m9ge0AecKjVAxSTIrlDr9SoK8uD6BD+jy81vnuE6WWEMhoSU+uwRtIiA7q9M9MRW9g5K1LkjOQtqRHTpsUqPxN66LWtBCuVbKb6RQLwuHhMocDesK6Oz/i1gJZQgoom7HDEbCQvhqIzDRAARnbwA+bytIGd9NXKjbDavMNrPKjXPpr5TqdPd2/51hS42kYPPP8plhvdb6ySx5dfU+sGnkO90GLxxVUKon85Ak4vwd8OKm2nneXOI0vW7wjx4CvzIaYtK4eiZhKd+y3tIhMCMKaJFvU/dk36mbyMEM367Itqusy3waFKhGhemj0oLNVKnkk3iFNIxOqeCvLRS5tomOVADzeqkLolzEiXr6BWDC/c+aaAWaHvCJ3ly7akPxPpnAp82HJkprqVwhg6sMn9WugTeUrXSRR/PE8mrlOlzkAyPTdDB+WiSXn6KEV2fN0upBHKSfTFvEYij6u6nttbS3SWD9mAL9nfSPqUHVRt6tz2p4Y2DuW/tdOg3qtUkG0cykI0+E=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new Version(1, 0, 0, 0));
            //manager.IncludeBeta = checkBox2.Checked;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
