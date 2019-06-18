// MainDialog.cs, 10.06.2019
// Copyright (C) Dominic Beger 17.06.2019

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Updating;

namespace nUpdate.UserInterfaceTest
{
    public partial class MainDialog : Form
    {
        private readonly UpdaterUI _updaterUI;

        public MainDialog()
        {
            InitializeComponent();

            UpdateManager manager = new UpdateManager(new Uri("http://localhost/test/updates.json"), "<RSAKeyValue><Modulus>v6ftlHsEjF+Mgtu439+GMXMdBDOmMlAdi+cQ4VVrU9J68ldgmJk3PmqWpeY5Pk1NALwG4Z3ByDHHG9nihSSavn/COOt2+4jqSJRcW1KshaZ1tVFmKl1KOPmEGdz0Snv4K8CkR5GjZ4IhgDrVYt1SHs1fTB2o5GE24zFofah31okw/UGVVFnQCnSLyt5Gr5fl6lILA91iTOy4vL+S9MLGgXrbYPJTsxL7cATDtqenTglbI60zAhl+kSwbpP+Gp+aELH+L8p3eo52toFBKTASi3mK2nK4op5LgApUJIqGEqcjFSjAphtsCw7Y7SV/KMIJTSOkMzzUTO0djF40y/fmOvJHY4Hn9dcaTByt9m48sQUFuB+eXBk6nSoGDHk5prlueOnZrd0jYcnZJMFPGLiNBeqZBbIgakIEpawy0sZTDu6e4xiRPhWuepcSw6jwxjCtpjLJ+o6V7Xc24spapV5z33cmqx014m0qQJP0XW/jWUgDwzhFiNxPFvJnQ7LEjl/3E0y00uOl8UB0y5aWZJdbC5fCLxpmo0N/Vmw9qUOrhj932eGqr1MOT2HfVm9901aACQHyqj/RpGCzW31OdyweYocs/i9CK3FHFRedVQFg1azgvIn8MR5v3Br8SnIFMua+COSMw5MgUkbLzk21j8jnbF8+cWzSuGo1H3CY4C4+4MFRjCpbOUiKoUOD24zw+0QV9Wz5tTrY4sySYmYBhiuefm7QgJtVmOn1pZGyE5e7Ytqhbw1sid2iYtUhyuLkD6b16UrEujR5K1736ROMfw5Bb3JP5ufiflvuCk+SuzfwuFU4QfglScZnX+qdXKMHDpf6/IVGAJgxWAA+KgE1wLstDDR8tqmy1oWwRfiZusEA/Cd5VRv6iAZ2hVQsMurWWS701P3uuOLOv0A3iHYcgxvM2fk9MZbdxO39atgsvMfG/EVMgUTDk5l/TIE9RWcqI4IFmKI1wPqUG3fGzzZOKCAAKMFgzLrMn9aVzC8+jyl5Wp24Xzn6/vWJRiHEwvuwyvXBtJW+g3EZa4PJLhRcsUkUz2VtToD0YRupKt8wPFTo2QvgmHgic+/llKp0SV1XZI2o77T8NSIJUOzetFCeckdEeE+a9NQbWJ3JNHnhQR+gSixaFx7nXMHbKux+JSmZZmtxtqNgHxsdnREEG4FYEHFSL3dwzH4hOAuUJ0539zTkh4gAovzvLWMA36VpY0SlX0PVWSHpTUWhdcTrII4zuSW8OsJV1v+pv6ZaL48rYxiJ4N/b2qkI3F2dqztwS0fdq64CCa70nHgRiNENUIIElC+HbeIRzpLeHTbmlF6csayu7P8S/ui1KEqOFYigBBPk+ZcGb3gG1yeAoBKMA6V1fl0Y8IQ==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new CultureInfo("en"));
            _updaterUI = new UpdaterUI(manager, SynchronizationContext.Current);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            _updaterUI.ShowUserInterface();
        }

        private void hiddenSearchCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            _updaterUI.UseHiddenSearch = true;
        }
    }
}