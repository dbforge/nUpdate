using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using nUpdate.Updating;

namespace nUpdate.UserInterfaceTest
{
    public partial class MainDialog : Form
    {
        public MainDialog()
        {
            InitializeComponent();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            var manager = new UpdateManager(new Uri("https://www.nupdate.net/test/updates.json"), "<RSAKeyValue><Modulus>pYYHTtXdJyf8L3qVO6iFmJqV2DcRB5rQwEEVvu+DfdmD1VmcWToxCsSsSx/bCQfayjnH0G3eGexWZtNhNG5zEx9phjIfYuK5L6+I8qC20IFU2sg9VQiPRMJRYRd9W5d9T+jLTrs3iXhXKqDoT/Nj1lgSNduYBiLtKk+B4JJjGPVi5bysVCPJyLU3XEL2Ks2ZhWj2tdZddQsVESno2S1umCXo+AWXxmBZTukl10VUQ3d2lHv/BVVAGsuFKTy8CzEd02WxFY4OA0LbXOIMxtLwXvlEnTlKp0/zDnk2saqNr+i5oxWWlUO3WuIPY+0bGmVcyP48di8tLTtqR0KE6pRKt8uWqckabXn9zyQxYhXNBkciBswhidnSkuBTBTkpnB7IreYCPm7mr0jk1W79jjg+BIUX/BpLM0C9Qpgl7kLmm6Mg/YEwFX1nbPlANqpHdbvi1qM05Cj29WG/tTXgBcSBrUibnJZkv+iJbuQGnfwz5a2ySzGwxOi/Xj6nNL1kKpmS85ZPEspUbxwnLGAyrI1yjj6IMedryW83wCpvgtHKxS3EO5gV6Kj4Orftfkz57zHvgGDTOmQgTFcHQ4CahZ11fTYYvYAvKms/2BAZYoIA/kg9fIYdQSjQLFfI+wntV7cOeKHeMLP77p2xpSK0jqns2dd+EifrbRg9uUEpHESanO3DH0qVmofpQ8SXMKaCOsNmnSIbI1+4Bk/B+Hb3ExFA4lpmlIT5wSGtKty+gJqYIviHYfFnoEt+HjSIKKis/fi6xeQz8kS5N/n4qMKVO/Cz6SbrZ0nCEaIMAjSf2zX9q9YbNoGiETSWb3R8M2SiBhq8zvr/TFO32bHw58xjR0yqBXb5z+y0eU62bvUvDIcWfwzkGcxM2Sr0tYPgkJWU8MW6nrtiX5oGA8xyAHs63CL6XVAo/fbS3KYq2o9PSStLARiExyiBzMeEoFguLTYeD2/XC3/QRcsQJS9G4Gb9N6yhjD+OWJSr4dPz2kVKyFXetkFtDBjecXXsms5UlMTiWalK64jEF4mlOZhqwcZ511m/5qpLaL2MPM3JHkgTB/p3+lYnGZY6f+mhU+ZiAxxK8g4pG+CTmUYDczzJMJRXXWiXFJ/bHX6XTp0p9BmI4qH0/l1VMXs07UxkZnWd9cX4m0+/w9UhkFIhYib4auKYDqFaC+ee6owFTUPTTOwkNIYkC9PSOrYcXDu+lLzOEL/JmA0GobyCpQuLZs2RwOU9B2/UijygOKvXyJhw31UAHHQBGLOYZgwhn0WPQkD5szYinORnrVyiFQKLCq1uTJdMbAyodItwLKIa/hg18Pi6eLp8NUzirfe1qo0p3QNImodgZvY/mWkH5gxmTYg58Y5jWkOWgw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>", new CultureInfo("zh-CN"), new UpdateVersion("1.0"));
            // manager.HttpAuthenticationCredentials = new NetworkCredential("trade", "test123");
            new UpdaterUI(manager, SynchronizationContext.Current).ShowUserInterface();
        }
    }
}